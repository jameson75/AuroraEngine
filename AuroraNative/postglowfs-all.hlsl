///*********************************************************************NVMH3****
//*******************************************************************************
//$Revision: #3 $
//
//Copyright NVIDIA Corporation 2008
//TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THIS SOFTWARE IS PROVIDED
//*AS IS* AND NVIDIA AND ITS SUPPLIERS DISCLAIM ALL WARRANTIES, EITHER EXPRESS
//OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY
//AND FITNESS FOR A PARTICULAR PURPOSE.  IN NO EVENT SHALL NVIDIA OR ITS SUPPLIERS
//BE LIABLE FOR ANY SPECIAL, INCIDENTAL, INDIRECT, OR CONSEQUENTIAL DAMAGES
//WHATSOEVER (INCLUDING, WITHOUT LIMITATION, DAMAGES FOR LOSS OF BUSINESS PROFITS,
//BUSINESS INTERRUPTION, LOSS OF BUSINESS INFORMATION, OR ANY OTHER PECUNIARY
//LOSS) ARISING OUT OF THE USE OF OR INABILITY TO USE THIS SOFTWARE, EVEN IF
//NVIDIA HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
//
//A full-screen glow effect using multiple passes
//
//To learn more about shading, shaders, and to bounce ideas off other shader
//authors and users, visit the NVIDIA Shader Library Forums at:
//
//http://developer.nvidia.com/forums/
//
//*******************************************************************************
//******************************************************************************/
//technique10 Main10 <
//{
//	pass BlurGlowBuffer_Horz 
//	{
//		SetVertexShader( CompileShader( vs_4_0, horiz9BlurVS(gGlowSpan,
//			ViewportSize,
//			ViewportOffset) ) );
//		SetGeometryShader( NULL );
//		SetPixelShader( CompileShader( ps_4_0, blur9PS(gSceneSampler) ) );
//
//		SetRasterizerState(DisableCulling);       
//		SetDepthStencilState(DepthDisabling, 0);
//		SetBlendState(DisableBlend,
//			float4( 0.0f, 0.0f, 0.0f, 0.0f ),
//			0xFFFFFFFF);
//	}
//
//	pass BlurGlowBuffer_Vert <
//
//	{
//		SetVertexShader( CompileShader( vs_4_0, vert9BlurVS(gGlowSpan,
//			ViewportSize,
//			ViewportOffset) ) );
//		SetGeometryShader( NULL );
//		SetPixelShader( CompileShader( ps_4_0, blur9PS(gGlowSamp1) ) );
//
//		SetRasterizerState(DisableCulling);       
//		SetDepthStencilState(DepthDisabling, 0);
//		SetBlendState(DisableBlend,
//			float4( 0.0f, 0.0f, 0.0f, 0.0f ),
//			0xFFFFFFFF);
//	}
//
//	pass GlowPass     
//	{
//		SetVertexShader( CompileShader( vs_4_0, ScreenQuadVS2(ViewportOffset) ) );
//		SetGeometryShader( NULL );
//		SetPixelShader( CompileShader( ps_4_0, GlowPS(gGlowness,gSceneness,
//			gSceneSampler,gGlowSamp2) ) );
//
//		SetRasterizerState(DisableCulling);       
//		SetDepthStencilState(DepthDisabling, 0);
//		SetBlendState(DisableBlend,
//			float4( 0.0f, 0.0f, 0.0f, 0.0f ),
//			0xFFFFFFFF);
//	}	
//
//}
