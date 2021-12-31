using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.Animation;


namespace CipherPark.Aurora.Core.Effects
{
    public class EdgeDraw : SurfaceEffect
    {
        public int VertexShaderConstantsBufferSize = 512;
        public int GeometryShaderConstantsBufferSize = 512;
        
        private GeometryShader _geometryShader = null;
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private SharpDX.Direct3D11.Buffer _vertexShaderConstants = null;
        private SharpDX.Direct3D11.Buffer _geometryShaderConstants = null;
        private byte[] _shaderByteCode = null;

        public EdgeDraw(IGameApp game)
            : base(game)
        {
            _shaderByteCode = LoadVertexShader("Content\\Shaders\\edgedraw-vs.cso", out _vertexShader);
            LoadGeometryShader("Content\\Shaders\\edgedraw-gs.cso", out _geometryShader);
            LoadPixelShader("Content\\Shaders\\edgedraw-ps.cso", out _pixelShader);
            _vertexShaderConstants = new SharpDX.Direct3D11.Buffer(GraphicsDevice, VertexShaderConstantsBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _geometryShaderConstants = new SharpDX.Direct3D11.Buffer(GraphicsDevice, GeometryShaderConstantsBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);           
        }

        public override void Apply()
        {
            WriteVertexConstants();
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _vertexShaderConstants);

            WriteGeometryConstants();
            GraphicsDevice.ImmediateContext.GeometryShader.SetConstantBuffer(0, _geometryShaderConstants);

            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);

            GraphicsDevice.ImmediateContext.GeometryShader.Set(_geometryShader);

            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
        }

        public override byte[] GetVertexShaderByteCode()
        {
            return _shaderByteCode;
        }

        private void WriteVertexConstants()
        {
            //Open Access to Buffer
            //---------------------
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_vertexShaderConstants, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, VertexShaderConstantsBufferSize);
            int offset = 0;

            //matWorldViewProj
            //----------------
            Matrix worldViewProj = this.World * this.View * this.Projection;
            worldViewProj.Transpose();
            dataBuffer.Set(offset, worldViewProj);
            offset += sizeof(float) * 16;

            //Close Access to Buffer
            //----------------------
            GraphicsDevice.ImmediateContext.UnmapSubresource(_vertexShaderConstants, 0);
        }

        private void WriteGeometryConstants()
        {
            //Open Access to Buffer
            //---------------------
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_vertexShaderConstants, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, VertexShaderConstantsBufferSize);
            int offset = 0;

            //matView
            //-------
            Matrix view = this.View;
            view.Transpose();
            dataBuffer.Set(offset, view);
            offset += sizeof(float) * 16;

            //Close Access to Buffer
            //----------------------
            GraphicsDevice.ImmediateContext.UnmapSubresource(_vertexShaderConstants, 0);
        }
    }
}
