namespace CS64.Core.Video
{
    //https://www.c64-wiki.com/wiki/CIA

    public class CIA
    {
        public uint PortA { get; private set; }
        public uint PortB { get; private set; }
        // Bit X: 0=Input (read only), 1=Output (read and write)
        private uint readwrite_mask_a;
        private uint readwrite_mask_b;

        public uint baseAddress;

        public CIA(uint address)
        {
            baseAddress = address;
        }

        public uint Read(uint address)
        {
            uint data = 0;
            address -= baseAddress;
            address &= 0x0F; // Mirror every 16 address bytes
            data = address switch
            {
                0x0 => PortA,
                0x1 => PortB,
                0x2 => readwrite_mask_a,
                0x3 => readwrite_mask_b,
                0xD => int_data,
                _ => data
            };
            return data;
        }

        private uint int_mask;
        private uint int_data;

        public void Write(uint address, uint data)
        {
            address -= baseAddress;
            address &= 0x0F; // Mirror every 16 address bytes

            switch (address)
            {
                case 0x0:
                    // mask the incoming data bits
                    data &= readwrite_mask_a;
                    // preserve the existing bits with an inverse of the mask
                    PortA &= readwrite_mask_a ^ 0xFF;
                    // merge the data
                    PortA |= data;
                    break;
                case 0x1:
                    data &= readwrite_mask_b;
                    PortB &= readwrite_mask_b ^ 0xFF;
                    PortB |= data;
                    break;
                case 0x2:
                    readwrite_mask_a = data;
                    break;
                case 0x3:
                    readwrite_mask_b = data;
                    break;
                case 0xD:
                    int_mask = data & 0b11111;
                    break;
            }
            return;
        }
    }
}