using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Common;
using RDP;
using F3DZEX.Command;
using System.Diagnostics;

namespace F3DZEX
{
    public class Disassembler
    {
        private class CCMode
        {
            public static readonly List<CCMode> Modes = new List<CCMode>()
            {
                new CCMode("G_CC_PRIMITIVE", G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_PRIMITIVE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_PRIMITIVE),
                new CCMode("G_CC_SHADE", G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_MODULATEI", G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_MODULATEIA", G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_0, G_ACMUX.G_ACMUX_TEXEL0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE, G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_MODULATEIDECALA", G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_MODULATEI_PRIM", G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_PRIMITIVE, G_CCMUX.G_CCMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_PRIMITIVE),
                new CCMode("G_CC_MODULATEIA_PRIM", G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_PRIMITIVE, G_CCMUX.G_CCMUX_0, G_ACMUX.G_ACMUX_TEXEL0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_PRIMITIVE, G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_MODULATEIDECALA_PRIM", G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_PRIMITIVE, G_CCMUX.G_CCMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_DECALRGB", G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_TEXEL0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_DECALRGBA", G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_TEXEL0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_BLENDI", G_CCMUX.G_CCMUX_ENVIRONMENT, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_BLENDIA", G_CCMUX.G_CCMUX_ENVIRONMENT, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_TEXEL0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE, G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_BLENDIDECALA", G_CCMUX.G_CCMUX_ENVIRONMENT, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_BLENDRGBA", G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_TEXEL0_ALPHA, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_BLENDRGBDECALA", G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_TEXEL0_ALPHA, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_ADDRGB", G_CCMUX.G_CCMUX_1, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_ADDRGBDECALA", G_CCMUX.G_CCMUX_1, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_REFLECTRGB", G_CCMUX.G_CCMUX_ENVIRONMENT, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_REFLECTRGBDECALA", G_CCMUX.G_CCMUX_ENVIRONMENT, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_HILITERGB", G_CCMUX.G_CCMUX_PRIMITIVE, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_HILITERGBA", G_CCMUX.G_CCMUX_PRIMITIVE, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_PRIMITIVE, G_ACMUX.G_ACMUX_SHADE, G_ACMUX.G_ACMUX_TEXEL0, G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_HILITERGBDECALA", G_CCMUX.G_CCMUX_PRIMITIVE, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_SHADEDECALA", G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_BLENDPE", G_CCMUX.G_CCMUX_PRIMITIVE, G_CCMUX.G_CCMUX_ENVIRONMENT, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_ENVIRONMENT, G_ACMUX.G_ACMUX_TEXEL0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE, G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_BLENDPEDECALA", G_CCMUX.G_CCMUX_PRIMITIVE, G_CCMUX.G_CCMUX_ENVIRONMENT, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_ENVIRONMENT, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("_G_CC_BLENDPE", G_CCMUX.G_CCMUX_ENVIRONMENT, G_CCMUX.G_CCMUX_PRIMITIVE, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_PRIMITIVE, G_ACMUX.G_ACMUX_TEXEL0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE, G_ACMUX.G_ACMUX_0),
                new CCMode("_G_CC_BLENDPEDECALA", G_CCMUX.G_CCMUX_ENVIRONMENT, G_CCMUX.G_CCMUX_PRIMITIVE, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_PRIMITIVE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("_G_CC_TWOCOLORTEX", G_CCMUX.G_CCMUX_PRIMITIVE, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE),
                new CCMode("_G_CC_SPARSEST", G_CCMUX.G_CCMUX_PRIMITIVE, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_LOD_FRACTION, G_CCMUX.G_CCMUX_TEXEL0, G_ACMUX.G_ACMUX_PRIMITIVE, G_ACMUX.G_ACMUX_TEXEL0, G_ACMUX.G_ACMUX_LOD_FRACTION, G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_TEMPLERP", G_CCMUX.G_CCMUX_TEXEL1, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_PRIM_LOD_FRAC, G_CCMUX.G_CCMUX_TEXEL0, G_ACMUX.G_ACMUX_TEXEL1, G_ACMUX.G_ACMUX_TEXEL0, G_ACMUX.G_ACMUX_PRIM_LOD_FRAC, G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_TRILERP", G_CCMUX.G_CCMUX_TEXEL1, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_LOD_FRACTION, G_CCMUX.G_CCMUX_TEXEL0, G_ACMUX.G_ACMUX_TEXEL1, G_ACMUX.G_ACMUX_TEXEL0, G_ACMUX.G_ACMUX_LOD_FRACTION, G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_INTERFERENCE", G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_TEXEL1, G_CCMUX.G_CCMUX_0, G_ACMUX.G_ACMUX_TEXEL0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_TEXEL1, G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_1CYUV2RGB", G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_K4, G_CCMUX.G_CCMUX_K5, G_CCMUX.G_CCMUX_TEXEL0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_YUV2RGB", G_CCMUX.G_CCMUX_TEXEL1, G_CCMUX.G_CCMUX_K4, G_CCMUX.G_CCMUX_K5, G_CCMUX.G_CCMUX_TEXEL1, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_PASS2", G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_COMBINED, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_COMBINED),
                new CCMode("G_CC_MODULATEI2", G_CCMUX.G_CCMUX_COMBINED, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_MODULATEIA2", G_CCMUX.G_CCMUX_COMBINED, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_0, G_ACMUX.G_ACMUX_COMBINED, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE, G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_MODULATEI_PRIM2", G_CCMUX.G_CCMUX_COMBINED, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_PRIMITIVE, G_CCMUX.G_CCMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_PRIMITIVE),
                new CCMode("G_CC_MODULATEIA_PRIM2", G_CCMUX.G_CCMUX_COMBINED, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_PRIMITIVE, G_CCMUX.G_CCMUX_0, G_ACMUX.G_ACMUX_COMBINED, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_PRIMITIVE, G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_DECALRGB2", G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_0, G_CCMUX.G_CCMUX_COMBINED, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_BLENDI2", G_CCMUX.G_CCMUX_ENVIRONMENT, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_COMBINED, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_BLENDIA2", G_CCMUX.G_CCMUX_ENVIRONMENT, G_CCMUX.G_CCMUX_SHADE, G_CCMUX.G_CCMUX_COMBINED, G_CCMUX.G_CCMUX_SHADE, G_ACMUX.G_ACMUX_COMBINED, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE, G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_CHROMA_KEY2", G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_CENTER, G_CCMUX.G_CCMUX_SCALE, G_CCMUX.G_CCMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_HILITERGB2", G_CCMUX.G_CCMUX_ENVIRONMENT, G_CCMUX.G_CCMUX_COMBINED, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_COMBINED, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_HILITERGBA2", G_CCMUX.G_CCMUX_ENVIRONMENT, G_CCMUX.G_CCMUX_COMBINED, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_COMBINED, G_ACMUX.G_ACMUX_ENVIRONMENT, G_ACMUX.G_ACMUX_COMBINED, G_ACMUX.G_ACMUX_TEXEL0, G_ACMUX.G_ACMUX_COMBINED),
                new CCMode("G_CC_HILITERGBDECALA2", G_CCMUX.G_CCMUX_ENVIRONMENT, G_CCMUX.G_CCMUX_COMBINED, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_COMBINED, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_HILITERGBPASSA2", G_CCMUX.G_CCMUX_ENVIRONMENT, G_CCMUX.G_CCMUX_COMBINED, G_CCMUX.G_CCMUX_TEXEL0, G_CCMUX.G_CCMUX_COMBINED, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_0, G_ACMUX.G_ACMUX_COMBINED),

            };

            public string Name;
            G_CCMUX a;
            G_CCMUX b;
            G_CCMUX c;
            G_CCMUX d;
            G_ACMUX Aa;
            G_ACMUX Ab;
            G_ACMUX Ac;
            G_ACMUX Ad;
            public CCMode(string name, G_CCMUX a, G_CCMUX b, G_CCMUX c, G_CCMUX d, G_ACMUX Aa, G_ACMUX Ab, G_ACMUX Ac, G_ACMUX Ad)
            {
                Name = name;
                this.a = a & (G_CCMUX)((1 << 4) - 1);
                this.b = b & (G_CCMUX)((1 << 4) - 1);
                this.c = c & (G_CCMUX)((1 << 5) - 1);
                this.d = d & (G_CCMUX)((1 << 3) - 1);
                this.Aa = Aa & (G_ACMUX)((1 << 3) - 1);
                this.Ab = Ab & (G_ACMUX)((1 << 3) - 1);
                this.Ac = Ac & (G_ACMUX)((1 << 3) - 1);
                this.Ad = Ad & (G_ACMUX)((1 << 3) - 1);
            }

            public bool Match(GSetCombine cmd, int cycle)
            {
                switch (cycle)
                {
                    case 0: return cmd.a0 == a && cmd.b0 == b && cmd.c0 == c && cmd.d0 == d && cmd.Aa0 == Aa && cmd.Ab0 == Ab && cmd.Ac0 == Ac && cmd.Ad0 == Ad;
                    case 1: return cmd.a1 == a && cmd.b1 == b && cmd.c1 == c && cmd.d1 == d && cmd.Aa1 == Aa && cmd.Ab1 == Ab && cmd.Ac1 == Ac && cmd.Ad1 == Ad;
                    default: return false;
                }
            }
        }

        private class OtherModeMacro
        {
            public static readonly List<OtherModeMacro> MacrosL = new List<OtherModeMacro>()
            {
                new OtherModeMacro("SetAlphaCompare", (int)G_MDSFT_L.G_MDSFT_ALPHACOMPARE, 2, "G_AC_NONE", 0, "G_AC_THRESHOLD", 1<<0, "G_AC_DITHER", 3<<0),
                new OtherModeMacro("SetDepthSource", (int)G_MDSFT_L.G_MDSFT_ZSRCSEL, 1, "G_ZS_PIXEL", 0, "G_ZS_PRIM", 1<<2),
            };
            public static readonly List<OtherModeMacro> MacrosH = new List<OtherModeMacro>()
            {
                new OtherModeMacro("SetAlphaDither", (int)G_MDSFT_H.G_MDSFT_ALPHADITHER, 2, "G_AD_PATTERN", 0, "G_AD_NOTPATTERN", 0x10, "G_AD_NOISE", 0x20, "G_AD_DISABLE", 0x30),
                new OtherModeMacro("SetColorDither", (int)G_MDSFT_H.G_MDSFT_RGBDITHER, 2, "G_CD_MAGICSQ", 0, "G_CD_BAYER", 0x40, "G_CD_NOISE", 0x80), // HW version 1
                new OtherModeMacro("SetCombineKey", (int)G_MDSFT_H.G_MDSFT_COMBKEY, 1, "G_CK_NONE", 0, "G_CK_KEY", 1<<8),
                new OtherModeMacro("SetTextureConvert", (int)G_MDSFT_H.G_MDSFT_TEXTCONV, 3, "G_TC_CONV", 0, "G_TC_FILTCONV", 5<<9, "G_TC_FILT", 6<<9),
                new OtherModeMacro("SetTextureFilter", (int)G_MDSFT_H.G_MDSFT_TEXTFILT, 2, "G_TF_POINT", 0, "G_TF_AVERAGE", 3<<12, "G_TF_BILERP", 2<<12),
                new OtherModeMacro("SetTextureLUT", (int)G_MDSFT_H.G_MDSFT_TEXTLUT, 2, "G_TT_NONE", 0, "G_TT_RGBA16", 2<<14, "G_TT_IA16", 3<<14),
                new OtherModeMacro("SetTextureLOD", (int)G_MDSFT_H.G_MDSFT_TEXTLOD, 1, "G_TL_TILE", 0, "G_TL_LOD", 1<<16),
                new OtherModeMacro("SetTextureDetail", (int)G_MDSFT_H.G_MDSFT_TEXTDETAIL, 2, "G_TD_CLAMP", 0, "G_TD_SHARPEN", 1<<17, "G_TD_DETAIL", 2<<17),
                new OtherModeMacro("SetTexturePersp", (int)G_MDSFT_H.G_MDSFT_TEXTPERSP, 1, "G_TP_PERSP", 1<<19, "G_TP_NONE", 0<<19),
                new OtherModeMacro("SetCycleType", (int)G_MDSFT_H.G_MDSFT_CYCLETYPE, 2, "G_CYC_1CYCLE", 0, "G_CYC_2CYCLE", 1<<20, "G_CYC_COPY", 2<<20, "G_CYC_FILL", 3<<20),
                new OtherModeMacro("SetCycleType", (int)G_MDSFT_H.G_MDSFT_CYCLETYPE, 2, "G_CYC_1CYCLE", 0, "G_CYC_2CYCLE", 1<<20, "G_CYC_COPY", 2<<20, "G_CYC_FILL", 3<<20),
                new OtherModeMacro("SetColorDither", (int)G_MDSFT_H.G_MDSFT_COLORDITHER, 2, "G_CD_MAGICSQ", 0, "G_CD_BAYER", 0x40, "G_CD_NOISE", 0x80),
                new OtherModeMacro("PipelineMode", (int)G_MDSFT_H.G_MDSFT_PIPELINE, 1, "G_PM_1PRIMITIVE", 1<<23, "G_PM_NPRIMITIVE", 0),
            };

            public string Name;
            int shift;
            int len;
            public List<Tuple<string, int>> values;

            public OtherModeMacro(string name, int shift, int len, List<Tuple<string, int>> values)
            {
                Name = name;
                this.shift = shift;
                this.len = len;
                this.values = values;
            }
            public OtherModeMacro(string name, int shift, int len, string name0 = "-1", int value0 = -1, string name1 = "-1", int value1 = -1, string name2 = "-1", int value2 = -1, string name3 = "-1", int value3 = -1)
                : this(name, shift, len, new List<Tuple<string, int>>() {
                    new Tuple<string, int>(name0, value0),
                    new Tuple<string, int>(name1, value1),
                    new Tuple<string, int>(name2, value2),
                    new Tuple<string, int>(name3, value3)
                })
            {

            }

            public bool Match(GSetOtherMode cmd)
            {
                return cmd.shift == shift && cmd.len == len;
            }
        }


        public class Config
        {
            public bool ShowAddress { get; set; }
            public bool RelativeAddress { get; set; }
            public bool DisasMultiCmdMacro { get; set; }
            public bool AddressLiteral { get; set; }
            public bool Static { get; set; }
        }

        public static Config StaticConfig { get; private set; } = new Config()
        {
            ShowAddress = true,
            RelativeAddress = false,
            DisasMultiCmdMacro = true,
            AddressLiteral = false,
            Static = true,
        };
        
        uint _wordLo;
        uint _wordHi;

        Config _cfg;
        Dlist _dlist = new Dlist();

        public Disassembler(Dlist dlist, Config cfg = null)
        {
            _cfg = cfg ?? StaticConfig;
            _dlist = dlist;
        }

        public List<string> Disassemble()
        {
            List<string> lines = new List<string>();
            _wordLo = _wordHi = 0;

            int off = 0;
            int i = 0;
            int toSkip = 0;
            foreach (var cmd in _dlist)
            {
                if (toSkip > 0)
                {
                    toSkip--;
                }
                else
                {
                    uint addr = (_cfg.RelativeAddress) ? (uint)off : cmd.addr;
                    string prefix = (_cfg.ShowAddress ? $"{addr:X8}: " : "");

                    for (int j = 0; j < cmd.depth; j++)
                        prefix += "  ";

                    string dis = DisassembleInstruction(cmd.cmd);
                    if (_cfg.DisasMultiCmdMacro)
                    {
                        string macroDis = FindMultiCmdMacro(i, out int cmdCount);
                        if (cmdCount > 0)
                        {
                            dis = macroDis;
                            toSkip = cmdCount-1;
                            lines.Add($"/* Multi Command Macro Found ({cmdCount} instructions) */");
                        }
                    }

                    if (!_cfg.Static)
                    {
                        dis = dis.Remove(1, 1);
                        dis = dis.Insert(dis.IndexOf('(')+1, "gfx++, ");
                        dis = dis.Replace("(gfx++, )", "(gfx++)");
                    }
                    dis += _cfg.Static ? "," : ";";


                    lines.Add(prefix + dis);
                }

                i++;
                off += cmd.cmd.GetSize();
            }
            return lines;
        }

        private static readonly List<MultiMacro> MultiMacros = new List<MultiMacro>()
        {
            new LoadTLutMacro(),
            new LoadTextureBlockMacro()
        };

        private string FindMultiCmdMacro(int index, out int cmdCount)
        {
            foreach (var entry in MultiMacros)
            {
                if (entry.IsCandidate(this, index) && entry.Disassemble(this, index, out string res))
                {
                    cmdCount = entry.GetCommandCount();
                    return res;
                }
            }
            cmdCount = 0;
            return null;
        }

        abstract class MultiMacro
        {
            CmdID[] _ids;

            protected T GetCmd<T>(Disassembler dis, int idx) => dis._dlist.AtIndex(idx).cmd.Convert<T>();

            protected MultiMacro(params CmdID[] ids)
            {
                _ids = ids;
            }

            public bool IsCandidate(Disassembler dis, int idx)
            {
                for (int i = 0; i < _ids.Length; i++)
                {
                    if (dis._dlist.AtIndex(idx + i).cmd.ID != _ids[i])
                        return false;
                }
                return true;
            }

            public int GetCommandCount() => _ids.Length;

            public abstract bool Disassemble(Disassembler dis, int idx, out string output);
        }

        class LoadTLutMacro : MultiMacro
        {
            public LoadTLutMacro() : base(
                CmdID.G_SETTIMG,
                CmdID.G_RDPTILESYNC,
                CmdID.G_SETTILE,
                CmdID.G_RDPLOADSYNC,
                CmdID.G_LOADTLUT,
                CmdID.G_RDPPIPESYNC
                )
            {

            }

            public override bool Disassemble(Disassembler dis, int idx, out string output)
            {
                output = null;

                var setTimg = GetCmd<GSetTImg>(dis, idx++);
                if (setTimg.fmt != G_IM_FMT.G_IM_FMT_RGBA || setTimg.width != 1)
                    return false;

                idx++; // G_RDPTILESYNC 

                var setTile = GetCmd<GSetTile>(dis, idx++);

                idx++; // G_RDPLOADSYNC

                var loadTlut = GetCmd<GLoadTlut>(dis, idx++);

                idx++; // G_RDPPIPESYNC

                output = $"gsDPLoadTLUT({loadTlut.count + 1}, 0x{setTile.tmem:X}, {dis.DisAddress(setTimg.imgaddr)})";
                return true;
            }
        }

        class LoadTextureBlockMacro : MultiMacro
        {
            const int G_TX_DXT_FRAC = 11;


            public LoadTextureBlockMacro() : base(
                CmdID.G_SETTIMG,
                CmdID.G_SETTILE,
                CmdID.G_RDPLOADSYNC,
                CmdID.G_LOADBLOCK,
                CmdID.G_RDPPIPESYNC,
                CmdID.G_SETTILE,
                CmdID.G_SETTILESIZE
                )
            {

            }

            private G_IM_SIZ SizLoadBlock(G_IM_SIZ siz)
            {
                switch (siz)
                {
                    case G_IM_SIZ.G_IM_SIZ_4b: 
                    case G_IM_SIZ.G_IM_SIZ_8b:
                    case G_IM_SIZ.G_IM_SIZ_16b:
                        return G_IM_SIZ.G_IM_SIZ_16b;
                    case G_IM_SIZ.G_IM_SIZ_32b:
                        return G_IM_SIZ.G_IM_SIZ_32b;
                    default:
                        throw new ArgumentException();
                }
            }

            private int SizBytes(G_IM_SIZ siz)
            {
                switch (siz)
                {
                    case G_IM_SIZ.G_IM_SIZ_4b: return 0;
                    case G_IM_SIZ.G_IM_SIZ_8b: return 1;
                    case G_IM_SIZ.G_IM_SIZ_16b: return 2;
                    case G_IM_SIZ.G_IM_SIZ_32b: return 4;
                    default: throw new ArgumentException();
                }
            }
            private int SizTileBytes(G_IM_SIZ siz)
            {
                switch (siz)
                {
                    case G_IM_SIZ.G_IM_SIZ_4b: return 0;
                    case G_IM_SIZ.G_IM_SIZ_8b: return 1;
                    case G_IM_SIZ.G_IM_SIZ_16b: return 2;
                    case G_IM_SIZ.G_IM_SIZ_32b: return 2;
                    default: throw new ArgumentException();
                }
            }
            private int SizLineBytes(G_IM_SIZ siz)
            {
                return SizTileBytes(siz);
            }

            private int SizShift(G_IM_SIZ siz)
            {
                switch (siz)
                {
                    case G_IM_SIZ.G_IM_SIZ_4b: return 2;
                    case G_IM_SIZ.G_IM_SIZ_8b: return 1;
                    case G_IM_SIZ.G_IM_SIZ_16b: return 0;
                    case G_IM_SIZ.G_IM_SIZ_32b: return 0;
                    default: throw new ArgumentException();
                }
            }
            private int SizIncr(G_IM_SIZ siz)
            {
                switch (siz)
                {
                    case G_IM_SIZ.G_IM_SIZ_4b: return 3;
                    case G_IM_SIZ.G_IM_SIZ_8b: return 1;
                    case G_IM_SIZ.G_IM_SIZ_16b: return 0;
                    case G_IM_SIZ.G_IM_SIZ_32b: return 0;
                    default: throw new ArgumentException();
                }
            }

            private int Txl2Words(int txls, int b_txl)
            {
                return Math.Max(1, ((txls) * (b_txl) / 8));
            }
            private int CalcDxt(int width, int b_txl)
            {
                return (((1 << G_TX_DXT_FRAC) + Txl2Words(width, b_txl) - 1) /
                    Txl2Words(width, b_txl));
            }

            private int Txl2Words_4b(int txls)
            {
                return Math.Max(1, ((txls) / 16));
            }
            private int CalcDxt_4b(int width)
            {
                return (((1 << G_TX_DXT_FRAC) + Txl2Words_4b(width) - 1) /
					Txl2Words_4b(width));
            }

            public override bool Disassemble(Disassembler dis, int idx, out string output)
            {
                output = null;

                var setTimg = GetCmd<GSetTImg>(dis, idx++);
                if (setTimg.width != 1)
                    return false;

                var setTile = GetCmd<GSetTile>(dis, idx++);

                idx++; // G_RDPLOADSYNC

                var loadBlock = GetCmd<GLoadBlock>(dis, idx++);

                idx++; // G_RDPPIPESYNC

                var setTile2 = GetCmd<GSetTile>(dis, idx++);

                var setTileSize = GetCmd<GLoadTile>(dis, idx++);

                var timg = setTimg.imgaddr;
                var fmt = setTile2.fmt;  
                var siz = setTile2.siz;
                int width = (int)(setTileSize.lrs.IntPart() + 1);
                int height = (int)(setTileSize.lrt.IntPart() + 1);
                var pal = setTile2.palette;
                var cmt = setTile2.cmT;
                var maskt = setTile2.maskT;
                var shiftt = setTile2.shiftT;
                var cms = setTile2.cmS;
                var masks = setTile2.maskS;
                var shifts = setTile2.shiftS;
                int tmem = setTile2.tmem;
                G_TX_TILE rtile = setTile2.tile;

                if (
                    setTimg.fmt == fmt &&
                    setTimg.siz == SizLoadBlock(siz) &&
                    setTimg.width == 1 &&

                    setTile.fmt == fmt &&
                    setTile.siz == SizLoadBlock(siz) &&
                    setTile.line == 0 &&
                    setTile.tmem == tmem &&
                    setTile.tile == G_TX_TILE.G_TX_LOADTILE &&
                    setTile.palette == 0 &&
                    setTile.cmT == cmt && setTile.maskT == maskt && setTile.shiftT == shiftt &&
                    setTile.cmS == cms && setTile.maskS == masks && setTile.shiftS == shifts &&

                    loadBlock.tile == G_TX_TILE.G_TX_LOADTILE &&
                    loadBlock.uls.Raw == 0 && loadBlock.ult.Raw == 0 &&
                    loadBlock.texels == ((width * height + SizIncr(siz)) >> SizShift(siz)) - 1 &&
                    //(loadBlock.dxt.Raw == 0 || loadBlock.dxt.Raw == CalcDxt(width, SizBytes(siz))) &&

                    //setTile2.line == ((width * SizLineBytes(siz)) + 7) >> 3 &&

                    setTileSize.uls.Raw == 0 && setTileSize.ult.Raw == 0
                    )
                {
                    string s = loadBlock.dxt.Raw == 0 ? "S" : "";

                    if (siz == G_IM_SIZ.G_IM_SIZ_4b)
                    {
                        if ((loadBlock.dxt.Raw == 0 || loadBlock.dxt.Raw == CalcDxt_4b(width)) &&
                            setTile2.line == ((width >> 1)+7) >> 3)
                        {
                            output = (tmem == 0 && rtile == G_TX_TILE.G_TX_RENDERTILE)
                               ? $"gsDPLoadTextureBlock_4b{s}({dis.DisAddress(timg)}, {fmt}, {width}, {height}, {pal}, {DisTexWrap(cms)}, {DisTexWrap(cmt)}, {masks}, {maskt}, {shifts}, {shiftt})"
                               : $"gsDPLoadMultiBlock_4b{s}({dis.DisAddress(timg)}, 0x{tmem:X}, {rtile}, {fmt}, {width}, {height}, {pal}, {DisTexWrap(cms)}, {DisTexWrap(cmt)}, {masks}, {maskt}, {shifts}, {shiftt})";
                        }
                    }
                    else
                    {
                        if ((loadBlock.dxt.Raw == 0 || loadBlock.dxt.Raw == CalcDxt(width, SizBytes(siz))) &&
                            setTile2.line == ((width * SizLineBytes(siz)) + 7) >> 3)
                        {
                            output = (tmem == 0 && rtile == G_TX_TILE.G_TX_RENDERTILE)
                               ? $"gsDPLoadTextureBlock{s}({dis.DisAddress(timg)}, {fmt}, {siz}, {width}, {height}, {pal}, {DisTexWrap(cms)}, {DisTexWrap(cmt)}, {masks}, {maskt}, {shifts}, {shiftt})"
                               : $"gsDPLoadMultiBlock{s}({dis.DisAddress(timg)}, 0x{tmem:X}, {rtile}, {fmt}, {siz}, {width}, {height}, {pal}, {DisTexWrap(cms)}, {DisTexWrap(cmt)}, {masks}, {maskt}, {shifts}, {shiftt})";
                        }
                    }
                }

                if (output == null)
                    Debug.WriteLine("weird LoadTextureBlock detected");

                return output != null;
            }
        }
        private string DisAddress(object addr) => _cfg.AddressLiteral ? $"0x{addr:X8}" : $"D_{addr:X8}";
       
        // These 2 functions are reimps of oot's code
        private static string DisCCM(G_CCMUX value, int idx)
        {
            switch (value)
            {
                case G_CCMUX.G_CCMUX_COMBINED:
                case G_CCMUX.G_CCMUX_TEXEL0:
                case G_CCMUX.G_CCMUX_TEXEL1:
                case G_CCMUX.G_CCMUX_PRIMITIVE:
                case G_CCMUX.G_CCMUX_SHADE:
                case G_CCMUX.G_CCMUX_ENVIRONMENT:
                    return value.ToString().Replace("G_CCMUX_", "");
                case (G_CCMUX)6:
                    return (idx == 2) ? "CENTER" : (idx == 3) ? "SCALE" : "1";
                case (G_CCMUX)7:
                    return (idx == 1) ? "NOISE" : (idx == 2) ? "K4" : (idx == 3) ? "COMBINED_ALPHA" : "0";
                default:
                    {
                        if (idx == 3)
                        {
                            switch (value)
                            {
                                case G_CCMUX.G_CCMUX_TEXEL0_ALPHA:
                                case G_CCMUX.G_CCMUX_TEXEL1_ALPHA:
                                case G_CCMUX.G_CCMUX_PRIMITIVE_ALPHA:
                                case G_CCMUX.G_CCMUX_SHADE_ALPHA:
                                case G_CCMUX.G_CCMUX_ENV_ALPHA:
                                case G_CCMUX.G_CCMUX_LOD_FRACTION:
                                case G_CCMUX.G_CCMUX_PRIM_LOD_FRAC:
                                case G_CCMUX.G_CCMUX_K5:
                                    return value.ToString().Replace("G_CCMUX_", "");
                                default:
                                    return "0";
                            }
                        }
                        else return "0";
                    }
            }
        }
        private static string DisACM(G_ACMUX value, int idx)
        {
            switch (value)
            {
                case 0:
                    return (idx == 3) ? "LOD_FRACTION" : "COMBINED";

                case G_ACMUX.G_ACMUX_TEXEL0:
                case G_ACMUX.G_ACMUX_TEXEL1:
                case G_ACMUX.G_ACMUX_PRIMITIVE:
                case G_ACMUX.G_ACMUX_SHADE:
                case G_ACMUX.G_ACMUX_ENVIRONMENT:
                case G_ACMUX.G_ACMUX_0:
                    return value.ToString().Replace("G_ACMUX_", "");
                case (G_ACMUX)6:
                    return (idx == 3) ? "PRIM_LOD_FRAC" : "1";
                default:
                    return "?";
            }
        }
        private static string DisMtxParams(int v)
        {
            G_MTX_PARAM param = (G_MTX_PARAM)v;
            string push = param.HasFlag(G_MTX_PARAM.G_MTX_PUSH) ? "G_MTX_PUSH" : "G_MTX_NOPUSH";
            string load = param.HasFlag(G_MTX_PARAM.G_MTX_LOAD) ? "G_MTX_LOAD" : "G_MTX_MUL";
            string projection = param.HasFlag(G_MTX_PARAM.G_MTX_PROJECTION) ? "G_MTX_PROJECTION" : "G_MTX_MODELVIEW";
            return $"{push} | {load} | {projection}";
        }
        private static string DisTexWrap(G_TX_TEXWRAP v)
        {
            var mirror = v & G_TX_TEXWRAP.G_TX_MIRROR;
            var wrap = v & G_TX_TEXWRAP.G_TX_CLAMP;
            return $"{mirror} | {wrap}";
        }
        private static string DisTexWrap(int v)
        {
            return DisTexWrap((G_TX_TEXWRAP)v);
        }

        private string DisassembleInstruction(CmdInfo info)
        {
            switch (info.ID)
            {
                case CmdID.G_NOOP: return $"gsDPNoOpTag(0x{(uint)info.Args["tag"]:X})";
                case CmdID.G_VTX: return $"gsSPVertex({DisAddress(info.Args["vaddr"])}, {info.Args["numv"]}, {info.Args["vbidx"]})";
                case CmdID.G_MODIFYVTX: return $"gsSPModifyVertex({info.Args["where"]}, {info.Args["vbidx"]}, {info.Args["val"]})";
                case CmdID.G_CULLDL: return $"gsSPCullDisplayList({info.Args["vfirst"]}, {info.Args["vlast"]})";
                case CmdID.G_BRANCH_Z: return $"gsSPBranchLessZraw({DisAddress(_wordHi)}, {info.Args["vbidx"]}, 0x{info.Args["zval"]:X})";
                case CmdID.G_TRI1: return $"gsSP1Triangle({info.Args["v0"]}, {info.Args["v1"]}, {info.Args["v2"]}, 0)";
                case CmdID.G_TRI2: return $"gsSP2Triangles({info.Args["v00"]}, {info.Args["v01"]}, {info.Args["v02"]}, 0, {info.Args["v10"]}, {info.Args["v11"]}, {info.Args["v12"]}, 0)";
                case CmdID.G_QUAD: return $"gsSPQuadrangle({info.Args["v0"]}, {info.Args["v1"]}, {info.Args["v2"]}, {info.Args["v3"]}, 0)";
                case CmdID.G_DMA_IO: return $"gsSPDma_io(0x{info.Args["flag"]:X}, 0x{info.Args["dmem"]:X}, 0x{ info.Args["dram"]:X}, 0x{info.Args["size"]:X})";
                case CmdID.G_TEXTURE: return $"gsSPTexture(0x{info.Args["scaleS"]:X}, 0x{info.Args["scaleT"]:X}, {info.Args["level"]}, {(G_TX_TILE)info.Args["tile"]}, {info.Args["on"]})";
                case CmdID.G_POPMTX: return $"gsSPPopMatrixN(G_MTX_MODELVIEW, {info.Args["num"]})";
                case CmdID.G_GEOMETRYMODE:
                    {
                        int clearbits = (int)info.Args["clearbits"];
                        int setbits = (int)info.Args["setbits"];
                        
                        if (clearbits == 0)
                        {
                            var flag = new BitFlag<G_GEO_MODE>((G_GEO_MODE)setbits);
                            return $"gsSPLoadGeometryMode({flag})";
                        }
                        else if (setbits == 0)
                        {
                            var flag = new BitFlag<G_GEO_MODE>((G_GEO_MODE)~clearbits);
                            return $"gsSPClearGeometryMode({flag})";
                        }
                        else if (clearbits == 0xFFFFFF)
                        {
                            var flag = new BitFlag<G_GEO_MODE>((G_GEO_MODE)setbits);
                            return $"gsSPSetGeometryMode({flag})";
                        }
                        else
                        {
                            var clearFlag = new BitFlag<G_GEO_MODE>((G_GEO_MODE)~clearbits);
                            var setFlag = new BitFlag<G_GEO_MODE>((G_GEO_MODE)setbits);
                            return $"gsSPGeometryMode({clearFlag}, {setFlag})";
                        }
                    }
                case CmdID.G_MTX: return $"gsSPMatrix({DisAddress(info.Args["mtxaddr"])}, {DisMtxParams((int)info.Args["param"])})";
                case CmdID.G_MOVEWORD: break;
                case CmdID.G_MOVEMEM: break;
                case CmdID.G_LOAD_UCODE: return $"gsSPLoadUcodeEx({DisAddress(info.Args["tstart"])}, {DisAddress(_wordHi)}, 0x{info.Args["dsize"]:X})";
                case CmdID.G_DL:
                    {
                        var branch = info.GetArg<bool>("branch");
                        return branch
                            ? $"gsSPBranchList({DisAddress(info.Args["dl"])})"
                            : $"gsSPDisplayList({DisAddress(info.Args["dl"])})";
                    }
                case CmdID.G_ENDDL: return $"gsSPEndDisplayList()";
                case CmdID.G_SPNOOP: return $"gsSPNoOp()";
                case CmdID.G_RDPHALF_1:
                    {
                        _wordHi = (uint)info.Args["word"];
                        break;
                    }
                case CmdID.G_SETOTHERMODE_L:
                    {
                        var cmd = info.Convert<GSetOtherMode>();

                        var macro = OtherModeMacro.MacrosL.Find(m => m.Match(cmd));
                        if (macro != null)
                        {
                            var value = macro.values.Find(v => (uint)v.Item2 == cmd.data);
                            return $"gsDP{macro.Name}({value?.Item1??("0x" +cmd.data.ToString("X"))})";
                        }

                        return $"gsSPSetOtherMode(G_SETOTHERMODE_L, {(G_MDSFT_L)cmd.shift}, {cmd.len}, 0x{cmd.data:X})";
                    }
                case CmdID.G_SETOTHERMODE_H:
                    {
                        var cmd = info.Convert<GSetOtherMode>();

                        var macro = OtherModeMacro.MacrosH.Find(m => m.Match(cmd));
                        if (macro != null)
                        {
                            var value = macro.values.Find(v => (uint)v.Item2 == cmd.data);
                            return $"gsDP{macro.Name}({value?.Item1 ?? ("0x" + cmd.data.ToString("X"))})";
                        }
                        return $"gsSPSetOtherMode(G_SETOTHERMODE_H, {(G_MDSFT_H)cmd.shift}, {cmd.len}, 0x{cmd.data:X})";
                    }
                case CmdID.G_TEXRECT:
                    {
                        var cmd = info.Convert<GTexRect>();
                        return $"gsSPTextureRectangle({cmd.ulx}, {cmd.uly}, {cmd.lrx}, {cmd.lry}, {cmd.tile}, {cmd.uls}, {cmd.ult}, {cmd.dsdx}, {cmd.dtdy})";
                    }
                case CmdID.G_TEXRECTFLIP:
                    {
                        var cmd = info.Convert<GTexRect>();
                        return $"gsSPTextureRectangleFlip({cmd.ulx}, {cmd.uly}, {cmd.lrx}, {cmd.lry}, {cmd.tile}, {cmd.uls}, {cmd.ult}, {cmd.dsdx}, {cmd.dtdy})";
                    }
                case CmdID.G_RDPLOADSYNC: return "gsDPLoadSync()";
                case CmdID.G_RDPPIPESYNC: return "gsDPPipeSync()";
                case CmdID.G_RDPTILESYNC: return "gsDPTileSync()";
                case CmdID.G_RDPFULLSYNC: return "gsDPFullSync()";
                case CmdID.G_SETKEYGB:
                    {
                        var cmd = info.Convert<GSetKeyGB>();
                        return $"gsDPSetKeyGB({cmd.centerG}, {cmd.scaleG}, {cmd.widthG}, {cmd.centerB}, {cmd.scaleB}, {cmd.widthB})";
                    }
                case CmdID.G_SETKEYR:
                    {
                        var cmd = info.Convert<GSetKeyR>();
                        return $"gsDPSetKeyR({cmd.centerR}, {cmd.widthR}, {cmd.scaleR})";
                    }
                case CmdID.G_SETCONVERT: return $"gsDPSetConvert({info.Args["k0"]}, {info.Args["k1"]}, {info.Args["k2"]}, {info.Args["k3"]}, {info.Args["k4"]}, {info.Args["k5"]})";
                case CmdID.G_SETSCISSOR:
                    {
                        var cmd = info.Convert<GSetScissor>();
                        if (cmd.lrx.FracPart() == 0 && cmd.lry.FracPart() == 0 && cmd.ulx.FracPart() == 0 && cmd.uly.FracPart() == 0)
                            return $"gsDPSetScissor({cmd.mode}, {cmd.ulx.IntPart()}, {cmd.uly.IntPart()}, {cmd.lrx.IntPart()}, {cmd.uly.IntPart()})";
                        else
                            return $"gsDPSetScissorFrac({cmd.mode}, {cmd.ulx}, {cmd.uly}, {cmd.lrx}, {cmd.lry})";
                    }
                case CmdID.G_SETPRIMDEPTH: return $"gsDPSetPrimDepth({info.Args["z"]}, {info.Args["dz"]})";
                case CmdID.G_RDPSETOTHERMODE: return $"gsDPSetOtherMode(0x{info.Args["omodeH"]:X}, 0x{info.Args["omodeL"]:X})";
                case CmdID.G_LOADTLUT: return $"gsDPLoadTLUTCmd({info.Args["tile"]}, {info.Args["count"]})";
                case CmdID.G_RDPHALF_2:
                    {
                        _wordLo = (uint)info.Args["word"];
                        break;
                    }
                case CmdID.G_SETTILESIZE:
                    {
                        var cmd = info.Convert<GLoadTile>();
                        return $"gsDPSetTileSize({cmd.tile}, {cmd.uls}, {cmd.ult}, {cmd.lrs}, {cmd.lrt})";
                    }
                case CmdID.G_LOADBLOCK:
                    {
                        var cmd = info.Convert<GLoadBlock>();
                        return $"gsDPLoadBlock({cmd.tile}, {cmd.uls}, {cmd.ult}, {cmd.texels}, {cmd.dxt})";
                    }
                case CmdID.G_LOADTILE:
                    {
                        var cmd = info.Convert<GLoadTile>();
                        return $"gsDPLoadTile({cmd.tile}, {cmd.uls}, {cmd.ult}, {cmd.lrs}, {cmd.lrt})";
                    }
                case CmdID.G_SETTILE:
                    {
                        var cmt = DisTexWrap((int)info.Args["cmT"]);
                        var cmS = DisTexWrap((int)info.Args["cmS"]);
                        return $"gsDPSetTile({info.Args["fmt"]}, {info.Args["siz"]}, {info.Args["line"]}, 0x{info.Args["tmem"]:X}, {(G_TX_TILE)info.Args["tile"]}, {info.Args["palette"]}, {cmt}, {info.Args["maskT"]}, {info.Args["shiftT"]}, {cmS}, {info.Args["maskS"]}, {info.Args["shiftS"]})";
                    }
                case CmdID.G_FILLRECT:
                    {
                        var cmd = info.Convert<GFillRect>();
                        return $"gsDPFillRectangle({cmd.ulx}, {cmd.uly}, {cmd.lrx}, {cmd.lry})";
                    }
                case CmdID.G_SETFILLCOLOR: return $"gsDPSetFillColor(0x{info.Args["color"]:X8})";
                case CmdID.G_SETFOGCOLOR: return $"gsDPSetFogColor({info.Args["R"]}, {info.Args["G"]}, {info.Args["B"]}, {info.Args["A"]})";
                case CmdID.G_SETBLENDCOLOR: return $"gsDPBlendColor({info.Args["R"]}, {info.Args["G"]}, {info.Args["B"]}, {info.Args["A"]})";
                case CmdID.G_SETPRIMCOLOR: return $"gsDPSetPrimColor(0x{info.Args["minlevel"]:X2}, 0x{info.Args["lodfrac"]:X2}, {info.Args["R"]}, {info.Args["G"]}, {info.Args["B"]}, {info.Args["A"]})";
                case CmdID.G_SETENVCOLOR: return $"gsDPSetEnvColor({info.Args["R"]}, {info.Args["G"]}, {info.Args["B"]}, {info.Args["A"]})";
                case CmdID.G_SETCOMBINE:
                    {
                        var cmd = info.Convert<GSetCombine>();

                        var mode0 = CCMode.Modes.Find(m => m.Match(cmd, 0));
                        var mode1 = CCMode.Modes.Find(m => m.Match(cmd, 1));
                        if (mode0 != null && mode1 != null)
                            return $"gsDPSetCombineMode({mode0.Name}, {mode1.Name})";

                        return $"gsDPSetCombineLERP({DisCCM(cmd.a0, 1)}, {DisCCM(cmd.b0, 2)}, {DisCCM(cmd.c0, 3)}, {DisCCM(cmd.d0, 4)}, {DisACM(cmd.Aa0, 1)}, {DisACM(cmd.Ab0, 2)}, {DisACM(cmd.Ac0, 3)}, {DisACM(cmd.Ad0, 4)}, {DisCCM(cmd.a1, 1)}, {DisCCM(cmd.b1, 2)}, {DisCCM(cmd.c1, 3)}, {DisCCM(cmd.d1, 4)}, {DisACM(cmd.Aa1, 1)}, {DisACM(cmd.Ab1, 2)}, {DisACM(cmd.Ac1, 3)}, {DisACM(cmd.Ad1, 4)})";
                    }
                case CmdID.G_SETTIMG: return $"gsDPSetTextureImage({info.Args["fmt"]}, {info.Args["siz"]}, {info.Args["width"]}, {DisAddress(info.Args["imgaddr"])})";
                case CmdID.G_SETZIMG: return $"gsDPSetDepthImage({DisAddress(info.Args["imgaddr"])})";
                case CmdID.G_SETCIMG: return $"gsDPSetColorImage({info.Args["fmt"]}, {info.Args["siz"]}, {info.Args["width"]}, {DisAddress(info.Args["imgaddr"])})";
                default:
                    break;
            }

            return $"Unsupported Instruction {info.ID}";
        }
    }
}
