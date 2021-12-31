///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Credits: NVIDIA
// Comments: Source was derived from NVIDIA Shader Library for FX Composer
//			 and coverted to shader 4.x syntax.
// License <see postglowfs-inc.hlsli>
///////////////////////////////////////////////////////////////////////////////

#include "postglowfs-inc.hlsli"

Texture2D SceneTexture /*: RENDERCOLORTARGET*/ 
	/*<
    float2 ViewPortRatio = {1.0,1.0};
    int MipLevels = 1;
    string Format = "X8R8G8B8" ;
    string UIWidget = "None";
>*/;

SamplerState SceneSampler /*sampler_state*/ {
    //texture = <SceneTexture>;
    AddressU = Clamp;
    AddressV = Clamp;
//#if DIRECT3D_VERSION >= 0xa00
	Filter = MIN_MAG_LINEAR_MIP_POINT;
//#else /* DIRECT3D_VERSION < 0xa00 */
//    MinFilter = Linear;
//    MagFilter = Linear;
//    MipFilter = Point;
//#endif /* DIRECT3D_VERSION */
};

//blur9PS
float4 main(NineTexelVertex IN) : SV_TARGET
{   
	SamplerState SrcSamp = SceneSampler;
    float4 OutCol = SceneTexture.Sample(SrcSamp, IN.UV4.zw) * KW_4;
    OutCol += SceneTexture.Sample(SrcSamp, IN.UV3.zw) * KW_3;
    OutCol += SceneTexture.Sample(SrcSamp, IN.UV2.zw) * KW_2;
    OutCol += SceneTexture.Sample(SrcSamp, IN.UV1.zw) * KW_1;
    OutCol += SceneTexture.Sample(SrcSamp, IN.UV) * KW_0;
    OutCol += SceneTexture.Sample(SrcSamp, IN.UV1.xy) * KW_1;
    OutCol += SceneTexture.Sample(SrcSamp, IN.UV2.xy) * KW_2;
    OutCol += SceneTexture.Sample(SrcSamp, IN.UV3.xy) * KW_3;
    OutCol += SceneTexture.Sample(SrcSamp, IN.UV4.xy) * KW_4;
    return OutCol;
} 