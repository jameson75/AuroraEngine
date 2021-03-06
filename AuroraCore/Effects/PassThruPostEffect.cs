using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Utils;
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
    public class PassThruPostEffect : PostEffect
    {
        private Mesh _quad = null;
        private byte[] _vertexShaderByteCode = null;
        private SamplerState _samplerState = null;
        private VertexShader _vertexShaderNoFix = null;
        private PixelShader _pixelShader = null;
        private VertexShader _vertexShaderWithFix = null;       

        public bool EnableTexelFix { get; set; }

        public BlendState BlendState { get; set; }

        public PassThruPostEffect(IGameApp game)
            : base(game)
        {
            string psFileName = "Assets\\Shaders\\postpassthru-ps.cso";
            string vsFileName = "Assets\\Shaders\\postpassthru-vs.cso";
            string vsFixFileName = "Assets\\Shaders\\postpassthru-fix-vs.cso";
            
            //Load Shaders
            //------------
            _vertexShaderByteCode = ReadByteStream(vsFileName);
            _samplerState = new SamplerState(game.GraphicsDevice, SamplerStateDescription.Default());
            _vertexShaderNoFix = new VertexShader(Game.GraphicsDevice, _vertexShaderByteCode);
            _pixelShader = new PixelShader(Game.GraphicsDevice, ReadByteStream(psFileName));
            _vertexShaderWithFix = new VertexShader(Game.GraphicsDevice, ReadByteStream(vsFixFileName));
            
            //Create Scree Quad
            //------------------
            _quad = ContentBuilder.BuildViewportQuad(Game.GraphicsDevice, _vertexShaderByteCode);
        }

        public override void Apply()
        {
            BlendState oldBlendState = null;
            
            //Setup Shaders
            //-------------
            if (!EnableTexelFix)
                Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderNoFix);
            else
                Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderWithFix);
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);            
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _samplerState);
            
            //Setup states
            //------------
            if (BlendState != null)
            {
                oldBlendState = Game.GraphicsDevice.ImmediateContext.OutputMerger.BlendState;
                Game.GraphicsDevice.ImmediateContext.OutputMerger.BlendState = BlendState;
            }

            //Render Screen quad
            //------------------
            _quad.Draw();            
            
            //Clean Up Shader
            //---------------
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);            

            //Clean Up State
            //--------------
            if (BlendState != null)            
                Game.GraphicsDevice.ImmediateContext.OutputMerger.BlendState = oldBlendState;            
        }

        public void Dispose()
        {            
            _samplerState.Dispose();
            _vertexShaderNoFix.Dispose();
            _pixelShader.Dispose();
            _vertexShaderWithFix.Dispose();
            _quad.Dispose();

            _vertexShaderByteCode = null;
            _samplerState = null;
            _vertexShaderNoFix = null;
            _pixelShader = null;
            _vertexShaderWithFix = null;            
            _quad = null;
        }
    }

    //public static class CommonBlendStates
    //{
    //    public static BlendState Create(Device Game.GraphicsDevice, BlendStatePresets preset)
    //    {
    //        BlendStateDescription desc = BlendStateDescription.Default();
    //        switch (preset)
    //        {
    //            case BlendStatePresets.Premultiplied:
    //                desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
    //                desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
    //                desc.RenderTarget[0].SourceBlend = BlendOption.SourceColor;
    //                desc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
    //                desc.RenderTarget[0].DestinationBlend = BlendOption.DestinationColor;
    //                desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
    //                break;
    //            case BlendStatePresets.NonPremultiplied:
    //                desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
    //                desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
    //                desc.RenderTarget[0].SourceBlend = BlendOption.SourceColor;
    //                desc.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
    //                desc.RenderTarget[0].DestinationBlend = BlendOption.DestinationColor;
    //                desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
    //                break;
    //            case BlendStatePresets.Additive:
    //                desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
    //                desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
    //                desc.RenderTarget[0].SourceBlend = BlendOption.SourceColor;
    //                desc.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
    //                desc.RenderTarget[0].DestinationBlend = BlendOption.DestinationColor;
    //                desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.One;
    //                break;
    //            case BlendStatePresets.Multiplicative:
    //                desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
    //                desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
    //                desc.RenderTarget[0].SourceBlend = BlendOption.SourceColor;
    //                desc.RenderTarget[0].SourceAlphaBlend = BlendOption.Zero;
    //                desc.RenderTarget[0].DestinationBlend = BlendOption.DestinationColor;
    //                desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.SourceColor;
    //                break;
    //        }
    //        return new BlendState(graphicsDevice, desc);
    //    }
    //}

    //public enum BlendStatePresets
    //{
    //    Premultiplied,
    //    NonPremultiplied,
    //    Additive,
    //    Multiplicative
    //}
}
