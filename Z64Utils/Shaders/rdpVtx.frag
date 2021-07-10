#version 330 core

in vec2 v_VtxTexCoords;
in vec3 v_VtxNormal;
in vec4 v_VtxColor;
flat in int v_VtxId;

uniform vec3 u_DirLight;
uniform sampler2D u_Tex0;
uniform sampler2D u_Tex1;

uniform vec4 u_PrimColor;
uniform float u_PrimLod;
uniform vec4 u_BlendColor;
uniform vec4 u_EnvColor;
uniform vec4 u_FogColor;

uniform uint u_OtherModeHi;
uniform uint u_OtherModeLo;
uniform uint u_GeoMode;

uniform ivec4 u_CombinerC1;
uniform ivec4 u_CombinerA1;
uniform ivec4 u_CombinerC2;
uniform ivec4 u_CombinerA2;

uniform vec3 u_ChromaKeyCenter;
uniform vec3 u_ChromaKeyScale;

uniform vec4 u_HighlightColor;
uniform bool u_HighlightEnabled;
uniform vec4 u_WireFrameColor;
uniform int u_ModelRenderMode;
uniform bool u_LigthingEnabled;

out vec4 FragColor;

// Render Mode
#define MODE_WIREFRAME      0
#define MODE_TEXTURED       1
#define MODE_SURFACE        2
#define MODE_NORMAL         3


// Geometry Mode
#define G_SHADE     4u
#define G_FOG       0x10000u
#define G_LIGHTING  0x20000u

// Color Combiner 
#define G_CCMUX_COMBINED 0
#define G_CCMUX_TEXEL0 1
#define G_CCMUX_TEXEL1 2
#define G_CCMUX_PRIMITIVE 3
#define G_CCMUX_SHADE 4
#define G_CCMUX_ENVIRONMENT 5
#define G_CCMUX_CENTER 6
#define G_CCMUX_SCALE 6
#define G_CCMUX_COMBINED_ALPHA 7
#define G_CCMUX_TEXEL0_ALPHA 8
#define G_CCMUX_TEXEL1_ALPHA 9
#define G_CCMUX_PRIMITIVE_ALPHA 10
#define G_CCMUX_SHADE_ALPHA 11
#define G_CCMUX_ENV_ALPHA 12
#define G_CCMUX_LOD_FRACTION 13
#define G_CCMUX_PRIM_LOD_FRAC 14
#define G_CCMUX_NOISE 7
#define G_CCMUX_K4 7
#define G_CCMUX_K5 15
#define G_CCMUX_1 6
#define G_CCMUX_0 31

#define G_ACMUX_COMBINED 0
#define G_ACMUX_TEXEL0 1
#define G_ACMUX_TEXEL1 2
#define G_ACMUX_PRIMITIVE 3
#define G_ACMUX_SHADE 4
#define G_ACMUX_ENVIRONMENT 5
#define G_ACMUX_LOD_FRACTION 0
#define G_ACMUX_PRIM_LOD_FRAC 6
#define G_ACMUX_1 6
#define G_ACMUX_0 7


// Other Mode LO
#define G_MDSFT_TEXTLUT     0b0000_0000_0000_0000_1100_0000_0000_0000
#define G_TT_NONE   0
#define G_TT_RGBA16 2
#define G_TT_IA16   3

#define G_MDSFT_CYCLETYPE   0b0000_0000_0011_0000_0000_0000_0000_0000
#define G_CYC_1CYCLE    0
#define G_CYC_2CYCLE    1
#define G_CYC_COPY      2
#define G_CYC_FILL      3

// Color Blender
#define G_BL_CLR_IN     0u
#define G_BL_CLR_MEM    1u
#define G_BL_CLR_BL     2u
#define G_BL_CLR_FOG    3u

#define G_BL_A_IN       0u
#define G_BL_A_FOG      1u
#define G_BL_A_SHADE    2u

#define G_BL_1MA        0u
#define G_BL_A_MEM      1u

#define G_BL_1          2u
#define G_BL_0          3u



const vec3 LigthFacing = vec3(0, 0, 1);


// this is technically phong shading but whatever
float gouraudShading(vec3 light, float ambient)
{
    float diffuse = max(0, dot(normalize(light), normalize(v_VtxNormal)));
    return (diffuse + ambient) * 0.6; // hack
}

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}


/* Color Blender */ 

vec4 blendFormula(vec4 P, float A, vec4 M, float B, bool first)
{
    vec4 x = (P * A + M * B);
    if (!first)
        x /= (A + B);
    return x;
}

