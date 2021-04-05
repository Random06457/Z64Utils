#version 330 core

layout (location = 0) in vec3 pos;
layout (location = 1) in vec4 color;

uniform mat4 u_Projection;
uniform mat4 u_View;
uniform mat4 u_Model;

out vec4 v_VtxColor;

void main()
{
	gl_Position = u_Projection * u_View * u_Model * vec4(pos, 1);
	v_VtxColor = color;
}