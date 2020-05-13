cbuffer Constants
{
	float4x4 matWorldView;
};

struct VSInput
{
	float4 pos : SV_POSITION;
	float3 normal1 : TEXCOORD0;
	float3 normal2 : TEXCOORD1;
	float3 normal3 : TEXCOORD2;
};

struct VSOutput
{
	float4 pos : SV_POSITION;
	float3 normal1 : TEXCOORD0;
	float3 normal2 : TEXCOORD1;
	float3 normal3 : TEXCOORD2;
};

VSOutput main( VSInput input )
{
	VSOutput output;
	output.pos = mul(input.pos, matWorldView);
	output.normal1 = mul(input.normal1, (float3x3)matWorldView);
	output.normal2 = mul(input.normal2, (float3x3)matWorldView);
	output.normal3 = mul(input.normal3, (float3x3)matWorldView);
	return output;
}