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

        public SHLightingEffect(Device graphicsDevice)
            : base(graphicsDevice)
        {
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\sh-vs.cso", out _vertexShader);
            LoadPixelShader("Content\\Shaders\\sh-ps.cso", out _pixelShader);
            _constantsBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, ConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }        

        public void Apply()
        {
            WriteConstants();
            _graphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantsBuffer);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);       
            _graphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);        
        }

        public byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;
        }

        private void WriteConstants()
        {
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_constantsBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
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
            GraphicsDevice.ImmediateContext.UnmapSubresource(_constantsBuffer, 0);
        }
    }
}
