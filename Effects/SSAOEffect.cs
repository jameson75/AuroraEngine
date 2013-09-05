using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Content;

namespace CipherPark.AngelJacket.Core.Effects
{
    public class SSAOEffect : PostEffect
    {
        public override void Apply()
        {
            ///////////////
            //Setup
            ///////////////
            //Cache render targets.
            RenderTargetView previousRenderTarget = null;
            DepthStencilView previousDepthStencil = null;
            previousRenderTarget = GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out previousDepthStencil)[0];
          
            //////////////
            //Pass0
            //////////////
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, CreateConstantBuffer());
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _normalsTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, _positionsTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(2, _backPositionsTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(3, _randomTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _normalsTextureSampler);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, _positionsTextureSampler);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(2, _backPositionsSampler);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(3, _randomTextureSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(previousRenderTarget);            
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShaderP0P1);
            _quad.Draw(0);           

            //////////////
            //Clean up
            //////////////
            //Clear constant buffer
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);
            //Clear shader resource views
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(2, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(3, null);
            //Clear shader samplers
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(2, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(3, null);
            //Reset render targets.  
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(previousDepthStencil, previousRenderTarget);

            base.Apply();
        }
    }
}
