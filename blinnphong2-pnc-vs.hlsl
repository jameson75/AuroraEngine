#include "blinnphong2-vs.hlsli"

struct VSINPUT 
{
	float4 Position : SV_POSITION;	
	float3 Normal : NORMAL;
	float4 Color : COLOR;
};

VSOUTPUT blinn_VS(VSINPUT IN)
{
    VSOUTPUT OUT = (VSOUTPUT)0;
    OUT.WorldNormal = mul(IN.Normal,WorldITXf).xyz;    
    float4 Po = IN.Position;
    float4 Pw = mul(Po,WorldXf);	
    OUT.WorldView = normalize(ViewIXf[3].xyz - Pw.xyz);
    OUT.HPosition = mul(Po,WvpXf);
	OUT.UVUV = IN.Color;
	OUT.PWorld = Pw;
    return OUT;
}