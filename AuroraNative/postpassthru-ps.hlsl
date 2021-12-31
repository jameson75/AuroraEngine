
Texture2D mainTexture;
SamplerState TextureSampler;

struct PSIN
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD0;
};

float4 main(PSIN input) : SV_TARGET
{
	return mainTexture.Sample(TextureSampler, input.texCoord);
}