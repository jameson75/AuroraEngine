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
        [Obsolete]
        public static Mesh BuildTechRings()
        {
            return null;
        }

        public static Mesh BuildQuad(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Color color)
        {   
            BasicVertexPositionColor[] verts = new BasicVertexPositionColor[4];
            short[] indices = new short[6] { 0, 1, 2, 2, 3, 0 };
            Vector3[] positions = CreateQuadPoints(dimension);
            verts[0] = new BasicVertexPositionColor(positions[0], color.ToVector4());
            verts[1] = new BasicVertexPositionColor(positions[1], color.ToVector4());
            verts[2] = new BasicVertexPositionColor(positions[2], color.ToVector4());
            verts[3] = new BasicVertexPositionColor(positions[3], color.ToVector4());           
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);         
            return BuildMesh<BasicVertexPositionColor>(game, shaderByteCode, verts, indices, BasicVertexPositionColor.InputElements, BasicVertexPositionColor.ElementSize, boundingBox);
        }    

        public static Mesh BuildTexturedQuad(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Vector2[] textureCoords = null)
        {
            BasicVertexPositionTexture[] verts = new BasicVertexPositionTexture[4];
            short[] indices = new short[6] { 0, 1, 2, 2, 3, 0 };
            Vector3[] positions = CreateQuadPoints(dimension);
            Vector2[] _textureCoords = (textureCoords != null) ? textureCoords : new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            verts[0] = new BasicVertexPositionTexture(positions[0], _textureCoords[0]);
            verts[1] = new BasicVertexPositionTexture(positions[1], _textureCoords[1]);
            verts[2] = new BasicVertexPositionTexture(positions[2], _textureCoords[2]);
            verts[3] = new BasicVertexPositionTexture(positions[3], _textureCoords[3]);                    
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);         
            return BuildMesh<BasicVertexPositionTexture>(game, shaderByteCode, verts, indices, BasicVertexPositionTexture.InputElements, BasicVertexPositionTexture.ElementSize, boundingBox);
        }

        public static Mesh BuildLitTexturedQuad(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Vector2[] textureCoords = null)
        {
            BasicVertexPositionNormalTexture[] verts = new BasicVertexPositionNormalTexture[4];
            short[] indices = new short[6] { 0, 1, 2, 2, 3, 0 };
            Vector3[] positions = CreateQuadPoints(dimension);
            Vector2[] _textureCoords = (textureCoords != null) ? textureCoords : CreateQuadTextureCoords();
            verts[0] = new BasicVertexPositionNormalTexture(positions[0], Vector3.UnitY, _textureCoords[0]);
            verts[1] = new BasicVertexPositionNormalTexture(positions[1], Vector3.UnitY, _textureCoords[1]);
            verts[2] = new BasicVertexPositionNormalTexture(positions[2], Vector3.UnitY, _textureCoords[2]);
            verts[3] = new BasicVertexPositionNormalTexture(positions[3], Vector3.UnitY, _textureCoords[2]);                 
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<BasicVertexPositionNormalTexture>(game, shaderByteCode, verts, indices, BasicVertexPositionNormalTexture.InputElements, BasicVertexPositionNormalTexture.ElementSize, boundingBox);
        }       

        public static Vector3[] CreateQuadPoints(Rectangle dimension, bool includeCenterPoint = false)
        {
            Vector3[] results = new Vector3[5] 
            {
                new Vector3(dimension.Left, 0, dimension.Top),
                new Vector3(dimension.Right, 0, dimension.Top),
                new Vector3(dimension.Right, 0, dimension.Bottom),
                new Vector3(dimension.Left, 0, dimension.Bottom),
                new Vector3(dimension.Left + (dimension.Width / 2.0f), 0, dimension.Bottom + (dimension.Height / 2.0f))
            };
            if (!includeCenterPoint)
                return results.Take(4).ToArray();
            else
                return results;
        }

        public static Vector2[] CreateQuadTextureCoords()
        {
            return new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
        }

        public static Mesh BuildViewportQuad(IGameApp game, byte[] shaderByteCode)
        {
            Rectangle dimension = new Rectangle(-1, 1, 1, -1);
            Vector2[] textureCoords = { new Vector2( 0, 0 ), new Vector2( 1, 0 ), new Vector2( 1, 1 ), new Vector2( 0, 1 ) };
            BasicVertexScreenTexture[] verts = new BasicVertexScreenTexture[6];            
            verts[0] = new BasicVertexScreenTexture(new Vector2(dimension.Left, dimension.Top), textureCoords[0]);
            verts[1] = new BasicVertexScreenTexture(new Vector2(dimension.Right, dimension.Top), textureCoords[1]);
            verts[2] = new BasicVertexScreenTexture(new Vector2(dimension.Right, dimension.Bottom), textureCoords[2]);
            verts[3] = new BasicVertexScreenTexture(new Vector2(dimension.Right, dimension.Bottom), textureCoords[2]);
            verts[4] = new BasicVertexScreenTexture(new Vector2(dimension.Left, dimension.Bottom), textureCoords[3]);
            verts[5] = new BasicVertexScreenTexture(new Vector2(dimension.Left, dimension.Top), textureCoords[0]);
            Vector3[] positions = (from v in verts select new Vector3(v.Position.X, v.Position.Y, 0)).ToArray();
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);         
            return BuildMesh<BasicVertexScreenTexture>(game, shaderByteCode, verts, BasicVertexScreenTexture.InputElements, BasicVertexScreenTexture.ElementSize, boundingBox);
        }
        
        //Constructs an equilateral triangle.
        public static Mesh BuildTriangle(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Color color)
        {
            BasicVertexPositionColor[] verts = new BasicVertexPositionColor[3];
            Vector3[] positions = CreateTrianglePoints(dimension);
            verts[0] = new BasicVertexPositionColor(positions[0], color.ToVector4());
            verts[1] = new BasicVertexPositionColor(positions[1], color.ToVector4());
            verts[2] = new BasicVertexPositionColor(positions[2], color.ToVector4());            
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);         
            return BuildMesh<BasicVertexPositionColor>(game, shaderByteCode, verts, BasicVertexPositionColor.InputElements, BasicVertexPositionColor.ElementSize, boundingBox);
        }

        public static Mesh BuildTexturedTriangle(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Vector2[] textureCoords = null)
        {
            BasicVertexPositionTexture[] verts = new BasicVertexPositionTexture[3];
            Vector3[] positions = CreateTrianglePoints(dimension);
            Vector2[] _textureCoords = (textureCoords != null) ? textureCoords : new Vector2[] { new Vector2(0.5f, 0.5f), new Vector2(1, 1), new Vector2(0, 1) };
            verts[0] = new BasicVertexPositionTexture(positions[0], _textureCoords[0]);
            verts[1] = new BasicVertexPositionTexture(positions[1], _textureCoords[1]);
            verts[2] = new BasicVertexPositionTexture(positions[2], _textureCoords[2]);            
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<BasicVertexPositionTexture>(game, shaderByteCode, verts, BasicVertexPositionTexture.InputElements, BasicVertexPositionTexture.ElementSize, boundingBox);
        }

        //Constructs an equilateral triangle.
        public static Mesh BuildLitTexturedTriangle(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Vector2[] textureCoords = null)
        {
            Vector2[] _textureCoords = (textureCoords != null) ? textureCoords : new Vector2[] { new Vector2(0.5f, 0), new Vector2(1, 1), new Vector2(0, 1)};
            BasicVertexPositionNormalTexture[] verts = new BasicVertexPositionNormalTexture[3];
            Vector3[] positions = CreateTrianglePoints(dimension);
            verts[0] = new BasicVertexPositionNormalTexture(positions[0], Vector3.UnitY, _textureCoords[0]);
            verts[1] = new BasicVertexPositionNormalTexture(positions[1], Vector3.UnitY, _textureCoords[1]);
            verts[2] = new BasicVertexPositionNormalTexture(positions[2], Vector3.UnitY, _textureCoords[2]);           
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<BasicVertexPositionNormalTexture>(game, shaderByteCode, verts, BasicVertexPositionNormalTexture.InputElements, BasicVertexPositionNormalTexture.ElementSize, boundingBox);
        }       

        public static Vector3[] CreateTrianglePoints(Rectangle dimension)
        {
            return new Vector3[3]
            {
                new Vector3(dimension.Left + (dimension.Size().X / 2.0f), 0, dimension.Top),
                new Vector3(dimension.Right, 0, dimension.Bottom),
                new Vector3(dimension.Left, 0, dimension.Bottom)
            };
        }

        public static Mesh BuildHexagon(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Color color)
        {
            short[] indices = new short[12] { 0, 4, 5, 0, 3, 4, 0, 1, 3, 1, 2, 3 };
            BasicVertexPositionColor[] verts = new BasicVertexPositionColor[6];
            Vector3[] positions = CreateHexagonPoints(dimension);
            verts[0] = new BasicVertexPositionColor(positions[0], color.ToVector4());
            verts[1] = new BasicVertexPositionColor(positions[1], color.ToVector4());
            verts[2] = new BasicVertexPositionColor(positions[2], color.ToVector4());
            verts[3] = new BasicVertexPositionColor(positions[3], color.ToVector4());
            verts[4] = new BasicVertexPositionColor(positions[4], color.ToVector4());
            verts[5] = new BasicVertexPositionColor(positions[5], color.ToVector4());            
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<BasicVertexPositionColor>(game, shaderByteCode, verts, indices, BasicVertexPositionColor.InputElements, BasicVertexPositionColor.ElementSize, boundingBox);
        }

        public static Mesh BuildTexturedHexagon(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Vector2[] textureCoords = null)
        {
            short[] indices = new short[12] { 0, 4, 5, 0, 3, 4, 0, 1, 3, 1, 2, 3 };
            BasicVertexPositionTexture[] verts = new BasicVertexPositionTexture[6];
            Vector3[] positions = CreateHexagonPoints(dimension);
            Vector2[] _textureCoords = (textureCoords != null) ? textureCoords : new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            verts[0] = new BasicVertexPositionTexture(positions[0], _textureCoords[0]);
            verts[1] = new BasicVertexPositionTexture(positions[1], _textureCoords[1]);
            verts[2] = new BasicVertexPositionTexture(positions[2], _textureCoords[2]);
            verts[3] = new BasicVertexPositionTexture(positions[3], _textureCoords[3]);
            verts[4] = new BasicVertexPositionTexture(positions[4], _textureCoords[4]);
            verts[5] = new BasicVertexPositionTexture(positions[5], _textureCoords[5]);            
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<BasicVertexPositionTexture>(game, shaderByteCode, verts, indices, BasicVertexPositionTexture.InputElements, BasicVertexPositionTexture.ElementSize, boundingBox);
        }

        public static Mesh BuildLitTexturedHexagon(IGameApp game, byte[] shaderByteCode, Rectangle dimension, Vector2[] textureCoords = null)
        {
            short[] indices = new short[12] { 0, 4, 5, 0, 3, 4, 0, 1, 3, 1, 2, 3 };
            BasicVertexPositionNormalTexture[] verts = new BasicVertexPositionNormalTexture[6];
            Vector3[] positions = CreateHexagonPoints(dimension);
            Vector2[] _textureCoords = (textureCoords != null) ? textureCoords : new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            verts[0] = new BasicVertexPositionNormalTexture(positions[0], Vector3.UnitY, _textureCoords[0]);
            verts[1] = new BasicVertexPositionNormalTexture(positions[1], Vector3.UnitY, _textureCoords[1]);
            verts[2] = new BasicVertexPositionNormalTexture(positions[2], Vector3.UnitY, _textureCoords[2]);
            verts[3] = new BasicVertexPositionNormalTexture(positions[3], Vector3.UnitY, _textureCoords[3]);
            verts[4] = new BasicVertexPositionNormalTexture(positions[4], Vector3.UnitY, _textureCoords[4]);
            verts[5] = new BasicVertexPositionNormalTexture(positions[5], Vector3.UnitY, _textureCoords[5]);           
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<BasicVertexPositionNormalTexture>(game, shaderByteCode, verts, indices, BasicVertexPositionNormalTexture.InputElements, BasicVertexPositionNormalTexture.ElementSize, boundingBox);
        }

        public static Vector3[] CreateHexagonPoints(Rectangle dimension)
        {
            return new Vector3[6]
            {
                new Vector3(dimension.Left + (dimension.Size().X / 4.0f), 0, dimension.Top),
                new Vector3(dimension.Left + ((dimension.Size().X * 3.0f) / 4.0f), 0, dimension.Top),
                new Vector3(dimension.Right, 0, dimension.Bottom + dimension.Size().Y / 2.0f), 
                new Vector3(dimension.Left + ((dimension.Size().X * 3.0f) / 4.0f), 0, dimension.Bottom),
                new Vector3(dimension.Left + (dimension.Size().X / 4.0f), 0, dimension.Bottom),
                new Vector3(dimension.Left, 0, dimension.Bottom + dimension.Size().Y / 2.0f)
            };    
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

        public static Mesh[] CreateSkyBox(IGameApp game, byte[] shaderByteCode, float distanceFromViewPoint)
        {
            BasicVertexPositionTexture[] front = new BasicVertexPositionTexture[4];
            BasicVertexPositionTexture[] back = new BasicVertexPositionTexture[4];
            BasicVertexPositionTexture[] left = new BasicVertexPositionTexture[4];
            BasicVertexPositionTexture[] right = new BasicVertexPositionTexture[4];
            BasicVertexPositionTexture[] top = new BasicVertexPositionTexture[4];
            BasicVertexPositionTexture[] bottom = new BasicVertexPositionTexture[4];
            Vector2[] _textureCoords = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            
            //front quad...
            front[0] = new BasicVertexPositionTexture(new Vector3(distanceFromViewPoint, distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[0]);
            front[1] = new BasicVertexPositionTexture(new Vector3(-distanceFromViewPoint, distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[1]);
            front[2] = new BasicVertexPositionTexture(new Vector3(-distanceFromViewPoint, -distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[2]);
            front[3] = new BasicVertexPositionTexture(new Vector3(distanceFromViewPoint, -distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[3]);
            
            //back quad...
            back[0] = new BasicVertexPositionTexture(new Vector3(-distanceFromViewPoint, distanceFromViewPoint, distanceFromViewPoint), _textureCoords[0]);
            back[1] = new BasicVertexPositionTexture(new Vector3(distanceFromViewPoint, distanceFromViewPoint, distanceFromViewPoint), _textureCoords[1]);
            back[2] = new BasicVertexPositionTexture(new Vector3(distanceFromViewPoint, -distanceFromViewPoint, distanceFromViewPoint), _textureCoords[2]);
            back[3] = new BasicVertexPositionTexture(new Vector3(-distanceFromViewPoint, -distanceFromViewPoint, distanceFromViewPoint), _textureCoords[3]);
            
            //left quad...
            left[0] = new BasicVertexPositionTexture(new Vector3(distanceFromViewPoint, distanceFromViewPoint, distanceFromViewPoint), _textureCoords[0]);
            left[1] = new BasicVertexPositionTexture(new Vector3(distanceFromViewPoint, distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[1]);
            left[2] = new BasicVertexPositionTexture(new Vector3(distanceFromViewPoint, -distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[2]);
            left[3] = new BasicVertexPositionTexture(new Vector3(distanceFromViewPoint, -distanceFromViewPoint, distanceFromViewPoint), _textureCoords[3]);
            
            //right quad...
            right[0] = new BasicVertexPositionTexture(new Vector3(-distanceFromViewPoint, distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[0]);
            right[1] = new BasicVertexPositionTexture(new Vector3(-distanceFromViewPoint, distanceFromViewPoint, distanceFromViewPoint), _textureCoords[1]);
            right[2] = new BasicVertexPositionTexture(new Vector3(-distanceFromViewPoint, -distanceFromViewPoint, distanceFromViewPoint), _textureCoords[2]);
            right[3] = new BasicVertexPositionTexture(new Vector3(-distanceFromViewPoint, -distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[3]);
            
            //top quad...
            top[0] = new BasicVertexPositionTexture(new Vector3(-distanceFromViewPoint, distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[0]);
            top[1] = new BasicVertexPositionTexture(new Vector3(distanceFromViewPoint, distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[1]);
            top[2] = new BasicVertexPositionTexture(new Vector3(distanceFromViewPoint, distanceFromViewPoint, distanceFromViewPoint), _textureCoords[2]);
            top[3] = new BasicVertexPositionTexture(new Vector3(-distanceFromViewPoint, distanceFromViewPoint, distanceFromViewPoint), _textureCoords[3]);
            
            //bottom quad...
            bottom[0] = new BasicVertexPositionTexture(new Vector3(-distanceFromViewPoint, -distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[0]);
            bottom[1] = new BasicVertexPositionTexture(new Vector3(-distanceFromViewPoint, -distanceFromViewPoint, distanceFromViewPoint), _textureCoords[1]);
            bottom[2] = new BasicVertexPositionTexture(new Vector3(distanceFromViewPoint, -distanceFromViewPoint, distanceFromViewPoint), _textureCoords[2]);
            bottom[3] = new BasicVertexPositionTexture(new Vector3(distanceFromViewPoint, -distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[3]);

            BoundingBox boundingBox = new BoundingBox(new Vector3(-distanceFromViewPoint, -distanceFromViewPoint, -distanceFromViewPoint), new Vector3(distanceFromViewPoint, distanceFromViewPoint, distanceFromViewPoint));
            short[] indices = new short[] { 0, 1, 2, 0, 2, 3 };
            Mesh frontQuad = BuildMesh<BasicVertexPositionTexture>(game, shaderByteCode, front, indices, BasicVertexPositionTexture.InputElements, BasicVertexPositionTexture.ElementSize, boundingBox);
            Mesh backQuad = BuildMesh<BasicVertexPositionTexture>(game, shaderByteCode, back, indices, BasicVertexPositionTexture.InputElements, BasicVertexPositionTexture.ElementSize, boundingBox);
            Mesh leftQuad = BuildMesh<BasicVertexPositionTexture>(game, shaderByteCode, left, indices, BasicVertexPositionTexture.InputElements, BasicVertexPositionTexture.ElementSize, boundingBox);
            Mesh rightQuad = BuildMesh<BasicVertexPositionTexture>(game, shaderByteCode, right, indices, BasicVertexPositionTexture.InputElements, BasicVertexPositionTexture.ElementSize, boundingBox);
            Mesh topQuad = BuildMesh<BasicVertexPositionTexture>(game, shaderByteCode, top, indices, BasicVertexPositionTexture.InputElements, BasicVertexPositionTexture.ElementSize, boundingBox);
            Mesh bottomQuad = BuildMesh<BasicVertexPositionTexture>(game, shaderByteCode, bottom, indices, BasicVertexPositionTexture.InputElements, BasicVertexPositionTexture.ElementSize, boundingBox);

            return new Mesh[] { frontQuad, backQuad, leftQuad, rightQuad, topQuad, bottomQuad };
        }

        public static Mesh BuildMesh<T>(IGameApp game, byte[] shaderByteCode, T[] verts, InputElement[] inputElements, int vertexSize, BoundingBox boundingBox) where T : struct
        {
            return BuildMesh<T>(game, shaderByteCode, verts, null, inputElements, vertexSize, boundingBox);
        }

        public static Mesh BuildMesh<T>(IGameApp game, byte[] shaderByteCode, T[] verts, short[] indices, InputElement[] inputElements, int vertexSize, BoundingBox boundingBox) where T : struct
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
            if (indices != null)
            {
                BufferDescription indexBufferDesc = new BufferDescription();
                indexBufferDesc.BindFlags = BindFlags.IndexBuffer;
                indexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
                indexBufferDesc.SizeInBytes = indices.Length * sizeof(short);
                indexBufferDesc.OptionFlags = ResourceOptionFlags.None;
                DXBuffer iBuffer = DXBuffer.Create<short>(game.GraphicsDevice, indices, indexBufferDesc);
                meshDesc.IndexCount = indices.Length;
                meshDesc.IndexBuffer = iBuffer;
            }
            meshDesc.Topology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            meshDesc.BoundingBox = boundingBox;
            return new Mesh(game, meshDesc);
        }       
    }
}
