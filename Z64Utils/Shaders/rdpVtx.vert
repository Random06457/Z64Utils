#version 330 core

layout(location = 0) in vec3 pos;
layout(location = 1) in int flag;
layout(location = 2) in ivec2 texCoords;
layout(location = 3) in vec4 color;

uniform mat4 u_View;
uniform mat4 u_Model;
uniform mat4 u_Projection;
uniform sampler2D u_Tex;

out vec2 v_VtxTexCoords;
out vec4 v_VtxColor;

float S105ToFloat(int fp)
{
    return fp >> 5;
}

vec2 decodeTexCoords(sampler2D tex, ivec2 coords)
{
    ivec2 size = textureSize(tex, 0);
    return vec2(S105ToFloat(coords.x) / size.x, S105ToFloat(coords.y) / size.y);
}

void main()
{
    /* The vertices coordinates are multiplied by the model view matrix during the G_VTX command processing */
    gl_Position = u_Projection * u_View /* u_Model*/ * vec4(pos, 1);
    v_VtxTexCoords = decodeTexCoords(u_Tex, texCoords);
    v_VtxColor = color;

}