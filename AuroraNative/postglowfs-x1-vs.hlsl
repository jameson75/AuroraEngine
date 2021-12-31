///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Credits: NVIDIA
// Comments: Source was derived from NVIDIA Shader Library for FX Composer
//			 and coverted to shader 4.x syntax.
// License <see postglowfs-inc.hlsli>
///////////////////////////////////////////////////////////////////////////////

#include "postglowfs-inc.hlsli"

// vertex shader to align blur samples horizontally
// horiz9BlurVS
NineTexelVertex main(
		float2 Position : SV_POSITION, 
		float2 UV : TEXCOORD0		
) {	
	float2 ViewportOffset = (float2(0.5,0.5)/ViewportSize);
	float2 RenderSize = ViewportSize;
	float2 TexelOffset = ViewportOffset;

    NineTexelVertex OUT = (NineTexelVertex)0;
    OUT.Position = float4(Position.xy, 0, 1);
    float TexelIncrement = GlowSpan/RenderSize.x;
    float2 Coord = float2(UV.xy + TexelOffset);
    OUT.UV = Coord;
    OUT.UV1 = float4(Coord.x + TexelIncrement, Coord.y,
		     Coord.x - TexelIncrement, Coord.y);
    OUT.UV2 = float4(Coord.x + TexelIncrement*2, Coord.y,
		     Coord.x - TexelIncrement*2, Coord.y);
    OUT.UV3 = float4(Coord.x + TexelIncrement*3, Coord.y,
		     Coord.x - TexelIncrement*3, Coord.y);
    OUT.UV4 = float4(Coord.x + TexelIncrement*4, Coord.y,
		     Coord.x - TexelIncrement*4, Coord.y);
    return OUT;
}