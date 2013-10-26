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
        private byte[] _vertexShaderByteCode = null;
        private SamplerState _inputTextureSamplerState = null;
        private ShaderResourceView _hBlurShaderResourceView = null;
        private RenderTargetView _hBlurRenderTargetView = null;
        private SamplerState _hBlurSamplerState = null;
        private ShaderResourceView _hvBlurShaderResourceView = null;
        private RenderTargetView _hvBlurRenderTargetView = null;
        private SamplerState _hvBlurSamplerState = null;
        private ShaderResourceView _glowMapShaderResourceView = null;
        private RenderTargetView _glowMapRenderTargetView = null;
        private SamplerState _glowMapSamplerState = null;

        public GlowRTTPostEffect(Device graphicsDevice, IGameApp gameApp)
            : base(graphicsDevice)
        {
            _game = gameApp;
            CreateShaders();
            CreateTargets();
            CreateConstantBuffers();
        }

        public float Sceneness { get; set; }

        public float Glowness { get; set; }

        public override void Apply()
        {
            /////////////////
            //Setup
            /////////////////
            
            //Cache render targets.
            //---------------------
            RenderTargetView previousRenderTarget = null;
            DepthStencilView previousDepthStencil = null;
            previousRenderTarget = GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out previousDepthStencil)[0];                         

            //Cache graphic states and setup new ones.
            //----------------------------------------
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

            //Set the constant variables that will be used by the vertex and pixel shaders on each pass.
            //------------------------------------------------------------------------------------------
            WriteConstants();     
           
            /////////
            //Pass0
            /////////                     
            //Input: InputTexture
            //Output: HBlur         
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer1);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _inputTextureSamplerState);
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
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer1);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _hBlurShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _hBlurSamplerState);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_hvBlurRenderTargetView);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_blurPixelShader);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

	        /////////
            //Pass2
            /////////                     
            //Input: HVBlur
            //Output: GlowMap
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer2);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _hvBlurShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _hvBlurSamplerState);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_glowMapRenderTargetView);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_glowPixelShader);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            //////////
            //Pass3
            //////////
            //Input: GlowMap
            //Output: Final (GlowMap overlayed).
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _glowMapShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, InputTexture);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _glowMapSamplerState);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, _inputTextureSamplerState);            
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(previousRenderTarget);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_glowPixelShader);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            ////////////
            //Restore
            ////////////                      

            //Reset graphic states.
            //---------------------
            GraphicsDevice.ImmediateContext.Rasterizer.State = oldRasterizerState;
            GraphicsDevice.ImmediateContext.OutputMerger.DepthStencilState = oldDepthStencilState;
            GraphicsDevice.ImmediateContext.OutputMerger.BlendState = oldBlendState;

            //Reset render targets.  
            //---------------------
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(previousDepthStencil, previousRenderTarget); 
        }

        private void CreateShaders()
        {
            _vertexShaderByteCode = LoadVertexShader(@"Content\Shaders\glowrtt-horz9-vs.cso", out _horzVertexShader);
            LoadVertexShader(@"Content\Shaders\glowrtt-vert9-vs.cso", out _vertVertexShader);
            LoadPixelShader(@"Content\Shaders\glowrtt-blur.cso", out _blurPixelShader);
            LoadPixelShader(@"Content\Shaders\glowrtt-glow.cso", out _glowPixelShader);
            LoadVertexShader(@"Content\Shaders\passthru-vs.cso", out _passThruVertexShader);
        }

        private void CreateTargets()
        {
            _quad = ContentBuilder.BuildViewportQuad(_game, _vertexShaderByteCode);

            Texture2DDescription textureDesc = new Texture2DDescription();
            textureDesc.ArraySize = 1;
            textureDesc.BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource;
            textureDesc.CpuAccessFlags = CpuAccessFlags.None;
            textureDesc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            textureDesc.Height = _game.RenderTarget.ResourceAs<Texture2D>().Description.Height;
            textureDesc.Width = _game.RenderTarget.ResourceAs<Texture2D>().Description.Width;
            textureDesc.MipLevels = 1;
            textureDesc.OptionFlags = ResourceOptionFlags.None;
            textureDesc.Usage = ResourceUsage.Default;
            textureDesc.SampleDescription.Count = 1;

            ShaderResourceViewDescription resourceViewDesc = new ShaderResourceViewDescription();
            resourceViewDesc.Format = textureDesc.Format;
            resourceViewDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            resourceViewDesc.Texture2D.MostDetailedMip = 0;
            resourceViewDesc.Texture2D.MipLevels = 1;

            RenderTargetViewDescription renderTargetViewDesc = new RenderTargetViewDescription();
            renderTargetViewDesc.Format = textureDesc.Format;
            renderTargetViewDesc.Dimension = RenderTargetViewDimension.Texture2D;
            renderTargetViewDesc.Texture2D.MipSlice = 0;

            SamplerStateDescription samplerStateDesc = SamplerStateDescription.Default();
            samplerStateDesc.Filter = Filter.MinMagMipLinear;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;
            samplerStateDesc.AddressV = TextureAddressMode.Clamp;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;

            //InputTexture (Sampler State)
            //------------            
            _inputTextureSamplerState = new SamplerState(GraphicsDevice, samplerStateDesc);

            //H-Blur
            //------
            Texture2D _hBlurTexture = new Texture2D(GraphicsDevice, textureDesc);
            _hBlurShaderResourceView = new ShaderResourceView(GraphicsDevice, _hBlurTexture, resourceViewDesc);
            _hBlurRenderTargetView = new RenderTargetView(GraphicsDevice, _hBlurTexture, renderTargetViewDesc);
            _hBlurSamplerState = new SamplerState(GraphicsDevice, samplerStateDesc);

            //HV-Blur
            //-------
            Texture2D _hvBlurTexture = new Texture2D(GraphicsDevice, textureDesc);
            _hvBlurShaderResourceView = new ShaderResourceView(GraphicsDevice, _hvBlurTexture, resourceViewDesc);
            _hvBlurRenderTargetView = new RenderTargetView(GraphicsDevice, _hvBlurTexture, renderTargetViewDesc);
            _hvBlurSamplerState = new SamplerState(GraphicsDevice, samplerStateDesc);

            //GlowMap
            //--------
            Texture2D _glowMapTexture = new Texture2D(GraphicsDevice, textureDesc);
            _glowMapShaderResourceView = new ShaderResourceView(GraphicsDevice, _glowMapTexture, resourceViewDesc);
            _glowMapRenderTargetView = new RenderTargetView(GraphicsDevice, _glowMapTexture, renderTargetViewDesc);
            _glowMapSamplerState = new SamplerState(GraphicsDevice, samplerStateDesc);         
        }

        private void CreateConstantBuffers()
        {
            _constantBuffer1 = new SharpDX.Direct3D11.Buffer(GraphicsDevice, ConstantBufferSize1, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _constantBuffer2 = new SharpDX.Direct3D11.Buffer(GraphicsDevice, ConstantBufferSize2, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _constantBuffer3 = new SharpDX.Direct3D11.Buffer(GraphicsDevice, ConstantBufferSize3, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
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

