#define MAX_LIGHTS 8

cbuffer Constants
{	
	bool EnableVertexColor;
};

Texture2D DiffuseTexture;
SamplerState DiffuseSampler;

struct PSINPUT
{
	float4 HPosition : SV_POSITION;
	float4 UVUV : TEXCOORD0;
};

float4 flat_PS(PSINPUT IN) : SV_TARGET
{	
	float4 diffuseColor = 0;
	if (EnableVertexColor)
		diffuseColor = float4(IN.UVUV.xyz, 1);
	else
		diffuseColor = DiffuseTexture.Sample(DiffuseSampler, IN.UVUV.xy).rgba;
	return diffuseColor;
}

