///*********************************************************/
///*********** pixel shader ********************************/
///*********************************************************/
//
//   float2 Dimensions = { SHADOW_SIZE, SHADOW_SIZE };
//   string Format = (SHADOW_FMT) ;
//   string UIWidget = "None";
//
////#define SHADOW_SIZE 1024
////#define SHADOW_FMT  "r32f"
////float3 gLamp0Color = {1.0f,1.0f,1.0f};
////float gLightSize = 0.05f;
////float gShadBias = 0.01;
////float gSceneScale = 1.0f;
////float3 gSurfaceColor = {1,1,1};
//
//texture gShadMap : RENDERCOLORTARGET /*<
//   float2 Dimensions = { SHADOW_SIZE, SHADOW_SIZE };
//   string Format = (SHADOW_FMT) ;
//   string UIWidget = "None";
//>*/;
//
//////////////////////////////////////////////////////////////////////
///// TECHNIQUES /////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////
//
//#if DIRECT3D_VERSION >= 0xa00
////
//// Standard DirectX10 Material State Blocks
////
//RasterizerState DisableCulling { CullMode = NONE; };
//DepthStencilState DepthEnabling { DepthEnable = TRUE; };
//DepthStencilState DepthDisabling {
//	DepthEnable = FALSE;
//	DepthWriteMask = ZERO;
//};
//BlendState DisableBlend { BlendEnable[0] = FALSE; };
//
//
//technique10 Main10 <
//       string Script = "Pass=MakeShadow;"
//		       "Pass=UseShadow;";
//> {
//       pass MakeShadow <
//               string Script = "RenderColorTarget0=gShadMap;"
//				"RenderDepthStencilTarget=gShadDepthTarget;"
//				"RenderPort=SpotLight0;"
//				"ClearSetColor=gShadowClearColor;"
//				"ClearSetDepth=gClearDepth;"
//				"Clear=Color;"
//				"Clear=Depth;"
//				"Draw=geometry;";
//       > {
//	    SetVertexShader( CompileShader( vs_4_0, shadVS(gWorldITXf,gWorldXf,
//				gViewIXf,gWvpXf,
//					   mul(gLampViewXf,gLampProjXf))));
//	    SetGeometryShader( NULL );
//	    SetPixelShader( CompileShader( ps_4_0, shadPS()));
//	       SetRasterizerState(DisableCulling);
//	    SetDepthStencilState(DepthEnabling, 0);
//	    SetBlendState(DisableBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);
//       }
//       pass UseShadow <
//               string Script = "RenderColorTarget0=;"
//			       "RenderDepthStencilTarget=;"
//			       "RenderPort=;"
//			       "ClearSetColor=gClearColor;"
//			       "ClearSetDepth=gClearDepth;"
//			       "Clear=Color;"
//			       "Clear=Depth;"
//			       "Draw=geometry;";
//       > {
//	    SetVertexShader( CompileShader( vs_4_0, mainCamVS(gWorldITXf,gWorldXf,
//				gViewIXf,gWvpXf,
//					   mul(gLampViewXf,gLampProjXf),
//					   gSpotLamp0Pos,
//					   gShadBias)));
//	    SetGeometryShader( NULL );
//	    SetPixelShader( CompileShader( ps_4_0, useShadowPS(
//					       gLamp0Color,
//					       gLightSize,
//					       gSceneScale,
//					       gShadBias,
//					       gKd,
//					       gSurfaceColor,
//					       gShadSampler,
//					       gFloorSampler
//					       )));
//	       SetRasterizerState(DisableCulling);
//	    SetDepthStencilState(DepthEnabling, 0);
//	    SetBlendState(DisableBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);
//       }
//}
//
//#endif /* DIRECT3D_VERSION >= 0xa00 */
//
//
///***************************** eof ***/
