using System;
using System.IO;
using System.Text;
using CS64.Core.Audio;
using CS64.Core.Video;

namespace CS64.Core.CPU
{
    public partial class MC6502State
    {
        //private bool running;
        private StringBuilder log;
        private int _instructionCyclesLeft;
        public bool Debug { get; set; }
        public VICII Vic;
        public CIA Cia1;
        public CIA Cia2;

        public MC6502State()
        {
            log = new StringBuilder();
            Vic = new VICII(this);
            Sid = new SID(this);
            Cia1 = new CIA();
            Cia2 = new CIA();
            // TODO: Hack - without this the ROM does not set the video matrix to the correct value as it's disabled
            // The C64 Debugger emulator seems to have this set to 0xFF by default, and is write-protected 
            Cia2.Write(0x2, 0xFF);// enable write on port A
        }


        public void Init()
        {
            MC6502InstructionSet.InitOpcodeTable();
            blip.SetRates((uint)cpuclockrate, 44100);
        }

        public uint Step()
        {
            Vic.Clock();
            if (BusAvailable)
            {
                ClockCpu();
            }
            // TODO: is the counter driven by the CPU clock?
            Cia1.Clock();
            Cia2.Clock();

            int s = Sid.EmitSample();

            if (s != old_s)
            {
                blip.AddDelta(Sid.sampleclock, s - old_s);
                old_s = s;
            }

            Sid.sampleclock++;

            return 1;
        }

        private void ClockCpu()
        {
            // TODO: is the counter driven by the CPU clock?
            Cia1.Count();
            Cia2.Count();

            if (_instructionCyclesLeft-- > 0)
            {
                return;
            }

            _instructionCyclesLeft += (int)ExecuteInstruction();
            Cycles += (uint)_instructionCyclesLeft;
            Instructions++;
        }

        public void Execute()
        {
            int ctr = 0;
            try
            {
                Debug = true;
                var running = true;
                while (running)
                {
                    Step();
                    if (ctr > 10000)
                    {
                        running = false;
                    }
                }

                File.WriteAllText("c64.log", log.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                File.WriteAllText("c64.log", log.ToString());
            }
        }

        /// <summary>
        /// Executes one instruction and return the number of cycles consumed
        /// </summary>
        /// <returns></returns>
        public uint ExecuteInstruction()
        {
            for (var i = 0; i < _interrupts.Length; i++)
            {
                if (_interrupts[i])
                {
                    _interrupts[i] = false;
                    switch ((InterruptTypeEnum)i)
                    {
                        case InterruptTypeEnum.NMI:
                            return NonMaskableInterrupt();
                        case InterruptTypeEnum.IRQ:
                            return InterruptRequest();
                    }
                }
            }

            switch (PC)
            {
                case 0xEA8E: // keyboard routine
                    {
                        var x = 1;
                        break;
                    }
            }

            var ins = BusRead(PC);
            var bytes = MC6502InstructionSet.bytes[ins];

            if (Debug)
            {
                Log(bytes);
            }

            // This could be moved into each instruction, but we would need to implement all 255 instructions separately
            switch (MC6502InstructionSet.addrmodes[ins])
            {
                case MC6502InstructionSet.ACC:
                case MC6502InstructionSet.IMP:
                    break;
                case MC6502InstructionSet.IMM:
                    AddrModeImmediate();
                    break;
                case MC6502InstructionSet.DP_:
                    AddrModeZeroPage();
                    break;
                case MC6502InstructionSet.DPX:
                    AddrModeZeroPageX();
                    break;
                case MC6502InstructionSet.DPY:
                    AddrModeZeroPageY();
                    break;
                case MC6502InstructionSet.IND:
                    if (ins == 0x6c)
                    {
                        AddrModeIndirect_JMP();
                    }
                    else
                    {
                        AddrModeIndirect();
                    }
                    break;
                case MC6502InstructionSet.IDX:
                    AddrModeIndirectX();
                    break;
                case MC6502InstructionSet.IDY:
                    AddrModeIndirectY();
                    break;
                case MC6502InstructionSet.ABS:
                    AddrModeAbsolute();
                    break;
                case MC6502InstructionSet.ABX:
                    AddrModeAbsoluteX();
                    break;
                case MC6502InstructionSet.ABY:
                    AddrModeAbsoluteY();
                    break;
                case MC6502InstructionSet.REL:
                    AddrModeRelative();
                    break;
                default:
                    //File.WriteAllText("mario.log", log.ToString());

                    throw new NotImplementedException();
            }

            PC += bytes;

            var pcycles = MC6502InstructionSet.cycles[ins];

            pcycles += MC6502InstructionSet.OpCodes[ins](this);

            if (PageBoundsCrossed)
            {
                //switch (ins)
                //{
                //    /*
                //        According to documentation, these modes are affected
                //        ADC
                //        AND
                //        CMP
                //        EOR
                //        LAX
                //        LDA
                //        LDX
                //        LDY
                //        NOP
                //        ORA
                //        SBC
                //        (indirect),Y
                //        absolute,X
                //        absolute,Y
                //     */
                //    case 0x71:
                //    case 0x7D:
                //    case 0x79:
                //    case 0x31:
                //    case 0x3D:
                //    case 0x39:
                //    case 0xD1:
                //    case 0xDD:
                //    case 0xD9:
                //    case 0x51:
                //    case 0x5D:
                //    case 0x59:
                //    case 0xB3:
                //    case 0xBF:
                //    case 0xB1:
                //    case 0xBD:
                //    case 0xB9:
                //    case 0xBE:
                //    case 0xBC:
                //    case 0x1C:
                //    case 0x3C:
                //    case 0x5C:
                //    case 0x7C:
                //    case 0xDC:
                //    case 0xFC:
                //    case 0x11:
                //    case 0x1D:
                //    case 0x19:
                //    case 0xF1:
                //    case 0xFD:
                //    case 0xF9:
                //    case 0xF0:
                //        pcycles++;
                //        break;
                //}
                pcycles++;
                PageBoundsCrossed = false;
            }

            switch (ins)
            {
                // Just to align with nestest.log
                case 0xce:
                    pcycles += 3;
                    break;
                case 0xf3:
                    pcycles += 4;
                    break;
            }

            return pcycles;
        }

        private void Log(uint bytes)
        {
            var sb = new StringBuilder(256);
            for (var i = 0u; i < bytes; i++)
            {
                sb.Append($"{BusRead(PC + i):X2} ");
            }

            for (var i = 0; i < 3 - bytes; i++)
            {
                sb.Append($"   ");
            }

            var logMessage =
                $"{PC:X4}  {sb} A:{A:X2} X:{X:X2} Y:{Y:X2} P:{P:X2} SP:{S:X2} CYC:{Cycles}";

            Console.WriteLine(logMessage);

            //log.AppendLine(logMessage);


        }

    }
}