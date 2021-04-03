#version 330 core

layout (triangles) in;
layout (line_strip, max_vertices = 6) out;

void main()
{

	for (int i = 0; i+1 < gl_in.length(); i++)
	{
		gl_Position = gl_in[i].gl_Position;
		EmitVertex();

		gl_Position = gl_in[i+1].gl_Position;
		EmitVertex();
		
		EndPrimitive();
	}

	gl_Position = gl_in[gl_in.length()-1].gl_Position;
	EmitVertex();

	gl_Position = gl_in[0].gl_Position;
	EmitVertex();
		
	EndPrimitive();

}