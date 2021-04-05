#version 330 core

uniform vec4 u_HighlightColor;
uniform bool u_HighlightEnabled;
uniform vec4 u_WireFrameColor;

out vec4 FragColor;


vec4 addHighlight(vec4 color)
{
	return (u_HighlightEnabled)
		? mix(color, u_HighlightColor, vec4(u_HighlightColor.a))
		: color;
}

void main()
{
	FragColor = u_WireFrameColor;
	
	FragColor = addHighlight(FragColor);

}