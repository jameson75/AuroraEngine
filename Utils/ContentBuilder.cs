using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CipherPark.AngelJacket.Core.World.Geometry;
using SharpDX;
using SharpDX.Direct3D11;
using DXBuffer = SharpDX.Direct3D11.Buffer;


namespace CipherPark.AngelJacket.Core.Utils
{
    public static class ContentBuilder
    {
        public static Mesh BuildQuad(IGameApp game, byte[] shaderByteCode, DrawingSizeF size)
        {   
            VertexPositionColor[] verts = new VertexPositionColor[6];
            verts[0] = new VertexPositionColor(new Vector3(-5, 0, 5), Color.Blue.ToVector4());
            verts[1] = new VertexPositionColor(new Vector3(5, 0, 5), Color.Blue.ToVector4());
            verts[2] = new VertexPositionColor(new Vector3(5, 0, -5), Color.Blue.ToVector4());
            verts[3] = new VertexPositionColor(new Vector3(5, 0, -5), Color.Blue.ToVector4());
            verts[4] = new VertexPositionColor(new Vector3(-5, 0, -5), Color.Blue.ToVector4());
            verts[5] = new VertexPositionColor(new Vector3(-5, 0, 5), Color.Blue.ToVector4());
            short[] indices = { 0, 1, 2, 3, 4, 5 };
            MeshDescription meshDesc = new MeshDescription();
            BufferDescription vertexBufferDesc = new BufferDescription();
            vertexBufferDesc.BindFlags = BindFlags.VertexBuffer;
            vertexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            vertexBufferDesc.SizeInBytes = verts.Length * VertexPositionColor.ElementSize;
            vertexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            vertexBufferDesc.StructureByteStride = 0;
            DXBuffer vBuffer = DXBuffer.Create<VertexPositionColor>(game.GraphicsDevice, verts, vertexBufferDesc);
            meshDesc.VertexBuffer = vBuffer;
            meshDesc.VertexCount = verts.Length;
            meshDesc.VertexLayout = new InputLayout(game.GraphicsDevice, shaderByteCode, VertexPositionColor.InputElements);
            meshDesc.VertexStride = VertexPositionColor.ElementSize;
            BufferDescription indexBufferDesc = new BufferDescription();
            indexBufferDesc.BindFlags = BindFlags.IndexBuffer;
            indexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            indexBufferDesc.SizeInBytes = indices.Length * sizeof(short);
            indexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            DXBuffer iBuffer = DXBuffer.Create<short>(game.GraphicsDevice, indices, indexBufferDesc);
            //meshDesc.IndexCount = indices.Length;
            //meshDesc.IndexBuffer = iBuffer;
            meshDesc.Topology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            return new Mesh(game, meshDesc);
        }
    }
}
