#version 330 core

layout (location = 0) in vec2 pos;
layout (location = 1) in vec2 texCoords;

out vec2 v_TexCoords;

void main()
{
	gl_Position = vec4(pos, 0., 1.);
	v_TexCoords = texCoords;
}