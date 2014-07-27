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
        private VertexShader _vertexShaderNoFix = null;
        private PixelShader _pixelShader = null;
        private VertexShader _vertexShaderWithFix = null;       

        public bool EnableTexelFix { get; set; }

        public BlendState BlendState { get; set; }

        public PassThruPostEffect(IGameApp game)
            : base(game)
        {
            string psFileName = "Content\\Shaders\\postpassthru-ps.cso";
            string vsFileName = "Content\\Shaders\\postpassthru-vs.cso";
            string vsFixFileName = "Content\\Shaders\\postpassthru-fix-vs.cso";
            
            //Load Shaders
            //------------
            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            _samplerState = new SamplerState(game.GraphicsDevice, SamplerStateDescription.Default());
            _vertexShaderNoFix = new VertexShader(Game.GraphicsDevice, _vertexShaderByteCode);
            _pixelShader = new PixelShader(Game.GraphicsDevice, System.IO.File.ReadAllBytes(psFileName));
            _vertexShaderWithFix = new VertexShader(Game.GraphicsDevice, System.IO.File.ReadAllBytes(vsFixFileName));
            
            //Create Scree Quad
            //------------------
            _quad = ContentBuilder.BuildBasicViewportQuad(Game.GraphicsDevice, _vertexShaderByteCode);
        }

        public override void Apply()
        {
            BlendState oldBlendState = null;
            
            //Setup Shaders
            //--------------
            if (!EnableTexelFix)
                Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderNoFix);
            else
                Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderWithFix);
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);            
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _samplerState);
            
            //Setup states.
            //-------------
            if (BlendState != null)
            {
                oldBlendState = Game.GraphicsDevice.ImmediateContext.OutputMerger.BlendState;
                Game.GraphicsDevice.ImmediateContext.OutputMerger.BlendState = BlendState;
            }

            //Render Screen quad.
            //-------------------
            _quad.Draw(null);            
            
            //Clean Up Shader
            //----------------
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);            

            //Clean Up State
            //--------------
            if (BlendState != null)            
                Game.GraphicsDevice.ImmediateContext.OutputMerger.BlendState = oldBlendState;            
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
