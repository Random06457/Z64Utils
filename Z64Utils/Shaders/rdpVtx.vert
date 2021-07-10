#version 330 core

layout(location = 0) in ivec3 rdpVtxPos;
layout(location = 1) in int rdpVtxFlag;
layout(location = 2) in ivec2 rdpVtxTexCoords;
layout(location = 3) in vec4 rdpVtxColor;
layout(location = 4) in mat4 rdpMatrix;

out vec2 v_VtxTexCoords;
out vec3 v_VtxNormal;
out vec4 v_VtxColor;
flat out int v_VtxId;

uniform mat4 u_View;
uniform mat4 u_Model;
uniform mat4 u_Projection;
uniform sampler2D u_Tex0;

float S105ToFloat(int fp)
{
    return float(fp >> 5);
}

int bomSwap16(int x)
{
    return (((x >> 0) & 0xFF) << 24) >> 16 |
        ((x >> 8) & 0xFF);
}

vec2 decodeTexCoords(sampler2D tex, ivec2 coords)
{
    ivec2 size = textureSize(tex, 0);
    return vec2(S105ToFloat(coords.x) / size.x, S105ToFloat(coords.y) / size.y);
}

vec2 getTexCoords()
{
    return decodeTexCoords(u_Tex0, ivec2(bomSwap16(rdpVtxTexCoords.x), bomSwap16(rdpVtxTexCoords.y)));
}

vec3 getPos()
{
    return vec3(float(bomSwap16(rdpVtxPos.x)), float(bomSwap16(rdpVtxPos.y)), float(bomSwap16(rdpVtxPos.z)));
}


vec3 decodeNormal(vec4 color)
{
    return vec3(color);
}

float SByteToByte(float x)
{
    if (x >= 0)
        return x / 2;
    else
        return 0.5 + (x + 1) / 2;
}

vec4 decodeColor(vec4 color)
{
    return vec4(SByteToByte(color.r), SByteToByte(color.g), SByteToByte(color.b), SByteToByte(color.a));
}


void main()
{
    v_VtxId = gl_VertexID;

    vec3 pos = getPos();
    vec3 normal = decodeNormal(rdpVtxColor);
    
    v_VtxTexCoords = getTexCoords();
    v_VtxColor = decodeColor(rdpVtxColor);

    mat4 view = u_View * rdpMatrix * u_Model;
    mat3 normalMatrix = mat3(transpose(inverse(view)));

    gl_Position = u_Projection * view * vec4(pos, 1);
    v_VtxNormal = normalize(normalMatrix * normal);
}