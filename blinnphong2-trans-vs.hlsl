cbuffer Constants
{
	float4x4 WorldITXf;
	float4x4 WorldXf;
	float4x4 ViewIXf;
	float4x4 WvpXf;
};

struct VSINPUT 
{
	float4 Position : SV_POSITION;	
	float4 Normal : NORMAL;
	float2 UV : TEXCOORD0;
	float2 UV2 : TEXCOORD1;
};

struct VSOUTPUT
{
	float4 HPosition :		SV_POSITION;	
	float3 WorldView :		TEXCOORD0;
	float3 WorldNormal :	TEXCOORD1;
	float2 UV :				TEXCOORD2;	
	float2 UV2 :			TEXCOORD3;
	float4 PWorld :			TEXCOORD4;	
};

VSOUTPUT blinn_VS(VSINPUT IN)
{
    VSOUTPUT OUT = (VSOUTPUT)0;
    OUT.WorldNormal = mul(IN.Normal,WorldITXf).xyz;    
    float4 Po = IN.Position;
    float4 Pw = mul(Po,WorldXf);	
    OUT.WorldView = normalize(ViewIXf[3].xyz - Pw.xyz);
    OUT.HPosition = mul(Po,WvpXf);
	OUT.UV = IN.UV.xy;
	OUT.UV2 = IN.UV2.xy;
	OUT.PWorld = Pw;
    return OUT;
}