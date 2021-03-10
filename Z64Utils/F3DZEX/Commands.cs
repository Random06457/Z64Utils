using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDP;

namespace F3DZEX
{
    public static partial class Command
    {
        [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        sealed class CmdAttribute : Attribute
        {
            readonly OpCodeID id;

            public CmdAttribute(OpCodeID id)
            {
                this.id = id;
            }

            public OpCodeID ID { get => id; }
        }

        [Cmd(OpCodeID.G_VTX)]
        public struct GVtx
        {
            public int numv { get; set; }
            public int vbidx { get; set; }
            public uint vaddr { get; set; }
        }

        [Cmd(OpCodeID.G_SETTIMG)]
        public struct GSetTImg
        {
            public Enums.G_IM_FMT fmt { get; set; }
            public Enums.G_IM_SIZ siz { get; set; }
            public int width { get; set; }
            public uint imgaddr { get; set; }
        }

        [Cmd(OpCodeID.G_SETTILESIZE)]
        [Cmd(OpCodeID.G_LOADTILE)]
        public struct GLoadTile
        {
            public FixedPoint uls { get; set; }
            public FixedPoint ult { get; set; }
            public Enums.G_TX_tile tile { get; set; }
            public FixedPoint lrs { get; set; }
            public FixedPoint lrt { get; set; }
        }

        [Cmd(OpCodeID.G_LOADTLUT)]
        public struct GLoadTlut
        {
            public Enums.G_TX_tile tile { get; set; }
            public int count { get; set; }
        }
        [Cmd(OpCodeID.G_SETTILE)]
        public struct GSetTile
        {
            public Enums.G_IM_FMT fmt { get; set; }
            public Enums.G_IM_SIZ siz { get; set; }
            public int line { get; set; }
            public int tmem { get; set; }
            public Enums.G_TX_tile tile { get; set; }
            public int palette { get; set; }
            public Enums.ClampMirrorFlag cmT { get; set; }
            public int maskT { get; set; }
            public int shiftT { get; set; }
            public Enums.ClampMirrorFlag cmS { get; set; }
            public int maskS { get; set; }
            public int shiftS { get; set; }
        }

        [Cmd(OpCodeID.G_LOADBLOCK)]
        public struct GLoadBlock
        {
            public FixedPoint uls { get; set; }
            public FixedPoint ult { get; set; }
            public Enums.G_TX_tile tile { get; set; }
            public int texels { get; set; }
            public FixedPoint dxt { get; set; }
        }

        [Cmd(OpCodeID.G_BRANCH_Z)]
        public struct GBranchZ
        {
            public int vbidx { get; set; }
            public uint zval { get; set; }
        }

        [Cmd(OpCodeID.G_RDPHALF_1)]
        [Cmd(OpCodeID.G_RDPHALF_2)]
        public struct GRdpHalf
        {
            public uint word { get; set; }
        }

        [Cmd(OpCodeID.G_DL)]
        public struct GDl
        {
            public bool branch { get; set; }
            public uint dl { get; set; }
        }

        [Cmd(OpCodeID.G_SETFOGCOLOR)]
        [Cmd(OpCodeID.G_SETBLENDCOLOR)]
        [Cmd(OpCodeID.G_SETENVCOLOR)]
        public struct GSetColor
        {
            public byte R { get; set; }
            public byte G { get; set; }
            public byte B { get; set; }
            public byte A { get; set; }
        }

        [Cmd(OpCodeID.G_SETPRIMCOLOR)]
        public struct GSetPrimColor
        {
            public byte minlevel { get; set; }
            public byte lodfrac { get; set; }
            public byte R { get; set; }
            public byte G { get; set; }
            public byte B { get; set; }
            public byte A { get; set; }
        }

        [Cmd(OpCodeID.G_TRI1)]
        public struct GTri1
        {
            public byte v0 { get; set; }
            public byte v1 { get; set; }
            public byte v2 { get; set; }
        }

        [Cmd(OpCodeID.G_TRI2)]
        public struct GTri2
        {
            public byte v00 { get; set; }
            public byte v01 { get; set; }
            public byte v02 { get; set; }
            public byte v10 { get; set; }
            public byte v11 { get; set; }
            public byte v12 { get; set; }
        }

        [Cmd(OpCodeID.G_TEXRECT)]
        [Cmd(OpCodeID.G_TEXRECTFLIP)]
        public struct GTexRect
        {
            public FixedPoint lrx { get; set; }
            public FixedPoint lry { get; set; }
            public Enums.G_TX_tile tile { get; set; }
            public FixedPoint ulx { get; set; }
            public FixedPoint uly { get; set; }
            public FixedPoint uls { get; set; }
            public FixedPoint ult { get; set; }
            public FixedPoint dsdx { get; set; }
            public FixedPoint dtdy { get; set; }
        }

        [Cmd(OpCodeID.G_SETKEYGB)]
        public struct GSetKeyGB
        {
            public FixedPoint widthG { get; set; }
            public FixedPoint widthB { get; set; }
            public byte centerG { get; set; }
            public byte scaleG { get; set; }
            public byte centerB { get; set; }
            public byte scaleB { get; set; }
        }
        [Cmd(OpCodeID.G_SETKEYR)]
        public struct GSetKeyR
        {
            public FixedPoint widthR { get; set; }
            public byte centerR { get; set; }
            public byte scaleR { get; set; }
        }
        [Cmd(OpCodeID.G_SETSCISSOR)]
        public struct GSetScissor
        {
            public FixedPoint ulx { get; set; }
            public FixedPoint uly { get; set; }
            public int mode { get; set; }
            public FixedPoint lrx { get; set; }
            public FixedPoint lry { get; set; }
        }

        [Cmd(OpCodeID.G_FILLRECT)]
        public struct GFillRect
        {
            public FixedPoint lrx { get; set; }
            public FixedPoint lry { get; set; }
            public FixedPoint ulx { get; set; }
            public FixedPoint uly { get; set; }
        }

        [Cmd(OpCodeID.G_SETCOMBINE)]
        public struct GSetCombine
        {
            public Enums.G_CCMUX a0 { get; set; }
            public Enums.G_CCMUX b0 { get; set; }
            public Enums.G_CCMUX c0 { get; set; }
            public Enums.G_CCMUX d0 { get; set; }
            public Enums.G_ACMUX Aa0 { get; set; }
            public Enums.G_ACMUX Ab0 { get; set; }
            public Enums.G_ACMUX Ac0 { get; set; }
            public Enums.G_ACMUX Ad0 { get; set; }
            public Enums.G_CCMUX a1 { get; set; }
            public Enums.G_CCMUX b1 { get; set; }
            public Enums.G_CCMUX c1 { get; set; }
            public Enums.G_CCMUX d1 { get; set; }
            public Enums.G_ACMUX Aa1 { get; set; }
            public Enums.G_ACMUX Ab1 { get; set; }
            public Enums.G_ACMUX Ac1 { get; set; }
            public Enums.G_ACMUX Ad1 { get; set; }
        }

        [Cmd(OpCodeID.G_SETOTHERMODE_L)]
        [Cmd(OpCodeID.G_SETOTHERMODE_H)]
        public struct GSetOtherMode
        {
            public int shift { get; set; }
            public int len { get; set; }
            public uint data { get; set; }
        }


        [Cmd(OpCodeID.G_POPMTX)]
        public struct GPopMtx
        {
            public uint num { get; set; }
        }

        [Cmd(OpCodeID.G_MTX)]
        public struct GMtx
        {
            public Enums.G_MtxParams param { get; set; }
            public uint mtxaddr { get; set; }
        }


        public class CommandInfo
        {
            public OpCodeID ID { get; private set; }
            public Dictionary<string, object> Args { get; private set; }

            public CommandInfo(OpCodeID id, Dictionary<string, object> args)
            {
                ID = id;
                Args = args;
            }
            public int GetSize()
            {
                switch (ID)
                {
                    case OpCodeID.G_NOOP:
                    case OpCodeID.G_VTX:
                    case OpCodeID.G_MODIFYVTX:
                    case OpCodeID.G_CULLDL:
                    case OpCodeID.G_BRANCH_Z:
                    case OpCodeID.G_TRI1:
                    case OpCodeID.G_TRI2:
                    case OpCodeID.G_QUAD:
                    case OpCodeID.G_DMA_IO:
                    case OpCodeID.G_TEXTURE:
                    case OpCodeID.G_POPMTX:
                    case OpCodeID.G_GEOMETRYMODE:
                    case OpCodeID.G_MTX:
                    case OpCodeID.G_MOVEWORD:
                    case OpCodeID.G_MOVEMEM:
                    case OpCodeID.G_LOAD_UCODE:
                    case OpCodeID.G_DL:
                    case OpCodeID.G_ENDDL:
                    case OpCodeID.G_SPNOOP:
                    case OpCodeID.G_RDPHALF_1:
                    case OpCodeID.G_SETOTHERMODE_L:
                    case OpCodeID.G_SETOTHERMODE_H:
                    case OpCodeID.G_RDPLOADSYNC:
                    case OpCodeID.G_RDPPIPESYNC:
                    case OpCodeID.G_RDPTILESYNC:
                    case OpCodeID.G_RDPFULLSYNC:
                    case OpCodeID.G_SETKEYGB:
                    case OpCodeID.G_SETKEYR:
                    case OpCodeID.G_SETCONVERT:
                    case OpCodeID.G_SETSCISSOR:
                    case OpCodeID.G_SETPRIMDEPTH:
                    case OpCodeID.G_RDPSETOTHERMODE:
                    case OpCodeID.G_LOADTLUT:
                    case OpCodeID.G_RDPHALF_2:
                    case OpCodeID.G_SETTILESIZE:
                    case OpCodeID.G_LOADBLOCK:
                    case OpCodeID.G_LOADTILE:
                    case OpCodeID.G_SETTILE:
                    case OpCodeID.G_FILLRECT:
                    case OpCodeID.G_SETFILLCOLOR:
                    case OpCodeID.G_SETFOGCOLOR:
                    case OpCodeID.G_SETBLENDCOLOR:
                    case OpCodeID.G_SETPRIMCOLOR:
                    case OpCodeID.G_SETENVCOLOR:
                    case OpCodeID.G_SETCOMBINE:
                    case OpCodeID.G_SETTIMG:
                    case OpCodeID.G_SETZIMG:
                    case OpCodeID.G_SETCIMG:
                        return 0x8;
                    case OpCodeID.G_TEXRECT:
                    case OpCodeID.G_TEXRECTFLIP:
                        return 0x18;
                    default:
                        return -1;
                }
            }

            public T GetArg<T>(string param) => (T)Args[param];
            public T Convert<T>() => (T)DecodeCommand(typeof(T), this);
        }

        private static object DecodeCommand(Type t, CommandInfo cmd)
        {
            // TODO: check parent class

            var attr = ((CmdAttribute[])t.GetCustomAttributes(typeof(CmdAttribute), false)).ToList();

            var match = attr.Find(a => a.ID == cmd.ID);
            if (match == null)
                throw new InvalidF3DZEXOpCodeException("Invalid ID");


            object obj = Activator.CreateInstance(t);

            foreach (var prop in t.GetProperties())
            {
                if (!cmd.Args.ContainsKey(prop.Name) || cmd.Args[prop.Name].GetType() != prop.PropertyType)
                    throw new Exception("???");

                t.GetProperty(prop.Name).SetValue(obj, cmd.Args[prop.Name]);
            }
            return obj;
        }
        public static T DecodeCommand<T>(byte[] ucode, int off) => (T)DecodeCommand(typeof(T), DecodeDList(ucode, off).First());
    }
}