vec4 blendCycle(vec4 x, bool first, float shadeAlpha)
{
    uint settings = u_OtherModeLo >> 16;
    if (first)
        settings >>= 2;

    uint pFlag = settings >> 12 & 3u;
    uint aFlag = settings >> 8 & 3u;
    uint mFlag = settings >> 4 & 3u;
    uint bFlag = settings >> 0 & 3u;

    vec4 p, m;
    float a, b;

    // P
    switch (pFlag)
    {
        case G_BL_CLR_IN:
            p = vec4(x.xyz, 1);
            break;
        case G_BL_CLR_MEM:
            p = vec4(0); // todo
            break;
        case G_BL_CLR_BL:
            p = u_BlendColor;
            break;
        case G_BL_CLR_FOG:
            p = u_FogColor;
            break;
    }

    // A
    switch (aFlag)
    {
        case G_BL_A_IN:
            a = x.a;
            break;
        case G_BL_A_FOG:
            a = u_FogColor.a;
            break;
        case G_BL_A_SHADE:
            a = shadeAlpha;
            break;
        case G_BL_0:
            a = 0;
            break;
    }

    // M
    switch (mFlag)
    {
        case G_BL_CLR_IN:
            m = vec4(x.xyz, 1);
            break;
        case G_BL_CLR_MEM:
            m = vec4(0); // todo
            break;
        case G_BL_CLR_BL:
            m = u_BlendColor;
            break;
        case G_BL_CLR_FOG:
            m = u_FogColor;
            break;
    }

    // B
    switch (bFlag)
    {
        case G_BL_1MA:
            b = 1.0 - a;
            break;
        case G_BL_A_MEM :
            b = 0; // todo
            break;
        case G_BL_1:
            a = 1;
            break;
        case G_BL_0:
            a = 0;
            break;
    }
    
    return blendFormula(p, a, m, b, first);
}

/* Color Combiner */ 
vec4 combineFormula(vec4 a, vec4 b, vec4 c, vec4 d)
{
     return (a - b) * c + d;
}

vec3 combineColorAny(vec4 x, int flag)
{
    switch (flag)
    {
        case G_CCMUX_COMBINED:
            return x.xyz;
        case G_CCMUX_TEXEL0:
            return texture(u_Tex0, v_VtxTexCoords).xyz;
        case G_CCMUX_TEXEL1:
            return texture(u_Tex1, v_VtxTexCoords).xyz;
        case G_CCMUX_PRIMITIVE:
            return u_PrimColor.xyz;
        case G_CCMUX_SHADE:
            return ((u_GeoMode & G_LIGHTING) != 0u)
                ? vec3(gouraudShading(LigthFacing, 0.5))
                : v_VtxColor.xyz;
        case G_CCMUX_ENVIRONMENT:
            return u_EnvColor.xyz;
    }
    
    // defaults to G_CCMUX_0
    return vec3(0);
}

vec3 combineA(vec4 x, int flag)
{
    vec3 ret = combineColorAny(x, flag);
    switch (flag)
    {
        case G_CCMUX_1:
            return vec3(1);
        case G_CCMUX_NOISE:
            return vec3(rand(v_VtxColor.xy), rand(v_VtxColor.yz), rand(v_VtxColor.zw));
    }

    // 0x08-0x0F : G_CCMUX_0
    return ret;
}

vec3 combineB(vec4 x, int flag)
{
    vec3 ret = combineColorAny(x, flag);
    switch (flag)
    {
        case G_CCMUX_CENTER:
            return u_ChromaKeyCenter;
        // case G_CCMUX_K4:
    }
    
    // 0x08-0x0F : G_CCMUX_0
    return ret;
}

vec3 combineC(vec4 x, int flag, float shadeAlpha)
{
    vec3 ret = combineColorAny(x, flag);
    switch (flag)
    {
        case G_CCMUX_SCALE:
            return u_ChromaKeyScale;
        case G_CCMUX_COMBINED_ALPHA:
            return x.aaa;
        case G_CCMUX_TEXEL0_ALPHA:
            return vec3(texture(u_Tex0, v_VtxTexCoords).a);
        case G_CCMUX_TEXEL1_ALPHA:
            return vec3(texture(u_Tex1, v_VtxTexCoords).a);
        case G_CCMUX_PRIMITIVE_ALPHA:
            return u_PrimColor.aaa;
        case G_CCMUX_SHADE_ALPHA:
            return vec3(shadeAlpha);
        case G_CCMUX_ENV_ALPHA:
            return u_EnvColor.aaa;
        case G_CCMUX_LOD_FRACTION:
            //return vec3(0);
            return vec3(u_PrimLod);
        // G_CCMUX_PRIM_LOD_FRAC
        // G_CCMUX_K5
    }
    
    // 0x10-0x1F : G_CCMUX_0
    return ret;
}

vec3 combineD(vec4 x, int flag)
{
    vec3 ret = combineColorAny(x, flag);
    switch (flag)
    {
        case G_CCMUX_1:
            return vec3(1);
    }
    // 0x07 : G_CCMUX_0
    // if (flag == G_CCMUX_SHADE)
    //     ret = vec3(0);
    return ret;
}

