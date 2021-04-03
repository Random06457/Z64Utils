#version 330 core

layout (triangles) in;
layout (line_strip, max_vertices = 6) out;
//layout (triangle_strip, max_vertices = 3) out;

in vec3 v_VtxNormal[];

out vec4 v_VtxColor;

uniform vec4 u_NrmColor;

void main()
{
	//v_VtxColor = u_NrmColor;
	v_VtxColor = vec4(1, 0, 0, 1);

	for (int i = 0; i < 3; i++)
	{
		vec3 normal = v_VtxNormal[i];

		gl_Position = gl_in[i].gl_Position;
		EmitVertex();
		
		gl_Position = gl_in[i].gl_Position + vec4(normal, 0) * 1000;
		EmitVertex();
		
		EndPrimitive();
	}
	
	/*
	for (int i = 0; i < gl_in.length(); i++)
	{
		v_VtxColor = vec4(1, 1, 0, 1); //u_NrmColor;
		gl_Position = gl_in[i].gl_Position;
		EmitVertex();
	}
	EndPrimitive();
	*/

}