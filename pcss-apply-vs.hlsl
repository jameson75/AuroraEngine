cbuffer AppBuffer
{
	float4x4 gWorldITXf;
	float4x4 gWorldXf;
	float4x4 gViewIXf;
	float4x4 gWvpXf;
	float4x4 gLampViewXf;
	float4x4 gLampProjXf;
	float3 gLamp0Pos;
	float3 gLamp0Color;
	float gShadBias;
};

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

/* data from application vertex buffer */
struct ShadowAppData {
   float3 Position     : SV_POSITION;
   float4 UV           : TEXCOORD0;
   float4 Normal       : NORMAL;
};

// Connector from vertex to pixel shader
struct ShadowVertexOutput {
   float4 HPosition    : SV_POSITION;
   float2 UV           : TEXCOORD0;
   float3 LightVec     : TEXCOORD1;
   float3 WNormal      : TEXCOORD2;
   float3 WView        : TEXCOORD3;
   float4 LP           : TEXCOORD4;    // current position in light-projection space
};

// from scene camera POV
ShadowVertexOutput mainCamVS(ShadowAppData IN) 
{
   ShadowVertexOutput OUT = (ShadowVertexOutput)0;
   float4x4 ShadowViewProjXf = mul(gLampViewXf, gLampProjXf);
   OUT.WNormal = mul(IN.Normal,gWorldITXf).xyz; // world coords
   float4 Po = float4(IN.Position.xyz,(float)1.0);     // "P" in object coordinates
   float4 Pw = mul(Po,gWorldXf);                        // "P" in world coordinates
   float4 Pl = mul(Pw,ShadowViewProjXf);  // "P" in light coords
   Pl.z -= gShadBias;	// factor in bias here to save pixel shader work
   OUT.LP = Pl;                                                       
// ...for pixel-shader shadow calcs
   OUT.WView = normalize(gViewIXf[3].xyz - Pw.xyz);     // world coords
   OUT.HPosition = mul(Po,gWvpXf);    // screen clipspace coords
   OUT.UV = IN.UV.xy;                                                 
// pass-thru
   OUT.LightVec = gLamp0Pos - Pw.xyz;               // world coords
   return OUT;
}
