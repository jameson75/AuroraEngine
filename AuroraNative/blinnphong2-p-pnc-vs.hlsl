#include "blinnphong2-vs.hlsli"

struct VSINPUT 
{
	float4 Position : SV_POSITION;	
	float3 Normal : NORMAL;
	float4 Color : COLOR;
	float4x4 LocalXf : MATRIX;	
};

VSOUTPUT blinn_VS(VSINPUT IN)
{
    VSOUTPUT OUT = (VSOUTPUT)0;
	float4x4 _WorldXf = mul(IN.LocalXf, WorldXf);
	float4x4 _WvpXf = mul(IN.LocalXf, WvpXf);

    OUT.WorldNormal = mul(IN.Normal, WorldITXf).xyz;    
    float4 Po = IN.Position;
    float4 Pw = mul(Po, _WorldXf);	
    OUT.WorldView = normalize(ViewIXf[3].xyz - Pw.xyz);
    OUT.HPosition = mul(Po, _WvpXf);
	OUT.UVUV = IN.Color;
	OUT.PWorld = Pw;
    return OUT;
}