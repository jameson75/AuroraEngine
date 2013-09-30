using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
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
    public class PixelVelocityBlur : PostEffect
    {
        private IGameApp _game = null;
        private byte[] _vertexShaderByteCode = null;
        private VertexShader _worldVertexShader = null;
        private PixelShader _worldVelocityPixelShader = null;
        private PixelShader _worldColorPixelShader = null;
        private PixelShader _blurPixelShader = null;
        private SharpDX.Direct3D11.Buffer _worldVertexConstantBuffer = null;
        private SharpDX.Direct3D11.Buffer _blurConstantBuffer = null;
        private RenderTargetView _meshTextureRenderTargetView = null;
        private ShaderResourceView _meshTextureShaderResourceView = null;        
        private SamplerState _meshTextureSampler = null;
        private RenderTargetView _currentVelocityTextureRenderTargetView = null;
        private ShaderResourceView _currentVelocityTextureShaderResourceView = null;
        private SamplerState _currentVelocityTextureSampler = null;
        private RenderTargetView _lastVelocityTextureRenderTargetView = null;
        private ShaderResourceView _lastVelocityTextureShaderResourceView = null;
        private SamplerState _lastVelocityTextureSampler = null;

        public PixelVelocityBlur(Device graphicsDevice, IGameApp game)
            : base(graphicsDevice)
        {
            _game = game;
            CreateConstantBuffers();
            CreateTextures();
            CreateShaders();
        }

        public override void Apply()
        {            
            base.Apply();
        }

        private void CreateConstantBuffers()
        {

        }

        private void CreateTextures()
        {
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

            Texture2DDescription highPercisionTextureDesc = textureDesc;
            highPercisionTextureDesc.Format = SharpDX.DXGI.Format.R16G16_Float;

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

        }

        private void CreateShaders()
        {
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\pixel-velocity-blur-calc-vs.cso", out _worldVertexShader);
            LoadPixelShader("Content\\Shaders\\pixel-velocity-blur-clr-ps.cso", out _worldColorPixelShader);
            LoadPixelShader("Content\\Shaders\\pixel-velocity-blur-vel-ps.cso", out _worldVelocityPixelShader);
            LoadPixelShader("Content\\Shaders\\pixel-velocity-blur-blr-ps.cso", out _blurPixelShader);  
        }
    }
}
