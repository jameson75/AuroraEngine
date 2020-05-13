#include "flat-vs.hlsli"

struct VSINPUT 
{
	float4 Position : SV_POSITION;	
	float2 UV : TEXCOORD0;
	float4x4 LocalXf : MATRIX;	
};

VSOUTPUT flat_VS(VSINPUT IN)
{
    VSOUTPUT OUT = (VSOUTPUT)0;
	float4x4 _WvpXf = mul(IN.LocalXf, WvpXf);     
    float4 Po = IN.Position;
    OUT.HPosition = mul(Po, _WvpXf);
	OUT.UVUV.xy = IN.UV.xy;
    return OUT;
}