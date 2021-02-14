cbuffer Constants
{
	float4x4 WorldITXf;
	float4x4 WorldXf;
	float4x4 ViewIXf;
	float4x4 WvpXf;
};

struct VSOUTPUT
{
	float4 HPosition :		SV_POSITION;	
	float3 WorldView :		TEXCOORD0;
	float3 WorldNormal :	TEXCOORD1;
	float4 UVUV :			TEXCOORD2;	
	float4 PWorld :			TEXCOORD4;	
};