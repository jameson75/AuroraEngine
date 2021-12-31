cbuffer Constants
{
	float Sceneness;
	float Glowness;
};

Texture2D ScnSampTexture;
SamplerState ScnSampSamplerState;

Texture2D GlowSampTexture;
SamplerState GlowSampSamplerState;

struct QuadVertexOutput
{
    float4 Position	: SV_POSITION;
    float2 UV	: TEXCOORD0;
};

float4 PS_GlowPass(QuadVertexOutput IN) : SV_TARGET
{   
	float4 scn = Sceneness * ScnSampTexture.Sample(ScnSampSamplerState, IN.UV);
	float3 glow = Glowness * GlowSampTexture.Sample(GlowSampSamplerState, IN.UV).xyz;
	return float4(scn.xyz+glow,scn.w);
}  
