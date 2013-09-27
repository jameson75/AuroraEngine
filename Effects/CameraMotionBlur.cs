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
    public class CameraMotionBlur : PostEffect
    {
        IGameApp _game = null;
        private byte[] _vertexShaderByteCode = null;
        private VertexShader _sumVertexShader = null;
        private PixelShader _sumPixelShader = null;
        private VertexShader _vs2 = null;
        private PixelShader _ps2 = null;
        private VertexShader _vs3 = null;
        private PixelShader _ps3 = null;
        private Mesh _quad = null;

        public CameraMotionBlur(Device graphicsDevice, IGameApp game) : base(graphicsDevice)
        {
            _game = game;
            CreateConstantBuffers();
            CreateResources();
            CreateShaders();            
        }

        public override void Apply()
        {
            ///////////////////////////////
            //Setup
            ///////////////////////////////
            WriteShaderConstants();
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantsBuffer);
            RenderTargetView originalRenderTarget = null;
            DepthStencilView originalDepthStencilView = null;
            originalRenderTarget = GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out originalDepthStencilView)[0];

            ////////////////////////////////
            //Pass0
            //Accumulate
            ////////////////////////////////
            ShaderResourceView baseMap = InputTexture;
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _inputTextureSampler);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, _sumTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, _sumTextureSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_tempTextureRenderTargetView);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_sumVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_sumPixelShader);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            ///////////////////////////////////////////////
            //Pass1
            //Store Accumulation for next call to Apply().
            ///////////////////////////////////////////////
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _tempTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _tempTextureSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_sumTextureRenderTargetView);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vs2);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_ps2);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);

            ///////////////////////////////////////////////
            //Pass2
            //Store Accumulation for next call to Apply().
            ///////////////////////////////////////////////
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _sumTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _sumTextureSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(originalRenderTarget);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vs3);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_ps3);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);

            ///////////////////////////////////
            //Clean up
            ///////////////////////////////////
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(originalDepthStencilView, originalRenderTarget);
        }

        public override byte[] SelectShaderByteCode()
        {
            return null;   
        }

        private void CreateConstantBuffers()
        {

        }

        private void CreateShaders()
        {

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

        private void WriteShaderConstants()
        {

        }
    }
}
