#version 330 core

uniform sampler2D u_Tex;
uniform vec4 u_Color;

in vec2 v_TexCoords;

void main()
{
	vec4 color = texture(u_Tex, v_TexCoords) * u_Color;
	if (color.a < 0.1)
		discard;
	gl_FragColor = color;
	//gl_FragColor = vec4(1, v_TexCoords.y, 0, 1);
	//gl_FragColor = vec4(v_TexCoords.x, 1, 0, 1);
	//gl_FragColor = vec4(1, 0, 0, 1);
}