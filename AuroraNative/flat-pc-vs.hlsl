#include "flat-vs.hlsli"
cbuffer Constants
{
	float4 GlobalColor;
};

struct VSINPUT 
{
	float4 Position : SV_POSITION;		
	float4 Color : COLOR;
};

VSOUTPUT flat_VS(VSINPUT IN)
{	
    VSOUTPUT OUT = (VSOUTPUT)0;  
    float4 Po = IN.Position;    
    OUT.HPosition = mul(Po, WvpXf);
	OUT.UVUV = IN.Color;
	[flatten] if (GlobalColor.w != 0)
	{
		OUT.UVUV = GlobalColor;
	}
    return OUT;
}