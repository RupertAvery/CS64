using CS64.Core.CPU;
using System;
using System.Runtime.CompilerServices;

namespace CS64.Core.Video
{
    //http://www.zimmers.net/cbmpics/cbm/c64/vic-ii.txt
    public class VICII
    {
        //         | Video  | # of  | Visible | Cycles/ |  Visible
        //  Type   | system | lines |  lines  |  line   | pixels/line
        //---------+--------+-------+---------+---------+------------
        //6567R56A | NTSC-M |  262  |   234   |   64    |    411
        // 6567R8  | NTSC-M |  263  |   235   |   65    |    418
        //  6569   |  PAL-B |  312  |   284   |   63    |    403

        private uint CyclesPerLine = 63;
        private uint MaxLines = 312;

        // Hack: Not sure about these values, just tweaked to lessen the top and right borders
        private const int HBlank = 24;
        private const int StartRaster = 16;

        public int Width = 403 - HBlank;
        public int Height = 284 - StartRaster;

        public uint[] buffer;
        private uint[] video_matrix_line = new uint[40];
        private uint[] color_line = new uint[40];
        private uint video_matrix;
        // called CB in documentation
        private uint character_generator; 

        private readonly MC6502State _c64;

        private uint video_counter;
        private uint video_counter_base;
        private uint row_counter;
        private uint video_matrix_line_index;

        private uint raster_line;
        private uint cycle = 1;
        private bool bad_line_condition;

        private uint g_data;
        private StateEnum state;

        private uint r_sel;
        private uint c_sel;

        private uint border_left = 31;
        private uint border_right = 335;
        private uint border_top = 55;
        private uint border_bottom = 247;

        // flip-flops
        private bool main_border;
        private bool vertical_border;

        public uint cycles;


        public uint[] palette =
        {
            0x000000,
            0xFFFFFF,
            0x313A8C,
            0xBDB563,
            0x943A8C,
            0x4AA552,
            0x8C3142,
            0x73CEBD,
            0x29528C,
            0x004252,
            0x636BBD,
            0x525252,
            0x7B7B7B,
            0x8CE694,
            0xC56B7B,
            0x9C9C9C
        };

        public VICII(MC6502State c64)
        {
            this._c64 = c64;
            buffer = new uint[Width * Height];
        }

        private uint den;
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

        private uint ecm;
        private uint bmm;
        private uint mcm;
        private uint video_mode;


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
            address &= 0x3F;

            switch (address)
            {
                case 0x11:
                    r_sel = (value & 0x8) >> 3;

                    if (r_sel == 0)
                    {
                        //text_lines = 24;
                        border_top = 55;
                        border_bottom = 246;
                    }
                    else
                    {
                        //text_lines = 25;
                        border_top = 51;
                        border_bottom = 250;
                    }

                    // save lower bit
                    video_mode &= 0x01;
                    //ecm = (value >> 6) & 1;
                    //bmm = (value >> 5) & 1;
                    video_mode |= ((value >> 4) & 0x6); // filter out lower bit
                    den = ((value >> 4) & 0x1);
                    y_scroll = value & 7;
                    raster_comparator |= ((value & 0x80) << 1);
                    break;
                case 0x12: raster_comparator |= value & 0xFF; break;
                //case 0x13: var x = 1; break;// LightPenX ; // Read only 
                //case 0x14: var y = 1; break;// LightPenY ; 
                case 0x15: sprite_enabled = value; break;
                case 0x16:
                    c_sel = (value & 0x8) >> 3;

                    if (c_sel == 0)
                    {
                        //characters = 38;
                        border_left = 31;
                        border_right = 334;
                    }
                    else
                    {
                        //characters = 40;
                        border_left = 24;
                        border_right = 343;
                    }
                    //mcm = (value >> 4) & 1;
                    video_mode |= (value >> 4) & 1;

                    x_scroll = value & 7;
                    break;
                case 0x17: sprite_y_expansion = value; break;
                case 0x18:
                    video_matrix = (value >> 4) & 0xF;
                    character_generator = (value >> 1) & 0x7;
                    break;
                case 0x19: interrupt_register = value; break;
                case 0x1A: interrupt_enabled = value; break;
                case 0x1B: sprite_data_priority = value; break;
                case 0x1C: sprite_multicolor = value; break;
                case 0x1D: sprite_x_expansion = value; break;
                case 0x1E: sprite_sprite_collision = value; break;
                case 0x1F: sprite_data_collision = value; break;
                case 0x20: border_color = (value & 0xF); break;
                case 0x21: background_color0 = (value & 0xF); break;
                case 0x22: background_color1 = (value & 0xF); break;
                case 0x23: background_color2 = (value & 0xF); break;
                case 0x24: background_color3 = (value & 0xF); break;
                case 0x25: sprite_multicolor0 = (value & 0xF); break;
                case 0x26: sprite_multicolor1 = (value & 0xF); break;
                case 0x27: color_sprite0 = (value & 0xF); break;
                case 0x28: color_sprite1 = (value & 0xF); break;
                case 0x29: color_sprite2 = (value & 0xF); break;
                case 0x2A: color_sprite3 = (value & 0xF); break;
                case 0x2B: color_sprite4 = (value & 0xF); break;
                case 0x2C: color_sprite5 = (value & 0xF); break;
                case 0x2D: color_sprite6 = (value & 0xF); break;
                case 0x2E: color_sprite7 = (value & 0xF); break;
                    //default:
                    //    throw new AccessViolationException();
            }
        }

