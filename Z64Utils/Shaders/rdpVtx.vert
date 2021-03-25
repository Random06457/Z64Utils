#version 330 core

layout(location = 0) in ivec3 pos;
layout(location = 1) in int flag;
layout(location = 2) in ivec2 texCoords;
layout(location = 3) in vec4 color;

uniform mat4 u_View;
uniform mat4 u_Model;
uniform mat4 u_Projection;
uniform sampler2D u_Tex;

out vec2 v_VtxTexCoords;
out vec4 v_VtxColor;
flat out int v_VtxId;

float S105ToFloat(int fp)
{
    return fp >> 5;
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
    return decodeTexCoords(u_Tex, ivec2(bomSwap16(texCoords.x), bomSwap16(texCoords.y)));
}

vec3 getPos()
{
    return vec3(float(bomSwap16(pos.x)), float(bomSwap16(pos.y)), float(bomSwap16(pos.z)));
}


void main()
{
    v_VtxId = gl_VertexID;

    /* The vertices coordinates are multiplied by the model view matrix during the G_VTX command processing */
    gl_Position = u_Projection * u_View /* u_Model*/ * vec4(getPos(), 1);
    v_VtxTexCoords = getTexCoords();
    v_VtxColor = color;
}