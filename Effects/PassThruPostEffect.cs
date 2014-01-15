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
    public class PassThruPostEffect : PostEffect
    {
        private Mesh _quad = null;
        private byte[] _vertexShaderByteCode = null;
        private SamplerState _samplerState = null;

        public VertexShader VertexShader { get; protected set; }
        public PixelShader PixelShader { get; protected set; }
        public VertexShader FixVertexShader { get; protected set; }
        public bool EnableTexelFix { get; set; }
        public BlendState BlendState { get; set; }

        public PassThruPostEffect(Device graphicsDevice, IGameApp game)
            : base(graphicsDevice)
        {
            string psFileName = "Content\\Shaders\\postpassthru-ps.cso";
            string vsFileName = "Content\\Shaders\\postpassthru-vs.cso";
            string vsFixFileName = "Content\\Shaders\\postpassthru-fix-vs.cso";
            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            _samplerState = new SamplerState(graphicsDevice, SamplerStateDescription.Default());
            VertexShader = new VertexShader(GraphicsDevice, _vertexShaderByteCode);
            PixelShader = new PixelShader(GraphicsDevice, System.IO.File.ReadAllBytes(psFileName));
            FixVertexShader = new VertexShader(GraphicsDevice, System.IO.File.ReadAllBytes(vsFixFileName));
            _quad = ContentBuilder.BuildBasicViewportQuad(game, _vertexShaderByteCode);
        }

        public override void Apply()
        {
            BlendState oldBlendState = null;
            if (BlendState != null)
            {
                oldBlendState = GraphicsDevice.ImmediateContext.OutputMerger.BlendState;
                GraphicsDevice.ImmediateContext.OutputMerger.BlendState = BlendState;
            }
            if (!EnableTexelFix)
                GraphicsDevice.ImmediateContext.VertexShader.Set(VertexShader);
            else
                GraphicsDevice.ImmediateContext.VertexShader.Set(FixVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(PixelShader);            
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _samplerState);
            _quad.Draw(0);            
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            if (oldBlendState != null)            
                GraphicsDevice.ImmediateContext.OutputMerger.BlendState = oldBlendState;            
        }

        public override byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;
        }
    }

    public static class CommonBlendStates
    {
        public static BlendState Create(Device graphicsDevice, BlendStatePresets preset)
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
            return new BlendState(graphicsDevice, desc);
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
