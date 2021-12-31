// Pixel shader combines the bloom image with the original
// scene, using tweakable intensity levels and saturation.
// This is the final step in applying a bloom postprocess.

Texture2D BloomTexture;
Texture2D BaseTexture;

SamplerState BloomSampler : register(s0);
SamplerState BaseSampler : register(s1);

cbuffer buffer
{
	float BloomIntensity;
	float BaseIntensity;
	float BloomSaturation;
	float BaseSaturation;
}

// Helper for modifying the saturation of a color.
float4 AdjustSaturation(float4 color, float saturation)
{
    // The constants 0.3, 0.59, and 0.11 are chosen because the
    // human eye is more sensitive to green light, and less to blue.
    float grey = dot(color, float3(0.3, 0.59, 0.11));

    return lerp(grey, color, saturation);
}

struct PSIN
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD0;
};

float4 main(PSIN input) : SV_TARGET
{
	float2 texCoord = input.texCoord;

    // Look up the bloom and original base image colors.
    float4 bloom = BloomTexture.Sample(BloomSampler, texCoord);
    float4 base = BaseTexture.Sample(BaseSampler, texCoord);
    
    // Adjust color saturation and intensity.
    bloom = AdjustSaturation(bloom, BloomSaturation) * BloomIntensity;
    base = AdjustSaturation(base, BaseSaturation) * BaseIntensity;
    
    // Darken down the base image in areas where there is a lot of bloom,
    // to prevent things looking excessively burned-out.
    base *= (1 - saturate(bloom));
    
    // Combine the two images.
    return base + bloom;
}

