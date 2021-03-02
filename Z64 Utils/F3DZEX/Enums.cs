﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F3DZEX
{
    public static class Enums
    {
        public enum G_MtxParams
        {
            G_MTX_NOPUSH = 0x00,
            G_MTX_PUSH = 0x01,
            G_MTX_MUL = 0x00,
            G_MTX_LOAD = 0x02,
            G_MTX_MODELVIEW = 0x00,
            G_MTX_PROJECTION = 0x04,
        }
        public enum G_TexOnOff
        {
            G_OFF,
            G_ON,
        }
        public enum G_CCMUX
        {
            G_CCMUX_COMBINED = 0,
            G_CCMUX_TEXEL0 = 1,
            G_CCMUX_TEXEL1 = 2,
            G_CCMUX_PRIMITIVE = 3,
            G_CCMUX_SHADE = 4,
            G_CCMUX_ENVIRONMENT = 5,
            G_CCMUX_CENTER = 6,
            G_CCMUX_SCALE = 6,
            G_CCMUX_COMBINED_ALPHA = 7,
            G_CCMUX_TEXEL0_ALPHA = 8,
            G_CCMUX_TEXEL1_ALPHA = 9,
            G_CCMUX_PRIMITIVE_ALPHA = 10,
            G_CCMUX_SHADE_ALPHA = 11,
            G_CCMUX_ENV_ALPHA = 12,
            G_CCMUX_LOD_FRACTION = 13,
            G_CCMUX_PRIM_LOD_FRAC = 14,
            G_CCMUX_NOISE = 7,
            G_CCMUX_K4 = 7,
            G_CCMUX_K5 = 15,
            G_CCMUX_1 = 6,
            G_CCMUX_0 = 31,
        }
        public enum G_ACMUX
        {
            G_ACMUX_COMBINED = 0,
            G_ACMUX_TEXEL0 = 1,
            G_ACMUX_TEXEL1 = 2,
            G_ACMUX_PRIMITIVE = 3,
            G_ACMUX_SHADE = 4,
            G_ACMUX_ENVIRONMENT = 5,
            G_ACMUX_LOD_FRACTION = 0,
            G_ACMUX_PRIM_LOD_FRAC = 6,
            G_ACMUX_1 = 6,
            G_ACMUX_0 = 7,
        }

        public enum G_MDSFT_L
        {
            G_MDSFT_ALPHACOMPARE = 0,
            G_MDSFT_ZSRCSEL = 2,
            G_MDSFT_RENDERMODE = 3,
            G_MDSFT_BLENDER = 16
        }
        public enum G_MDSFT_H
        {
            G_MDSFT_BLENDMASK = 0,
            G_MDSFT_ALPHADITHER = 4,
            G_MDSFT_RGBDITHER = 6,

            G_MDSFT_COMBKEY = 8,
            G_MDSFT_TEXTCONV = 9,
            G_MDSFT_TEXTFILT = 12,
            G_MDSFT_TEXTLUT = 14,
            G_MDSFT_TEXTLOD = 16,
            G_MDSFT_TEXTDETAIL = 17,
            G_MDSFT_TEXTPERSP = 19,
            G_MDSFT_CYCLETYPE = 20,
            G_MDSFT_COLORDITHER = 22,
            G_MDSFT_PIPELINE = 23
        }

        public enum G_IM_FMT
        {
            G_IM_FMT_RGBA = 0,
            G_IM_FMT_YUV = 1,
            G_IM_FMT_CI = 2,
            G_IM_FMT_IA = 3,
            G_IM_FMT_I = 4,
        }
        public enum G_IM_SIZ
        {
            G_IM_SIZ_4b = 0,
            G_IM_SIZ_8b = 1,
            G_IM_SIZ_16b = 2,
            G_IM_SIZ_32b = 3,
        }

        public enum ClampMirrorFlag
        {
            G_TX_NOMIRROR = 0,
            G_TX_MIRROR = 1,

            G_TX_WRAP = 0,
            G_TX_CLAMP = 2,
        }

        public enum G_TX_tile
        {
            G_TX_RENDERTILE = 0,
            G_TX_LOADTILE = 7,
        }

        public enum GeometryMode
        {
            G_ZBUFFER = 0x00000001,
            G_TEXTURE_ENABLE = 0x00000000,
            G_SHADE = 0x00000004,
            G_SHADING_SMOOTH = 0x00200000,
            G_CULL_FRONT = 0x00000200,
            G_CULL_BACK = 0x00000400,
            G_FOG = 0x00010000,
            G_LIGHTING = 0x00020000,
            G_TEXTURE_GEN = 0x00040000,
            G_TEXTURE_GEN_LINEAR = 0x00080000,
            G_LOD = 0x00100000
        }

        public static string ParseMtxParam(int v)
        {
            G_MtxParams param =(G_MtxParams)v;
            string push = param.HasFlag(G_MtxParams.G_MTX_PUSH) ? "G_MTX_PUSH" : "G_MTX_NOPUSH";
            string load = param.HasFlag(G_MtxParams.G_MTX_LOAD) ? "G_MTX_LOAD" : "G_MTX_MUL";
            string projection = param.HasFlag(G_MtxParams.G_MTX_PROJECTION) ? "G_MTX_PROJECTION" : "G_MTX_MODELVIEW";
            return $"{push} | {load} | {projection}";
        }

        public static string ParseMirrorClamFlag(int v)
        {
            var mirror = (ClampMirrorFlag)v & ClampMirrorFlag.G_TX_MIRROR;
            var wrap = (ClampMirrorFlag)v & ClampMirrorFlag.G_TX_CLAMP;
            return $"{mirror} | {wrap}";
        }

    }
}