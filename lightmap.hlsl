Texture2D lightMapTexture;
SamplerState lightMapTextureSampler;

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
	float4 light = lightMapTexture.Sample(lightMapTextureSampler, input.texCoord);
	return color * light;
}