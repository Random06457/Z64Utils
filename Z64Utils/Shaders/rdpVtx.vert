#version 330 core

// Input
layout(location = 0) in ivec3 rdpVtxPos;
layout(location = 1) in int rdpVtxFlag;
layout(location = 2) in ivec2 rdpVtxTexCoords;
layout(location = 3) in vec4 rdpVtxColor;
layout(location = 4) in mat4 rdpMatrix;

// Ouput
out vec2 v_VtxTexCoords;
out vec3 v_VtxNormal;
out vec4 v_VtxColor;
flat out int v_VtxId;

// Uniforms
uniform mat4 u_View;
uniform mat4 u_Model;
uniform mat4 u_Projection;

// Constants
vec4 Red = vec4(1, 0, 0, 1);
vec4 Green = vec4(0, 1, 0, 1);
vec4 Blue = vec4(0, 0, 1, 1);


int bomSwapS16(int x)
{
    return (((x >> 0) & 0xFF) << 24) >> 16 |
        ((x >> 8) & 0xFF);
}

float S105ToFloat(int fp)
{
    return float(fp >> 5);
}

int decBE105(int x)
{
    return bomSwapS16(x) >> 5;
}

float SByteToByte(float x)
{
    if (x >= 0)
        return x / 2;
    else
        return 0.5 + (x + 1) / 2;
}


vec2 decodeTexCoords()
{
    ivec2 iCoords = ivec2(decBE105(rdpVtxTexCoords.x), decBE105(rdpVtxTexCoords.y));
    
    return vec2(float(iCoords.x), float(iCoords.y));
}

vec3 decodeVtxPos()
{
    return vec3(float(bomSwapS16(rdpVtxPos.x)), float(bomSwapS16(rdpVtxPos.y)), float(bomSwapS16(rdpVtxPos.z)));
}

vec3 decodeNormal()
{
    return vec3(rdpVtxColor);
}

vec4 decodeColor()
{
    return vec4(SByteToByte(rdpVtxColor.r), SByteToByte(rdpVtxColor.g), SByteToByte(rdpVtxColor.b), SByteToByte(rdpVtxColor.a));
}


void main()
{
    v_VtxId = gl_VertexID;

    vec3 vtxPos = decodeVtxPos();
    vec3 normal = decodeNormal();
    
    v_VtxTexCoords = decodeTexCoords();
    v_VtxColor = decodeColor();

    mat4 view = u_View * rdpMatrix * u_Model;
    mat3 normalMatrix = mat3(transpose(inverse(view)));

    gl_Position = u_Projection * view * vec4(vtxPos, 1);
    v_VtxNormal = normalize(normalMatrix * normal);
}