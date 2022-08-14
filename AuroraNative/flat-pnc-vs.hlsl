cbuffer Constants
{
	float4x4 WvpXf;
	float4 GlobalColor;	
};

struct VSINPUT 
{
	float4 Position : SV_POSITION;
	float4 Normal : NORMAL;
	float4 Color : COLOR;
};

struct VSOUTPUT
{
	float4 HPosition :		SV_POSITION;
	float4 UVUV :			TEXCOORD0;
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