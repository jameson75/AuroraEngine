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
        private SharpDX.Direct3D11.Buffer _constantBuffer1 = null;
        private SharpDX.Direct3D11.Buffer _constantBuffer2 = null;
        private SharpDX.Direct3D11.Buffer _constantBuffer3 = null;
        private const int ConstantBufferSize1 = 16;
        private const int ConstantBufferSize2 = 16;
        private const int ConstantBufferSize3 = 16;
        private Mesh _quad = null;
        private IGameApp _game = null;

        public GlowRTTPostEffect(Device graphicsDevice, IGameApp gameApp)
            : base(graphicsDevice)
        {
            _game = gameApp;
            CreateShaders();
            CreateTargets();
        }

        public float Sceneness { get; set; }

        public float Glowness { get; set; }

        public void Apply()
        {
            /////////////////
            //Setup
            /////////////////
            
            //Cache render targets.
            //---------------------
            RenderTargetView previousRenderTarget = null;
            DepthStencilView previousDepthStencil = null;
            previousRenderTarget = GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out previousDepthStencil)[0];            
            
            //Set the constant variables that will be used by the vertex and pixel shaders on each pass.
            //------------------------------------------------------------------------------------------
            WriteConstants();           

            //Cache graphic states and setup new ones.
            //------------------------------------------
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
            //Input: InputTexture
            //Output: HBlur         
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer1);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _inputTextureSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_hBlurRenderTargetView);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_horzVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_blurPixelShader);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

		    /////////
            //Pass1
            /////////                     
            //Input: HBlur
            //Output: HVBlur
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
            //Input: HVBlur
            //Output: GlowMap
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _sceneTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _sceneTextureSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_glowMap1RenderTargetView);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_glowPixelShader);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            //////////
            //Pass3
            //////////
            //Input: GlowMap
            //Output: Final (GlowMap overlayed).
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

        private void CreateShaders()
        {
            LoadVertexShader(@"Content\Shaders\glowrtt-horz9-vs.cso", out _horzVertexShader);
            LoadVertexShader(@"Content\Shaders\glowrtt-vert9-vs.cso", out _vertVertexShader);
            LoadPixelShader(@"Content\Shaders\glowrtt-blur.cso", out _blurPixelShader);
            LoadPixelShader(@"Content\Shaders\glowrtt-glow.cso", out _glowPixelShader);
            LoadVertexShader(@"Content\Shaders\passthru-vs.cso", out _passThruVertexShader);
        }

        private void CreateTargets()
        {

        }

        private void WriteConstants()
        {
            DataBox dataBox;
            int inputTextureWidth = InputTexture.ResourceAs<Texture2D>().Description.Width;
            int inputTextureHeight = InputTexture.ResourceAs<Texture2D>().Description.Height;
            float sceneness = this.Sceneness;
            float glowness = this.Glowness;

            Vector2 quadTexelOffset = Vector2.Zero;
            Vector2 quadScreenSize = new Vector2(inputTextureWidth, inputTextureWidth);

            //Write to ConstantBuffer1
            dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer1, 0, MapMode.WriteDiscard, MapFlags.None);            
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector2>(dataBox.DataPointer, ref quadTexelOffset);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer1, 0);

            //Write to Guassian Horizontal Buffer
            dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer2, 0, MapMode.WriteDiscard, MapFlags.None);
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector2>(dataBox.DataPointer, ref quadScreenSize);
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector2>(dataBox.DataPointer, ref quadTexelOffset);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer2, 0);

            //Write to Guassian Horizontal Buffer
            dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer3, 0, MapMode.WriteDiscard, MapFlags.None);            
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref sceneness);
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref glowness);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer3, 0);        
        }
    }
}

