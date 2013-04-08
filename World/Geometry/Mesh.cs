using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using DXBuffer = SharpDX.Direct3D11.Buffer;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public class Mesh
    {
        private DXBuffer _vertexBuffer = null;
        private DXBuffer _indexBuffer = null;
        private IGameApp _app = null;        
        private int _vertexCount = 0;
        private int _indexCount = 0;
        private int _vertexStride = 0;
        private InputLayout _vertexLayout = null;
        private PrimitiveTopology _topology = PrimitiveTopology.TriangleList;

        public BoundingBox BoundingBox { get; private set; }

        public Mesh(IGameApp app, MeshDescription meshDescription)
        {
            _app = app;
            _vertexStride = meshDescription.VertexStride;
            _vertexLayout = meshDescription.VertexLayout;
            _topology = meshDescription.Topology;
            _vertexCount = meshDescription.VertexCount;
            _vertexBuffer = meshDescription.VertexBuffer;
            BoundingBox = meshDescription.BoundingBox;

            if (meshDescription.IndexBuffer != null)
            {
                _indexCount = meshDescription.IndexCount;
                _indexBuffer = meshDescription.IndexBuffer;
            }
        }      

        public void Draw(long gameTime)
        {
            if (_vertexBuffer == null)
                throw new InvalidOperationException("Vertex buffer was not created.");

            if (_vertexLayout == null)
                throw new InvalidOperationException("Input layout was not set.");

            _app.GraphicsDeviceContext.InputAssembler.InputLayout = _vertexLayout;
            _app.GraphicsDeviceContext.InputAssembler.PrimitiveTopology = _topology;
            _app.GraphicsDeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, _vertexStride, 0));
            if (_indexBuffer != null)
            {
                _app.GraphicsDeviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R16_SInt, 0);
                _app.GraphicsDeviceContext.DrawIndexed(_indexCount, 0, 0);
            }
            else
                _app.GraphicsDeviceContext.Draw(_vertexCount, 0);            
        }
    }

    public struct MeshDescription
    {
        public DXBuffer IndexBuffer { get; set; }
        public DXBuffer VertexBuffer { get; set; }
        public int VertexStride { get; set; }
        public int VertexCount { get; set; }
        public int IndexCount { get; set; }
        public InputLayout VertexLayout { get; set; }
        public PrimitiveTopology Topology { get; set; }
        public BoundingBox BoundingBox { get; set; }
    }
}
