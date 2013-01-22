using System;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core;
using DXBuffer = SharpDX.Direct3D11.Buffer;

namespace CipherPark.AngelJacket.Core.Sandbox
{
    public class TestEffect
    {
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private Device _graphicsDevice = null;
        private DXBuffer _constantBuffer = null;

        public byte[] VertexShaderByteCode { get; private set; }
        public byte[] PixelShaderByteCode { get; private set; }
        public Matrix World { get; set; }
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }

        public TestEffect(Device graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void Load(string compiledVertexShaderFileName, string compiledPixelShaderFileName)
        {
            VertexShaderByteCode = System.IO.File.ReadAllBytes(compiledVertexShaderFileName);
            PixelShaderByteCode = System.IO.File.ReadAllBytes(compiledPixelShaderFileName);
            Load(VertexShaderByteCode, PixelShaderByteCode);
        }

        public void Load(byte[] vertexShaderByteCode, byte[] pixelShaderByteCode)
        {
            VertexShaderByteCode = vertexShaderByteCode;
            PixelShaderByteCode = pixelShaderByteCode;
            _vertexShader = new VertexShader(_graphicsDevice, VertexShaderByteCode);
            _pixelShader = new PixelShader(_graphicsDevice, PixelShaderByteCode);
            _constantBuffer = new DXBuffer(_graphicsDevice, sizeof(float) * 16, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        public void Apply()
        {
            _graphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            _graphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            SetShaderConstants();
            _graphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer);
        }

        private void SetShaderConstants()
        {
            DataBox dataBox = _graphicsDevice.ImmediateContext.MapSubresource(_constantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Matrix worldViewProjection = this.World * this.View * this.Projection;
            worldViewProjection.Transpose();
            Utilities.Write<Matrix>(dataBox.DataPointer, ref worldViewProjection);
            _graphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer, 0);
        }
    }
}