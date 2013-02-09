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
    public class PlexicNodeEffect
    {
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private byte[] _vertexShaderByteCode = null;
        private Device _graphicsDevice = null;
        private SharpDX.Direct3D11.Buffer _constantBuffer = null;

        public Device GraphicsDevice { get { return _graphicsDevice; } }

        public Matrix World { get; set; }
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }
        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }

        public PlexicNodeEffect(Device graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            ForegroundColor = Color.Transparent;
            BackgroundColor = Color.Transparent;
            World = Matrix.Identity;
            View = Matrix.Identity;
            Projection = Matrix.Identity;

            string psFileName = "Content\\Shaders\\plexicnode-ps.cso";
            string vsFileName = "Content\\Shaders\\plexicnode-vs.cso";
            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            _vertexShader = new VertexShader(GraphicsDevice, _vertexShaderByteCode);
            _pixelShader = new PixelShader(GraphicsDevice, System.IO.File.ReadAllBytes(psFileName));
            int bufferSize = sizeof(float) * 16 + sizeof(float) * 4 * 2; //size of WorldViewProj + ForegroundColor + BackgroundColor
            _constantBuffer = new SharpDX.Direct3D11.Buffer(graphicsDevice, bufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        public void Apply()
        {
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            SetShaderConstants();
            _graphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer);
            _graphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantBuffer);
        }

        public byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;
        }

        private void SetShaderConstants()
        {
            DataBox dataBox = _graphicsDevice.ImmediateContext.MapSubresource(_constantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Matrix worldViewProjection = this.World * this.View * this.Projection;
            worldViewProjection.Transpose();
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref worldViewProjection);
            Vector4 vForegroundColor = ForegroundColor.ToVector4();
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector4>(dataBox.DataPointer, ref vForegroundColor);
            Vector4 vBackgroundColor = BackgroundColor.ToVector4();
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector4>(dataBox.DataPointer, ref vBackgroundColor);
            _graphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer, 0);
        }
    }
}
