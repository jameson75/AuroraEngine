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
        private SharpDX.Direct3D11.Buffer _vertexConstantsBuffer = null;
        private SharpDX.Direct3D11.Buffer _pixelConstantsBuffer = null;
        private int VertexConstantBufferSize = 1040;
        private int PixelConstantBufferSize = 208;
        private PixelShader _pixelShader = null;
        private VertexShader _vertexShader = null;
        private byte[] _vertexShaderByteCode = null;

        public Matrix WorldInverseTranspose
        {
            get { return Matrix.Invert(Matrix.Transpose(this.World)); }
        }

        public Matrix ViewInverse
        {
            get { return Matrix.Invert(this.View); }
        }

        public Matrix WorldViewProjection
        {
            get { return World * View * Projection; }
        }

        public Vector3 LampDirPos { get; set; }

        public Color LampColor { get; set; }

        public Color AmbientColor { get; set; }

        public float SpecularPower { get; set; }

        public float Eccentricity { get; set; }

        public BlinnPhongEffect(Device graphicsDevice)
            : base(graphicsDevice)
        {
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\blinnphong-vs.cso", out _vertexShader);
            LoadPixelShader("Content\\Shaders\\blinnphong-ps.cso", out _pixelShader);
            _vertexConstantsBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, VertexConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _pixelConstantsBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, PixelConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        public override void Apply()
        {
            WriteVertexConstants();
            WritePixelConstants();
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _vertexConstantsBuffer);            
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _pixelConstantsBuffer);            
        }

        public override byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;
        }

        private void WriteVertexConstants()
        {
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_vertexConstantsBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, VertexConstantBufferSize);
            int offset = 0;
            Matrix worldITXf = this.WorldInverseTranspose;
            worldITXf.Transpose();
            dataBuffer.Set(offset, worldITXf);
            offset += sizeof(float) * 64;
            Matrix world = this.World;
            world.Transpose();
            dataBuffer.Set(offset, world);
            offset += sizeof(float) * 64;
            Matrix viewIXf = this.ViewInverse;
            viewIXf.Transpose();
            dataBuffer.Set(offset, viewIXf);
            offset += sizeof(float) * 64;            
            Matrix wvpXf = this.WorldViewProjection;
            wvpXf.Transpose();            
            dataBuffer.Set(offset, wvpXf);
            offset += sizeof(float) * 64;
            dataBuffer.Set(offset, new Vector4(LampDirPos, 1.0f));            
            GraphicsDevice.ImmediateContext.UnmapSubresource(_vertexConstantsBuffer, 0);
        }

        private void WritePixelConstants()
        {
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_pixelConstantsBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, PixelConstantBufferSize);
            int offset = 0;         
            dataBuffer.Set(offset, LampColor.ToVector4());
            offset += sizeof(float) * 16;                        
            dataBuffer.Set(offset, AmbientColor.ToVector4());
            offset += sizeof(float) * 16;            
            dataBuffer.Set(offset, SpecularPower);
            offset += sizeof(float) * 16;
            dataBuffer.Set(offset, Eccentricity);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_pixelConstantsBuffer, 0);
        }
    }
}
