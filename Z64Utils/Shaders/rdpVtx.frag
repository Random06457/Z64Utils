#version 330 core

in vec2 v_VtxTexCoords;
in vec3 v_VtxNormal;
in vec4 v_VtxColor;
flat in int v_VtxId;

uniform vec3 u_DirLight;
uniform sampler2D u_Tex;
uniform vec4 u_PrimColor;

uniform vec4 u_HighlightColor;
uniform bool u_HighlightEnabled;
uniform vec4 u_WireFrameColor;
uniform int u_ModelRenderMode;

#define MODE_WIREFRAME		0
#define MODE_TEXTURED		1
#define MODE_SURFACE		2
#define MODE_NORMAL			3


const vec3 LigthFacing = vec3(0, 0, 1);


vec4 addHighlight(vec4 color)
{
	return (u_HighlightEnabled)
		? mix(color, u_HighlightColor, vec4(u_HighlightColor.a))
		: color;
}

vec4 addShading(vec4 color, vec3 light, float ambiantLight)
{
	float brightness = max(0, dot(light, v_VtxNormal)) + ambiantLight;

	return vec4(color.xyz * brightness, color.a);
}

void main()
{
	if (u_ModelRenderMode == MODE_WIREFRAME)
	{
		gl_FragColor = u_WireFrameColor;
	}
	else if (u_ModelRenderMode == MODE_SURFACE)
	{
		gl_FragColor = vec4(1);
		//gl_FragColor = addShading(gl_FragColor, LigthFacing, 0.3);
	}
	else if (u_ModelRenderMode == MODE_TEXTURED)
	{
		/* texture */
		gl_FragColor = texture(u_Tex, v_VtxTexCoords);
		
		/* lazy alpha check */
		if (gl_FragColor.a < 0.1)
			discard;
			
		//gl_FragColor = addShading(gl_FragColor, LigthFacing, 0.3);
	}
	else if (u_ModelRenderMode == MODE_NORMAL)
	{
		gl_FragColor = v_VtxColor;
	}
	
	/* blending */
	// FragColor *= v_VtxColor;

	/* highlight */
	gl_FragColor = addHighlight(gl_FragColor);

	/* Debug Vertex ID */
	/*
	if (v_VtxId % 3 == 0)
		gl_FragColor = vec4(1, 0, 0, 1);
	else if (v_VtxId % 3 == 1)
		gl_FragColor = vec4(0, 1, 0, 1);
	else
		gl_FragColor = vec4(0, 0, 1, 1);
	*/

	/* Debug Depth */
	/*
	float z = gl_FragCoord.z;
	z -= 0.999;
	z *= 1000;
	z -= 0.4;
    gl_FragColor *= vec4(vec3(z), 1.0);
	*/
}