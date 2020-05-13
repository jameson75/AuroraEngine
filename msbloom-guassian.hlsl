// Pixel shader applies a one dimensional gaussian blur filter.
// This is used twice by the bloom postprocess, first to
// blur horizontally, and then again to blur vertically.

Texture2D Texture;
SamplerState TextureSampler : register(s0);

#define SAMPLE_COUNT 15
cbuffer buffer
{
	float2 SampleOffsets[SAMPLE_COUNT];
	float SampleWeights[SAMPLE_COUNT];
}

float4 main(float2 texCoord : TEXCOORD0) : SV_TARGET
{
    float4 c = 0;
    
    // Combine a number of weighted image filter taps.
    //for (int i = 0; i < SAMPLE_COUNT; i++)
    //{
		c += Texture.Sample(TextureSampler, texCoord + SampleOffsets[0]) * SampleWeights[0];
		c += Texture.Sample(TextureSampler, texCoord + SampleOffsets[1]) * SampleWeights[1];
		c += Texture.Sample(TextureSampler, texCoord + SampleOffsets[2]) * SampleWeights[2];
		c += Texture.Sample(TextureSampler, texCoord + SampleOffsets[3]) * SampleWeights[3];
		c += Texture.Sample(TextureSampler, texCoord + SampleOffsets[4]) * SampleWeights[4];
		c += Texture.Sample(TextureSampler, texCoord + SampleOffsets[5]) * SampleWeights[5];
		c += Texture.Sample(TextureSampler, texCoord + SampleOffsets[6]) * SampleWeights[6];
		c += Texture.Sample(TextureSampler, texCoord + SampleOffsets[7]) * SampleWeights[7];
		c += Texture.Sample(TextureSampler, texCoord + SampleOffsets[8]) * SampleWeights[8];
		c += Texture.Sample(TextureSampler, texCoord + SampleOffsets[9]) * SampleWeights[9];
		c += Texture.Sample(TextureSampler, texCoord + SampleOffsets[10]) * SampleWeights[10];
		c += Texture.Sample(TextureSampler, texCoord + SampleOffsets[11]) * SampleWeights[11];
		c += Texture.Sample(TextureSampler, texCoord + SampleOffsets[12]) * SampleWeights[12];
		c += Texture.Sample(TextureSampler, texCoord + SampleOffsets[13]) * SampleWeights[13];
		c += Texture.Sample(TextureSampler, texCoord + SampleOffsets[14]) * SampleWeights[14];
    
    return c;
}

