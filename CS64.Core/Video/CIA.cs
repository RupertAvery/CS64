using System;
using System.Runtime.CompilerServices;

namespace CS64.Core.Video
{
    //https://www.c64-wiki.com/wiki/CIA

    public class CIA
    {
        public uint PortA { get; set; }
        public uint PortB { get; set; }
        // Bit X: 0=Input (read only), 1=Output (read and write)
        private uint readwrite_mask_a;
        private uint readwrite_mask_b;

        public Func<uint> PortARead { get; set; }
        public Action<uint> PortAWrite { get; set; }

        public Func<uint> PortBRead { get; set; }
        public Action<uint> PortBWrite { get; set; }

        public Action RequestInterrupt { get; set; }
        public CIA()
        {
        }

        public uint Read(uint address)
        {
            uint data = 0;
            address &= 0x0F; // Mirror every 16 address bytes
            data = address switch
            {
                0x0 => PortARead?.Invoke() ?? PortA,
                0x1 => PortBRead?.Invoke() ?? PortA,
                0x2 => readwrite_mask_a,
                0x3 => readwrite_mask_b,
                0x4 => (uint)timer_A & 0xFF,
                0x5 => (uint)(timer_A >> 8) & 0xFF,
                0x6 => (uint)timer_B & 0xFF,
                0x7 => (uint)(timer_B >> 8) & 0xFF,
                0x8 => rtc_tenths,
                0x9 => rtc_seconds | (rtc_tenseconds << 4),
                0xA => rtc_minutes | (rtc_tenminutes << 4),
                0xB => rtc_hours | (rtc_tenhours << 4) | (rtc_am_pm << 7),
                0xC => shift_register,
                0xD => ReadInterrupts(),
                _ => data
            };
            return data;
        }

        private uint int_mask;
        private uint int_data;



        private uint ReadInterrupts()
        {
            var temp = int_data;
            int_data = 0; //Flags will be cleared after reading the register!
            return temp;
        }

        public void Write(uint address, uint data)
        {
            address &= 0x0F; // Mirror every 16 address bytes

            switch (address)
            {
                case 0x0:
                    // mask the incoming data bits
                    data &= readwrite_mask_a;
                    // preserve the existing bits with an inverse of the mask
                    if (PortAWrite != null)
                    {
                        PortAWrite(data);
                    }
                    else
                    {
                        PortA &= readwrite_mask_a ^ 0xFF;
                        // merge the data
                        PortA |= data;
                    }

                    break;
                case 0x1:
                    data &= readwrite_mask_b;
                    if (PortBWrite != null)
                    {
                        PortBWrite(data);
                    }
                    else
                    {
                        PortB &= readwrite_mask_b ^ 0xFF;
                        PortB |= data;
                    }

                    break;
                case 0x2:
                    readwrite_mask_a = data;
                    break;
                case 0x3:
                    readwrite_mask_b = data;
                    break;
                case 0x4:
                    timer_A_latch |= (int)data;
                    break;
                case 0x5:
                    timer_A_latch |= (int)(data << 8);
                    break;
                case 0x6:
                    timer_B_latch |= (int)data;
                    break;
                case 0x7:
                    timer_B_latch |= (int)(data << 8);
                    break;
                case 0x8:
                    if (tod_set_mode == 1)
                    {
                        alarm_tenths = data & 0xF;
                    }
                    else
                    {
                        rtc_tenths = data & 0xF;
                    }
                    tod_stopped = false;
                    break;
                case 0x9:
                    if (tod_set_mode == 1)
                    {
                        alarm_seconds = data & 0xF;
                        alarm_tenseconds = (data >> 4) & 0x7;
                    }
                    else
                    {
                        rtc_seconds = data & 0xF;
                        rtc_tenseconds = (data >> 4) & 0x7;
                    }
                    break;
                case 0xA:
                    if (tod_set_mode == 1)
                    {
                        alarm_minutes = data & 0xF;
                        alarm_tenminutes = (data >> 4) & 0x7;
                    }
                    else
                    {
                        rtc_minutes = data & 0xF;
                        rtc_tenminutes = (data >> 4) & 0x7;
                    }
                    break;
                case 0xB:
                    if (tod_set_mode == 1)
                    {
                        alarm_hours = data & 0xF;
                        alarm_tenhours = (data >> 4) & 0x7;
                        alarm_am_pm = (data >> 7) & 1;
                    }
                    else
                    {
                        rtc_hours = data & 0xF;
                        rtc_tenhours = (data >> 8) & 0x7;
                        rtc_am_pm = (data >> 7) & 1;
                    }
                    // TODO: Writing into this register stops TOD, until register 8(TOD 10THS) will be read.
                    tod_stopped = true;
                    break;
                case 0xC:
                    shift_register = data;
                    break;
                case 0xD:
                    int_mask = data & 0b11111;
                    break;
                case 0xE:
                    timer_A_start = (byte)(data & 1);
                    timer_A_underflow_stop = (byte)((data >> 3) & 1);
                    timer_A_load = (byte)((data >> 4) & 1);
                    // should this be done here?
                    if (timer_A_load == 1)
                    {
                        timer_A = timer_A_latch;
                    }
                    timer_A_count_mode = (byte)((data >> 5) & 1);
                    shift_register_direction = (byte)((data >> 6) & 1);
                    tod_frame_divider = 5 + ((data >> 7) & 1); // value will be 5 or 6
                    // tod_frame_divider = (((data >> 7) & 1) == 0 ? 5U : 6U); // divide frame rate by this value
                    break;
                case 0xF:
                    timer_B_start = (byte)(data & 1);
                    timer_B_underflow_stop = (byte)((data >> 3) & 1);
                    timer_B_load = (byte)((data >> 4) & 1);
                    // should this be done here?
                    if (timer_B_load == 1)
                    {
                        timer_B = timer_B_latch;
                    }
                    timer_B_count_mode = (byte)((data >> 5) & 3);
                    tod_set_mode = (data >> 7) & 1;
                    break;
            }
            return;
        }

