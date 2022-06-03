using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using DXBuffer = SharpDX.Direct3D11.Buffer;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Geometry
{
    public struct MeshDescription
    {       
        public BufferDescription VertexBufferDescription { get; set; }
        public BufferDescription IndexBufferDescription { get; set; }
        public BufferDescription InstanceBufferDescription { get; set; }

        public int VertexStride { get; set; }
        public int VertexCount { get; set; }
        public int VertexBufferIncrement { get; set; }
        public int VertexBufferMaxElements { get; set; }      
        
        public int IndexCount { get; set; }
        public int IndexBufferIncrement { get; set; }
        public int IndexBufferMaxElements { get; set; }
        
        public int InstanceStride { get; set; }
        public int InstanceCount { get; set; } 
        public int InstanceBufferIncrement { get; set; }
        public int InstanceBufferMaxElements { get; set; }
      
        public InputLayout VertexLayout { get; set; } 
        public PrimitiveTopology Topology { get; set; }
        public BoundingBox BoundingBox { get; set; }
    }
}
