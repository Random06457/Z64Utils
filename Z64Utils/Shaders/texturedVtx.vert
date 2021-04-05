#version 330 core

uniform mat4 u_Projection;
uniform mat4 u_View;
uniform mat4 u_Model;

layout (location = 0) in vec3 coords;
layout (location = 1) in vec2 texCoords;

out vec2 v_TexCoords;

void main()
{
	gl_Position = u_Projection * u_View * u_Model * vec4(coords, 1);
	v_TexCoords = texCoords;
}