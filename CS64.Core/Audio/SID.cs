using CS64.Core.CPU;
using System;
using System.Collections.Generic;
using System.Text;

namespace CS64.Core.Audio
{
    public class SID
    {
        public uint sampleclock;
        private MC6502State mC6502State;

        public SID(MC6502State mC6502State)
        {
            this.mC6502State = mC6502State;
        }

        public int EmitSample()
        {
            return 0;
        }

        public uint Read(uint address)
        {
            return 0;
        }

        public void Write(uint address, uint value)
        {
        }
    }
}
