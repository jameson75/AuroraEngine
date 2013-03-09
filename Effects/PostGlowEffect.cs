using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Effects
{
    class PostGlowEffect : PostEffect
    {
        private IGameApp _game = null;
        private const int ConstantBufferSize = 32;
        private SharpDX.Direct3D11.Buffer _constantBuffer = null;
        private VertexShader _vertexShaderP0 = null;
        private PixelShader _pixelShaderP0P1 = null;
        private VertexShader _vertexShaderP1 = null;
        private VertexShader _vertexShaderP2 = null;
        private PixelShader _pixelShaderP2 = null;
        private VertexShader _vertexShaderP3 = null;
        private PixelShader _pixelShaderP3 = null;
        private byte[] _vertexShaderByteCode = null;
        private RenderTargetView _tempRenderTarget = null;
        private ShaderResourceView _tempShaderResource = null;
        private SamplerState _tempSamplerState = null;
        private Mesh _quad = null;

        public float GlowSpan { get; set; }
        public float Glowness { get; set; }
        public float Sceneness { get; set; }
        public float ClearDepth { get; set; }
        public Vector4 ClearColor { get; set; }

        public PostGlowEffect(Device graphicsDevice, IGameApp app)
            : base(graphicsDevice)
        {
            _game = app;
            _constantBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, ConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            CreateShaders();
            CreateShaderTargets();
        }

        public override void Apply()
        {
            //********************************************************************************************
            //NOTE: This method assumes that the input shader resource view (InputTexture) is bound to the same texture
            //as the current render target.
            //********************************************************************************************

            RenderTargetView previousRenderTarget = null;
            DepthStencilView previousDepthStencil = null;
            previousRenderTarget = GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out previousDepthStencil)[0];

            WriteConstants();
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantBuffer);
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer);

            /////////
            //Pass0
            /////////
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_tempRenderTarget);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderP0);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShaderP0P1);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);
            
            /////////
            //Pass1
            /////////
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _tempShaderResource);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(previousRenderTarget);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderP1);
            //NOTE: We're using the same pixel shader from pass0.
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            //////////
            //Pass2
            //////////
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_tempRenderTarget);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderP2);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShaderP2);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            //////////
            //Pass3
            //////////
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _tempShaderResource);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(previousRenderTarget);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderP3);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShaderP3);

            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, null);

            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(previousDepthStencil, previousRenderTarget);
        }

        private void CreateShaders()
        {
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\postglowfs-x1-vs.cso", out _vertexShaderP0);                       
            LoadPixelShader("Content\\Shaders\\postglowfs-x1x2-ps.cso", out _pixelShaderP0P1);
            LoadVertexShader("Content\\Shaders\\postflowfs-x2-vs.cso", out _vertexShaderP1);
            LoadVertexShader("Content\\Shaders\\postflowfs-x3-vs.cso", out _vertexShaderP2);
            LoadPixelShader("Content\\Shaders\\postflowfs-x3-ps.cso", out _pixelShaderP2);
            LoadVertexShader("Content\\Shaders\\postflowfs-x4-vs.cso", out _vertexShaderP3);
            LoadPixelShader("Content\\Shaders\\postflowfs-x4-ps.cso", out _pixelShaderP3);
        }

        private void CreateShaderTargets()
        {
            _quad = ContentBuilder.BuildViewportQuad(_game, _vertexShaderByteCode);
            
            SamplerStateDescription samplerDesc = SamplerStateDescription.Default();
            samplerDesc.Filter = Filter.MinMagMipLinear;
            samplerDesc.AddressU = TextureAddressMode.Clamp;
            samplerDesc.AddressV = TextureAddressMode.Clamp;
            samplerDesc.AddressU = TextureAddressMode.Clamp;
            _tempSamplerState = new SamplerState(GraphicsDevice, samplerDesc);

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

            Texture2D tempTexture = new Texture2D(_game.GraphicsDevice, textureDesc);
            RenderTargetViewDescription targetDesc = new RenderTargetViewDescription();
            targetDesc.Format = textureDesc.Format;
            targetDesc.Dimension = RenderTargetViewDimension.Texture2D;
            targetDesc.Texture2D.MipSlice = 0;
            _tempRenderTarget = new RenderTargetView(_game.GraphicsDevice, tempTexture, targetDesc);
            ShaderResourceViewDescription resourceDesc = new ShaderResourceViewDescription();
            resourceDesc.Format = targetDesc.Format;
            resourceDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            resourceDesc.Texture2D.MostDetailedMip = 0;
            resourceDesc.Texture2D.MipLevels = 1;
            _tempShaderResource = new ShaderResourceView(GraphicsDevice, tempTexture, resourceDesc);           
        }

        private void WriteConstants()
        {
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
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

            GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer, 0);
        }
    }
}
