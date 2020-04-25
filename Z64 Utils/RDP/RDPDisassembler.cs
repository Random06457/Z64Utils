using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Common;

namespace RDP
{
    public class RDPDisassembler
    {
        private class CCMode
        {
            public static readonly List<CCMode> Modes = new List<CCMode>()
            {
                new CCMode("G_CC_PRIMITIVE", RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_PRIMITIVE),
                new CCMode("G_CC_SHADE", RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_MODULATEI", RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_MODULATEIA", RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_MODULATEIDECALA", RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_MODULATEI_PRIM", RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_PRIMITIVE),
                new CCMode("G_CC_MODULATEIA_PRIM", RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_PRIMITIVE, RDPEnum.G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_MODULATEIDECALA_PRIM", RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_DECALRGB", RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_DECALRGBA", RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_BLENDI", RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_BLENDIA", RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_BLENDIDECALA", RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_BLENDRGBA", RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0_ALPHA, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_BLENDRGBDECALA", RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0_ALPHA, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_ADDRGB", RDPEnum.G_CCMUX.G_CCMUX_1, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_ADDRGBDECALA", RDPEnum.G_CCMUX.G_CCMUX_1, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_REFLECTRGB", RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_REFLECTRGBDECALA", RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_HILITERGB", RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_HILITERGBA", RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_PRIMITIVE, RDPEnum.G_ACMUX.G_ACMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_HILITERGBDECALA", RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_SHADEDECALA", RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_BLENDPE", RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_BLENDPEDECALA", RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("_G_CC_BLENDPE", RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0),
                new CCMode("_G_CC_BLENDPEDECALA", RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("_G_CC_TWOCOLORTEX", RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE),
                new CCMode("_G_CC_SPARSEST", RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_LOD_FRACTION, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_PRIMITIVE, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_LOD_FRACTION, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_TEMPLERP", RDPEnum.G_CCMUX.G_CCMUX_TEXEL1, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_PRIM_LOD_FRAC, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL1, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_PRIM_LOD_FRAC, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_TRILERP", RDPEnum.G_CCMUX.G_CCMUX_TEXEL1, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_LOD_FRACTION, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL1, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_LOD_FRACTION, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_INTERFERENCE", RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_TEXEL1, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL1, RDPEnum.G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_1CYUV2RGB", RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_K4, RDPEnum.G_CCMUX.G_CCMUX_K5, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_YUV2RGB", RDPEnum.G_CCMUX.G_CCMUX_TEXEL1, RDPEnum.G_CCMUX.G_CCMUX_K4, RDPEnum.G_CCMUX.G_CCMUX_K5, RDPEnum.G_CCMUX.G_CCMUX_TEXEL1, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_PASS2", RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_COMBINED),
                new CCMode("G_CC_MODULATEI2", RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_MODULATEIA2", RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_ACMUX.G_ACMUX_COMBINED, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_MODULATEI_PRIM2", RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_PRIMITIVE),
                new CCMode("G_CC_MODULATEIA_PRIM2", RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_ACMUX.G_ACMUX_COMBINED, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_PRIMITIVE, RDPEnum.G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_DECALRGB2", RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_BLENDI2", RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_BLENDIA2", RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_CCMUX.G_CCMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_COMBINED, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE, RDPEnum.G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_CHROMA_KEY2", RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_CENTER, RDPEnum.G_CCMUX.G_CCMUX_SCALE, RDPEnum.G_CCMUX.G_CCMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0),
                new CCMode("G_CC_HILITERGB2", RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_SHADE),
                new CCMode("G_CC_HILITERGBA2", RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_ACMUX.G_ACMUX_ENVIRONMENT, RDPEnum.G_ACMUX.G_ACMUX_COMBINED, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0, RDPEnum.G_ACMUX.G_ACMUX_COMBINED),
                new CCMode("G_CC_HILITERGBDECALA2", RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_TEXEL0),
                new CCMode("G_CC_HILITERGBPASSA2", RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT, RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_CCMUX.G_CCMUX_TEXEL0, RDPEnum.G_CCMUX.G_CCMUX_COMBINED, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_0, RDPEnum.G_ACMUX.G_ACMUX_COMBINED),

            };

            public string Name;
            RDPEnum.G_CCMUX a;
            RDPEnum.G_CCMUX b;
            RDPEnum.G_CCMUX c;
            RDPEnum.G_CCMUX d;
            RDPEnum.G_ACMUX Aa;
            RDPEnum.G_ACMUX Ab;
            RDPEnum.G_ACMUX Ac;
            RDPEnum.G_ACMUX Ad;
            public CCMode(string name, RDPEnum.G_CCMUX a, RDPEnum.G_CCMUX b, RDPEnum.G_CCMUX c, RDPEnum.G_CCMUX d, RDPEnum.G_ACMUX Aa, RDPEnum.G_ACMUX Ab, RDPEnum.G_ACMUX Ac, RDPEnum.G_ACMUX Ad)
            {
                Name = name;
                this.a = a & (RDPEnum.G_CCMUX)((1 << 4) - 1);
                this.b = b & (RDPEnum.G_CCMUX)((1 << 4) - 1);
                this.c = c & (RDPEnum.G_CCMUX)((1 << 5) - 1);
                this.d = d & (RDPEnum.G_CCMUX)((1 << 3) - 1);
                this.Aa = Aa & (RDPEnum.G_ACMUX)((1 << 3) - 1);
                this.Ab = Ab & (RDPEnum.G_ACMUX)((1 << 3) - 1);
                this.Ac = Ac & (RDPEnum.G_ACMUX)((1 << 3) - 1);
                this.Ad = Ad & (RDPEnum.G_ACMUX)((1 << 3) - 1);
            }

            public bool Match(F3DZEX.GSetCombine cmd, int cycle)
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
                new OtherModeMacro("SetAlphaCompare", (int)RDPEnum.G_MDSFT_L.G_MDSFT_ALPHACOMPARE, 2, "G_AC_NONE", 0, "G_AC_THRESHOLD", 1<<0, "G_AC_DITHER", 3<<0),
                new OtherModeMacro("SetDepthSource", (int)RDPEnum.G_MDSFT_L.G_MDSFT_ZSRCSEL, 1, "G_ZS_PIXEL", 0, "G_ZS_PRIM", 1<<2),
            };
            public static readonly List<OtherModeMacro> MacrosH = new List<OtherModeMacro>()
            {
                new OtherModeMacro("SetAlphaDither", (int)RDPEnum.G_MDSFT_H.G_MDSFT_ALPHADITHER, 2, "G_AD_PATTERN", 0, "G_AD_NOTPATTERN", 0x10, "G_AD_NOISE", 0x20, "G_AD_DISABLE", 0x30),
                new OtherModeMacro("SetColorDither", (int)RDPEnum.G_MDSFT_H.G_MDSFT_RGBDITHER, 2, "G_CD_MAGICSQ", 0, "G_CD_BAYER", 0x40, "G_CD_NOISE", 0x80), // HW version 1
                new OtherModeMacro("SetCombineKey", (int)RDPEnum.G_MDSFT_H.G_MDSFT_COMBKEY, 1, "G_CK_NONE", 0, "G_CK_KEY", 1<<8),
                new OtherModeMacro("SetTextureConvert", (int)RDPEnum.G_MDSFT_H.G_MDSFT_TEXTCONV, 3, "G_TC_CONV", 0, "G_TC_FILTCONV", 5<<9, "G_TC_FILT", 6<<9),
                new OtherModeMacro("SetTextureFilter", (int)RDPEnum.G_MDSFT_H.G_MDSFT_TEXTFILT, 2, "G_TF_POINT", 0, "G_TF_AVERAGE", 3<<12, "G_TF_BILERP", 2<<12),
                new OtherModeMacro("SetTextureLUT", (int)RDPEnum.G_MDSFT_H.G_MDSFT_TEXTLUT, 2, "G_TT_NONE", 0, "G_TT_RGBA16", 2<<14, "G_TT_IA16", 3<<14),
                new OtherModeMacro("SetTextureLOD", (int)RDPEnum.G_MDSFT_H.G_MDSFT_TEXTLOD, 1, "G_TL_TILE", 0, "G_TL_LOD", 1<<16),
                new OtherModeMacro("SetTextureDetail", (int)RDPEnum.G_MDSFT_H.G_MDSFT_TEXTDETAIL, 2, "G_TD_CLAMP", 0, "G_TD_SHARPEN", 1<<17, "G_TD_DETAIL", 2<<17),
                new OtherModeMacro("SetTexturePersp", (int)RDPEnum.G_MDSFT_H.G_MDSFT_TEXTPERSP, 1, "G_TP_PERSP", 1<<19, "G_TP_NONE", 0<<19),
                new OtherModeMacro("SetCycleType", (int)RDPEnum.G_MDSFT_H.G_MDSFT_CYCLETYPE, 2, "G_CYC_1CYCLE", 0, "G_CYC_2CYCLE", 1<<20, "G_CYC_COPY", 2<<20, "G_CYC_FILL", 3<<20),
                new OtherModeMacro("SetCycleType", (int)RDPEnum.G_MDSFT_H.G_MDSFT_CYCLETYPE, 2, "G_CYC_1CYCLE", 0, "G_CYC_2CYCLE", 1<<20, "G_CYC_COPY", 2<<20, "G_CYC_FILL", 3<<20),
                new OtherModeMacro("SetColorDither", (int)RDPEnum.G_MDSFT_H.G_MDSFT_COLORDITHER, 2, "G_CD_MAGICSQ", 0, "G_CD_BAYER", 0x40, "G_CD_NOISE", 0x80),
                new OtherModeMacro("PipelineMode", (int)RDPEnum.G_MDSFT_H.G_MDSFT_PIPELINE, 1, "G_PM_1PRIMITIVE", 1<<23, "G_PM_NPRIMITIVE", 0),
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

            public bool Match(F3DZEX.GSetOtherMode cmd)
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

        public static Config Configuration { get; private set; } = new Config()
        {
            ShowAddress = true,
            RelativeAddress = false,
            DisasMultiCmdMacro = true,
            AddressLiteral = false,
            Static = true,
        };
        
        uint _vaddr;
        uint _wordLo;
        uint _wordHi;
        List<F3DZEX.CommandInfo> _dlist = new List<F3DZEX.CommandInfo>();


        public RDPDisassembler(byte[] ucode, uint vaddr) : this(F3DZEX.DecodeDList(ucode), vaddr)
        {
        }
        public RDPDisassembler(List<F3DZEX.CommandInfo> dlist, uint vaddr)
        {
            _dlist = dlist;
            _vaddr = vaddr;
        }

        public List<string> Disassemble()
        {
            List<string> lines = new List<string>();
            _wordLo = _wordHi = 0;

            int off = 0;
            for (int i = 0; i < _dlist.Count; i++)
            {
                var ins = _dlist[i];

                uint addr = (Configuration.RelativeAddress) ? (uint)off : (uint)(_vaddr + off);
                string prefix = (Configuration.ShowAddress ? $"{addr:X8}: " : "");

                string dis = DisassembleInstruction(ins);
                if (Configuration.DisasMultiCmdMacro)
                {
                    string macroDis = FindMultiCmdMacro(i, out int cmdCount);
                    if (cmdCount > 0)
                    {
                        dis = macroDis;
                        i += cmdCount-1;
                        off += (cmdCount*8)-8;
                        lines.Add($"/* Multi Command Macro Found ({cmdCount} instructions) */");
                    }
                }

                if (!Configuration.Static)
                {
                    dis = dis.Remove(1, 1);
                    dis = dis.Insert(dis.IndexOf('(')+1, "gfx++, ");
                    dis = dis.Replace("(gfx++, )", "(gfx++)");
                }
                dis += Configuration.Static ? "," : ";";


                lines.Add(prefix + dis);

                off += ins.GetSize();
            }
            return lines;
        }

        private string FindMultiCmdMacro(int index, out int cmdCount)
        {
            foreach (var entry in MultiCmdMacros)
            {
                var ret = entry(this, index);
                if (ret.Item1 > 0)
                {
                    cmdCount = ret.Item1;
                    return ret.Item2;
                }
            }
            cmdCount = 0;
            return null;
        }

        // TODO : fix this
        private static readonly List<Func<RDPDisassembler, int, Tuple<int, string>>> MultiCmdMacros = new List<Func<RDPDisassembler, int, Tuple<int, string>>>()
        {
            //gsDPLoadTLUT(count, tmemaddr, dram)
            (dis, idx) =>
            {
                int cmdCount = 6;
                if (idx + cmdCount >= dis._dlist.Count)
                    return new Tuple<int, string>(0, null);
                //copy the instructions
                List<F3DZEX.CommandInfo> cmds = new List<F3DZEX.CommandInfo>();
                for (int i = 0; i < cmdCount; i++)
                    cmds.Add(dis._dlist[idx+i]);

                uint dram = 0;
                int tmemaddr = 0;
                int count = 0;

                if (cmds[0].ID == F3DZEX.OpCodeID.G_SETTIMG &&
                    (RDPEnum.G_IM_FMT)cmds[0].Args["fmt"] == RDPEnum.G_IM_FMT.G_IM_FMT_RGBA &&
                    (int)cmds[0].Args["width"] == 1
                    )
                    dram = (uint)cmds[0].Args["imgaddr"];
                else return new Tuple<int, string>(0, null);

                if (cmds[1].ID != F3DZEX.OpCodeID.G_RDPTILESYNC)
                    return new Tuple<int, string>(0, null);

                if (cmds[2].ID == F3DZEX.OpCodeID.G_SETTILE)
                    tmemaddr = (int)cmds[2].Args["tmem"];
                else return new Tuple<int, string>(0, null);

                if (cmds[3].ID != F3DZEX.OpCodeID.G_RDPLOADSYNC)
                    return new Tuple<int, string>(0, null);

                if (cmds[4].ID == F3DZEX.OpCodeID.G_LOADTLUT)
                    count = (int)cmds[4].Args["count"];
                else return new Tuple<int, string>(0, null);

                if (cmds[5].ID != F3DZEX.OpCodeID.G_RDPPIPESYNC)
                    return new Tuple<int, string>(0, null);

                return new Tuple<int, string>(cmdCount, $"gsDPLoadTLUT({count+1}, 0x{tmemaddr:X}, {dis.DisAddress(dram)})");
            },

            //gsDPLoadTextureBlock(timg, fmt, siz, width, height, pal, cms, cmt, masks, maskt, shifts, shiftt)
            (dis, idx) =>
            {
                int cmdCount = 7;
                if (idx + cmdCount >= dis._dlist.Count)
                    return new Tuple<int, string>(0, null);
                //copy the instructions
                List<F3DZEX.CommandInfo> cmds = new List<F3DZEX.CommandInfo>();
                for (int i = 0; i < cmdCount; i++)
                    cmds.Add(dis._dlist[idx+i]);

                uint timg;
                RDPEnum.G_IM_FMT fmt;
                RDPEnum.G_IM_SIZ siz;
                int width;
                int height;
                int pal;
                string cms;
                string cmt;
                int masks;
                int maskt;
                int shifts;
                int shiftt;

                var info = cmds[0];
                if (info.ID == F3DZEX.OpCodeID.G_SETTIMG &&
                    (int)info.Args["width"] == 1
                    )
                {
                    timg = (uint)info.Args["imgaddr"];
                }
                else return new Tuple<int, string>(0, null);

                info = cmds[1];
                if (info.ID == F3DZEX.OpCodeID.G_SETTILE)
                {
                    cmt = RDPEnum.ParseMirrorClamFlag((int)info.Args["cmT"]);
                    cms = RDPEnum.ParseMirrorClamFlag((int)info.Args["cmS"]);
                    maskt = (int)info.Args["maskT"];
                    masks = (int)info.Args["maskS"];
                    shiftt = (int)info.Args["shiftT"];
                    shifts = (int)info.Args["shiftS"];
                }
                else return new Tuple<int, string>(0, null);

                info = cmds[2];
                if (info.ID != F3DZEX.OpCodeID.G_RDPLOADSYNC)
                    return new Tuple<int, string>(0, null);

                info = cmds[3];
                if (info.ID != F3DZEX.OpCodeID.G_LOADBLOCK)
                    return new Tuple<int, string>(0, null);

                info = cmds[4];
                if (info.ID != F3DZEX.OpCodeID.G_RDPPIPESYNC)
                    return new Tuple<int, string>(0, null);

                info = cmds[5];
                if (info.ID == F3DZEX.OpCodeID.G_SETTILE)
                {
                    fmt = (RDPEnum.G_IM_FMT)info.Args["fmt"];
                    siz = (RDPEnum.G_IM_SIZ)info.Args["siz"];
                    pal = (int)info.Args["palette"];
                }
                else return new Tuple<int, string>(0, null);

                info = cmds[6];
                if (info.ID == F3DZEX.OpCodeID.G_SETTILESIZE)
                {
                    width = (int)((FixedPoint)info.Args["lrs"]).IntPart()+1;
                    height = (int)((FixedPoint)info.Args["lrt"]).IntPart()+1;
                }
                else return new Tuple<int, string>(0, null);

                return new Tuple<int, string>(cmdCount, $"gsDPLoadTextureBlock({dis.DisAddress(timg)}, {fmt}, {siz}, {width}, {height}, {pal}, {cms}, {cmt}, {masks}, {maskt}, {shifts}, {shiftt})");
            }
        };

        private string DisAddress(object addr) => Configuration.AddressLiteral ? $"0x{addr:X8}" : $"D_{addr:X8}";
       
        // These 2 functions are reimps of oot's code
        private string DisCCM(RDPEnum.G_CCMUX value, int idx)
        {
            switch (value)
            {
                case RDPEnum.G_CCMUX.G_CCMUX_COMBINED:
                case RDPEnum.G_CCMUX.G_CCMUX_TEXEL0:
                case RDPEnum.G_CCMUX.G_CCMUX_TEXEL1:
                case RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE:
                case RDPEnum.G_CCMUX.G_CCMUX_SHADE:
                case RDPEnum.G_CCMUX.G_CCMUX_ENVIRONMENT:
                    return value.ToString().Replace("G_CCMUX_", "");
                case (RDPEnum.G_CCMUX)6:
                    return (idx == 2) ? "CENTER" : (idx == 3) ? "SCALE" : "1";
                case (RDPEnum.G_CCMUX)7:
                    return (idx == 1) ? "NOISE" : (idx == 2) ? "K4" : (idx == 3) ? "COMBINED_ALPHA" : "0";
                default:
                    {
                        if (idx == 3)
                        {
                            switch (value)
                            {
                                case RDPEnum.G_CCMUX.G_CCMUX_TEXEL0_ALPHA:
                                case RDPEnum.G_CCMUX.G_CCMUX_TEXEL1_ALPHA:
                                case RDPEnum.G_CCMUX.G_CCMUX_PRIMITIVE_ALPHA:
                                case RDPEnum.G_CCMUX.G_CCMUX_SHADE_ALPHA:
                                case RDPEnum.G_CCMUX.G_CCMUX_ENV_ALPHA:
                                case RDPEnum.G_CCMUX.G_CCMUX_LOD_FRACTION:
                                case RDPEnum.G_CCMUX.G_CCMUX_PRIM_LOD_FRAC:
                                case RDPEnum.G_CCMUX.G_CCMUX_K5:
                                    return value.ToString().Replace("G_CCMUX_", "");
                                default:
                                    return "0";
                            }
                        }
                        else return "0";
                    }
            }
        }
        private string DisACM(RDPEnum.G_ACMUX value, int idx)
        {
            switch (value)
            {
                case 0:
                    return (idx == 3) ? "LOD_FRACTION" : "COMBINED";

                case RDPEnum.G_ACMUX.G_ACMUX_TEXEL0:
                case RDPEnum.G_ACMUX.G_ACMUX_TEXEL1:
                case RDPEnum.G_ACMUX.G_ACMUX_PRIMITIVE:
                case RDPEnum.G_ACMUX.G_ACMUX_SHADE:
                case RDPEnum.G_ACMUX.G_ACMUX_ENVIRONMENT:
                case RDPEnum.G_ACMUX.G_ACMUX_0:
                    return value.ToString().Replace("G_ACMUX_", "");
                case (RDPEnum.G_ACMUX)6:
                    return (idx == 3) ? "PRIM_LOD_FRAC" : "1";
                default:
                    return "?";
            }
        }

        private string DisassembleInstruction(F3DZEX.CommandInfo info)
        {
            switch (info.ID)
            {
                case F3DZEX.OpCodeID.G_NOOP: return $"gsDPNoOpTag(0x{(uint)info.Args["tag"]:X})";
                case F3DZEX.OpCodeID.G_VTX: return $"gsSPVertex({DisAddress(info.Args["vaddr"])}, {info.Args["numv"]}, {info.Args["vbidx"]})";
                case F3DZEX.OpCodeID.G_MODIFYVTX: return $"gsSPModifyVertex({info.Args["where"]}, {info.Args["vbidx"]}, {info.Args["val"]})";
                case F3DZEX.OpCodeID.G_CULLDL: return $"gsSPCullDisplayList({info.Args["vfirst"]}, {info.Args["vlast"]})";
                case F3DZEX.OpCodeID.G_BRANCH_Z: return $"gsSPBranchLessZraw({DisAddress(info.Args["newdl"])}, {info.Args["vbidx"]}, 0x{info.Args["zval"]:X})";
                case F3DZEX.OpCodeID.G_TRI1: return $"gsSP1Triangle({info.Args["v0"]}, {info.Args["v1"]}, {info.Args["v2"]}, 0)";
                case F3DZEX.OpCodeID.G_TRI2: return $"gsSP2Triangles({info.Args["v00"]}, {info.Args["v01"]}, {info.Args["v02"]}, 0, {info.Args["v10"]}, {info.Args["v11"]}, {info.Args["v12"]}, 0)";
                case F3DZEX.OpCodeID.G_QUAD: return $"gsSPQuadrangle({info.Args["v0"]}, {info.Args["v1"]}, {info.Args["v2"]}, {info.Args["v3"]}, 0)";
                case F3DZEX.OpCodeID.G_DMA_IO: return $"gsSPDma_io({info.Args["flag"]}, 0x{info.Args["dmem"]:X}, 0x{ info.Args["dram"]:X}, 0x{info.Args["size"]:X})";
                case F3DZEX.OpCodeID.G_TEXTURE: return $"gsSPTexture(0x{info.Args["scaleS"]:X}, 0x{info.Args["scaleT"]:X}, {info.Args["level"]}, {(RDPEnum.G_TX_tile)info.Args["tile"]}, {info.Args["on"]})";
                case F3DZEX.OpCodeID.G_POPMTX: return $"gsSPPopMatrixN(G_MTX_MODELVIEW, {info.Args["num"]})";
                case F3DZEX.OpCodeID.G_GEOMETRYMODE:
                    {
                        int clearbits = (int)info.Args["clearbits"];
                        int setbits = (int)info.Args["setbits"];
                        
                        if (clearbits == 0)
                        {
                            var flag = new BitFlag<RDPEnum.GeometryMode>((RDPEnum.GeometryMode)setbits);
                            return $"gsSPLoadGeometryMode({flag})";
                        }
                        else if (setbits == 0)
                        {
                            var flag = new BitFlag<RDPEnum.GeometryMode>((RDPEnum.GeometryMode)~clearbits);
                            return $"gsSPClearGeometryMode({flag})";
                        }
                        else if (clearbits == 0xFFFFFF)
                        {
                            var flag = new BitFlag<RDPEnum.GeometryMode>((RDPEnum.GeometryMode)setbits);
                            return $"gsSPSetGeometryMode({flag})";
                        }
                        else
                        {
                            var clearFlag = new BitFlag<RDPEnum.GeometryMode>((RDPEnum.GeometryMode)~clearbits);
                            var setFlag = new BitFlag<RDPEnum.GeometryMode>((RDPEnum.GeometryMode)setbits);
                            return $"gsSPGeometryMode({clearFlag}, {setFlag})";
                        }
                    }
                case F3DZEX.OpCodeID.G_MTX: return $"gsSPMatrix({DisAddress(info.Args["mtxaddr"])}, { info.Args["param"]})";
                case F3DZEX.OpCodeID.G_MOVEWORD: break;
                case F3DZEX.OpCodeID.G_MOVEMEM: break;
                case F3DZEX.OpCodeID.G_LOAD_UCODE: return $"gsSPLoadUcodeEx({DisAddress(info.Args["tstart"])}, {DisAddress(_wordHi)}, 0x{info.Args["dsize"]:X})";
                case F3DZEX.OpCodeID.G_DL:
                    {
                        var branch = info.GetArg<bool>("branch");
                        return branch
                            ? $"gsSPBranchList({DisAddress(info.Args["dl"])})"
                            : $"gsSPDisplayList({DisAddress(info.Args["dl"])})";
                    }
                case F3DZEX.OpCodeID.G_ENDDL: return $"gsSPEndDisplayList()";
                case F3DZEX.OpCodeID.G_SPNOOP: return $"gsSPNoOp()";
                case F3DZEX.OpCodeID.G_RDPHALF_1:
                    {
                        _wordHi = (uint)info.Args["word"];
                        break;
                    }
                case F3DZEX.OpCodeID.G_SETOTHERMODE_L:
                    {
                        var cmd = info.Convert<F3DZEX.GSetOtherMode>();

                        var macro = OtherModeMacro.MacrosL.Find(m => m.Match(cmd));
                        if (macro != null)
                        {
                            var value = macro.values.Find(v => (uint)v.Item2 == cmd.data);
                            return $"gsDP{macro.Name}({value?.Item1??("0x" +cmd.data.ToString("X"))})";
                        }

                        return $"gsSPSetOtherMode(G_SETOTHERMODE_L, {(RDPEnum.G_MDSFT_L)cmd.shift}, {cmd.len}, 0x{cmd.data:X})";
                    }
                case F3DZEX.OpCodeID.G_SETOTHERMODE_H:
                    {
                        var cmd = info.Convert<F3DZEX.GSetOtherMode>();

                        var macro = OtherModeMacro.MacrosH.Find(m => m.Match(cmd));
                        if (macro != null)
                        {
                            var value = macro.values.Find(v => (uint)v.Item2 == cmd.data);
                            return $"gsDP{macro.Name}({value?.Item1 ?? ("0x" + cmd.data.ToString("X"))})";
                        }
                        return $"gsSPSetOtherMode(G_SETOTHERMODE_H, {(RDPEnum.G_MDSFT_H)cmd.shift}, {cmd.len}, 0x{cmd.data:X})";
                    }
                case F3DZEX.OpCodeID.G_TEXRECT:
                    {
                        var cmd = info.Convert<F3DZEX.GTexRect>();
                        return $"gsSPTextureRectangle({cmd.ulx}, {cmd.uly}, {cmd.lrx}, {cmd.lry}, {cmd.tile}, {cmd.uls}, {cmd.ult}, {cmd.dsdx}, {cmd.dtdy})";
                    }
                case F3DZEX.OpCodeID.G_TEXRECTFLIP:
                    {
                        var cmd = info.Convert<F3DZEX.GTexRect>();
                        return $"gsSPTextureRectangleFlip({cmd.ulx}, {cmd.uly}, {cmd.lrx}, {cmd.lry}, {cmd.tile}, {cmd.uls}, {cmd.ult}, {cmd.dsdx}, {cmd.dtdy})";
                    }
                case F3DZEX.OpCodeID.G_RDPLOADSYNC: return "gsDPLoadSync()";
                case F3DZEX.OpCodeID.G_RDPPIPESYNC: return "gsDPPipeSync()";
                case F3DZEX.OpCodeID.G_RDPTILESYNC: return "gsDPTileSync()";
                case F3DZEX.OpCodeID.G_RDPFULLSYNC: return "gsDPFullSync()";
                case F3DZEX.OpCodeID.G_SETKEYGB:
                    {
                        var cmd = info.Convert<F3DZEX.GSetKeyGB>();
                        return $"gsDPSetKeyGB({cmd.centerG}, {cmd.scaleG}, {cmd.widthG}, {cmd.centerB}, {cmd.scaleB}, {cmd.widthB})";
                    }
                case F3DZEX.OpCodeID.G_SETKEYR:
                    {
                        var cmd = info.Convert<F3DZEX.GSetKeyR>();
                        return $"gsDPSetKeyR({cmd.centerR}, {cmd.widthR}, {cmd.scaleR})";
                    }
                case F3DZEX.OpCodeID.G_SETCONVERT: return $"gsDPSetConvert({info.Args["k0"]}, {info.Args["k1"]}, {info.Args["k2"]}, {info.Args["k3"]}, {info.Args["k4"]}, {info.Args["k5"]})";
                case F3DZEX.OpCodeID.G_SETSCISSOR:
                    {
                        var cmd = info.Convert<F3DZEX.GSetScissor>();
                        if (cmd.lrx.FracPart() == 0 && cmd.lry.FracPart() == 0 && cmd.ulx.FracPart() == 0 && cmd.uly.FracPart() == 0)
                            return $"gsDPSetScissor({cmd.mode}, {cmd.ulx.IntPart()}, {cmd.uly.IntPart()}, {cmd.lrx.IntPart()}, {cmd.uly.IntPart()})";
                        else
                            return $"gsDPSetScissorFrac({cmd.mode}, {cmd.ulx}, {cmd.uly}, {cmd.lrx}, {cmd.lry})";
                    }
                case F3DZEX.OpCodeID.G_SETPRIMDEPTH: return $"gsDPSetPrimDepth({info.Args["z"]}, {info.Args["dz"]})";
                case F3DZEX.OpCodeID.G_RDPSETOTHERMODE: return $"gsDPSetOtherMode(0x{info.Args["omodeH"]:X}, 0x{info.Args["omodeL"]:X})";
                case F3DZEX.OpCodeID.G_LOADTLUT: return $"gsDPLoadTLUTCmd({info.Args["tile"]}, {info.Args["count"]})";
                case F3DZEX.OpCodeID.G_RDPHALF_2:
                    {
                        _wordLo = (uint)info.Args["word"];
                        break;
                    }
                case F3DZEX.OpCodeID.G_SETTILESIZE:
                    {
                        var cmd = info.Convert<F3DZEX.GLoadTile>();
                        return $"gsDPSetTileSize({cmd.tile}, {cmd.uls}, {cmd.ult}, {cmd.lrs}, {cmd.lrt})";
                    }
                case F3DZEX.OpCodeID.G_LOADBLOCK:
                    {
                        var cmd = info.Convert<F3DZEX.GLoadBlock>();
                        return $"gsDPLoadBlock({cmd.tile}, {cmd.uls}, {cmd.ult}, {cmd.texels}, {cmd.dxt})";
                    }
                case F3DZEX.OpCodeID.G_LOADTILE:
                    {
                        var cmd = info.Convert<F3DZEX.GLoadTile>();
                        return $"gsDPLoadTile({cmd.tile}, {cmd.uls}, {cmd.ult}, {cmd.lrs}, {cmd.lrt})";
                    }
                case F3DZEX.OpCodeID.G_SETTILE:
                    {
                        var cmt = RDPEnum.ParseMirrorClamFlag((int)info.Args["cmT"]);
                        var cmS = RDPEnum.ParseMirrorClamFlag((int)info.Args["cmS"]);
                        return $"gsDPSetTile({info.Args["fmt"]}, {info.Args["siz"]}, {info.Args["line"]}, 0x{info.Args["tmem"]:X}, {(RDPEnum.G_TX_tile)info.Args["tile"]}, {info.Args["palette"]}, {cmt}, {info.Args["maskT"]}, {info.Args["shiftT"]}, {cmS}, {info.Args["maskS"]}, {info.Args["shiftS"]})";
                    }
                case F3DZEX.OpCodeID.G_FILLRECT:
                    {
                        var cmd = info.Convert<F3DZEX.GFillRect>();
                        return $"gsDPFillRectangle({cmd.ulx}, {cmd.uly}, {cmd.lrx}, {cmd.lry})";
                    }
                case F3DZEX.OpCodeID.G_SETFILLCOLOR: return $"gsDPSetFillColor(0x{info.Args["color"]:X8})";
                case F3DZEX.OpCodeID.G_SETFOGCOLOR: return $"gsDPSetFogColor({info.Args["R"]}, {info.Args["G"]}, {info.Args["B"]}, {info.Args["A"]})";
                case F3DZEX.OpCodeID.G_SETBLENDCOLOR: return $"gsDPBlendColor({info.Args["R"]}, {info.Args["G"]}, {info.Args["B"]}, {info.Args["A"]})";
                case F3DZEX.OpCodeID.G_SETPRIMCOLOR: return $"gsDPSetPrimColor(0x{info.Args["minlevel"]:X2}, 0x{info.Args["lodfrac"]:X2}, {info.Args["R"]}, {info.Args["G"]}, {info.Args["B"]}, {info.Args["A"]})";
                case F3DZEX.OpCodeID.G_SETENVCOLOR: return $"gsDPSetEnvColor({info.Args["R"]}, {info.Args["G"]}, {info.Args["B"]}, {info.Args["A"]})";
                case F3DZEX.OpCodeID.G_SETCOMBINE:
                    {
                        var cmd = info.Convert<F3DZEX.GSetCombine>();

                        var mode0 = CCMode.Modes.Find(m => m.Match(cmd, 0));
                        var mode1 = CCMode.Modes.Find(m => m.Match(cmd, 1));
                        if (mode0 != null && mode1 != null)
                            return $"gsDPSetCombineMode({mode0.Name}, {mode1.Name})";

                        return $"gsDPSetCombineLERP({DisCCM(cmd.a0, 1)}, {DisCCM(cmd.b0, 2)}, {DisCCM(cmd.c0, 3)}, {DisCCM(cmd.d0, 4)}, {DisACM(cmd.Aa0, 1)}, {DisACM(cmd.Ab0, 2)}, {DisACM(cmd.Ac0, 3)}, {DisACM(cmd.Ad0, 4)}, {DisCCM(cmd.a1, 1)}, {DisCCM(cmd.b1, 2)}, {DisCCM(cmd.c1, 3)}, {DisCCM(cmd.d1, 4)}, {DisACM(cmd.Aa1, 1)}, {DisACM(cmd.Ab1, 2)}, {DisACM(cmd.Ac1, 3)}, {DisACM(cmd.Ad1, 4)})";
                    }
                case F3DZEX.OpCodeID.G_SETTIMG: return $"gsDPSetTextureImage({info.Args["fmt"]}, {info.Args["siz"]}, {info.Args["width"]}, {DisAddress(info.Args["imgaddr"])})";
                case F3DZEX.OpCodeID.G_SETZIMG: return $"gsDPSetDepthImage({DisAddress(info.Args["imgaddr"])})";
                case F3DZEX.OpCodeID.G_SETCIMG: return $"gsDPSetColorImage({info.Args["fmt"]}, {info.Args["siz"]}, {info.Args["width"]}, {DisAddress(info.Args["imgaddr"])})";
                default:
                    break;
            }

            return $"Unsupported Instruction {info.ID}";
        }
    }
}
