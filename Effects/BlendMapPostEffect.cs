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
    public class BlendMapEffect : PostEffect
    {
        private const int ConstantBufferSize = 16;
        private IGameApp _game = null;
        private byte[] _vertexShaderByteCode = null;
        private Mesh _quad = null;
        private VertexShader _passThruVertexShader = null;
        private PixelShader _transparencyPixelShader = null;
        private SharpDX.Direct3D11.Buffer _constantBuffer = null;       

        Color TransparencyKey { get; set; }
        public BlendState BlendState { get; set; }
        public ShaderResourceView BlendMap { get; set; }

        public override void Apply()
        {
            //TODO: This method is incomplete. Needs finishing.
            //-------------------------------------------------

            /////////////
            //Pass0
            /////////////
            //Input: Scene Texture
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantBuffer);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_transparencyPixelShader);
            _quad.Draw(0);
        }
    
        public BlendMapEffect(Device graphicsDevice, IGameApp game) : base(graphicsDevice)
        {
            CreateConstantBuffers();
            CreateShaders();
            CreateResources();
        }

        private void CreateResources()
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
        }

        private void CreateShaders()
        {
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\passthru-vs.cso", out _passThruVertexShader);
            LoadPixelShader("Content\\Shaders\\msbloom-guassian-ps.cso", out _transparencyPixelShader);
        }

        private void CreateConstantBuffers()
        {
            _constantBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, ConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        private void WriteShaderConstants()
        {
            DataBox dataBox;         
            dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Vector4 transparencyKey = this.TransparencyKey.ToVector4();
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector4>(dataBox.DataPointer, ref transparencyKey);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer, 0);
        }

        public void InitializeEffectBlendStateFromPreset(BlendStatePresets preset)
        {
            BlendStateDescription desc = BlendStateDescription.Default();
            switch (preset)
            {
                case BlendStatePresets.Premultiplied:
                    desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].SourceBlend = BlendOption.SourceColor;
                    desc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
                    desc.RenderTarget[0].DestinationBlend = BlendOption.DestinationColor;
                    desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
                    break;
                case BlendStatePresets.NonPremultiplied:
                    desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].SourceBlend = BlendOption.SourceColor;
                    desc.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
                    desc.RenderTarget[0].DestinationBlend = BlendOption.DestinationColor;
                    desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
                    break;
                case BlendStatePresets.Additive:
                    desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].SourceBlend = BlendOption.SourceColor;
                    desc.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
                    desc.RenderTarget[0].DestinationBlend = BlendOption.DestinationColor;
                    desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.One;
                    break;
                case BlendStatePresets.Multiplicative:
                    desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].SourceBlend = BlendOption.SourceColor;
                    desc.RenderTarget[0].SourceAlphaBlend = BlendOption.Zero;
                    desc.RenderTarget[0].DestinationBlend = BlendOption.DestinationColor;
                    desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.SourceColor;
                    break;
            }
            BlendState = new BlendState(this.GraphicsDevice, desc);            
        }
    }

    public enum BlendStatePresets
    {
        Premultiplied,
        NonPremultiplied,
        Additive,
        Multiplicative
    }
}
