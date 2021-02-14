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
    public class SSAOPostEffect : PostEffect
    {
        private const int ConstantBufferSize = 32;

        private Mesh _quad = null;     
        private byte[] _vertexShaderByteCode = null;
        private SharpDX.Direct3D11.Buffer _constantBuffer = null;
        VertexShader _vertexShader = null;
        PixelShader _pixelShader = null;
        ShaderResourceView _normalsTextureShaderResourceView = null;
        SamplerState _normalsTextureSampler = null;
        ShaderResourceView _positionsTextureShaderResourceView = null;
        SamplerState _positionsTextureSampler = null;
        ShaderResourceView _backPositionsTextureShaderResourceView = null;
        SamplerState _backPositionsTextureSampler = null;
        ShaderResourceView _randomTextureShaderResourceView = null;
        SamplerState _randomTextureSampler = null;

        public float RandomSize { get; set; }
        public float SampleRadius { get; set; }
        public float Intensity { get; set; }
        public float Scale { get; set; }
        public float Bias { get; set; }
        public Vector2 ScreenSize { get; set; }
        public bool EnableBackOcclusion { get; set; }

        public SSAOPostEffect(IGameApp game) : base(game)
        {    
            _constantBuffer = new SharpDX.Direct3D11.Buffer(game.GraphicsDevice, ConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\ssao-vs.cso", out _vertexShader);
            LoadPixelShader("Content\\Shaders\\ssao-ps.cso", out _pixelShader);
            CreateShaderTargets();
        }

        private void CreateShaderTargets()
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

            Texture2D _normalsTexture = new Texture2D(Game.GraphicsDevice, textureDesc);
            _normalsTextureShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, _normalsTexture, resourceViewDesc);
            _normalsTextureSampler = new SamplerState(Game.GraphicsDevice, samplerStateDesc);

            Texture2D _positionsTexture = new Texture2D(Game.GraphicsDevice, textureDesc);
            _positionsTextureShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, _positionsTexture, resourceViewDesc);
            _positionsTextureSampler = new SamplerState(Game.GraphicsDevice, samplerStateDesc);

            Texture2D _backPositionsTexture = new Texture2D(Game.GraphicsDevice, textureDesc);
            _backPositionsTextureShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, _backPositionsTexture, resourceViewDesc);
            _backPositionsTextureSampler = new SamplerState(Game.GraphicsDevice, samplerStateDesc);

            Texture2D _randomTexture = new Texture2D(Game.GraphicsDevice, textureDesc);
            _randomTextureShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, _randomTexture, resourceViewDesc);
            _randomTextureSampler = new SamplerState(Game.GraphicsDevice, samplerStateDesc);
        }

        public override void Apply()
        {
            ///////////////
            //Setup
            ///////////////
            //Cache render targets.
            RenderTargetView previousRenderTarget = null;
            DepthStencilView previousDepthStencil = null;
            previousRenderTarget = Game.GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out previousDepthStencil)[0];
            WriteConstants();
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantBuffer);

            //////////////
            //Pass0
            //////////////            
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _normalsTextureShaderResourceView);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, _positionsTextureShaderResourceView);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(2, _backPositionsTextureShaderResourceView);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(3, _randomTextureShaderResourceView);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _normalsTextureSampler);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, _positionsTextureSampler);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(2, _backPositionsTextureSampler);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(3, _randomTextureSampler);
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(previousRenderTarget);            
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            _quad.Draw();           

            //////////////
            //Clean up
            //////////////
            //Clear constant buffer
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);
            //Clear shader resource views
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(2, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(3, null);
            //Clear shader samplers
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(2, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(3, null);
            //Reset render targets.  
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(previousDepthStencil, previousRenderTarget);
            //COM Release swap chain resource.
            previousRenderTarget.Dispose();
            previousDepthStencil.Dispose(); ;

            base.Apply();
        }

        private void WriteConstants()
        {
            DataBox dataBox = Game.GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, ConstantBufferSize);
            int offset = 0;
            dataBuffer.Set(offset, RandomSize);
            offset += sizeof(float);
            dataBuffer.Set(offset, SampleRadius);
            offset += sizeof(float);
            dataBuffer.Set(offset, Intensity);
            offset += sizeof(float);
            dataBuffer.Set(offset, Scale);
            offset += sizeof(float);
            dataBuffer.Set(offset, Bias);
            offset += sizeof(float);
            dataBuffer.Set(offset, ScreenSize);
            offset += (sizeof(float) * 2);
            dataBuffer.Set(offset, EnableBackOcclusion);
            offset += sizeof(bool);
            Game.GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer, 0);
        }
    }
}
