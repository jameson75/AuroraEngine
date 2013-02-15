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
    class PostGlowEffect : PostEffect
    {
        private const int ConstantBufferSize = 32;
        private SharpDX.Direct3D11.Buffer _constantBuffer = null;

        public float GlowSpan { get; set; }
        public float Glowness { get; set; }
        public float Sceneness { get; set; }
        public float ClearDepth { get; set; }
        public Vector4 ClearColor { get; set; }

        public PostGlowEffect(Device graphicsDevice, IGameApp app)
            : base(graphicsDevice)
        {
            _constantBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, ConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        public override void Apply()
        {
            WriteConstants();
        }

        private void WriteConstants()
        {
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, ConstantBufferSize);
            
            int offset = 0;
            dataBuffer.Set(offset, GlowSpan);
            offset += sizeof(float);
            dataBuffer.Set(offset, Glowness);
            offset += sizeof(float);
            dataBuffer.Set(offset, Sceneness);
            offset += sizeof(float);
            dataBuffer.Set(offset, ClearDepth);
            offset += sizeof(float);
            dataBuffer.Set(offset, ClearColor);

            GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer, 0);
        }
    }
}
