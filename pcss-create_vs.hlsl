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

cbuffer AppBuffer
{
	float4x4 WorldITXf;
	float4x4 gWorldXf;
	float4x4 gViewIXf;
	float4x4 gWvpXf;
	float4x4 gLampViewXf;
	float4x4 gLampProjXf;
};

/* data from application vertex buffer */
struct ShadowAppData {
   float3 Position     : SV_POSITION;
   float4 UV           : TEXCOORD0;
   float4 Normal       : NORMAL;
};

// Connector from vertex to pixel shader
struct JustShadowVertexOutput {
   float4 HPosition    : SV_POSITION;
   float4 LP           : TEXCOORD0;    // current position in light-projection space
};

JustShadowVertexOutput shadVS(ShadowAppData IN)
{
   float4x4 ShadowViewProjXf = mul(gLampViewXf, gLampProjXf);
   JustShadowVertexOutput OUT = (JustShadowVertexOutput)0;
   float4 Po = float4(IN.Position.xyz,(float)1.0);     // object coordinates
   float4 Pw = mul(Po,gWorldXf);                        // "P" in world coordinates
   float4 Pl = mul(Pw,ShadowViewProjXf);  // "P" in light coords
   OUT.LP = Pl;                // view coords (also lightspace projection coords in this case)
   OUT.HPosition = Pl; // screen clipspace coords
   return OUT;
}


