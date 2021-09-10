using CS64.Core.CPU;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace CS64.Core.Video
{
    //http://www.zimmers.net/cbmpics/cbm/c64/vic-ii.txt
    public class VICII
    {
        public uint[] buffer = new uint[320 * 200];
        public uint[] register = new uint[47];
        public uint[] video_matrix_color_line = new uint[40];
        public uint video_matrix;
        public uint CB;
        public uint chargen;

        private MC6502State _c64;

        public uint video_counter;
        public uint video_counter_base;
        public uint row_counter;
        public uint video_matrix_line_index;

        //private uint[] video_matrix = new uint[40 * 25];

        // https://lospec.com/palette-list/colodore
        //http://unusedino.de/ec64/technical/misc/vic656x/colors/
        //https://www.pepto.de/projects/colorvic/

        public uint[] palette =
        {
            0x000000,
            0x4a4a4a,
            0x7b7b7b,
            0xb2b2b2,
            0xffffff,
            0x813338,
            0xc46c71,
            0x553800,
            0x8e5029,
            0xedf171,
            0xa9ff9f,
            0x56ac4d,
            0x75cec8,
            0x706deb,
            0x2e2c9b,
            0x8e3c97
        };

        public VICII(MC6502State c64)
        {
            this._c64 = c64;
        }

        private uint text_lines;
        private uint characters;
        private uint first_line;
        private uint last_line;
        private uint first_x;
        private uint last_x;
        private uint x_scroll;
        private uint y_scroll;

        private uint memory_pointer;

        private uint border_color;
        private uint background_color0;
        private uint background_color1;
        private uint background_color2;
        private uint background_color3;
        private uint sprite_multicolor0;
        private uint sprite_multicolor1;

        private uint color_sprite0;
        private uint color_sprite1;
        private uint color_sprite2;
        private uint color_sprite3;
        private uint color_sprite4;
        private uint color_sprite5;
        private uint color_sprite6;
        private uint color_sprite7;


        //RSEL|  Display window height   | First line  | Last line
        //----+--------------------------+-------------+----------
        //0 | 24 text lines/192 pixels |   55 ($37)  | 246 ($f6)
        //1 | 25 text lines/200 pixels |   51 ($33)  | 250 ($fa)

        //CSEL|   Display window width   | First X coo. | Last X coo.
        //----+--------------------------+--------------+------------
        //0 | 38 characters/304 pixels |   31 ($1f)   |  334 ($14e)
        //1 | 40 characters/320 pixels |   24 ($18)   |  343 ($157)

        public void Write(uint address, uint value)
        {
            register[address - 0xD000] = (byte)value;
            switch (address)
            {
                case 0xD011:
                    var rsel = (value & 0x8) >> 3;
                    if (rsel == 0)
                    {
                        text_lines = 24;
                        first_line = 55;
                        last_line = 246;
                    }
                    else
                    {
                        text_lines = 25;
                        first_line = 51;
                        last_line = 250;
                    }

                    x_scroll = value & 7;
                    break;
                case 0xD016:
                    var csel = (value & 0x8) >> 3;
                    if (csel == 0)
                    {
                        characters = 38;
                        first_x= 31;
                        last_x = 334;
                    }
                    else
                    {
                        characters = 40;
                        first_x = 24;
                        last_x = 343;
                    }
                    y_scroll = value & 7;
                    break;
                case 0xD018:
                    // VM13|VM12|VM11|VM10|CB13|CB12|CB11|  - 
                    video_matrix = (value >> 4) & 0xF;
                    CB = (value >> 1) & 0x7;

                    break;
                case 0xD020: border_color = (value & 0xF); break;
                case 0xD021: background_color0 = (value & 0xF); break;
                case 0xD022: background_color1 = (value & 0xF); break;
                case 0xD023: background_color2 = (value & 0xF); break;
                case 0xD024: background_color3 = (value & 0xF); break;
                case 0xD025: sprite_multicolor0 = (value & 0xF); break;
                case 0xD026: sprite_multicolor1 = (value & 0xF); break;
                case 0xD027: color_sprite0 = (value & 0xF); break;
                case 0xD028: color_sprite1 = (value & 0xF); break;
                case 0xD029: color_sprite2 = (value & 0xF); break;
                case 0xD02A: color_sprite3 = (value & 0xF); break;
                case 0xD02B: color_sprite4 = (value & 0xF); break;
                case 0xD02C: color_sprite5 = (value & 0xF); break;
                case 0xD02D: color_sprite6 = (value & 0xF); break;
                case 0xD02E: color_sprite7 = (value & 0xF); break;

            }
            register[address - 0xD000] = value;
        }

        public uint Read(uint address)
        {
            uint data = 0;
            if (address == 0xD012)
            {
                data = raster_line;
            }
            return data;
        }

        public void VicWrite(uint address, uint value)
        {
            if (address >= 0x1000 && address < 0x1FFF)
            {
                //mC6502State.CHAROM[address - 0x1000];
            }
            else if (address >= 0x9000 && address < 0x9FFF)
            {
                //mC6502State.CHAROM[address - 0x1000];
            }
            else
            {
                _c64.RAM[_bankSelect * 0x4000 + address] = (byte)value;
            }
        }

        private int _bankSelect;

        public uint VicRead(uint address)
        {
            uint data = 0;
            if (address >= 0x1000 && address < 0x1FFF)
            {
                data = _c64.CHAROM[address - 0x1000];
            }
            else if (address >= 0x9000 && address < 0x9FFF)
            {
                data = _c64.CHAROM[address - 0x9000];
            }
            else
            {
                data = _c64.RAM[_bankSelect * 0x4000 + address];
            }
            return data;
        }

        private uint raster_line;
        private uint cycle = 1;
        private bool bad_line_condition;

        private uint c_color;
        private uint c_data;
        private uint g_data;

        public void Clock()
        {
            
 //         | Video  | # of  | Visible | Cycles/ |  Visible
 //  Type   | system | lines |  lines  |  line   | pixels/line
 //---------+--------+-------+---------+---------+------------
 //6567R56A | NTSC-M |  262  |   234   |   64    |    411
 // 6567R8  | NTSC-M |  263  |   235   |   65    |    418
 //  6569   |  PAL-B |  312  |   284   |   63    |    403

 //         | First  |  Last  |              |   First    |   Last
 //         | vblank | vblank | First X coo. |  visible   |  visible
 //  Type   |  line  |  line  |  of a line   |   X coo.   |   X coo.
 //---------+--------+--------+--------------+------------+-----------
 //6567R56A |   13   |   40   |  412 ($19c)  | 488 ($1e8) | 388 ($184)
 // 6567R8  |   13   |   40   |  412 ($19c)  | 489 ($1e9) | 396 ($18c)
 //  6569   |  300   |   15   |  404 ($194)  | 480 ($1e0) | 380 ($17c)

            cycle++;

            if (cycle == 14)
            {
                video_counter = video_counter_base;
                video_matrix_line_index = 0;
                if (bad_line_condition)
                {
                    row_counter = 0;
                }
            }

            if (cycle >= 12 && cycle <= 54)
            {
                // If there is a Bad Line Condition in cycles 12-54, BA is set low and the
                // c-accesses are started. Once started, one c-access is done in the second
                // phase of every clock cycle in the range 15-54. The read data is stored
                // in the video matrix/color line at the position specified by VMLI. These
                // data is internally read from the position specified by VMLI as well on
                // each g-access in display state.
            }

            if (cycle == 58)
            {
                if (row_counter == 7)
                {
                    video_counter_base = video_counter;
                    // if video logic in display state?
                    // row_counter++
                }
            }
            // In the first phase of cycle 58, the VIC checks if RC = 7.If so, the video
            // logic goes to idle state and VCBASE is loaded from VC(VC->VCBASE).If
            // the video logic is in display state afterwards(this is always the case
            // if there is a Bad Line Condition), RC is incremented

            if (raster_line == 0)
            {
                video_counter_base = 0;
            }

            void p_access(uint sprite)
            {

            }


            void c_access()
            {
                // d018 |VM13|VM12|VM11|VM10|CB13|CB12|CB11|  - | Memory pointers
                var address = video_matrix << 10;
                address |= video_counter;

                //c_data = VicRead(address);
                video_matrix_color_line[video_matrix_line_index] = VicRead(address);
                c_color = _c64.COLOR[address & 0x3FF];
            }

            void g_access()
            {
                var address = CB << 11 | c_data << 4 | (row_counter & 7);
                g_data = VicRead(address);
                //  VC and VMLI are incremented after each g-access in display state.
                video_counter++;
                video_matrix_line_index++;
            }

            var offsetx = 3;

            // First few odd cycles of the raster line access sprite data 3 - 9
            if (cycle <= 9)
            {
                if (cycle % 2 == 1)
                {
                    var spr =  3 + (cycle - 1) / 2;
                    p_access(spr);
                }
            }

            // cycle 15 preemptively loads character data for the first column
            if (cycle == 15)
            {
                c_access();
            }

            // visible cycles access the current character generator data, and data for the next column
            if (cycle >= 16 && cycle <= 54)
            {
                g_access();
                c_access();
            }

            // last one
            if (cycle == 55)
            {
                g_access();
            }
            
            if (cycle == 58)
            {
                if (row_counter == 7)
                {
                    video_counter_base = video_counter;
                }
                // . If the video logic is in display state afterwards(this is always the case
                // if there is a Bad Line Condition), RC is incremented.
                row_counter++;
            }

            // cycles 60 and 62 access sprite 1 & 2
            if (cycle == 60 || cycle == 62)
            {
                var spr = 1 + (cycle - 60) / 2;
                p_access(spr);
            }


            if (cycle >= 16 && cycle <= 55 && raster_line >= 31 && raster_line <= 230)
            {
                var x = (cycle - 16) * 8;
                var y = raster_line - 31;

                for (var i = 0; i < 8; i++)
                {
                    buffer[x + i + y * 320] = ((g_data >> (7 - i)) & 1) == 0 ? palette[background_color0] : palette[c_color];
                }

            }

            cycles++;

            //if (cycle == 65)
            //{
            //    cycle = 0;
            //}

            if (cycle == 64)
            {
                // IRQ
                cycle = 1;
                raster_line++;
                if (raster_line >= 262)
                {
                    raster_line = 0;
                    video_counter_base = 0;
                }
            }

        }

        private uint cycles;
        private int dot;
        private int scanline;
    }
}
