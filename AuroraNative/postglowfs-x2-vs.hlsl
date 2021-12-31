///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Credits: NVIDIA
// Comments: Source was derived from NVIDIA Shader Library for FX Composer
//			 and coverted to shader 4.x syntax.
// License <see postglowfs-inc.hlsli>
///////////////////////////////////////////////////////////////////////////////

#include "postglowfs-inc.hlsli"

// vertex shader to align blur samples vertically
NineTexelVertex main(
		float2 Position : SV_POSITION, 
		float2 UV : TEXCOORD0
) {	
	float2 ViewportOffset = (float2(0.5,0.5)/ViewportSize);
	float2 RenderSize = ViewportSize;
	float2 TexelOffset = ViewportOffset;

    NineTexelVertex OUT = (NineTexelVertex)0;
    OUT.Position = float4(Position.xy, 0, 1);
    float TexelIncrement = GlowSpan/RenderSize.y;
    float2 Coord = float2(UV.xy + TexelOffset);
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
