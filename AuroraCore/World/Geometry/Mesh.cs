using System;
using CipherPark.Aurora.Core.Utils.Toolkit;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using DXBuffer = SharpDX.Direct3D11.Buffer;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Geometry
{
    public class Mesh : IDisposable
    {       
        private SharpDX.Direct3D11.Device _device = null;
        private MeshDescription _description;
        private DXBuffer _vertexBuffer;
        private DXBuffer _indexBuffer;
        private DXBuffer _instanceBuffer;
        private bool _isDisposed = false;       

        public string Name { get; set; }

        public BoundingBox BoundingBox { get; private set; }

        private Mesh(SharpDX.Direct3D11.Device device, MeshDescription description)
        {
            _device = device;
            _description = description;
        }

        public static Mesh Create<Tv>(SharpDX.Direct3D11.Device device, MeshDescription description, Tv[] verts, short[] indices)
             where Tv : struct
        {
            if (verts == null && description.VertexBufferDescription.Usage != ResourceUsage.Dynamic)
                throw new InvalidOperationException("No vertices were specified and vertex buffer is not dynamic");

            var mesh = new Mesh(device, description);
            
            mesh._vertexBuffer = (verts != null) ? 
                DXBuffer.Create<Tv>(device, verts, description.VertexBufferDescription) : 
                new DXBuffer(device, description.VertexBufferDescription);

            if (indices != null || description.IndexBufferMaxElements > 0)
            {
                mesh._indexBuffer = (indices != null) ?
                    DXBuffer.Create<short>(device, indices, description.IndexBufferDescription) :
                    new DXBuffer(device, description.IndexBufferDescription);
            }            

            return mesh;
        }

        public static Mesh Create<Tv, Ti>(SharpDX.Direct3D11.Device device, MeshDescription description, Tv[] verts, short[] indices, Ti[] instances)
            where Tv : struct
            where Ti : struct
        {
            if (verts == null && description.VertexBufferDescription.Usage != ResourceUsage.Dynamic)
                throw new InvalidOperationException("No vertices were specified and vertex buffer is not dynamic");

            var mesh = new Mesh(device, description);

            mesh._vertexBuffer = (verts != null) ?
                DXBuffer.Create<Tv>(device, verts, description.VertexBufferDescription) :
                new DXBuffer(device, description.VertexBufferDescription);

            if (indices != null || description.IndexBufferMaxElements > 0)
            {
                mesh._indexBuffer = (indices != null) ?
                    DXBuffer.Create<short>(device, indices, description.IndexBufferDescription) :
                    new DXBuffer(device, description.IndexBufferDescription);
            }

            if (instances != null || description.InstanceBufferMaxElements > 0)
            {
                mesh._instanceBuffer = (instances != null) ? 
                    DXBuffer.Create<Ti>(device, instances, description.InstanceBufferDescription) : 
                    new DXBuffer(device, description.InstanceBufferDescription);
            }

            return mesh;
        }

        public MeshDescription Description { get => _description; }
        
        public bool IsInstanced
        {
            get { return _instanceBuffer != null; }
        }
        
        public void UpdateVertexStream<T>(T[] data, int offset = 0) where T : struct
        {   
            var isDynamic = (_vertexBuffer.Description.CpuAccessFlags & CpuAccessFlags.Write) != 0;
            
            if (!isDynamic)
                throw new InvalidOperationException("Attempted to update a mesh vertex stream which is not dynamic");         
           
            if (data.Length > GetVertexBufferSizeInElements())
            {
                if (data.Length > _description.VertexBufferMaxElements)
                    throw new InvalidOperationException("Data offset is greater than the maximum size of the vertex buffer.");

                ResizeBuffer(ref _vertexBuffer, 
                             data.Length, GetVertexBufferSizeInElements(), 
                             _description.VertexCount, 
                             _description.VertexBufferIncrement, 
                             _description.VertexStride);
            }
           
            DataBox box = _device.ImmediateContext.MapSubresource(_vertexBuffer, 0, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None);
            MarshalHelper.CopyToDataPointer(data, box.DataPointer, 0, 0, data.Length);            
            _device.ImmediateContext.UnmapSubresource(_vertexBuffer, 0);

            _description.VertexCount = data.Length;        
        }    

        public void UpdateIndexStream(short[] indices, int offset = 0)
        {
            var isDynamic = (_indexBuffer.Description.CpuAccessFlags & CpuAccessFlags.Write) != 0;

            if (!isDynamic)
                throw new InvalidOperationException("Attempted to update a mesh index stream which is not dynamic");
            
            if (indices.Length > GetIndexBufferSizeInElements())
            {
                if (indices.Length > _description.IndexBufferMaxElements)
                    throw new IndexOutOfRangeException("Data offset is greater than the maximum size of the index buffer.");

                ResizeBuffer(
                    ref _indexBuffer, 
                    indices.Length, 
                    GetIndexBufferSizeInElements(), 
                    _description.IndexCount, 
                    _description.IndexBufferIncrement, 
                    sizeof(short));
            }

            var box = _device.ImmediateContext.MapSubresource(_indexBuffer, 0, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None);
            MarshalHelper.CopyToDataPointer(indices, box.DataPointer, 0, 0, indices.Length);
            _device.ImmediateContext.UnmapSubresource(_indexBuffer, 0);
            _description.IndexCount = indices.Length;
        }

        public void UpdateInstanceStream<T>(T[] data, int offset = 0) where T : struct
        {
            var isDynamic = (_instanceBuffer.Description.CpuAccessFlags & CpuAccessFlags.Write) != 0;

            if (!isDynamic)
                throw new InvalidOperationException("Attempted to update a mesh instance stream which is not dynamic");

            if (data.Length > GetInstanceBufferSizeInElements())
            {
                if (data.Length > _description.IndexBufferMaxElements)
                    throw new InvalidOperationException("Data offset is greater than the maximum size of the instance buffer.");

                ResizeBuffer(ref _instanceBuffer,
                             data.Length, GetInstanceBufferSizeInElements(),
                             _description.InstanceCount,
                             _description.InstanceBufferIncrement,
                             _description.InstanceStride);
            }

            DataBox box = _device.ImmediateContext.MapSubresource(_instanceBuffer, 0, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None);
            MarshalHelper.CopyToDataPointer(data, box.DataPointer, 0, 0, data.Length);
            _device.ImmediateContext.UnmapSubresource(_instanceBuffer, 0);

            _description.InstanceCount = data.Length;
        }

        public void Draw()
        {
            Format indexBufferFormat = Format.R16_UInt;

            if (_vertexBuffer == null)
                throw new InvalidOperationException("Vertex buffer was not created.");

            if (_description.VertexLayout == null)
                throw new InvalidOperationException("Input layout was not set.");

            _device.ImmediateContext.InputAssembler.InputLayout = _description.VertexLayout;
            _device.ImmediateContext.InputAssembler.PrimitiveTopology = _description.Topology;
            
            if (_instanceBuffer == null)
            {
                _device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding[] 
                    {
                        new VertexBufferBinding(_vertexBuffer, _description.VertexStride, 0),
                        new VertexBufferBinding(null, 0, 0)
                    });
                if (_indexBuffer == null)
                    _device.ImmediateContext.Draw(_description.VertexCount, 0);
                else
                {
                    _device.ImmediateContext.InputAssembler.SetIndexBuffer(_indexBuffer, indexBufferFormat, 0);
                    _device.ImmediateContext.DrawIndexed(_description.IndexCount, 0, 0);
                }
            }
            else
            {
                _device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding[] 
                { 
                    new VertexBufferBinding(_vertexBuffer, _description.VertexStride, 0),
                    new VertexBufferBinding(_instanceBuffer, _description.InstanceStride, 0)
                });
                if (_indexBuffer == null)
                    _device.ImmediateContext.DrawInstanced(_description.VertexCount, _description.InstanceCount, 0, 0);
                else
                {
                    _device.ImmediateContext.InputAssembler.SetIndexBuffer(_indexBuffer, indexBufferFormat, 0);
                    _device.ImmediateContext.DrawIndexedInstanced(_description.IndexCount, _description.InstanceCount, 0, 0, 0);
                }
            }           
        }

        public void Dispose()
        {
            if (_indexBuffer != null)
                _indexBuffer.Dispose();           

            if (_instanceBuffer != null)            
                _instanceBuffer.Dispose();             

            if (_vertexBuffer != null)            
                _vertexBuffer.Dispose();
                        
            _isDisposed = true;            
        }

        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        private int GetVertexBufferSizeInElements()
        {
            var vertexBufferSize = _vertexBuffer?.Description.SizeInBytes ?? 0;
            return _description.VertexStride != 0 ? vertexBufferSize / _description.VertexStride : 0;
        }

        private int GetIndexBufferSizeInElements()
        {
            var indexBufferSize = _indexBuffer?.Description.SizeInBytes ?? 0;
            return indexBufferSize / sizeof(short);
        }

        private int GetInstanceBufferSizeInElements()
        {
            var instanceBufferSize = _instanceBuffer?.Description.SizeInBytes ?? 0;
            return _description.InstanceStride != 0 ? instanceBufferSize / _description.InstanceStride : 0;
        }

        private void ResizeBuffer(ref DXBuffer buffer, int sourceElementCount, int destinationSizeInElements, int desitinationElementCount, int incrementSizeInElements, int elementSize)
        {
            var newBufferDesc = buffer.Description;
            var newSizeInElements = BufferResizeCalculator.Calculate(sourceElementCount, destinationSizeInElements, desitinationElementCount, incrementSizeInElements);
            newBufferDesc.SizeInBytes += newSizeInElements * elementSize;
            var newBuffer = new DXBuffer(_device, newBufferDesc);
            buffer.Dispose();
            buffer = newBuffer;
        }
    }
}
