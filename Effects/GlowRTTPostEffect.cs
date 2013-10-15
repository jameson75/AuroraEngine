using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Content;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Effects
{
    public class GlowRTTPostEffect : PostEffect
    {
        public void Apply()
        {
            technique Glow_9Tap <
	string Script =
	"RenderColorTarget0=ScnMap;"
	"RenderDepthStencilTarget=DepthBuffer;"
		"ClearSetColor=ClearColor;"
		"ClearSetDepth=ClearDepth;"
		"Clear=Color;"
		"Clear=Depth;"
	    "ScriptExternal=color;"
	"Pass=BlurGlowBuffer_Horz;"
	"Pass=BlurGlowBuffer_Vert;"
	"Pass=GlowPass;";
> {
    pass BlurGlowBuffer_Horz <
		string Script = "RenderColorTarget=GlowMap1;"
						"RenderDepthStencilTarget=DepthBuffer;"
						"Draw=Buffer;";
	> {
		VertexShader = compile vs_2_0 horiz9BlurVS();
		cullmode = none;
		ZEnable = false;
		ZWriteEnable = false;
		AlphaBlendEnable = false;
		PixelShader  = compile ps_2_a blur9PS(ScnSamp);
	}
    pass BlurGlowBuffer_Vert <
		string Script = "RenderColorTarget=GlowMap2;"
						"RenderDepthStencilTarget=DepthBuffer;"
						"Draw=Buffer;";
	> {
		VertexShader = compile vs_2_0 vert9BlurVS();
		cullmode = none;
		ZEnable = false;
		ZWriteEnable = false;
		AlphaBlendEnable = false;
	    PixelShader  = compile ps_2_a blur9PS(GlowSamp1);
	}
    pass GlowPass <
       	string Script= "RenderColorTarget0=;"
						"RenderDepthStencilTarget=;"
						"Draw=Buffer;";        	
	>
	{
		cullmode = none;
		ZEnable = false;
		ZWriteEnable = false;
		AlphaBlendEnable = false;
		VertexShader = compile vs_1_1 ScreenQuadVS();
		PixelShader = compile ps_2_0 PS_GlowPass();	
    }
}

        }
    }
}
