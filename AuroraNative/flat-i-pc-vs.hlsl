#include "flat-vs.hlsli"
cbuffer Constants
{
	float4 GlobalColor;
};

struct VSINPUT 
{
	float4 Position : SV_POSITION;	
	float4 Color : COLOR;
	float4x4 LocalXf : MATRIX;
};

VSOUTPUT flat_VS(VSINPUT IN)
{
    VSOUTPUT OUT = (VSOUTPUT)0;
	float4x4 _WvpXf = mul(IN.LocalXf, WvpXf);
    float4 Po = IN.Position;   
    OUT.HPosition = mul(Po, _WvpXf);
	OUT.UVUV = IN.Color;
	[flatten] if (GlobalColor.w != 0)
	{
		OUT.UVUV = GlobalColor;
	}
    return OUT;
}