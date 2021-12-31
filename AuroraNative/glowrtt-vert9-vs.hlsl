#define BLUR_STRIDE 1.0

cbuffer Constants
{
	float2 QuadScreenSize;
	float2 QuadTexelOffsets = float2(0,0);
};

struct VSIN 
{
	float4 Position	: SV_POSITION;
    float2 TexCoord	: TEXCOORD0;
};

// nine texcoords, to sample (usually) nine in-line texels
struct VSOUT
{
    float4 Position : SV_POSITION;
    float2 UV  : TEXCOORD0;
    float4 UV1 : TEXCOORD1; // these contain xy and zw pairs
    float4 UV2 : TEXCOORD2;
    float4 UV3 : TEXCOORD3;
    float4 UV4 : TEXCOORD4;
};

// vertex shader to align blur samples vertically
VSOUT vert9BlurVS(VSIN input) 
{
    VSOUT OUT = (VSOUT)0;
    OUT.Position = input.Position;
    float TexelIncrement = BLUR_STRIDE/QuadScreenSize.y;
    float2 Coord = float2(input.TexCoord.xy + QuadTexelOffsets);
    OUT.UV = Coord;
    OUT.UV1 = float4(Coord.x, Coord.y + TexelIncrement,
			 Coord.x, Coord.y - TexelIncrement);
    OUT.UV2 = float4(Coord.x, Coord.y + TexelIncrement*2,
			 Coord.x, Coord.y - TexelIncrement*2);
    OUT.UV3 = float4(Coord.x, Coord.y + TexelIncrement*3,
			 Coord.x, Coord.y - TexelIncrement*3);
    OUT.UV4 = float4(Coord.x, Coord.y + TexelIncrement*4,
			 Coord.x, Coord.y - TexelIncrement*4);
    return OUT;
}