        private uint raster_comparator;
        private uint sprite_enabled;
        private uint sprite_data_priority;
        private uint sprite_multicolor;
        private uint sprite_x_expansion;
        private uint sprite_y_expansion;
        private uint sprite_sprite_collision;
        private uint sprite_data_collision;
        private uint interrupt_register;
        private uint interrupt_enabled;

        public uint Read(uint address)
        {
            address &= 0x3F;

            uint data = address switch
            {
                0x12 => raster_line,
                0x18 => (video_matrix << 4) | (character_generator << 1),
                //The bit 7 in the latch $d019 reflects the inverted state of the IRQ output
                //    of the VIC.
                0x19 => ReadInterrupts(),
                0x1A => interrupt_enabled,
                0x21 => background_color0,
                _ => throw new AccessViolationException()
            };
            return data;
        }

        private bool den_frame_set;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint ReadInterrupts()
        {
            // reading from the register clears it (acknowledge)
            var temp = interrupt_register;
            interrupt_register = 0;
            return temp;
        }

        public uint VicRead(uint address)
        {
            // Bank
            // 0x00 3 0xC000 - 0xFFFF
            // 0x01 2 0x8000 - 0xBFFF
            // 0x02 1 0x4000 - 0x7FFF
            // 0x03 0 0x0000 - 0x3FFF
            var bank = 3 - (_c64.Cia2.PortA & 3);

            address = (bank * 0x4000) | address;

            uint data = address switch
            {
                >= 0x1000 and < 0x1FFF => _c64.CHAROM[address - 0x1000],
                >= 0x9000 and < 0x9FFF => _c64.CHAROM[address - 0x9000],
                _ => _c64.RAM[address]
            };
            return data;
        }


