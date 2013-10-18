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
        private VertexShader _horzVertexShader = null;
        private VertexShader _vertVertexShader = null;
        private PixelShader _blurPixelShader = null;
        private PixelShader _glowPixelShader = null;
        private VertexShader _passThruVertexShader = null;

        private Mesh _quad = null;

        public GlowRTTPostEffect(Device graphicsDevice, IGameApp gameApp)
            : base(graphicsDevice)
        {
            LoadVertexShader(@"Content\Shaders\glowrtt-horz9-vs.cso", out _horzVertexShader);
            LoadVertexShader(@"Content\Shaders\glowrtt-vert9-vs.cso", out _vertVertexShader);
            LoadPixelShader(@"Content\Shaders\glowrtt-blur.cso", out _blurPixelShader);
            LoadPixelShader(@"Content\Shaders\glowrtt-glow.cso", out _glowPixelShader);
            LoadVertexShader(@"Content\Shaders\passthru-vs.cso", out _passThruVertexShader);
        }

        public void Apply()
        {
            /////////////////
            //Setup
            /////////////////
            //Cache render targets.
            RenderTargetView previousRenderTarget = null;
            DepthStencilView previousDepthStencil = null;
            previousRenderTarget = GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out previousDepthStencil)[0];
            //Copy InputTexture (the current rendertarget texture) to the SceneTexture.
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_sceneTextureRenderTargetView);          
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);
            //Set the constant variables that will be used by the vertex and pixel shaders on each pass.
            WriteConstants();
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantBuffer);
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer);
            ////Cache graphic states and setup new ones.
            RasterizerState oldRasterizerState = GraphicsDevice.ImmediateContext.Rasterizer.State;
            RasterizerStateDescription newRasterizerStateDesc = (oldRasterizerState != null) ? oldRasterizerState.Description : RasterizerStateDescription.Default();
            newRasterizerStateDesc.CullMode = CullMode.None;
            GraphicsDevice.ImmediateContext.Rasterizer.State = new RasterizerState(GraphicsDevice, newRasterizerStateDesc);
            DepthStencilState oldDepthStencilState = GraphicsDevice.ImmediateContext.OutputMerger.DepthStencilState;
            DepthStencilStateDescription newDepthStencilStateDesc = (oldDepthStencilState != null) ? oldDepthStencilState.Description : DepthStencilStateDescription.Default();
            newDepthStencilStateDesc.IsDepthEnabled = false;
            newDepthStencilStateDesc.DepthWriteMask = 0;            
            GraphicsDevice.ImmediateContext.OutputMerger.DepthStencilState = new DepthStencilState(GraphicsDevice, newDepthStencilStateDesc);
            BlendState oldBlendState = GraphicsDevice.ImmediateContext.OutputMerger.BlendState;
            BlendStateDescription newBlendStateDesc = (oldBlendState != null) ? oldBlendState.Description : BlendStateDescription.Default();
            for (int i = 0; i < newBlendStateDesc.RenderTarget.Length; i++)
                newBlendStateDesc.RenderTarget[i].IsBlendEnabled = false;
            GraphicsDevice.ImmediateContext.OutputMerger.BlendState = new BlendState(GraphicsDevice, newBlendStateDesc);
           
            /////////
            //Pass0
            /////////                     
            //Input: SceneTexture
            //Output: GlowMap1
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _sceneTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _sceneTextureSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_glowMap1RenderTargetView);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_horzVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_blurPixelShader);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

		    /////////
            //Pass1
            /////////                     
            //Input: SceneTexture
            //Output: GlowMap1
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _sceneTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _sceneTextureSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_glowMap1RenderTargetView);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_blurPixelShader);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

	        /////////
            //Pass2
            /////////                     
            //Input: SceneTexture
            //Output: GlowMap1
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _sceneTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _sceneTextureSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_glowMap1RenderTargetView);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_glowPixelShader);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            ////////////
            //Clean up
            ////////////           
            //Clear constant buffers.
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, null);
            //Clear shader resources.
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, null);
            ////Clear samplers.
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, null);
            //Reset graphic states.
            GraphicsDevice.ImmediateContext.Rasterizer.State = oldRasterizerState;
            GraphicsDevice.ImmediateContext.OutputMerger.DepthStencilState = oldDepthStencilState;
            GraphicsDevice.ImmediateContext.OutputMerger.BlendState = oldBlendState;
            //Reset render targets.  
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(previousDepthStencil, previousRenderTarget); 
        }
    }
}

