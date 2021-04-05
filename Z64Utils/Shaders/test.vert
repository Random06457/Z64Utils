#version 330 core

layout(location = 0) in vec3 pos;

uniform mat4 u_Projection;
uniform mat4 u_View;

void main()
{
	gl_Position = u_Projection * u_View * vec4(pos, 1);
}