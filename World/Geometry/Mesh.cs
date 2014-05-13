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

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public class Mesh     
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
        private int _maxInstanceCount = 0;        

        private PrimitiveTopology _topology = PrimitiveTopology.TriangleList;

        public string Name { get; set; }

        public BoundingBox BoundingBox { get; private set; }

        public Mesh(IGameApp app, MeshDescription description)
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
                _maxInstanceCount = _instanceCount;
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

        public void UpdateIndices(short[] indices, int offset = 0)
        {
            if (!IsDynamic)
                throw new IndexOutOfRangeException("Attempted to update a mesh which is not dynamic");
            
            //TODO: The index buffer needs to be updated too for indexed data
            DataBox box = _app.GraphicsDeviceContext.MapSubresource(_indexBuffer, 0, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(box.DataPointer, indices.Length * sizeof(short));
            dataBuffer.Set<short>(offset * sizeof(short), indices);
            _app.GraphicsDeviceContext.UnmapSubresource(_indexBuffer, 0);
        }

        public void UpdateVertexData<T>(T[] data, int offset = 0) where T : struct
        {            
            if (!IsDynamic)
                throw new InvalidOperationException("Attempted to update a mesh which is not dynamic");

            DXBuffer dynamicBuffer = (!IsInstanced) ? _vertexBuffer : _instanceBuffer;          
            int dataStride = (!IsInstanced) ? _vertexStride : _instanceStride;
            DataBox box = _app.GraphicsDeviceContext.MapSubresource(dynamicBuffer, 0, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(box.DataPointer, data.Length * dataStride);
            dataBuffer.Set<T>(offset * dataStride, data);            
            _app.GraphicsDeviceContext.UnmapSubresource(dynamicBuffer, 0);
            if(!IsInstanced)
               _vertexCount = data.Length;
            else
               _instanceCount = data.Length;
        }    

        public void Draw(GameTime gameTime)
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
                _app.GraphicsDeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding[] 
                    {
                        new VertexBufferBinding(_vertexBuffer, _vertexStride, 0),
                        new VertexBufferBinding(null, 0, 0)
                    });
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

    public struct MeshDescription
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
