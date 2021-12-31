///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Credits: NVIDIA
// Comments: Source was derived from NVIDIA Shader Library for FX Composer
//			 and coverted to shader 4.x syntax.
///////////////////////////////////////////////////////////////////////////////

#include "postglowfs-inc.hlsli"

Texture2D GlowMap1 /*: RENDERCOLORTARGET*/ /*<
    float2 ViewPortRatio = {1.0,1.0};
    int MipLevels = 1;
    string Format = "X8R8G8B8" ;
    string UIWidget = "None";
>*/;

SamplerState GlowSamp1 {
    //texture = <gGlowMap1>;
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

//GlowPS
float4 main(OneTexelVertex IN
) : SV_TARGET
{  	
	SamplerState ScnSampler = SceneSampler;
	SamplerState GlowSampler = GlowSamp1;
    float4 scn = Sceneness * SceneTexture.Sample(ScnSampler, IN.UV);
    float3 glow = Glowness * GlowMap1.Sample(GlowSampler, IN.UV).xyz;
    return float4(scn.xyz+glow,scn.w);
}  
