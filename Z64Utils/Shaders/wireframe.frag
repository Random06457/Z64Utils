#version 330 core

uniform vec4 u_HighlightColor;
uniform bool u_HighlightEnabled;
uniform vec4 u_WireFrameColor;


vec4 addHighlight(vec4 color)
{
	return (u_HighlightEnabled)
		? mix(color, u_HighlightColor, vec4(u_HighlightColor.a))
		: color;
}

void main()
{
	gl_FragColor = u_WireFrameColor;
	
	gl_FragColor = addHighlight(gl_FragColor);

}