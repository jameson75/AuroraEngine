cbuffer Constants
{	
	float4x4 WvpXf;
};

struct VSOUTPUT
{
	float4 HPosition :		SV_POSITION;		
	float4 UVUV :			TEXCOORD0;		
};