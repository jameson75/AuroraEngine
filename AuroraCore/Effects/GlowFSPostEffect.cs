using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Content;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public class GlowFSPostEffect : PostEffect
    {      
        private const int ConstantBufferSize = 48;
        private SharpDX.Direct3D11.Buffer _constantBuffer = null;
        private VertexShader _vertexShaderP0 = null;
        private PixelShader _pixelShaderP0P1 = null;
        private VertexShader _vertexShaderP1 = null;
        private VertexShader _vertexShaderP2 = null;
        private PixelShader _pixelShaderP2 = null;
        private byte[] _vertexShaderByteCode = null;
        private RenderTargetView _sceneTextureRenderTargetView = null;
        private ShaderResourceView _sceneTextureShaderResourceView = null;
        private SamplerState _sceneTextureSampler = null;
        private RenderTargetView _glowMap1RenderTargetView = null;
        private ShaderResourceView _glowMap1ShaderResourceView = null;
        private SamplerState _glowMap1Sampler = null;
        private RenderTargetView _glowMap2RenderTargetView = null;
        private ShaderResourceView _glowMap2ShaderResourceView = null;
        private SamplerState _glowMap2Sampler = null;
        private Mesh _quad = null;
        private PassThruPostEffect _passThruEffect = null;

        public float GlowSpan { get; set; }
        public float Glowness { get; set; }
        public float Sceneness { get; set; }
        public float ClearDepth { get; set; }
        public Vector4 ClearColor { get; set; }
        public Vector2 ViewportSize { get; set; }

        public GlowFSPostEffect(IGameApp game)
            : base(game)
        {
            _constantBuffer = new SharpDX.Direct3D11.Buffer(game.GraphicsDevice, ConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _passThruEffect = new PassThruPostEffect(game);
            CreateShaders();
            CreateResources();
        }

        public override void Apply()
        {            
            //***********************************************************************************************************
            //NOTE: This method safe guards against the case where the input shader resource view (InputTexture) is
            //bound to the same texture as the current render target.
            //***********************************************************************************************************                    

            ///////////////////////////////////////////////////////////////////////////////////
            //This line was added in way after the original implementation of this method
            //so that it would be compatible to changes made to the post processing pipeline.            
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(OutputTexture);
            //////////////////////////////////////////////////////////////////////////////////

            /////////////////
            //Setup
            ////////////////
            //Cache render targets.
            RenderTargetView previousRenderTarget = null;
            DepthStencilView previousDepthStencil = null;
            previousRenderTarget = GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out previousDepthStencil)[0];
            //Copy InputTexture (the current rendertarget texture) to the SceneTexture.
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_sceneTextureRenderTargetView);
            _passThruEffect.InputTexture = this.InputTexture;
            _passThruEffect.Apply();
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
            newDepthStencilStateDesc.IsDepthEnabled = true;
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
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderP0);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShaderP0P1);
            _quad.Draw();
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            /////////
            //Pass1
            /////////
            //Input: GlowMap1
            //Output: GlowMap2
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _glowMap1ShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _glowMap1Sampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_glowMap2RenderTargetView);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderP1);
            //NOTE: We're using the same pixel shader from pass0.
            _quad.Draw();
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            //////////
            //Pass2
            //////////
            //Input: GlowMap2, SceneTexture
            //Output: OriginalRenderTarget
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _glowMap2ShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _glowMap2Sampler);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, _sceneTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, _sceneTextureSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(previousRenderTarget);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderP2);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShaderP2);
            _quad.Draw();
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, null);
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
            //Immediately COM Release() swap chain resources.
            previousRenderTarget.Dispose();
            previousDepthStencil.Dispose();
        }

        private void CreateShaders()
        {
            _vertexShaderByteCode = LoadVertexShader("Assets\\Shaders\\postglowfs-x1-vs.cso", out _vertexShaderP0);                       
            LoadPixelShader("Assets\\Shaders\\postglowfs-x1x2-ps.cso", out _pixelShaderP0P1);
            LoadVertexShader("Assets\\Shaders\\postglowfs-x2-vs.cso", out _vertexShaderP1);
            LoadVertexShader("Assets\\Shaders\\postglowfs-x3-vs.cso", out _vertexShaderP2);
            LoadPixelShader("Assets\\Shaders\\postglowfs-x3-ps.cso", out _pixelShaderP2);           
        }

        private void CreateResources()
        {
            _quad = ContentBuilder.BuildViewportQuad(Game.GraphicsDevice, _vertexShaderByteCode);            
            
            Texture2DDescription textureDesc = new Texture2DDescription();
            textureDesc.ArraySize = 1;
            textureDesc.BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource;
            textureDesc.CpuAccessFlags = CpuAccessFlags.None;
            textureDesc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            textureDesc.Height = Game.RenderTargetView.GetTextureDescription().Height;
            textureDesc.Width = Game.RenderTargetView.GetTextureDescription().Width;
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
            
            //SceneTexture
            //------------
            Texture2D _sceneTexture = new Texture2D(Game.GraphicsDevice, textureDesc);
            _sceneTextureShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, _sceneTexture, resourceViewDesc);       
            _sceneTextureRenderTargetView = new RenderTargetView(Game.GraphicsDevice, _sceneTexture, renderTargetViewDesc);  
            _sceneTextureSampler = new SamplerState(Game.GraphicsDevice, samplerStateDesc);         

            //GlowMap1
            //--------
            Texture2D _glowMap1 = new Texture2D(Game.GraphicsDevice, textureDesc);
            _glowMap1ShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, _glowMap1, resourceViewDesc);
            _glowMap1RenderTargetView = new RenderTargetView(Game.GraphicsDevice, _glowMap1, renderTargetViewDesc);
            _glowMap1Sampler = new SamplerState(Game.GraphicsDevice, samplerStateDesc);

            //GlowMap2
            //--------
            Texture2D _glowMap2 = new Texture2D(Game.GraphicsDevice, textureDesc);
            _glowMap2ShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, _glowMap2, resourceViewDesc);
            _glowMap2RenderTargetView = new RenderTargetView(Game.GraphicsDevice, _glowMap2, renderTargetViewDesc);
            _glowMap2Sampler = new SamplerState(Game.GraphicsDevice, samplerStateDesc);
        }

        private void WriteConstants()
        {
            DataBox dataBox = Game.GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, ConstantBufferSize);            
            int offset = 0;
            dataBuffer.Set(offset, GlowSpan);
            offset += sizeof(float);
            dataBuffer.Set(offset, Glowness);
            offset += sizeof(float);
            dataBuffer.Set(offset, Sceneness);
            offset += sizeof(float);
            dataBuffer.Set(offset, ClearDepth);
            offset += sizeof(float);
            dataBuffer.Set(offset, ClearColor);
            offset += (sizeof(float) * 4);
            dataBuffer.Set(offset, ViewportSize);
            Game.GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer, 0);
        }
    }
}
