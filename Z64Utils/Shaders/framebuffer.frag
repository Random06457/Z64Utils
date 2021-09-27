#version 330 core

uniform sampler2D u_FrameTexture;

in vec2 v_TexCoords;

out vec4 FragColor;

void main()
{
	FragColor = texture(u_FrameTexture, v_TexCoords);
}