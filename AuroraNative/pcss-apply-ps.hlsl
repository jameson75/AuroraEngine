#include "pcss-apply-inc.hlsli"

/*
Copyright NVIDIA Corporation 2008
TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THIS SOFTWARE IS PROVIDED
*AS IS* AND NVIDIA AND ITS SUPPLIERS DISCLAIM ALL WARRANTIES, EITHER EXPRESS
OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE.  IN NO EVENT SHALL NVIDIA OR ITS SUPPLIERS
BE LIABLE FOR ANY SPECIAL, INCIDENTAL, INDIRECT, OR CONSEQUENTIAL DAMAGES
WHATSOEVER (INCLUDING, WITHOUT LIMITATION, DAMAGES FOR LOSS OF BUSINESS PROFITS,
BUSINESS INTERRUPTION, LOSS OF BUSINESS INFORMATION, OR ANY OTHER PECUNIARY
LOSS) ARISING OUT OF THE USE OF OR INABILITY TO USE THIS SOFTWARE, EVEN IF
NVIDIA HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
*/

// Connector from vertex to pixel shader
struct ShadowVertexOutput {
   float4 HPosition    : SV_POSITION;
   float2 UV           : TEXCOORD0;
   float3 LightVec     : TEXCOORD1;
   float3 WNormal      : TEXCOORD2;
   float3 WView        : TEXCOORD3;
   float4 LP           : TEXCOORD4;    // current position in light-projection space
};

cbuffer AppBuffer
{
	float3 gLamp0Color;
	float gLightSize;
	float3 gSurfaceColor;
	float gSceneScale;
	float gShadBias;
	float gKd;
};

Texture2D gShadMap;
SamplerState gShadSampler;

Texture2D gFloorTexture;
SamplerState gFloorSampler;

float4 useShadowPS(ShadowVertexOutput IN) : SV_TARGET
{
   // Generic lighting code 
   float3 Nn = normalize(IN.WNormal);
   float3 Vn = normalize(IN.WView);
   float3 Ln = normalize(IN.LightVec);
   float ldn = dot(Ln,Nn);
   float3 diffContrib = gSurfaceColor*(gKd*ldn * gLamp0Color);
   // float3 result = diffContrib;
   
   // Visualize lighting and shadows
   float shadowed = caclSoftShadow(IN.LP, 
							gShadBias,
							gSceneScale,
							gLightSize,
							gShadMap,
							gShadSampler);
   
   float3 floorColor = gFloorTexture.Sample(gFloorSampler, IN.UV*2).rgb;
   //return floorColor;
   //return shadowed;
   
   return float4((shadowed*diffContrib*floorColor),1);
}