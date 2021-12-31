// Pixel shader extracts the brighter areas of an image.
// This is the first step in applying a bloom postprocess.
Texture2D Texture;
SamplerState TextureSampler;

cbuffer buffer
{
	float BloomThreshold;
}

struct PSIN
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD0;
};

float4 main(PSIN input) : SV_TARGET
{
    // Look up the original image color.
    float4 c = Texture.Sample(TextureSampler, input.texCoord);
    // Adjust it to keep only values brighter than the specified threshold.
	return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}

