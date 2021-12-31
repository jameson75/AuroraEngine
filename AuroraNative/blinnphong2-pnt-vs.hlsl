#include "blinnphong2-vs.hlsli"

struct VSINPUT 
{
	float4 Position : SV_POSITION;	
	float4 Normal : NORMAL;
	float2 UV : TEXCOORD0;
};

VSOUTPUT blinn_VS(VSINPUT IN)
{
    VSOUTPUT OUT = (VSOUTPUT)0;
    OUT.WorldNormal = mul(IN.Normal,WorldITXf).xyz;    
    float4 Po = IN.Position;
    float4 Pw = mul(Po,WorldXf);	
    OUT.WorldView = normalize(ViewIXf[3].xyz - Pw.xyz);
    OUT.HPosition = mul(Po,WvpXf);
	OUT.UVUV = float4(IN.UV.xy, 0, 0);
	OUT.PWorld = Pw;
    return OUT;
}