        public void Clock()
        {
            //         | Video  | # of  | Visible | Cycles/ |  Visible
            //  Type   | system | lines |  lines  |  line   | pixels/line
            //---------+--------+-------+---------+---------+------------
            //6567R56A | NTSC-M |  262  |   33    |   64    |    411
            // 6567R8  | NTSC-M |  263  |   235   |   65    |    418
            //  6569   |  PAL-B |  312  |   284   |   63    |    403

            //         | First  |  Last  |              |   First    |   Last
            //         | vblank | vblank | First X coo. |  visible   |  visible
            //  Type   |  line  |  line  |  of a line   |   X coo.   |   X coo.
            //---------+--------+--------+--------------+------------+-----------
            //6567R56A |   13   |   40   |  412 ($19c)  | 488 ($1e8) | 388 ($184)
            // 6567R8  |   13   |   40   |  412 ($19c)  | 489 ($1e9) | 396 ($18c)
            //  6569   |  300   |   15   |  404 ($194)  | 480 ($1e0) | 380 ($17c)

            //The graphics data sequencer is capable of 8 different graphics modes that
            //are selected by the bits ECM, BMM and MCM(Extended Color Mode, Bit Map
            //Mode and Multi Color Mode) in the registers $d011 and $d016(of the 8
            //possible bit combinations, 3 are "invalid" and generate the same output,
            //    the color black).The idle state is a bit special in that no c - accesses
            //occur in it and the sequencer uses "0" bits for the video matrix data.

            //    The sequencer outputs the graphics data in every raster line in the area of
            //    the display column as long as the vertical border flip - flop is reset(see
            //    section 3.9.). Outside of the display column and if the flip - flop is set,
            //the last current background color is displayed(this area is normally
            //    covered by the border). The heart of the sequencer is an 8 bit shift
            //register that is shifted by 1 bit every pixel and reloaded with new
            //    graphics data after every g-access.With XSCROLL from register $d016 the
            //reloading can be delayed by 0 - 7 pixels, thus shifting the display up to 7
            //pixels to the right.

            void p_access(uint sprite)
            {

            }

            void s_access()
            {

            }

            void c_access()
            {
                if (bad_line_condition)
                {
                    // d018 |VM13|VM12|VM11|VM10|CB13|CB12|CB11|  - | Memory pointers
                    var address = video_matrix << 10;
                    address |= video_counter;

                    //The read data is stored
                    //in the video matrix/color line at the position specified by VMLI. These
                    //data is internally read from the position specified by VMLI as well on
                    //each g-access in display state.

                    video_matrix_line[video_matrix_line_index] = VicRead(address);
                    color_line[video_matrix_line_index] = _c64.COLOR[address & 0x3FF];
                }
            }

            void g_access()
            {
                var c_data = video_matrix_line[video_matrix_line_index];

                // TEST: Display incrementing data to show character set
                //c_data = video_counter;

                var address = (character_generator << 11) | (c_data << 3) | (row_counter & 7);
                g_data = VicRead(address);
                if (state == StateEnum.DisplayState)
                {
                    //  VC and VMLI are incremented after each g-access in display state.
                    video_counter++;
                    video_matrix_line_index++;
                }
            }


            //As long as register $d011 is not modified in the middle of a frame, the
            //display logic is in display state within the display window and in idle
            //state outside of it.If you set a YSCROLL other than 3 in a 25 line display
            //window and store a value not equal to zero in $3fff you can see the stripes
            //generated by the sequencer in idle state on the upper or lower side of the
            //window.

            if (raster_line == 0x30 && den == 1)
            {
                den_frame_set = true;
            }

            //A Bad Line Condition is given at any arbitrary clock cycle, if at the
            //negative edge of ø0 at the beginning of the cycle RASTER >= $30 and RASTER
            //<= $f7 and the lower three bits of RASTER are equal to YSCROLL and if the
            //DEN bit was set during an arbitrary cycle of raster line $30.


            if (raster_line >= 0x30 && raster_line <= 0xF7 && ((raster_line & 7) == y_scroll) && den_frame_set)
            {
                state = StateEnum.DisplayState;
                bad_line_condition = true;
            }

            //If there is a Bad Line Condition in cycles 12-54, BA is set low and the
            //c-accesses are started. Once started, one c-access is done in the second
            //phase of every clock cycle in the range 15-54. 
            if (cycle >= 12 && cycle <= 54 && bad_line_condition)
            {
                _c64.BusAvailable = false;
            }


            //In the first phase of cycle 14 of each line, VC is loaded from VCBASE
            //(VCBASE->VC) and VMLI is cleared.If there is a Bad Line Condition in
            //this phase, RC is also reset to zero.

            switch (cycle)
            {
                // First few odd cycles of the raster line access sprite data 3 - 9
                case <= 9:
                    {
                        if (cycle % 2 == 1)
                        {
                            var spr = 3 + (cycle - 1) / 2;
                            p_access(spr);
                        }
                    }
                    break;
                // cycle 15 preemptively loads character data for the first column
                case 14:
                    {
                        video_counter = video_counter_base;
                        video_matrix_line_index = 0;
                        if (bad_line_condition)
                        {
                            row_counter = 0;
                        }

                        break;
                    }
                // visible cycles access the current character generator data, and data for the next column
                case 15:
                    c_access();
                    break;
                case >= 16 and <= 54:
                    g_access();
                    c_access();
                    break;
                case 55:
                    g_access();
                    break;
                case 58:
                    {
                        // not sure when this is supposed to happen
                        _c64.BusAvailable = true;

                        // In the first phase of cycle 58, the VIC checks if RC = 7.If so, the video
                        // logic goes to idle state and VCBASE is loaded from VC(VC->VCBASE)
                        if (row_counter == 7)
                        {
                            video_counter_base = video_counter;
                            state = StateEnum.IdleState;
                        }
                        if (state == StateEnum.DisplayState)
                        {
                            // . If the video logic is in display state afterwards(this is always the case
                            // if there is a Bad Line Condition), RC is incremented.
                            row_counter++;
                        }

                        var spr = (cycle - 58) / 2;
                        p_access(spr);

                        break;
                    }
                // cycles 58, 60 and 62 access sprite 0, 1 & 2
                case 60:
                case 62:
                    {
                        var spr = (cycle - 58) / 2;
                        p_access(spr);
                    }
                    break;

            }

            var y = raster_line - StartRaster;

            var c_color = color_line[video_matrix_line_index > 39 ? 0 : video_matrix_line_index];

            if (y <= Height - 1)
            {
                for (var i = 0; i < 8; i++)
                {
                    // Some magic offset
                    var x = (cycle - 13) * 8 + i;

                    if ((x <= Width - 1))
                    {
                        //There are 2×2 comparators belonging to each of the two flip flops. There
                        //comparators compare the X/Y position of the raster beam with one of two
                        //hardwired values (depending on the state of the CSEL/RSEL bits) to control
                        //the flip flops. The comparisons only match if the values are reached
                        //precisely. There is no comparison with an interval.


                        //The flip flops are switched according to the following rules:

                        //1. If the X coordinate reaches the right comparison value, the main border
                        //   flip flop is set.
                        //2. If the Y coordinate reaches the bottom comparison value in cycle 63, the
                        //   vertical border flip flop is set.
                        //3. If the Y coordinate reaches the top comparison value in cycle 63 and the
                        //   DEN bit in register $d011 is set, the vertical border flip flop is
                        //   reset.
                        //4. If the X coordinate reaches the left comparison value and the Y
                        //   coordinate reaches the bottom one, the vertical border flip flop is set.
                        //5. If the X coordinate reaches the left comparison value and the Y
                        //   coordinate reaches the top one and the DEN bit in register $d011 is set,
                        //   the vertical border flip flop is reset.
                        //6. If the X coordinate reaches the left comparison value and the vertical
                        //   border flip flop is not set, the main flip flop is reset.

                        if (x == border_right)
                        {
                            main_border = true;
                        }
                        if (y == border_bottom - StartRaster && cycle == CyclesPerLine)
                        {
                            vertical_border = true;
                        }
                        if (y == border_top - StartRaster && den == 1)
                        {
                            vertical_border = false;
                        }
                        if (x == border_left && y == border_bottom - StartRaster)
                        {
                            vertical_border = true;
                        }
                        if (x == border_left && y == border_top - StartRaster && den == 1)
                        {
                            vertical_border = false;
                        }
                        if (x == border_left && !vertical_border)
                        {
                            main_border = false;
                        }

                        //So the Y coordinate is checked once or twice within each raster line: In
                        //cycle 63 and if the X coordinate reaches the left comparison value.

                        if (main_border || vertical_border)
                        {
                            buffer[x + y * Width] = palette[border_color];
                        }
                        else
                        {
                            if (state == StateEnum.DisplayState)
                            {
                                buffer[x + y * Width] = ((g_data >> (7 - i)) & 1) == 0
                                    ? palette[background_color0]
                                    : palette[c_color];
                            }
                            else
                            {
                                buffer[x + y * Width] = palette[border_color];
                            }
                        }

                    }
                }
            }

            cycle++;

            if (cycle >= CyclesPerLine)
            {
                // IRQ
                // The negative edge of IRQ on a raster interrupt has been used to define the
                // beginning of a line(this is also the moment in which the RASTER register
                // is incremented).Raster line 0 is, however, an exception: In this line, IRQ
                // and incrementing(resp.resetting) of RASTER are performed one cycle later
                // than in the other lines.But for simplicity we assume equal line lengths
                // and define the beginning of raster line 0 to be one cycle before the
                // occurrence of the IRQ.


                cycle = 1;
                raster_line++;

                if (raster_line == raster_comparator)
                {
                    if ((interrupt_enabled & 1) == 1)
                    {
                        //The bit 7 in the latch $d019 reflects the inverted state of the IRQ output
                        //    of the VIC.
                        interrupt_register |= 0x1;
                        _c64.TriggerInterrupt(InterruptTypeEnum.IRQ);
                    }
                }

                // hack - when should we clear BLC?
                bad_line_condition = false;

                if (raster_line >= MaxLines)
                {
                    raster_line = 0;

                    // Should this be here?
                    den_frame_set = false;

                    video_counter_base = 0;
                }
            }

            cycles++;

        }

    }
}
