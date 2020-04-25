using Z64;
using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using Common;

namespace RDP
{
    public struct RDPVec2
    {
        public short X { get; set; }
        public short Y { get; set; }

        public RDPVec2(short x, short y)
        {
            X = x;
            Y = y;
        }
    }
    public struct RDPVec3
    {
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }

        public RDPVec3(short x, short y, short z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public class RDPVtx
    {
        public const int SIZE = 0x10;

        public short X;
        public short Y;
        public short Z;
        public ushort Flag { get; set; }
        public short TexX;
        public short TexY;
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public RDPVtx()
        {

        }
        public RDPVtx(BinaryStream br)
        {
            Parse(br);
        }
        public RDPVtx(byte[] rawVtx)
        {
            using (MemoryStream ms = new MemoryStream(rawVtx))
            {
                BinaryStream br = new BinaryStream(ms);
                br.ByteConverter = Syroot.BinaryData.ByteConverter.Big;
                Parse(br);
            }
        }
        private void Parse(BinaryStream br)
        {
                X = br.ReadInt16();
                Y = br.ReadInt16();
                Z = br.ReadInt16();
                Flag = br.ReadUInt16();
                TexX = br.ReadInt16();
                TexY = br.ReadInt16();
                R = br.Read1Byte();
                G = br.Read1Byte();
                B = br.Read1Byte();
                A = br.Read1Byte();
        }
        public void Write(BinaryStream bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
            bw.Write(Flag);
            bw.Write(TexX);
            bw.Write(TexY);
            bw.Write(R);
            bw.Write(G);
            bw.Write(B);
            bw.Write(A);
        }
    }

    public class SegmentedAddress
    {
        public bool Segmented { get; set; }
        public int SegmentId { get; set; }
        public uint SegmentOff { get; set; }
        public uint VAddr { get => Segmented ? ((uint)(SegmentId << 24) | SegmentOff) : SegmentOff; }

        public SegmentedAddress(int segId, int segOff) : this((uint)(segId << 24) | (uint)segOff)
        {

        }
        public SegmentedAddress(uint vaddr)
        {
            int segId = (int)(vaddr >> 24);
            Segmented = (segId >= 0 && segId < 16);
            if (Segmented)
            {
                SegmentId = (int)(vaddr >> 24);
                SegmentOff = vaddr & 0xFFFFFF;
            }
            else
            {
                SegmentId = -1;
                SegmentOff = vaddr;
            }
        }
    }

    public struct FixedPoint
    {
        public int Raw;
        public readonly int IntBits;
        public readonly int FracBits;
        public readonly bool Signed;

        public bool SignBit() => Signed ? (((Raw >> (FracBits + IntBits)) & 1) == 1) : false;
        public uint IntPart() => (uint)((Raw >> FracBits) & ((1 << IntBits) - 1));
        public uint FracPart() => (uint)(Raw & ((1 << FracBits) - 1));
        public float Float()
        {
            float ret = IntPart() + ((float)FracPart() / (1 << FracBits));
            if (SignBit())
                ret *= -1;

            return ret;
        }
        public override string ToString()
        {
            List<string> parts = new List<string>();
            if (SignBit())
                parts.Add($"(1<<{IntBits + FracBits})");

            uint intPart = IntPart();
            if (intPart != 0)
                parts.Add($"({intPart}<<{FracBits})");

            uint fracPart = FracPart();
            if (fracPart != 0)
                parts.Add($"{fracPart}");

            if (parts.Count == 0)
                return "0";
            return string.Join(" | ", parts.ToArray());
        }

        public FixedPoint(int raw, int intBits, int fracBits, bool signed = false)
        {
            Raw = raw;
            IntBits = intBits;
            FracBits = fracBits;
            Signed = signed;

            if (intBits < 0)
                throw new ArgumentOutOfRangeException(nameof(intBits));
            if (fracBits < 0)
                throw new ArgumentOutOfRangeException(nameof(fracBits));

            if ((intBits + fracBits) > 32)
                throw new ArgumentOutOfRangeException($"{nameof(intBits)}/{nameof(fracBits)}");
        }
    }

    public static class RdpUtils
    {
        public static float ConvertFixedPoint(int value, int intBits, int fracBits, bool signed = false)
        {
            if (intBits < 0)
                throw new ArgumentOutOfRangeException(nameof(intBits));
            if (fracBits < 0)
                throw new ArgumentOutOfRangeException(nameof(fracBits));

            if ((intBits + fracBits) > 32)
                throw new ArgumentOutOfRangeException($"{nameof(intBits)}/{nameof(fracBits)}");


            bool neg = signed
                ? ((value >> (fracBits + intBits)) & 1) == 1
                : false;

            float intPart = (uint)((value >> fracBits) & ((1<<intBits)-1));
            float fracPart = (uint)(value & ((1 << fracBits) - 1));

            float ret = intPart + (fracPart / (1 << fracBits));
            if (neg)
                ret *= -1;

            return ret;
        }

    }

}
