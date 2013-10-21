using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class BlinnPhongEffect : Effect
    {
        private SharpDX.Direct3D11.Buffer _constantsBuffer = null;
        private int ConstantBufferSize = 160;
        private PixelShader _pixelShader = null;
        private VertexShader _vertexShader = null;
        private byte[] _vertexShaderByteCode = null;

        public Matrix WorldViewProjection { get { return World * View * Projection; } }
        public Vector4 VectorLightDirection { get; set; }
        public Vector4 VectorEye { get; set; }

        public BlinnPhongEffect(Device graphicsDevice)
            : base(graphicsDevice)
        {
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\blinnphong-vs.cso", out _vertexShader);
            LoadPixelShader("Content\\Shaders\\blinnphong-ps.cso", out _pixelShader);
            _constantsBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, ConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        public override void Apply()
        {
            WriteConstants();
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantsBuffer);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);
        }

        public override byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;
        }

        private void WriteConstants()
        {
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_constantsBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, ConstantBufferSize);
            int offset = 0;
            dataBuffer.Set(offset, WorldViewProjection);
            offset += sizeof(float) * 64;
            dataBuffer.Set(offset, World);
            offset += sizeof(float) * 64;
            dataBuffer.Set(offset, VectorLightDirection);
            offset += sizeof(float) * 16;
            dataBuffer.Set(offset, VectorEye);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_constantsBuffer, 0);
        }
    }
}
