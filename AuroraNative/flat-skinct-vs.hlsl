#define MAX_BONES 72
#define MAX_BONES_PER_VERTEX 4

cbuffer Constants
{
	float4x4 WvpXf;	
	float4x3 Bones[MAX_BONES];
};

struct VSINPUT 
{
	float4 Position : SV_POSITION;	
	float4 Normal : NORMAL;
	float2 UV : TEXCOORD0;
	uint4  Indices  : BLENDINDICES0;
    float4 Weights  : BLENDWEIGHT0;
};

struct VSOUTPUT
{
	float4 HPosition :		SV_POSITION;	
	float4 UVUV :			TEXCOORD0;	
};

void Skin(inout VSINPUT vin, uniform int boneCount)
{
    float4x3 skinning = 0;
    for (int i = 0; i < boneCount; i++)
    {
        skinning += Bones[vin.Indices[i]] * vin.Weights[i];
    }
    vin.Position.xyz = mul(vin.Position, skinning);
    vin.Normal = float4(mul(vin.Normal.xyz, (float3x3)skinning), 0);
}

VSOUTPUT blinn_VS(VSINPUT IN)
{
    VSOUTPUT OUT = (VSOUTPUT)0;
	Skin(IN, MAX_BONES_PER_VERTEX);   
    float4 Po = IN.Position;   
    OUT.HPosition = mul(Po,WvpXf);
	OUT.UVUV = float4(IN.UV.xy, 0, 0);
    return OUT;
}