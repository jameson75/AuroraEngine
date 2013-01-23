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
        public static Mesh BuildQuad(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Color color)
        {   
            BasicVertexPositionColor[] verts = new BasicVertexPositionColor[6];
            verts[0] = new BasicVertexPositionColor(new Vector3(dimension.Left, 0, dimension.Top), color.ToVector4());
            verts[1] = new BasicVertexPositionColor(new Vector3(dimension.Right, 0, dimension.Top), color.ToVector4());
            verts[2] = new BasicVertexPositionColor(new Vector3(dimension.Right, 0, dimension.Bottom ), color.ToVector4());
            verts[3] = new BasicVertexPositionColor(new Vector3(dimension.Right, 0, dimension.Bottom), color.ToVector4());
            verts[4] = new BasicVertexPositionColor(new Vector3(dimension.Left, 0, dimension.Bottom), color.ToVector4());
            verts[5] = new BasicVertexPositionColor(new Vector3(dimension.Left, 0, dimension.Top), color.ToVector4());
            return BuildQuad<BasicVertexPositionColor>(game, shaderByteCode, verts, BasicVertexPositionColor.InputElements, BasicVertexPositionColor.ElementSize);
        }

        public static Mesh BuildTexturedQuad(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Vector2[] textureCoords)
        {
            BasicVertexPositionTexture[] verts = new BasicVertexPositionTexture[6];
            verts[0] = new BasicVertexPositionTexture(new Vector3(dimension.Left, 0, dimension.Top), textureCoords[0]);
            verts[1] = new BasicVertexPositionTexture(new Vector3(dimension.Right, 0, dimension.Top), textureCoords[1]);
            verts[2] = new BasicVertexPositionTexture(new Vector3(dimension.Right, 0, dimension.Bottom), textureCoords[2]);
            verts[3] = new BasicVertexPositionTexture(new Vector3(dimension.Right, 0, dimension.Bottom), textureCoords[2]);
            verts[4] = new BasicVertexPositionTexture(new Vector3(dimension.Left, 0, dimension.Bottom), textureCoords[3]);
            verts[5] = new BasicVertexPositionTexture(new Vector3(dimension.Left, 0, dimension.Top), textureCoords[0]);
            return BuildQuad<BasicVertexPositionTexture>(game, shaderByteCode, verts, BasicVertexPositionTexture.InputElements, BasicVertexPositionTexture.ElementSize);
        }

        public static Mesh BuildViewportQuad(IGameApp game, byte[] shaderByteCode)
        {
            Rectangle dimension = new Rectangle(-1, 1, 1, -1);
            Vector2[] textureCoords = { new Vector2( 0, 0 ), new Vector2( 1, 0 ), new Vector2( 1, 1 ), new Vector2( 0, 1 ) };
            ScreenVertexPositionTexture[] verts = new ScreenVertexPositionTexture[6];            
            verts[0] = new ScreenVertexPositionTexture(new Vector2(dimension.Left, dimension.Top), textureCoords[0]);
            verts[1] = new ScreenVertexPositionTexture(new Vector2(dimension.Right, dimension.Top), textureCoords[1]);
            verts[2] = new ScreenVertexPositionTexture(new Vector2(dimension.Right, dimension.Bottom), textureCoords[2]);
            verts[3] = new ScreenVertexPositionTexture(new Vector2(dimension.Right, dimension.Bottom), textureCoords[2]);
            verts[4] = new ScreenVertexPositionTexture(new Vector2(dimension.Left, dimension.Bottom), textureCoords[3]);
            verts[5] = new ScreenVertexPositionTexture(new Vector2(dimension.Left, dimension.Top), textureCoords[0]);
            return BuildQuad<ScreenVertexPositionTexture>(game, shaderByteCode, verts, ScreenVertexPositionTexture.InputElements, ScreenVertexPositionTexture.ElementSize);
        }

        private static Mesh BuildQuad<T>(IGameApp game, byte[] shaderByteCode, T[] verts, InputElement[] inputElements, int vertexSize) where T : struct
        {
            //short[] indices = {0, 1, 2, 2, 3, 0};
            MeshDescription meshDesc = new MeshDescription();
            BufferDescription vertexBufferDesc = new BufferDescription();
            vertexBufferDesc.BindFlags = BindFlags.VertexBuffer;
            vertexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            vertexBufferDesc.SizeInBytes = verts.Length * vertexSize;
            vertexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            vertexBufferDesc.StructureByteStride = 0;
            DXBuffer vBuffer = DXBuffer.Create<T>(game.GraphicsDevice, verts, vertexBufferDesc);
            meshDesc.VertexBuffer = vBuffer;
            meshDesc.VertexCount = verts.Length;
            meshDesc.VertexLayout = new InputLayout(game.GraphicsDevice, shaderByteCode, inputElements);
            meshDesc.VertexStride = vertexSize;
            BufferDescription indexBufferDesc = new BufferDescription();
            indexBufferDesc.BindFlags = BindFlags.IndexBuffer;
            indexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            //indexBufferDesc.SizeInBytes = indices.Length * sizeof(short);
            indexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            //DXBuffer iBuffer = DXBuffer.Create<short>(game.GraphicsDevice, indices, indexBufferDesc);
            //meshDesc.IndexCount = indices.Length;
            //meshDesc.IndexBuffer = iBuffer;
            meshDesc.Topology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            return new Mesh(game, meshDesc);
        }
    }
}
