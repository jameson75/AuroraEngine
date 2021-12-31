cbuffer Constants
{
	float2 QuadTexelOffsets = float2(0,0);
};

struct QuadVertexInput
{
    float4 Position	: SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};

struct QuadVertexOutput
{
    float4 Position	: SV_POSITION;
    float2 UV : TEXCOORD0;
};

QuadVertexOutput ScreenQuadVS(QuadVertexInput IN)
{
    QuadVertexOutput OUT;
    OUT.Position = IN.Position;
    OUT.UV = float2(IN.TexCoord.xy + QuadTexelOffsets); 
    return OUT;
}
