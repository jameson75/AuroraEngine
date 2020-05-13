
cbuffer Parameters
{       
    float4x4 WorldViewProj;
	float4x4 view;
    float4 ForegroundColor;
    float4 BackgroundColor;
};

struct VS_IN
{
	float4 Position : SV_POSITION;
	float4 TexCoord : TEXCOORD0;
};

struct VS_OUT
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

VS_OUT main(VS_IN input)
{
	VS_OUT output;	
	float2 offset = input.TexCoord.zw;
	float3 xAxis = float3(view._11, view._21, view._31);
	float3 yAxis = float3(view._12, view._22, view._32);
	float3 pos = input.Position.xyz + (offset.x * xAxis) + (offset.y * yAxis);
	output.Position = mul(float4(pos, 1.0f), WorldViewProj);
	output.TexCoord = input.TexCoord.xy;	
	return output;
}