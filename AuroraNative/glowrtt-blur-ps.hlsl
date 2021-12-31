/////////////////////////////////////////
/////// FILTER WEIGHTS //////////////////
/////////////////////////////////////////

//
// Relative filter weights for each texel.
// The default here is for symmetric distribution.
// To assign your own filter weights, just define WT9_0 through WT9_4,
//   *before* including this file.....
//
// WT9+ are for 9-tap filters, WT5+ are for 5-tap
//

// weights for 9x9 filtering

#ifndef WT9_0
// Relative filter weights indexed by distance (in texels) from "home" texel
//	(WT9_0 is the "home" or center of the filter, WT9_4 is four texels away)
#define WT9_0 1.0
#define WT9_1 0.9
#define WT9_2 0.55
#define WT9_3 0.18
#define WT9_4 0.1
#endif /* WT9_0 */

////////////////////////////////////////

// these values are based on WT9_0 through WT9_4
#define WT9_NORMALIZE (WT9_0+2.0*(WT9_1+WT9_2+WT9_3+WT9_4))
#define K9_0 (WT9_0/WT9_NORMALIZE)
#define K9_1 (WT9_1/WT9_NORMALIZE)
#define K9_2 (WT9_2/WT9_NORMALIZE)
#define K9_3 (WT9_3/WT9_NORMALIZE)
#define K9_4 (WT9_4/WT9_NORMALIZE)

Texture2D SrcSampTexture;

SamplerState SrcSampSampler;

// nine texcoords, to sample (usually) nine in-line texels
struct ScreenAligned9TexelPSIN
{
    float4 Position : SV_POSITION;
    float2 UV  : TEXCOORD0;
    float4 UV1 : TEXCOORD1; // these contain xy and zw pairs
    float4 UV2 : TEXCOORD2;
    float4 UV3 : TEXCOORD3;
    float4 UV4 : TEXCOORD4;
};

float4 blur9PS(ScreenAligned9TexelPSIN IN) : SV_TARGET
{   
    float3 OutCol = SrcSampTexture.Sample(SrcSampSampler, IN.UV4.zw).rgb * K9_4;
    OutCol += SrcSampTexture.Sample(SrcSampSampler, IN.UV3.zw).rgb * K9_3;
    OutCol += SrcSampTexture.Sample(SrcSampSampler, IN.UV2.zw).rgb * K9_2;
    OutCol += SrcSampTexture.Sample(SrcSampSampler, IN.UV1.zw).rgb * K9_1;
    OutCol += SrcSampTexture.Sample(SrcSampSampler, IN.UV).rgb * K9_0;
    OutCol += SrcSampTexture.Sample(SrcSampSampler, IN.UV1.xy).rgb * K9_1;
    OutCol += SrcSampTexture.Sample(SrcSampSampler, IN.UV2.xy).rgb * K9_2;
    OutCol += SrcSampTexture.Sample(SrcSampSampler, IN.UV3.xy).rgb * K9_3;
    OutCol += SrcSampTexture.Sample(SrcSampSampler, IN.UV4.xy).rgb * K9_4;
    return float4(OutCol.rgb,1.0);
} 
