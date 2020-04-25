using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace N64
{
    public static class N64CheckSum
    {
        [Serializable]
        public class N64CheckSumException : Exception
        {
            public N64CheckSumException() { }
            public N64CheckSumException(string message) : base(message) { }
            public N64CheckSumException(string message, Exception inner) : base(message, inner) { }
            protected N64CheckSumException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
        
        public static bool Validate(N64Rom rom, int cic)
        {
            var sums = Compute(rom, cic);
            return (sums.Item1 == rom.CRC1 && sums.Item2 == rom.CRC2);
        }

        private const uint CHECKSUM_START = 0x1000;
        private const uint CHECKSUM_LENGTH = 0x100000;
        private const uint CHECKSUM_END = CHECKSUM_START + CHECKSUM_LENGTH;
        private static uint ROL(uint i, int b) => ((i << b) | (i >> (32 - b)));
        private static uint BomSwap(uint a) => (a << 24) | ((a & 0xFF00) << 8) | ((a & 0xFF0000) >> 8) | (a >> 24);

        public unsafe static Tuple<uint, uint> Compute(N64Rom rom, int cic)
        {
            if (rom.RawRom.Length < CHECKSUM_START)
                throw new N64CheckSumException("Invalid File Lenght");

            fixed(byte* data8 = rom.RawRom)
            {
                uint* data32 = (uint*)data8;

                uint seed;
                uint t1, t2, t3, t4, t5, t6;
                uint pos;

                // initial seed
                switch (cic)
                {
                    case 6101:
                    case 6102:
                        seed = 0xF8CA4DDC;
                        break;
                    case 6103:
                        seed = 0xA3886759;
                        break;
                    case 6105:
                        seed = 0xDF26F436;
                        break;
                    case 6106:
                        seed = 0x1FEA617A;
                        break;
                    default:
                        throw new N64CheckSumException("Invalid CIC");
                }

                t1 = t2 = t3 = t4 = t5 = t6 = seed;

                for (pos = CHECKSUM_START; pos < CHECKSUM_END; pos += 4)
                {
                    uint d = BomSwap(data32[pos / 4]);
                    uint r = ROL(d, (int)(d & 0x1F));

                    // increment t4 if t6 overflows
                    if ((t6 + d) < t6)
                        t4++;

                    t6 += d;
                    t3 ^= d;
                    t5 += r;

                    if (t2 > d)
                        t2 ^= r;
                    else
                        t2 ^= t6 ^ d;

                    if (cic == 6105)
                        t1 += BomSwap(*(uint*)&data8[0x0750 + (pos & 0xFF)]) ^ d;
                    else
                        t1 += t5 ^ d;
                }

                if (cic == 6103)
                {
                    return new Tuple<uint, uint>(
                        (t6 ^ t4) + t3,
                        (t5 ^ t2) + t1);
                }
                else if (cic == 6106)
                {
                    return new Tuple<uint, uint>(
                        (t6 * t4) + t3,
                        (t5 * t2) + t1);
                }
                else
                {
                    return new Tuple<uint, uint>(
                        t6 ^ t4 ^ t3,
                        t5 ^ t2 ^ t1);
                }
            }
        }

        public static void Update(N64Rom rom, int cic)
        {
            var sums = Compute(rom, cic);
            rom.CRC1 = sums.Item1;
            rom.CRC2 = sums.Item2;
        }
    }
}