        private int timer_A;
        private int timer_B;
        private int timer_A_latch;
        private int timer_B_latch;
        private uint rtc_tenths;
        private uint rtc_seconds;
        private uint rtc_tenseconds;
        private uint rtc_minutes;
        private uint rtc_tenminutes;
        private uint rtc_hours;
        private uint rtc_tenhours;
        private uint rtc_am_pm;

        private uint alarm_tenths;
        private uint alarm_seconds;
        private uint alarm_tenseconds;
        private uint alarm_minutes;
        private uint alarm_tenminutes;
        private uint alarm_hours;
        private uint alarm_tenhours;
        private uint alarm_am_pm;

        private uint tod_set_mode; // 0 = clock, 1 = alarm
        private bool tod_stopped;

        private uint shift_register_bit;
        private uint shift_register_direction; // 0 = input, 1 = output
        private uint shift_register;

        private byte int_underflow_A;
        private byte int_underflow_B;
        private byte int_alarm;


        private byte timer_A_start;
        private byte timer_A_underflow_portB6;
        private byte timer_A_underflow_portB6_type;
        private byte timer_A_underflow_stop;
        private byte timer_A_load;
        private byte timer_A_count_mode;

        private byte timer_B_start;
        private byte timer_B_underflow_portB6;
        private byte timer_B_underflow_portB6_type;
        private byte timer_B_underflow_stop;
        private byte timer_B_load;
        private byte timer_B_count_mode;

        private uint tod_frame_divider; // 5 = 50Hz, 6 = 60hz;
        private uint tod_frame_counter;
        public void TimeOfDay()
        {
            if(tod_stopped) return;

            tod_frame_counter++;
            if (tod_frame_counter > tod_frame_divider)
            {
                tod_frame_counter = 0;
                rtc_tenths++;
                if (rtc_tenths > 9)
                {
                    rtc_tenths = 0;
                    rtc_seconds++;
                    if (rtc_seconds > 9)
                    {
                        rtc_seconds = 0;
                        rtc_tenseconds++;
                        if (rtc_tenseconds > 5)
                        {
                            rtc_tenseconds = 0;
                            rtc_minutes++;
                            if (rtc_minutes > 9)
                            {
                                rtc_minutes = 0;
                                rtc_tenminutes++;
                                if (rtc_tenminutes > 5)
                                {
                                    rtc_tenminutes = 0;
                                    rtc_hours++;
                                    if (rtc_tenhours < 2 && rtc_hours > 9)
                                    {
                                        rtc_hours = 0;
                                        rtc_tenhours++;
                                    }
                                    else if (rtc_tenhours == 2 && rtc_hours > 3)
                                    {
                                        rtc_tenths = 0;
                                        rtc_seconds = 0;
                                        rtc_tenseconds = 0;
                                        rtc_minutes = 0;
                                        rtc_tenminutes = 0;
                                        rtc_hours = 0;
                                        rtc_tenhours = 0;
                                    }
                                }
                            }
                        }
                    }
                    Console.WriteLine($"{rtc_tenhours}{rtc_hours}:{rtc_tenminutes}{rtc_minutes}:{rtc_tenseconds}{rtc_seconds}");
                }
            }

        }

        public void Count()
        {
            // TODO: shift  shift_register into shift_register_bit

            if (timer_A_count_mode == 1)
            {
                UpdateTimerA();
            }
            if (timer_B_count_mode == 1)
            {
                UpdateTimerB();
            }
        }


        private void UpdateTimerA()
        {
            if (timer_A_start == 1)
            {
                timer_A--;
                if (timer_A < 0)
                {
                    if ((int_mask & 1) == 1)
                    {
                        int_data |= 1;
                        RequestInterrupt?.Invoke();
                    }
                    if (timer_A_underflow_stop == 0)
                    {
                        timer_A = timer_A_latch;
                        if (timer_B_count_mode == 2)
                        {
                            //0b10 = Timer counts underflow of timer A
                            //TODO: 0b11 = Timer counts underflow of timer A if the CNT - pin is high
                            UpdateTimerB();
                        }
                    }
                }
            }
        }



        private void UpdateTimerB()
        {
            if (timer_B_start == 1)
            {
                timer_B--;
                if (timer_B < 0)
                {
                    if ((int_mask & 2) == 2)
                    {
                        int_data |= 2;
                        RequestInterrupt?.Invoke();
                    }
                    if (timer_B_underflow_stop == 0)
                    {
                        timer_B = timer_B_latch;
                    }
                }
            }
        }

        public void Clock()
        {
            if (timer_A_count_mode == 0)
            {
                UpdateTimerA();
            }
            if (timer_B_count_mode == 0)
            {
                UpdateTimerB();
            }
        }
    }
}