float combineAlphaAny(vec4 x, int flag, float shadeAlpha)
{
    switch (flag)
    {
        case G_ACMUX_TEXEL0:
            return texture(u_Tex0, v_VtxTexCoords).a;
        case G_ACMUX_TEXEL1:
            return texture(u_Tex1, v_VtxTexCoords).a;
        case G_ACMUX_PRIMITIVE:
            return u_PrimColor.a;
        case G_ACMUX_SHADE:
            return shadeAlpha;
        case G_ACMUX_ENVIRONMENT:
            return u_EnvColor.a;
        case G_ACMUX_0:
            return 0;
    }
    return 0;
}

float combineAlphaABD(vec4 x, int flag, float shadeAlpha)
{
    float ret = combineAlphaAny(x, flag, shadeAlpha);
    switch (flag)
    {
        case G_ACMUX_COMBINED:
            return x.a;
        case G_ACMUX_1:
            return 1;
    }
    return ret;
}

float combineAlphaC(vec4 x, int flag, float shadeAlpha)
{
    float ret = combineAlphaAny(x, flag, shadeAlpha);
    switch (flag)
    {
        case G_ACMUX_LOD_FRACTION:
            //return 0;
            return u_PrimLod;
        // G_ACMUX_PRIM_LOD_FRAC 
    }
    return ret;
}



vec4 combineCycle(vec4 x, ivec4 cFlag, ivec4 aFlag, float shadeAlpha)
{
    vec4 a, b, c, d;

    a = vec4(combineA(x, cFlag.x), combineAlphaABD(x, aFlag.x, shadeAlpha));
    b = vec4(combineB(x, cFlag.y), combineAlphaABD(x, aFlag.y, shadeAlpha));
    c = vec4(combineC(x, cFlag.z, shadeAlpha), combineAlphaC(x, aFlag.z, shadeAlpha));
    d = vec4(combineD(x, cFlag.w), combineAlphaABD(x, aFlag.w, shadeAlpha));

    return combineFormula(a, b, c, d);
}

// CC1 -> CC2 -> BL1 -> BL2
vec4 calcColor()
{
    vec4 x = vec4(0);

    float shadeAlpha = v_VtxColor.a;
    if ((u_GeoMode & G_FOG) != 0u)
        shadeAlpha = 0.0;

    // CC1
    x = combineCycle(x, u_CombinerC1, u_CombinerA1, shadeAlpha);
    // CC2
    x = combineCycle(x, u_CombinerC2, u_CombinerA2, shadeAlpha);
    // BL1
    x = blendCycle(x, true, shadeAlpha);
    // BL2
    x = blendCycle(x, false, shadeAlpha);

    vec4 red = vec4(1, 0, 0, 1);
    vec4 green = vec4(0, 1, 0, 1);

    return x;
}


vec4 addHighlight(vec4 color)
{
    return (u_HighlightEnabled)
        ? mix(color, u_HighlightColor, vec4(u_HighlightColor.a))
        : color;
}

float getDiffuse(vec3 light)
{
    return max(0, dot(normalize(light), normalize(v_VtxNormal)));
}

vec4 addLighting(vec4 color)
{
    if (!u_LigthingEnabled)
        return color;

    float ambient = 0.5;
    float diffuse = getDiffuse(LigthFacing);

    return vec4(color.xyz * (diffuse + ambient), color.a);
}

vec4 addBlending(vec4 color)
{
    // todo
    return color;
}

/* Debugging */

vec4 debugDepth(vec4 color)
{
    float z = gl_FragCoord.z;
    z -= 0.999;
    z *= 1000;
    z -= 0.4;
    return color * vec4(vec3(z), 1.0);
}

vec4 debugVertexId()
{
    if (v_VtxId % 3 == 0)
        return  vec4(1, 0, 0, 1);
    else if (v_VtxId % 3 == 1)
        return vec4(0, 1, 0, 1);
    else
        return vec4(0, 0, 1, 1);
}


void main()
{
    if (u_ModelRenderMode == MODE_WIREFRAME)
    {
        FragColor = u_WireFrameColor;
    }
    else if (u_ModelRenderMode == MODE_SURFACE)
    {
        FragColor = vec4(1);
        FragColor = addLighting(FragColor);
    }
    else if (u_ModelRenderMode == MODE_TEXTURED)
    {
        /* texture */
        FragColor = texture(u_Tex0, v_VtxTexCoords);
        
        /* lazy alpha check */
        if (FragColor.a < 0.1)
            discard;
            
        //FragColor = addBlending(FragColor);
        //FragColor = addLighting(FragColor);

        FragColor = calcColor();
    }
    else if (u_ModelRenderMode == MODE_NORMAL)
    {
        FragColor = v_VtxColor;
    }
    else // invalid mode
    {
        FragColor = vec4(1, 0, 0, 1);
    }

    /* highlight */
    FragColor = addHighlight(FragColor);

}