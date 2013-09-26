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
        private byte[] _shaderByteCode = null;
        private VertexShader _vs1 = null;
        private PixelShader _ps1 = null;
        private VertexShader _vs2 = null;
        private PixelShader _ps2 = null;
        private VertexShader _vs3 = null;
        private PixelShader _ps3 = null;
        private Mesh _screenQuad = null;

        public CameraMotionBlur(Device graphicsDevice, IGameApp game) : base(graphicsDevice)
        {
            _game = game;
            CreateShaders();
            CreateShaderTargets();
        }

        public override void Apply()
        {
            /////////////////////////////////
            ////Setup
            /////////////////////////////////
            //WriteShaderConstants();
            //GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantsBuffer);
            //RenderTargetView originalRenderTarget = null;
            //DepthStencilView originalDepthStencilView = null;
            //originalRenderTarget = GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out originalDepthStencilView)[0];
  
            //////////////////////////////////
            ////Pass0
            ////Accumulate
            //////////////////////////////////
            //ShaderResourceView baseMap = InputTexture;
            //GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            //GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _inputTextureSampler);
            //GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, _sumTextureShaderResourceView);
            //GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, _sumTextureSampler);
            //GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_tempTextureRenderTargetView);
            //GraphicsDevice.ImmediateContext.VertexShader.Set(_vs1);
            //GraphicsDevice.ImmediateContext.PixelShader.Set(_ps1);
            //_screenQuad.Draw(0);
            //GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            //GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            //GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, null);
            //GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, null);
            //GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            /////////////////////////////////////////////////
            ////Pass1
            ////Store Accumulation for next call to Apply().
            /////////////////////////////////////////////////
            //GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _tempTextureShaderResourceView);
            //GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _tempTextureSampler);
            //GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_sumTextureRenderTargetView);
            //GraphicsDevice.ImmediateContext.VertexShader.Set(_vs2);
            //GraphicsDevice.ImmediateContext.PixelShader.Set(_ps2);
            //_screenQuad.Draw(0);
            //GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            //GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);

            /////////////////////////////////////////////////
            ////Pass1
            ////Store Accumulation for next call to Apply().
            /////////////////////////////////////////////////
            //GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _sumTextureShaderResourceView);
            //GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _sumTextureSampler);
            //GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(originalRenderTarget);
            //GraphicsDevice.ImmediateContext.VertexShader.Set(_vs3);
            //GraphicsDevice.ImmediateContext.PixelShader.Set(_ps3);
            //_screenQuad.Draw(0);
            //GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            //GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);

            /////////////////////////////////////
            ////Clean up
            /////////////////////////////////////
            //GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);
            //GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(originalDepthStencilView, originalRenderTarget);
        }

        public override byte[] SelectShaderByteCode()
        {
            return null;   
        }

        private void CreateShaders()
        {

        }

        private void CreateShaderTargets()
        {

        }

        private void WriteShaderConstants()
        {

        }
    }
}
