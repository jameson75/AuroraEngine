﻿using System;
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
    public class PassThruPostEffect : PostEffect
    {
        private Mesh _quad = null;
        private byte[] _vertexShaderByteCode = null;
        private SamplerState _samplerState = null;

        public VertexShader VertexShader { get; protected set; }
        public PixelShader PixelShader { get; protected set; }

        public PassThruPostEffect(Device graphicsDevice, IGameApp game)
            : base(graphicsDevice)
        {
            string psFileName = "Content\\Shaders\\postpassthru-ps.cso";
            string vsFileName = "Content\\Shaders\\postpassthru-vs.cso";
            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            _samplerState = new SamplerState(graphicsDevice, SamplerStateDescription.Default());
            VertexShader = new VertexShader(GraphicsDevice, _vertexShaderByteCode);
            PixelShader = new PixelShader(GraphicsDevice, System.IO.File.ReadAllBytes(psFileName));
            _quad = ContentBuilder.BuildViewportQuad(game, _vertexShaderByteCode);
        }

        public override void Apply()
        {
            GraphicsDevice.ImmediateContext.VertexShader.Set(VertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(PixelShader);            
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _samplerState);
            _quad.Draw(0);
            //Un-bind Texture from pixel shader input so it can possibly be used later as a render target.
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
        }

        public override byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;
        }
    }  
}
