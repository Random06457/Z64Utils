#version 330 core

in vec4 v_VtxColor;

void main()
{
	gl_FragColor = v_VtxColor;
}