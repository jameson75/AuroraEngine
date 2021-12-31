using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;

namespace CipherPark.AngelJacket.Core.Effects
{
    /*
    public class GlitchPostEffect : PostEffect
    {
        public float RedChannelOffset { get; set; }
        public float BlueChannelOffset { get; set; }
        public float GreenChannelOffset { get; set; }
        public bool EnableRGBShift { get; set; }

        public GlitchPostEffect(Device graphicsDevice, IGameApp app)
             : base(graphicsDevice)
        {

        }

        private void CreatShaders()
        {
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\postglowfs-x1-vs.cso", out _vertexShader);
            LoadPixelShader("Content\\Shaders\\postglowfs-x1x2-ps.cso", out _pixelShader);      
        }

        public override void Apply()
        {
            //Cache render targets.
            RenderTargetView previousRenderTarget = null;
            DepthStencilView previousDepthStencil = null;
            previousRenderTarget = GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out previousDepthStencil)[0];

            foreach (RGBSplitChannel channel in _rgbSplitChannels)
            {
 eContext.PixelShader.SetShaderResource(0, InputTexture);
                GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _sceneTextureSampler);
                GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_glowMap1RenderTargetView);
                GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
                GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
                
                _quad.Draw(null);
                               WriteRGBShiftConstants();

                GraphicsDevice.Immediat
                GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
                GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
                GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);
                GraphicsDevice.ImmediateContext.VertexShader.Set(null);
                GraphicsDevice.ImmediateContext.PixelShader.Set(null);
            }
        }

        private void WriteRGBShiftConstants()
        {
            
        }
    }
    */
}