using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;

namespace CS64.Core.CPU
{
    public enum InterruptTypeEnum
    {
        NMI,
        IRQ,
        BRK
    }

    public partial class MC6502State
    {
        public byte[] RAM = new byte[0x10000];
        public byte[] KERNAL = new byte[0x2000];
        public byte[] BASIC = new byte[0x2000];
        public byte[] CHAROM = new byte[0x1000];
        public byte[] COLOR = new byte[0x400];

        public uint A;
        public uint X;
        public uint Y;
        public uint S;

        public uint PC;
        public uint N;  // bit 7
        public uint V;  // bit 6
        public uint U;  // bit 5
        public uint B;  // bit 4
        public uint D;  // bit 3
        public uint I { get; set; }  // bit 2
        public uint Z;  // bit 1
        public uint C;  // bit 0

        public uint Cycles;
        public uint Instructions;

        public bool BusAvailable = true;

        public uint dma_page;
        public uint dma_address;
        public byte dma_data;

        public bool dma_transfer;
        public bool dma_dummy;

        public uint[] Controller = new uint[2];
        public uint[] ControllerRegister = new uint[2];
        public uint P
        {
            get =>
                (uint)(
                    (N << 7) +
                    (V << 6) +
                    (U << 5) +
                    (B << 4) +
                    (D << 3) +
                    (I << 2) +
                    (Z << 1) +
                    (C << 0)
                );
            set
            {
                N = (value >> 7 & 1);
                V = (value >> 6 & 1);
                U = (value >> 5 & 1);
                B = (value >> 4 & 1);
                D = (value >> 3 & 1);
                I = (value >> 2 & 1);
                Z = (value >> 1 & 1);
                C = (value >> 0 & 1);
            }
        }


        public uint EffectiveAddr;
        public bool PageBoundsCrossed;


        public bool[] _interrupts = new bool[3];

        public void TriggerInterrupt(InterruptTypeEnum type)
        {
            if (I != 1 || type == InterruptTypeEnum.NMI)
            {
                _interrupts[(int)type] = true;
            }
        }

        public void WriteState(Stream stream)
        {
            var w = new BinaryWriter(stream);
            w.Write(RAM, 0, RAM.Length);
            w.Write((byte)A);
            w.Write((byte)X);
            w.Write((byte)Y);
            w.Write((byte)S);
            w.Write((byte)P);
            w.Write((ushort)PC);
            w.Write(Cycles);
        }

        public void ReadState(Stream stream)
        {
            var w = new BinaryReader(stream);
            w.Read(RAM, 0, RAM.Length);
            A = w.ReadByte();
            X = w.ReadByte();
            Y = w.ReadByte();
            S = w.ReadByte();
            P = w.ReadByte();
            PC = w.ReadUInt16();
            Cycles = w.ReadUInt32();
        }

        public void Reset()
        {
            // https://www.pagetable.com/?p=410
            S = 0xFD; // Actually 0xFF, but 3 Stack Pushes are done with writes supressed, 
            P = 0x24; // Just to align with nestest.log: I is set, U shouldn't exist, but...
            PC = BusRead(0xFFFC) + BusRead(0xFFFD) * 0x100; // Fetch the reset vector
            I = 1;
            Cycles = 7; // takes 7 cycles to reset
            
            IO_Port_DR = 0x1F; // enable KERNAL, I/O, BASIC

            _instructionCyclesLeft = 0;
            dma_page = 0;
            dma_address = 0x00;
            dma_transfer = false;
            for (var i = 0; i < _interrupts.Length; i++)
            {
                _interrupts[i] = false;
            }
        }

        public uint BusRead(uint address)
        {
            uint data = address switch
            {
                >= 0xE000 and <= 0xFFFF => KERNAL[address - 0xE000],
                >= 0xA000 and <= 0xBFFF => BASIC[address - 0xA000],
                >= 0xD000 and <= 0xDFFF => CharROMRead(address),
                0x0001 => IO_Port_DR,
                0x0000 => IO_Port_DDR,
                _ => RAM[address]
            };

            return data;
        }

