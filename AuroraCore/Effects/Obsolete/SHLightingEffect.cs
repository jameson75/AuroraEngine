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

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Effects
{
    public class SHLightingEffect : Effect
    {
        private SharpDX.Direct3D11.Buffer _constantsBuffer = null;
        private int ConstantBufferSize = 160;
        private PixelShader _pixelShader = null;
        private VertexShader _vertexShader = null;
        private byte[] _vertexShaderByteCode = null;

        public Matrix WorldViewProjection { get; set; }
        public Matrix RedSHCoefficients { get; set; }
        public Matrix GreenSHCoefficients { get; set; }
        public Matrix BlueSHCoefficients { get; set; }
        public Matrix WorldView { get; set; }

        public SHLightingEffect(IGameApp game)
            : base(game)
        {
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\sh-vs.cso", out _vertexShader);
            LoadPixelShader("Content\\Shaders\\sh-ps.cso", out _pixelShader);
            _constantsBuffer = new SharpDX.Direct3D11.Buffer(game.GraphicsDevice, ConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }        

        public override void Apply()
        {
            WriteConstants();
            Game.GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantsBuffer);
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);        
        }     

        private void WriteConstants()
        {
            DataBox dataBox = Game.GraphicsDevice.ImmediateContext.MapSubresource(_constantsBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, ConstantBufferSize);
            int offset = 0;
            dataBuffer.Set(offset, WorldViewProjection);
            offset += sizeof(float) * 16;
            dataBuffer.Set(offset, RedSHCoefficients);
            offset += sizeof(float) * 16;
            dataBuffer.Set(offset, GreenSHCoefficients);
            offset += sizeof(float) * 16;
            dataBuffer.Set(offset, BlueSHCoefficients);
            offset += sizeof(float) * 16;
            dataBuffer.Set(offset, WorldView);           
            Game.GraphicsDevice.ImmediateContext.UnmapSubresource(_constantsBuffer, 0);
        }
    }
}
