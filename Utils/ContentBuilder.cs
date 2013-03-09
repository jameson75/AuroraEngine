using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CipherPark.AngelJacket.Core.World.Geometry;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using DXBuffer = SharpDX.Direct3D11.Buffer;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Utils
{
    public static class ContentBuilder
    {
        public static Mesh BuildTechRings()
        {
            return null;
        }

        public static Mesh BuildQuad(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Color color)
        {   
            BasicVertexPositionColor[] verts = new BasicVertexPositionColor[6];
            verts[0] = new BasicVertexPositionColor(new Vector3(dimension.Left, 0, dimension.Top), color.ToVector4());
            verts[1] = new BasicVertexPositionColor(new Vector3(dimension.Right, 0, dimension.Top), color.ToVector4());
            verts[2] = new BasicVertexPositionColor(new Vector3(dimension.Right, 0, dimension.Bottom ), color.ToVector4());
            verts[3] = new BasicVertexPositionColor(new Vector3(dimension.Right, 0, dimension.Bottom), color.ToVector4());
            verts[4] = new BasicVertexPositionColor(new Vector3(dimension.Left, 0, dimension.Bottom), color.ToVector4());
            verts[5] = new BasicVertexPositionColor(new Vector3(dimension.Left, 0, dimension.Top), color.ToVector4());
            Vector3[] positions = (from v in verts select new Vector3(v.Position.X, v.Position.Y, v.Position.Z)).ToArray();
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);         
            return BuildMesh<BasicVertexPositionColor>(game, shaderByteCode, verts, BasicVertexPositionColor.InputElements, BasicVertexPositionColor.ElementSize, boundingBox);
        }

        public static Mesh BuildTexturedQuad(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Vector2[] textureCoords = null)
        {
            BasicVertexPositionTexture[] verts = new BasicVertexPositionTexture[6];
            Vector2[] _textureCoords = (textureCoords != null) ? textureCoords : new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            verts[0] = new BasicVertexPositionTexture(new Vector3(dimension.Left, 0, dimension.Top), _textureCoords[0]);
            verts[1] = new BasicVertexPositionTexture(new Vector3(dimension.Right, 0, dimension.Top), _textureCoords[1]);
            verts[2] = new BasicVertexPositionTexture(new Vector3(dimension.Right, 0, dimension.Bottom), _textureCoords[2]);
            verts[3] = new BasicVertexPositionTexture(new Vector3(dimension.Right, 0, dimension.Bottom), _textureCoords[2]);
            verts[4] = new BasicVertexPositionTexture(new Vector3(dimension.Left, 0, dimension.Bottom), _textureCoords[3]);
            verts[5] = new BasicVertexPositionTexture(new Vector3(dimension.Left, 0, dimension.Top), _textureCoords[0]);
            Vector3[] positions = (from v in verts select new Vector3(v.Position.X, v.Position.Y, v.Position.Z)).ToArray();
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);         
            return BuildMesh<BasicVertexPositionTexture>(game, shaderByteCode, verts, BasicVertexPositionTexture.InputElements, BasicVertexPositionTexture.ElementSize, boundingBox);
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
            Vector3[] positions = (from v in verts select new Vector3(v.Position.X, v.Position.Y, 0)).ToArray();
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);         
            return BuildMesh<ScreenVertexPositionTexture>(game, shaderByteCode, verts, ScreenVertexPositionTexture.InputElements, ScreenVertexPositionTexture.ElementSize, boundingBox);
        }
        
        //Constructs an equilateral triangle.
        public static Mesh BuildTriangle(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Color color)
        {
            BasicVertexPositionColor[] verts = new BasicVertexPositionColor[3];
            verts[0] = new BasicVertexPositionColor(new Vector3(0, 0, dimension.Top), color.ToVector4());
            verts[1] = new BasicVertexPositionColor(new Vector3(dimension.Right, 0, 0), color.ToVector4());
            verts[2] = new BasicVertexPositionColor(new Vector3(dimension.Left, 0, 0), color.ToVector4());
            Vector3[] positions = (from v in verts select new Vector3(v.Position.X, v.Position.Y, v.Position.Z)).ToArray();
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);         
            return BuildMesh<BasicVertexPositionColor>(game, shaderByteCode, verts, BasicVertexPositionColor.InputElements, BasicVertexPositionColor.ElementSize, boundingBox);
        }

        //Constructs an equilateral triangle.
        public static Mesh BuildLitTexturedTriangle(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Vector2[] textureCoords = null)
        {
            Vector2[] _textureCoords = (textureCoords != null) ? textureCoords : new Vector2[] { new Vector2(0.5f, 0), new Vector2(1, 1), new Vector2(0, 1)};
            BasicVertexPositionNormalTexture[] verts = new BasicVertexPositionNormalTexture[3];
            verts[0] = new BasicVertexPositionNormalTexture(new Vector3(0, 0, dimension.Top), Vector3.UnitY, _textureCoords[0]);
            verts[1] = new BasicVertexPositionNormalTexture(new Vector3(dimension.Right, 0, 0), Vector3.UnitY, _textureCoords[1]);
            verts[2] = new BasicVertexPositionNormalTexture(new Vector3(dimension.Left, 0, 0), Vector3.UnitY, _textureCoords[2]);
            Vector3[] positions = (from v in verts select new Vector3(v.Position.X, v.Position.Y, v.Position.Z)).ToArray();
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<BasicVertexPositionNormalTexture>(game, shaderByteCode, verts, BasicVertexPositionNormalTexture.InputElements, BasicVertexPositionNormalTexture.ElementSize, boundingBox);
        }

        public static Mesh BuildReferenceGrid(IGameApp game, byte[] shaderByteCode, DrawingSizeF gridSize, Vector2 gridSteps, Color gridColor)
        {
            //Transform = Matrix.Identity;

            //*******************************************************************
            //TODO: Port this logic into a static ReferenceGrid.Create() method.*
            //*******************************************************************
            //_effect = new BasicEffect(game.GraphicsDevice);
            //_effect.SetWorld(Matrix.Identity);
            //_effect.SetVertexColorEnabled(true);
            //byte[] shaderCode = _effect.SelectShaderByteCode();            
            int xSteps = (int)gridSteps.X;
            int zSteps = (int)gridSteps.Y;
            float halfWidth = gridSize.Width / 2.0f;
            float halfHeight = gridSize.Height / 2.0f;
            int nVertices = 2 * (xSteps + zSteps);
            float xStepSize = (float)gridSize.Width / (gridSteps.X - 1);
            float zStepSize = (float)gridSize.Height / (gridSteps.Y - 1);
            BasicVertexPositionColor[] vertices = new BasicVertexPositionColor[nVertices];
            Vector3 vOrigin = new Vector3(-halfWidth, 0f, halfHeight);
            Vector3 vBackRight = new Vector3(gridSize.Width - halfWidth, 0f, halfHeight);
            Vector3 vFrontLeft = new Vector3(-halfWidth, 0f, -gridSize.Height + halfHeight);
            Vector3 vx1 = vOrigin;
            Vector3 vx2 = vFrontLeft;
            for (int i = 0; i < xSteps; i++)
            {
                vertices[i * 2] = new BasicVertexPositionColor(vx1, gridColor.ToVector4());
                vertices[i * 2 + 1] = new BasicVertexPositionColor(vx2, gridColor.ToVector4());
                vx1 += new Vector3(xStepSize, 0f, 0f);
                vx2 += new Vector3(xStepSize, 0f, 0f);
            }
            int k = zSteps * 2;
            Vector3 vz1 = vOrigin;
            Vector3 vz2 = vBackRight;
            for (int i = 0; i < zSteps; i++)
            {
                vertices[i * 2 + k] = new BasicVertexPositionColor(vz1, gridColor.ToVector4());
                vertices[i * 2 + 1 + k] = new BasicVertexPositionColor(vz2, gridColor.ToVector4());
                vz1 += new Vector3(0f, 0f, -zStepSize);
                vz2 += new Vector3(0f, 0f, -zStepSize);
            }
            //NOTE: We must get the shader byte code
            MeshDescription meshDesc = new MeshDescription();
            BufferDescription vertexBufferDesc = new BufferDescription();
            vertexBufferDesc.BindFlags = BindFlags.VertexBuffer;
            vertexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            vertexBufferDesc.SizeInBytes = vertices.Length * BasicVertexPositionColor.ElementSize;
            vertexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            vertexBufferDesc.StructureByteStride = 0;
            meshDesc.VertexCount = vertices.Length;
            meshDesc.VertexBuffer = DXBuffer.Create<BasicVertexPositionColor>(game.GraphicsDevice, vertices, vertexBufferDesc);
            meshDesc.VertexLayout = new InputLayout(game.GraphicsDevice, shaderByteCode, BasicVertexPositionColor.InputElements);
            meshDesc.VertexStride = BasicVertexPositionColor.ElementSize;
            meshDesc.Topology = PrimitiveTopology.LineList;
            Vector3[] positions = (from v in vertices select new Vector3(v.Position.X, v.Position.Y, v.Position.Z)).ToArray();
            BoundingBox boundingBox = BoundingBox.FromPoints(positions); 
            return new Mesh(game, meshDesc); 
        }

        public static Mesh BuildLitTexturedQuad(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Vector2[] textureCoords = null)
        {
            BasicVertexPositionNormalTexture[] verts = new BasicVertexPositionNormalTexture[6];
            Vector2[] _textureCoords = (textureCoords != null) ? textureCoords : new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            verts[0] = new BasicVertexPositionNormalTexture(new Vector3(dimension.Left, 0, dimension.Top), Vector3.UnitY, _textureCoords[0]);
            verts[1] = new BasicVertexPositionNormalTexture(new Vector3(dimension.Right, 0, dimension.Top), Vector3.UnitY, _textureCoords[1]);
            verts[2] = new BasicVertexPositionNormalTexture(new Vector3(dimension.Right, 0, dimension.Bottom), Vector3.UnitY, _textureCoords[2]);
            verts[3] = new BasicVertexPositionNormalTexture(new Vector3(dimension.Right, 0, dimension.Bottom), Vector3.UnitY, _textureCoords[2]);
            verts[4] = new BasicVertexPositionNormalTexture(new Vector3(dimension.Left, 0, dimension.Bottom), Vector3.UnitY, _textureCoords[3]);
            verts[5] = new BasicVertexPositionNormalTexture(new Vector3(dimension.Left, 0, dimension.Top), Vector3.UnitY, _textureCoords[0]);
            Vector3[] positions = (from v in verts select new Vector3(v.Position.X, v.Position.Y, v.Position.Z)).ToArray();
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<BasicVertexPositionNormalTexture>(game, shaderByteCode, verts, BasicVertexPositionNormalTexture.InputElements, BasicVertexPositionNormalTexture.ElementSize, boundingBox);
        }

        private static Mesh BuildMesh<T>(IGameApp game, byte[] shaderByteCode, T[] verts, InputElement[] inputElements, int vertexSize, BoundingBox boundingBox) where T : struct
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
            meshDesc.BoundingBox = boundingBox;
            return new Mesh(game, meshDesc);
        }       
    }
}