        public void BusWrite(uint address, uint value)
        {
            switch (address)
            {
                case >= 0xD000 and <= 0xDFFF:
                    CharROMWrite(address, value);
                    break;
                case 0x0001:
                    value &= IO_Port_DDR;
                    IO_Port_DR &= IO_Port_DDR ^ 0xFF;
                    IO_Port_DR |= value;
                    break;
                case 0x0000:
                    IO_Port_DDR = value;
                    break;
                case >= 0 and <= 0xFFFF:
                    // fall through to RAM
                    RAM[address] = (byte)value;
                    break;
            }
        }

        // https://www.c64-wiki.com/wiki/Bank_Switching

        //data direction register
        public uint IO_Port_DDR;
        //data register
        public uint IO_Port_DR;

        public uint CharROMRead(uint address)
        {
            uint data = 0;
            // CHAREN = bit 2 of I/O
            if ((IO_Port_DR & 0x04) == 0x04)
            {
                data = address switch
                {
                    >= 0xD000 and <= 0xD3FF => Vic.Read(address),
                    >= 0xD400 and <= 0xD7FF => Sid.Read(address),
                    // maybe OR with the upper nybble of previous contents of the data bus - for accuracy
                    >= 0xD800 and <= 0xDBFF => (COLOR[address - 0xD800] & 0xFU),
                    >= 0xDC00 and <= 0xDCFF => Cia1.Read(address), // Keyboard
                    >= 0xDD00 and <= 0xDDFF => Cia2.Read(address), // VIC bankswitch
                    >= 0xDE00 and <= 0xDEFF => 0, // I/O 1
                    >= 0xDF00 and <= 0xDFFF => 0, // I/O 2
                };
            }
            else
            {
                data = CHAROM[address - 0xD000];
            }
            return data;
        }

        public void CharROMWrite(uint address, uint value)
        {
            // CHAREN = bit 2 of I/O port
            if ((IO_Port_DR & 0x04) == 0x04)
            {
                if (address >= 0xD000 && address <= 0xD3FF)
                {
                    Vic.Write(address, value);
                }
                else if (address >= 0xD400 && address <= 0xD7FF)
                {
                    Sid.Write(address, value);
                }
                else if (address >= 0xD800 && address <= 0xDBFF)
                {
                    // Technically, only the lower 4 bits are connected 
                    COLOR[address - 0xD800] = (byte)(COLOR[address - 0xD800] | (value & 0xFU));
                }
                else if (address >= 0xDC00 && address <= 0xDCFF)
                {
                    Cia1.Write(address, value);
                }
                else if (address >= 0xDD00 && address <= 0xDDFF)
                {
                    Cia2.Write(address, value); // VIC bankswitch
                }
            }
            else
            {
                // fall through to RAM
                RAM[address] = (byte)value;
            }
        }



        public uint NonMaskableInterrupt()
        {
            Push((PC >> 8) & 0xFF); // Push the high byte of the PC
            Push((PC & 0xFF)); // Push the low byte of the PC
            B = 0;
            U = 1;
            Push(P);
            I = 1;
            PC = BusRead(0xFFFA) + BusRead(0xFFFB) * 0x100; // Jump to NMI handler
            return 7;
        }


        public uint InterruptRequest()
        {
            Push((PC >> 8) & 0xFF); // Push the high byte of the PC
            Push((PC & 0xFF)); // Push the low byte of the PC
            B = 0;
            U = 1;
            Push(P);
            I = 1;
            PC = BusRead(0xFFFE) + BusRead(0xFFFF) * 0x100; // Jump to IRQ handler
            return 7;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Push(uint value)
        {
            BusWrite(S + 0x100, value);
            S -= 1;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Pop()
        {
            S += 1;
            return BusRead(S + 0x100);
        }

        public void LoadMemory(uint address, byte[] data)
        {
            if (address == 0xE000)
            {
                Array.Copy(data, 0, KERNAL, 0, data.Length);
            }
            else if (address == 0xA000)
            {
                Array.Copy(data, 0, BASIC, 0, data.Length);
            }
            else if (address == 0xD000)
            {
                Array.Copy(data, 0, CHAROM, 0, data.Length);
            }
        }
    }
}
