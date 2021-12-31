
cbuffer Constants
{
	float4 transparencyKey;
};

Texture2D mainTexture;
SamplerState TextureSampler;

struct PSIN
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD0;
};

float4 main(PSIN input) : SV_TARGET
{
	float4 color = mainTexture.Sample(TextureSampler, input.texCoord);
	bool isTransparent = any((color - transparencyKey));
	if (isTransparent)
		return color;
	else 
		return float4(0,0,0,0);
}