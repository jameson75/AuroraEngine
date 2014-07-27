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

        public GlowRTTPostEffect(IGameApp game)
            : base(game)
        {
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
            previousRenderTarget = Game.GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out previousDepthStencil)[0];                         

            //Cache graphic states and setup new ones.
            //----------------------------------------
            RasterizerState oldRasterizerState = Game.GraphicsDevice.ImmediateContext.Rasterizer.State;
            RasterizerStateDescription newRasterizerStateDesc = (oldRasterizerState != null) ? oldRasterizerState.Description : RasterizerStateDescription.Default();
            newRasterizerStateDesc.CullMode = CullMode.None;
            Game.GraphicsDevice.ImmediateContext.Rasterizer.State = new RasterizerState(GraphicsDevice, newRasterizerStateDesc);
            DepthStencilState oldDepthStencilState = Game.GraphicsDevice.ImmediateContext.OutputMerger.DepthStencilState;
            DepthStencilStateDescription newDepthStencilStateDesc = (oldDepthStencilState != null) ? oldDepthStencilState.Description : DepthStencilStateDescription.Default();
            newDepthStencilStateDesc.IsDepthEnabled = false;
            newDepthStencilStateDesc.DepthWriteMask = 0;
            Game.GraphicsDevice.ImmediateContext.OutputMerger.DepthStencilState = new DepthStencilState(GraphicsDevice, newDepthStencilStateDesc);
            BlendState oldBlendState = Game.GraphicsDevice.ImmediateContext.OutputMerger.BlendState;
            BlendStateDescription newBlendStateDesc = (oldBlendState != null) ? oldBlendState.Description : BlendStateDescription.Default();
            for (int i = 0; i < newBlendStateDesc.RenderTarget.Length; i++)
                newBlendStateDesc.RenderTarget[i].IsBlendEnabled = false;
            Game.GraphicsDevice.ImmediateContext.OutputMerger.BlendState = new BlendState(GraphicsDevice, newBlendStateDesc);

            //Set the constant variables that will be used by the vertex and pixel shaders on each pass.
            //------------------------------------------------------------------------------------------
            WriteConstants();     
           
            /////////
            //Pass0
            /////////                     
            //Input: InputTexture
            //Output: HBlur         
            Game.GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer1);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _inputTextureSamplerState);
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_hBlurRenderTargetView);
            Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_horzVertexShader);
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_blurPixelShader);
            _quad.Draw(null);
            Game.GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

		    /////////
            //Pass1
            /////////                     
            //Input: HBlur
            //Output: HVBlur
            Game.GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer1);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _hBlurShaderResourceView);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _hBlurSamplerState);
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_hvBlurRenderTargetView);
            Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_vertVertexShader);
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_blurPixelShader);
            _quad.Draw(null);
            Game.GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

	        /////////
            //Pass2
            /////////                     
            //Input: HVBlur
            //Output: GlowMap
            Game.GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer2);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _hvBlurShaderResourceView);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _hvBlurSamplerState);
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_glowMapRenderTargetView);
            Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruVertexShader);
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_glowPixelShader);
            _quad.Draw(null);
            Game.GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            //////////
            //Pass3
            //////////
            //Input: GlowMap
            //Output: Final (GlowMap overlayed).
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _glowMapShaderResourceView);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, InputTexture);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _glowMapSamplerState);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, _inputTextureSamplerState);
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(previousRenderTarget);
            Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruVertexShader);
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_glowPixelShader);
            _quad.Draw(null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, null);
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            ////////////
            //Restore
            ////////////                      

            //Reset graphic states.
            //---------------------
            Game.GraphicsDevice.ImmediateContext.Rasterizer.State = oldRasterizerState;
            Game.GraphicsDevice.ImmediateContext.OutputMerger.DepthStencilState = oldDepthStencilState;
            Game.GraphicsDevice.ImmediateContext.OutputMerger.BlendState = oldBlendState;

            //Reset render targets.  
            //---------------------
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(previousDepthStencil, previousRenderTarget); 
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
            _quad = ContentBuilder.BuildBasicViewportQuad(Game.GraphicsDevice, _vertexShaderByteCode);

            Texture2DDescription textureDesc = new Texture2DDescription();
            textureDesc.ArraySize = 1;
            textureDesc.BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource;
            textureDesc.CpuAccessFlags = CpuAccessFlags.None;
            textureDesc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            textureDesc.Height = Game.RenderTarget.ResourceAs<Texture2D>().Description.Height;
            textureDesc.Width =  Game.RenderTarget.ResourceAs<Texture2D>().Description.Width;
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
            _inputTextureSamplerState = new SamplerState(Game.GraphicsDevice, samplerStateDesc);

            //H-Blur
            //------
            Texture2D _hBlurTexture = new Texture2D(Game.GraphicsDevice, textureDesc);
            _hBlurShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, _hBlurTexture, resourceViewDesc);
            _hBlurRenderTargetView = new RenderTargetView(Game.GraphicsDevice, _hBlurTexture, renderTargetViewDesc);
            _hBlurSamplerState = new SamplerState(Game.GraphicsDevice, samplerStateDesc);

            //HV-Blur
            //-------
            Texture2D _hvBlurTexture = new Texture2D(Game.GraphicsDevice, textureDesc);
            _hvBlurShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, _hvBlurTexture, resourceViewDesc);
            _hvBlurRenderTargetView = new RenderTargetView(Game.GraphicsDevice, _hvBlurTexture, renderTargetViewDesc);
            _hvBlurSamplerState = new SamplerState(Game.GraphicsDevice, samplerStateDesc);

            //GlowMap
            //--------
            Texture2D _glowMapTexture = new Texture2D(Game.GraphicsDevice, textureDesc);
            _glowMapShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, _glowMapTexture, resourceViewDesc);
            _glowMapRenderTargetView = new RenderTargetView(Game.GraphicsDevice, _glowMapTexture, renderTargetViewDesc);
            _glowMapSamplerState = new SamplerState(Game.GraphicsDevice, samplerStateDesc);         
        }

        private void CreateConstantBuffers()
        {
            _constantBuffer1 = new SharpDX.Direct3D11.Buffer(Game.GraphicsDevice, ConstantBufferSize1, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _constantBuffer2 = new SharpDX.Direct3D11.Buffer(Game.GraphicsDevice, ConstantBufferSize2, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _constantBuffer3 = new SharpDX.Direct3D11.Buffer(Game.GraphicsDevice, ConstantBufferSize3, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
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
            dataBox = Game.GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer1, 0, MapMode.WriteDiscard, MapFlags.None);            
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector2>(dataBox.DataPointer, ref quadTexelOffset);
            Game.GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer1, 0);

            //Write to Guassian Horizontal Buffer
            dataBox = Game.GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer2, 0, MapMode.WriteDiscard, MapFlags.None);
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector2>(dataBox.DataPointer, ref quadScreenSize);
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector2>(dataBox.DataPointer, ref quadTexelOffset);
            Game.GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer2, 0);

            //Write to Guassian Horizontal Buffer
            dataBox = Game.GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer3, 0, MapMode.WriteDiscard, MapFlags.None);            
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref sceneness);
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref glowness);
            Game.GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer3, 0);        
        }
    }
}

