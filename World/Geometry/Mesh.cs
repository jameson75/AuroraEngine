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
    public class Geometry     
    {
        private DXBuffer _instanceBuffer = null;
        private DXBuffer _vertexBuffer = null;
        private DXBuffer _indexBuffer = null;
        private IGameApp _app = null;
        private int _instanceCount = 0;
        private int _vertexCount = 0;
        private int _indexCount = 0;
        private int _vertexStride = 0;
        private int _instanceStride = 0;
        private InputLayout _vertexLayout = null;

        private PrimitiveTopology _topology = PrimitiveTopology.TriangleList;

        public string Name { get; set; }

        public BoundingBox BoundingBox { get; private set; }

        public Geometry(IGameApp app, GeometryDescription description)
        {
            _app = app;
            _vertexStride = description.VertexStride;
            _vertexLayout = description.VertexLayout;
            _topology = description.Topology;
            _vertexCount = description.VertexCount;
            _vertexBuffer = description.VertexBuffer;
            BoundingBox = description.BoundingBox;
            if (description.IndexBuffer != null)
            {
                _indexCount = description.IndexCount;
                _indexBuffer = description.IndexBuffer;
            }
            if (description.InstanceBuffer != null)
            {
                _instanceStride = description.InstanceStride;
                _instanceCount = description.InstanceCount;
                _instanceBuffer = description.InstanceBuffer;
            }
        }
        
        public bool IsInstanced
        {
            get { return _instanceBuffer != null; }
        }

        public bool IsDynamic
        {
            get
            {
                DXBuffer xBuffer = (!IsInstanced) ? _vertexBuffer : _instanceBuffer;
                return ((xBuffer.Description.CpuAccessFlags & CpuAccessFlags.Write) != 0);
            }
        }

        public void Update<T>(T[] data) where T : struct
        {
            DXBuffer dynamicBuffer = (!IsInstanced) ? _vertexBuffer : _instanceBuffer;
            int elementSize = (!IsInstanced) ? _vertexStride : _instanceStride;
            if (dynamicBuffer == null)
                throw new InvalidOperationException("Buffer is not available.");
            DataBox box = _app.GraphicsDeviceContext.MapSubresource(dynamicBuffer, 0, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(box.DataPointer, data.Length * elementSize);
            dataBuffer.Set<T>(0, data);
            _app.GraphicsDeviceContext.UnmapSubresource(dynamicBuffer, 0);
            if(!IsInstanced)
               _vertexCount = data.Length;
            else
                _instanceCount = data.Length;
        }    

        public void Draw(long gameTime)
        {
            Format indexBufferFormat = Format.R16_UInt;

            if (_vertexBuffer == null)
                throw new InvalidOperationException("Vertex buffer was not created.");

            if (_vertexLayout == null)
                throw new InvalidOperationException("Input layout was not set.");

            _app.GraphicsDeviceContext.InputAssembler.InputLayout = _vertexLayout;
            _app.GraphicsDeviceContext.InputAssembler.PrimitiveTopology = _topology;
            if (_instanceBuffer == null)
            {
                _app.GraphicsDeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, _vertexStride, 0));
                if (_indexBuffer == null)
                    _app.GraphicsDeviceContext.Draw(_vertexCount, 0);
                else
                {
                    _app.GraphicsDeviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, indexBufferFormat, 0);
                    _app.GraphicsDeviceContext.DrawIndexed(_indexCount, 0, 0);
                }
            }
            else
            {
                if (_instanceBuffer == null)
                    throw new InvalidOperationException("Instance buffer was not created.");

                _app.GraphicsDeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding[] 
                { 
                    new VertexBufferBinding(_vertexBuffer, _vertexStride, 0),
                    new VertexBufferBinding(_instanceBuffer, _instanceStride, 0)
                });
                if (_indexBuffer == null)
                    _app.GraphicsDeviceContext.DrawInstanced(_vertexCount, _instanceCount, 0, 0);
                else
                {
                    _app.GraphicsDeviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, indexBufferFormat, 0);
                    _app.GraphicsDeviceContext.DrawIndexedInstanced(_indexCount, _instanceCount, 0, 0, 0);
                }
            }
        }
    }

    public class Mesh : Geometry 
    {
        public Mesh(IGameApp app, GeometryDescription description)
            : base(app, description)
        { }
    }

    public struct GeometryDescription
    {
        public DXBuffer IndexBuffer { get; set; }
        public DXBuffer VertexBuffer { get; set; }
        public DXBuffer InstanceBuffer { get; set; }
        public int VertexStride { get; set; }
        public int VertexCount { get; set; }
        public int InstanceStride { get; set; }
        public int InstanceCount { get; set; }
        public int IndexCount { get; set; }
        public InputLayout VertexLayout { get; set; }       
        public PrimitiveTopology Topology { get; set; }
        public BoundingBox BoundingBox { get; set; }
    }
}
