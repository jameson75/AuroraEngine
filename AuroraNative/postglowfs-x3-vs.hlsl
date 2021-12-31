///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Credits: NVIDIA
// Comments: Source was derived from NVIDIA Shader Library for FX Composer
//			 and coverted to shader 4.x syntax.
// License <see postglowfs-inc.hlsli>
///////////////////////////////////////////////////////////////////////////////

#include "postglowfs-inc.hlsli"

//ScreenQuadVS2
OneTexelVertex main(
		float2 Position : SV_POSITION, 
		float2 UV	: TEXCOORD0
) {
	float2 ViewportOffset = (float2(0.5,0.5)/ViewportSize);
	float2 TexelOffset = ViewportOffset;
    OneTexelVertex OUT = (OneTexelVertex)0;
    OUT.Position = float4(Position.xy, 0, 1);
    OUT.UV = float2(UV.xy + TexelOffset);
    return OUT;
}