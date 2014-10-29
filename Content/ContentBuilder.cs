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
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Content
{
    public static class ContentBuilder
    {
        #region Quad
        public static Mesh BuildBasicQuad(Device game, byte[] shaderByteCode, RectangleF dimension, Color color)
        {   
            VertexPositionColor[] verts = new VertexPositionColor[4];
            short[] indices = CreateQuadIndices();
            Vector3[] positions = CreateQuadPoints(dimension);
            for(int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionColor(positions[i], color.ToVector4());                      
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);         
            return BuildMesh<VertexPositionColor>(game, shaderByteCode, verts, indices, VertexPositionColor.InputElements, VertexPositionColor.ElementSize, boundingBox);
        }

        public static Mesh BuildBasicLitQuad(Device game, byte[] shaderByteCode, RectangleF dimension, Color color)
        {
            VertexPositionNormalColor[] verts = new VertexPositionNormalColor[4];
            short[] indices = CreateQuadIndices();
            Vector3[] positions = CreateQuadPoints(dimension);
            for(int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionNormalColor(positions[i], Vector3.UnitY, color.ToVector4());
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<VertexPositionNormalColor>(game, shaderByteCode, verts, indices, VertexPositionNormalColor.InputElements, VertexPositionNormalColor.ElementSize, boundingBox);
        }

        public static Mesh BuildParticleLitQuad(Device game, byte[] shaderByteCode, RectangleF dimension, Color color, int maxInstances)
        {
            ParticleVertexPositionNormalColor[] verts = new ParticleVertexPositionNormalColor[4];
            short[] indices = CreateQuadIndices();
            Vector3[] positions = CreateQuadPoints(dimension);
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new ParticleVertexPositionNormalColor(positions[i], Vector3.UnitY, color);
            
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            ParticleInstanceVertexData[] instanceData = new ParticleInstanceVertexData[maxInstances];
            return BuildDynamicInstancedMesh<ParticleVertexPositionNormalColor, ParticleInstanceVertexData>(game, shaderByteCode, verts, indices, indices.Length, ParticleVertexPositionNormalColor.InputElements, ParticleVertexPositionNormalColor.ElementSize, instanceData, instanceData.Length, ParticleInstanceVertexData.SizeInBytes, boundingBox);
        }

        public static Mesh BuildBasicTexturedQuad(Device game, byte[] shaderByteCode, RectangleF dimension, Vector2[] textureCoords = null)
        {
            VertexPositionTexture[] verts = new VertexPositionTexture[4];
            short[] indices = CreateQuadIndices();
            Vector3[] positions = CreateQuadPoints(dimension);
            Vector2[] _textureCoords = (textureCoords != null) ? textureCoords : CreateQuadTextureCoords();
            for(int i = 0; i < positions.Length; i++)
                verts[0] = new VertexPositionTexture(positions[i], _textureCoords[i]);                            
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);         
            return BuildMesh<VertexPositionTexture>(game, shaderByteCode, verts, indices, VertexPositionTexture.InputElements, VertexPositionTexture.ElementSize, boundingBox);
        }

        public static Mesh BuildBasicLitTexturedQuad(Device game, byte[] shaderByteCode, RectangleF dimension, Vector2[] textureCoords = null)
        {
            VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[4];
            short[] indices = CreateQuadIndices();
            Vector3[] positions = CreateQuadPoints(dimension);
            Vector2[] _textureCoords = (textureCoords != null) ? textureCoords : CreateQuadTextureCoords();
            for(int i = 0; i < positions.Length; i++)
                verts[0] = new VertexPositionNormalTexture(positions[i], Vector3.UnitY, _textureCoords[i]);          
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<VertexPositionNormalTexture>(game, shaderByteCode, verts, indices, VertexPositionNormalTexture.InputElements, VertexPositionNormalTexture.ElementSize, boundingBox);
        }

        public static Vector3[] CreateQuadPoints(RectangleF dimension, bool includeCenterPoint = false)
        {
            Vector3[] results = new Vector3[5] 
            {
                new Vector3(dimension.Left, 0, dimension.Top),
                new Vector3(dimension.Right, 0, dimension.Top),
                new Vector3(dimension.Right, 0, dimension.Bottom),
                new Vector3(dimension.Left, 0, dimension.Bottom),
                new Vector3(dimension.Left + (dimension.Width / 2.0f), 0, dimension.Top + (dimension.Height / 2.0f))
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

        public static short[] CreateQuadIndices()
        {
            return new short[6] { 0, 1, 2, 2, 3, 0 };
        }
        #endregion 

        #region Special Quad
        public static Mesh BuildBillboardQuad(Device game, byte[] shaderByteCode, DrawingSizeF size)
        {
            BillboardVertex[] verts = new BillboardVertex[4];
            short[] indices = CreateQuadIndices();
            Vector3[] positions = CreateQuadPoints(new RectangleF(0, 0, 0, 0));
            Vector2[] _textureCoords = CreateQuadTextureCoords();
            verts[0] = new BillboardVertex(positions[0], _textureCoords[0], new Vector2(-size.Width, size.Height));
            verts[1] = new BillboardVertex(positions[1], _textureCoords[1], new Vector2(size.Width, size.Height));
            verts[2] = new BillboardVertex(positions[2], _textureCoords[2], new Vector2(size.Width, -size.Height));
            verts[3] = new BillboardVertex(positions[3], _textureCoords[3], new Vector2(-size.Width, -size.Height));
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<BillboardVertex>(game, shaderByteCode, verts, indices, BillboardVertex.InputElements, BillboardVertex.ElementSize, boundingBox); 
        }

        public static Mesh BuildBillboardInstanceQuad(Device game, byte[] shaderByteCode, DrawingSizeF size, int maxInstances)
        {
            BillboardInstanceVertex[] verts = new BillboardInstanceVertex[4];
            short[] indices = CreateQuadIndices();
            Vector3[] positions = CreateQuadPoints(new RectangleF(0, 0, 0, 0));
            Vector2[] _textureCoords = CreateQuadTextureCoords();
            verts[0] = new BillboardInstanceVertex(positions[0], _textureCoords[0], new Vector2(-size.Width, size.Height));
            verts[1] = new BillboardInstanceVertex(positions[1], _textureCoords[1], new Vector2(size.Width, size.Height));
            verts[2] = new BillboardInstanceVertex(positions[2], _textureCoords[2], new Vector2(size.Width, -size.Height));
            verts[3] = new BillboardInstanceVertex(positions[3], _textureCoords[3], new Vector2(-size.Width, -size.Height));
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            Matrix[] instanceData = new Matrix[maxInstances];           
            return BuildInstancedMesh<BillboardInstanceVertex, Matrix>(game, shaderByteCode, verts, indices, BillboardInstanceVertex.InputElements, BillboardInstanceVertex.ElementSize, instanceData, BillboardInstanceVertex.InstanceSize ); 
        }

        public static Mesh BuildBasicViewportQuad(Device game, byte[] shaderByteCode)
        {
            Rectangle dimension = new Rectangle(-1, 1, 1, -1);
            Vector2[] textureCoords = { new Vector2( 0, 0 ), new Vector2( 1, 0 ), new Vector2( 1, 1 ), new Vector2( 0, 1 ) };
            ScreenVertex[] verts = new ScreenVertex[6];            
            verts[0] = new ScreenVertex(new Vector2(dimension.Left, dimension.Top), textureCoords[0]);
            verts[1] = new ScreenVertex(new Vector2(dimension.Right, dimension.Top), textureCoords[1]);
            verts[2] = new ScreenVertex(new Vector2(dimension.Right, dimension.Bottom), textureCoords[2]);
            verts[3] = new ScreenVertex(new Vector2(dimension.Right, dimension.Bottom), textureCoords[2]);
            verts[4] = new ScreenVertex(new Vector2(dimension.Left, dimension.Bottom), textureCoords[3]);
            verts[5] = new ScreenVertex(new Vector2(dimension.Left, dimension.Top), textureCoords[0]);
            Vector3[] positions = (from v in verts select new Vector3(v.Position.X, v.Position.Y, 0)).ToArray();
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);         
            return BuildMesh<ScreenVertex>(game, shaderByteCode, verts, ScreenVertex.InputElements, ScreenVertex.ElementSize, boundingBox);
        }             
        #endregion

        #region Quad3D
        public static Mesh BuildBasicQuad3D(Device game, byte[] shaderByteCode, BoundingBox dimension, Color color)
        {
            VertexPositionColor[] verts = new VertexPositionColor[8];
            short[] indices = CreateQuadIndices3D();
            Vector3[] positions = CreateQuadPoints3D(dimension);
            for(int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionColor(positions[i], color.ToVector4());          
            return BuildMesh<VertexPositionColor>(game, shaderByteCode, verts, indices, VertexPositionColor.InputElements, VertexPositionColor.ElementSize, dimension);
        }

        public static Mesh BuildBasicLitQuad3D(Device game, byte[] shaderByteCode, BoundingBox dimension, Color color)
        {
            VertexPositionNormalColor[] verts = new VertexPositionNormalColor[8];
            short[] indices = CreateQuadIndices3D();
            Vector3[] positions = CreateQuadPoints3D(dimension);
            Vector3[] normals = GenerateNormals(positions, indices);
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionNormalColor(positions[i], normals[i], color.ToVector4());
            return BuildMesh<VertexPositionNormalColor>(game, shaderByteCode, verts, indices, VertexPositionNormalColor.InputElements, VertexPositionNormalColor.ElementSize, dimension);
        }

        public static Mesh BuildBasicLitQuad3DNI(Device game, byte[] shaderByteCode, BoundingBox dimension, Color color)
        {
            VertexPositionNormalColor[] verts = new VertexPositionNormalColor[36];           
            Vector3[] positions = CreateQuadPoints3DNI(dimension);
            Vector3[] normals = GenerateNormals(positions, null);
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionNormalColor(positions[i], normals[i], color.ToVector4());
            return BuildMesh<VertexPositionNormalColor>(game, shaderByteCode, verts, VertexPositionNormalColor.InputElements, VertexPositionNormalColor.ElementSize, dimension);
        }

        public static Mesh BuildBasicTexturedQuad3D(Device game, byte[] shaderByteCode, BoundingBox dimension, Vector2[] texCoords = null)
        {
            VertexPositionTexture[] verts = new VertexPositionTexture[36];
            Vector3[] positions = CreateQuadPoints3D(dimension);
            Vector2[] _texCoords = (texCoords != null) ? texCoords : CreateQuadTextureCoords3DNI();
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionTexture(positions[i], _texCoords[i]);
            return BuildMesh<VertexPositionTexture>(game, shaderByteCode, verts, VertexPositionTexture.InputElements, VertexPositionTexture.ElementSize, dimension);
        }

        public static Mesh BuildBasicLitTexturedQuad3DNI(Device game, byte[] shaderByteCode, BoundingBox dimension, Vector2[] texCoords = null)
        {
            VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[36];
            Vector3[] positions = CreateQuadPoints3DNI(dimension);
            Vector3[] normals = GenerateNormals(positions, null);
            Vector2[] _texCoords = (texCoords != null) ? texCoords : CreateQuadTextureCoords3DNI();
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionNormalTexture(positions[i], normals[i], _texCoords[i]);
            return BuildMesh<VertexPositionNormalTexture>(game, shaderByteCode, verts, VertexPositionNormalTexture.InputElements, VertexPositionNormalTexture.ElementSize, dimension);
        }

        public static Vector2[] CreateQuadTextureCoords3DNI()
        {
            Vector2[] quad2DTexCoords = CreateQuadTextureCoords();
            short[] indices = CreateQuadTextureIndices3D();
            return indices.Select((i) => quad2DTexCoords[i]).ToArray();
        }

        public static Vector3[] CreateQuadPoints3DNI(BoundingBox dimension)
        {
            Vector3[] positions = CreateQuadPoints3D(dimension);
            short[] indices = CreateQuadIndices3D();
            return indices.Select((i) => positions[i]).ToArray();
        }

        public static Vector3[] CreateQuadPoints3D(BoundingBox dimension)
        {
            Vector3[] corners = dimension.GetCorners();
            //We map the corners returned by SharpDX's bounding box (where far/back plane is 0-3 and front/near plane is 4-7)
            //to this application's 3d quad (where top plane is 0-3, and bottom plane is 4-7).
            int[] mappedIndices = new int[8] { 0, 1, 5, 4, 3, 2, 6, 7 };
            return mappedIndices.Select( (m) => corners[m]).ToArray();
        }

        public static short[] CreateQuadIndices3D()
        {
            return new short[36] { 0, 1, 2, 2, 3, 0, //TOP
                                    5, 4, 7, 7, 6, 5, //BOTTOM
                                    0, 3, 7, 7, 4, 0, //LEFT                                             
                                    2, 1, 5, 5, 6, 2, //RIGHT
                                    3, 2, 6, 6, 7, 3, //FRONT
                                    1, 0, 4, 4, 5, 1, //BACK                                             
                                };
        }

        public static short[] CreateQuadTextureIndices3D()
        {
            return new short[36] { 0, 1, 2, 2, 3, 0,
                                   0, 1, 2, 2, 3, 0,
                                   0, 1, 2, 2, 3, 0,
                                   0, 1, 2, 2, 3, 0,
                                   0, 1, 2, 2, 3, 0,
                                   0, 1, 2, 2, 3, 0 };
        }
      
        #endregion

        #region Triangle
        //Constructs an equilateral triangle.
        public static Mesh BuildBasicTriangle(Device device, byte[] shaderByteCode, RectangleF dimension, Color color)
        {
            VertexPositionColor[] verts = new VertexPositionColor[3];
            Vector3[] positions = CreateTrianglePoints(dimension);
            verts[0] = new VertexPositionColor(positions[0], color.ToVector4());
            verts[1] = new VertexPositionColor(positions[1], color.ToVector4());
            verts[2] = new VertexPositionColor(positions[2], color.ToVector4());            
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);         
            return BuildMesh<VertexPositionColor>(device, shaderByteCode, verts, VertexPositionColor.InputElements, VertexPositionColor.ElementSize, boundingBox);
        }

        //Constructs an equilateral triangle.
        public static Mesh BuildBasicLitTriangle(Device device, byte[] shaderByteCode, RectangleF dimension, Color color)
        {
            VertexPositionNormalColor[] verts = new VertexPositionNormalColor[3];
            Vector3[] positions = CreateTrianglePoints(dimension);
            verts[0] = new VertexPositionNormalColor(positions[0], Vector3.UnitY, color.ToVector4());
            verts[1] = new VertexPositionNormalColor(positions[1], Vector3.UnitY, color.ToVector4());
            verts[2] = new VertexPositionNormalColor(positions[2], Vector3.UnitY, color.ToVector4());
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<VertexPositionNormalColor>(device, shaderByteCode, verts, VertexPositionNormalColor.InputElements, VertexPositionNormalColor.ElementSize, boundingBox);
        }     

        public static Mesh BuildBasicTexturedTriangle(Device device, byte[] shaderByteCode, RectangleF dimension, Vector2[] textureCoords = null)
        {
            VertexPositionTexture[] verts = new VertexPositionTexture[3];
            Vector3[] positions = CreateTrianglePoints(dimension);
            Vector2[] _textureCoords = (textureCoords != null) ? textureCoords : new Vector2[] { new Vector2(0.5f, 0.5f), new Vector2(1, 1), new Vector2(0, 1) };
            verts[0] = new VertexPositionTexture(positions[0], _textureCoords[0]);
            verts[1] = new VertexPositionTexture(positions[1], _textureCoords[1]);
            verts[2] = new VertexPositionTexture(positions[2], _textureCoords[2]);            
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<VertexPositionTexture>(device, shaderByteCode, verts, VertexPositionTexture.InputElements, VertexPositionTexture.ElementSize, boundingBox);
        }

        //Constructs an equilateral triangle.
        public static Mesh BuildBasicLitTexturedTriangle(Device device, byte[] shaderByteCode, RectangleF dimension, Vector2[] textureCoords = null)
        {
            Vector2[] _textureCoords = (textureCoords != null) ? textureCoords : new Vector2[] { new Vector2(0.5f, 0), new Vector2(1, 1), new Vector2(0, 1)};
            VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[3];
            Vector3[] positions = CreateTrianglePoints(dimension);
            verts[0] = new VertexPositionNormalTexture(positions[0], Vector3.UnitY, _textureCoords[0]);
            verts[1] = new VertexPositionNormalTexture(positions[1], Vector3.UnitY, _textureCoords[1]);
            verts[2] = new VertexPositionNormalTexture(positions[2], Vector3.UnitY, _textureCoords[2]);           
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<VertexPositionNormalTexture>(device, shaderByteCode, verts, VertexPositionNormalTexture.InputElements, VertexPositionNormalTexture.ElementSize, boundingBox);
        }        

        public static Vector3[] CreateTrianglePoints(RectangleF dimension)
        {
            return new Vector3[3]
            {
                new Vector3(dimension.Left + (dimension.Size().Width / 2.0f), 0, dimension.Top),
                new Vector3(dimension.Right, 0, dimension.Bottom),
                new Vector3(dimension.Left, 0, dimension.Bottom)
            };
        }
        #endregion 

        #region Triangle3D

        public static Mesh BuildBasicTriangle3D(Device game, byte[] shaderByteCode, BoundingBox dimension, Color color)
        {
            VertexPositionColor[] verts = new VertexPositionColor[6];
            short[] indices = CreateTriangleIndices3D();
            Vector3[] positions = CreateTrianglePoints3D(dimension);
            for(int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionColor(positions[i], color.ToVector4());          
            return BuildMesh<VertexPositionColor>(game, shaderByteCode, verts, indices, VertexPositionColor.InputElements, VertexPositionColor.ElementSize, dimension);
        }

        public static Mesh BuildBasicLitTriangle3D(Device game, byte[] shaderByteCode, BoundingBox dimension, Color color)
        {
            VertexPositionNormalColor[] verts = new VertexPositionNormalColor[6];
            short[] indices = CreateTriangleIndices3D();
            Vector3[] positions = CreateTrianglePoints3D(dimension);
            Vector3[] normals = GenerateNormals(positions, indices);
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionNormalColor(positions[i], normals[i], color.ToVector4());
            return BuildMesh<VertexPositionNormalColor>(game, shaderByteCode, verts, indices, VertexPositionNormalColor.InputElements, VertexPositionNormalColor.ElementSize, dimension);
        }

        public static Mesh BuildBasicLitTriangle3DNI(Device game, byte[] shaderByteCode, BoundingBox dimension, Color color)
        {
            VertexPositionNormalColor[] verts = new VertexPositionNormalColor[24];            
            Vector3[] positions = CreateTrianglePoints3DNI(dimension);
            Vector3[] normals = GenerateNormals(positions, null);
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionNormalColor(positions[i], normals[i], color.ToVector4());
            return BuildMesh<VertexPositionNormalColor>(game, shaderByteCode, verts, VertexPositionNormalColor.InputElements, VertexPositionNormalColor.ElementSize, dimension);
        }

        public static Mesh BuildBasicTexturedTriangle3D(Device game, byte[] shaderByteCode, BoundingBox dimension, Vector2[] texCoords = null)
        {
            VertexPositionTexture[] verts = new VertexPositionTexture[6];
            short[] indices = CreateTriangleIndices3D();
            Vector3[] positions = CreateTrianglePoints3D(dimension);
            Vector2[] _texCoords = (texCoords != null) ? texCoords : CreateTriangleTextureCoords3D();
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionTexture(positions[i], _texCoords[i]);
            return BuildMesh<VertexPositionTexture>(game, shaderByteCode, verts, indices, VertexPositionTexture.InputElements, VertexPositionTexture.ElementSize, dimension);
        }

        public static Mesh BuildBasicLitTexturedTriangle3D(Device game, byte[] shaderByteCode, BoundingBox dimension, Vector2[] texCoords = null)
        {
            VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[6];
            short[] indices = CreateTriangleIndices3D();
            Vector3[] positions = CreateTrianglePoints3D(dimension);
            Vector3[] normals = GenerateNormals(positions, indices);
            Vector2[] _texCoords = (texCoords != null) ? texCoords : CreateTriangleTextureCoords3D();
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionNormalTexture(positions[i], normals[i], _texCoords[i]);
            return BuildMesh<VertexPositionNormalTexture>(game, shaderByteCode, verts, indices, VertexPositionNormalTexture.InputElements, VertexPositionNormalTexture.ElementSize, dimension);
        }

        public static Mesh BuildBasicLitTexturedTriangle3DNI(Device game, byte[] shaderByteCode, BoundingBox dimension, Vector2[] texCoords = null)
        {
            VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[24];            
            Vector3[] positions = CreateTrianglePoints3D(dimension);
            Vector3[] normals = GenerateNormals(positions, null);
            Vector2[] _texCoords = (texCoords != null) ? texCoords : CreateTriangleTextureCoords3D();
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionNormalTexture(positions[i], normals[i], _texCoords[i]);
            return BuildMesh<VertexPositionNormalTexture>(game, shaderByteCode, verts, VertexPositionNormalTexture.InputElements, VertexPositionNormalTexture.ElementSize, dimension);
        }      

        public static Vector2[] CreateTriangleTextureCoords3D()
        {
            throw new NotImplementedException();
        }

        public static short[] CreateTriangleIndices3D()
        {
            return new short[24] { 0, 1, 2, 
                                   3, 5, 4,
                                   0, 2, 5, 5, 3, 0,
                                   1, 0, 3, 3, 4, 1,
                                   2, 1, 4, 4, 5, 2
                                };
        }

        public static Vector3[] CreateTrianglePoints3D(BoundingBox dimension)
        {
            float halfWidth = dimension.GetLengthX() / 2.0f;
            Vector3[] corners = dimension.GetCorners();
            return new Vector3[6]
            {
                corners[0] + new Vector3(halfWidth, 0, 0),
                corners[5],
                corners[4],
                corners[3] + new Vector3(halfWidth, 0, 0),
                corners[6],
                corners[7]
            };
        }

        public static Vector3[] CreateTrianglePoints3DNI(BoundingBox dimension)
        {
            Vector3[] positions = CreateTrianglePoints3D(dimension);
            short[] indices = CreateTriangleIndices3D();
            return indices.Select((i) => positions[i]).ToArray();
        }

        #endregion

        #region Hexagon
        public static Mesh BuildBasicHexagon(Device game, byte[] shaderByteCode, RectangleF dimension, Color color)
        {
            short[] indices = CreateHexagonIndices();
            VertexPositionColor[] verts = new VertexPositionColor[6];
            Vector3[] positions = CreateHexagonPoints(dimension);
            for(int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionColor(positions[i], color.ToVector4());                  
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<VertexPositionColor>(game, shaderByteCode, verts, indices, VertexPositionColor.InputElements, VertexPositionColor.ElementSize, boundingBox);
        }

        public static Mesh BuildBasicLitHexagon(Device game, byte[] shaderByteCode, RectangleF dimension, Color color)
        {
            short[] indices = CreateHexagonIndices();
            VertexPositionNormalColor[] verts = new VertexPositionNormalColor[6];
            Vector3[] positions = CreateHexagonPoints(dimension);           
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionNormalColor(positions[i], Vector3.UnitY, color.ToVector4());
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<VertexPositionNormalColor>(game, shaderByteCode, verts, indices, VertexPositionNormalColor.InputElements, VertexPositionNormalColor.ElementSize, boundingBox);
        }

        public static Mesh BuildBasicTexturedHexagon(Device game, byte[] shaderByteCode, RectangleF dimension, Vector2[] textureCoords = null)
        {
            short[] indices = CreateHexagonIndices();
            VertexPositionTexture[] verts = new VertexPositionTexture[6];
            Vector3[] positions = CreateHexagonPoints(dimension);
            Vector2[] _textureCoords = (textureCoords != null) ? textureCoords : new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionTexture(positions[i], _textureCoords[i]);        
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<VertexPositionTexture>(game, shaderByteCode, verts, indices, VertexPositionTexture.InputElements, VertexPositionTexture.ElementSize, boundingBox);
        }

        public static Mesh BuildBasicLitTexturedHexagon(Device game, byte[] shaderByteCode, RectangleF dimension, Vector2[] textureCoords = null)
        {
            short[] indices = CreateHexagonIndices();
            VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[6];
            Vector3[] positions = CreateHexagonPoints(dimension);
            Vector2[] _textureCoords = (textureCoords != null) ? textureCoords : new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionNormalTexture(positions[i], Vector3.UnitY, _textureCoords[i]);       
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            return BuildMesh<VertexPositionNormalTexture>(game, shaderByteCode, verts, indices, VertexPositionNormalTexture.InputElements, VertexPositionNormalTexture.ElementSize, boundingBox);
        }

        public static Mesh BuildParticleLitTexturedHexagon(Device game, byte[] shaderByteCode, float radius, int maxInstances)
        {
            
            short[] indices = CreateHexagonIndices();
            Vector3[] positions = CreateHexagonPoints(radius);
            Vector2[] textureCoords = CreateHexagonTextureCoords();
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            
            if (maxInstances <= 0)
                throw new ArgumentException("Maxium instances is less than or equal to zero", "maxInstances");
        
            ParticleVertexPostionNormalTexture[] verts = new ParticleVertexPostionNormalTexture[6];
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new ParticleVertexPostionNormalTexture()
                    {
                        Position = new Vector4(positions[i], 1.0f),
                        TextureCoord = textureCoords[i]
                    };
            
            ParticleInstanceVertexData[] data = new ParticleInstanceVertexData[maxInstances * verts.Length];
            return BuildInstancedMesh<ParticleVertexPostionNormalTexture, ParticleInstanceVertexData>(game, shaderByteCode, verts, indices, ParticleVertexPostionNormalTexture.InputElements, ParticleVertexPostionNormalTexture.ElementSize, data, ParticleInstanceVertexData.SizeInBytes);                
        }

        public static Vector2[] CreateHexagonTextureCoords()
        {
            return new Vector2[] {
                new Vector2(0, 0.25f),
                new Vector2(0, 0.75f),
                new Vector2(0.5f, 1),
                new Vector2(1, 0.75f),
                new Vector2(1, 0.25f),
                new Vector2(0, 0.5f)
            };
        }

        public static Vector3[] CreateHexagonPoints(RectangleF dimension)
        {
            float quaterWidth = dimension.Width * 0.25f;
            float halfHeight = dimension.Height * 0.5f;
            return new Vector3[6]
            {                
                new Vector3(dimension.Left + quaterWidth, 0, dimension.Top),
                new Vector3(dimension.Right - quaterWidth, 0, dimension.Top),
                new Vector3(dimension.Right, 0, dimension.Top + halfHeight), 
                new Vector3(dimension.Right - quaterWidth, 0, dimension.Bottom),
                new Vector3(dimension.Left + quaterWidth, 0, dimension.Bottom),
                new Vector3(dimension.Left, 0, dimension.Top + halfHeight)
            };    
        }

        public static Vector3[] CreateHexagonPoints(float radius)
        {
            int sides = 6;            
            float angle_stepsize = MathUtil.TwoPi / (float)sides;
            float angle = angle_stepsize * 3;          
            Vector3[] points = new Vector3[6];
            for (int i = 0; i < sides; i++)
            {               
                points[i] = new Vector3(radius * (float)Math.Cos(-angle), radius * (float)Math.Sin(-angle), 0);                                   
                angle -= angle_stepsize;                
            }
            return points;
        }

        public static short[] CreateHexagonIndices()
        {
            return new short[12] { 0, 4, 5, 0, 3, 4, 0, 1, 3, 1, 2, 3 }; 
        }
        #endregion

        #region Hexagon3D

        public static Mesh BuildBasicHexagon3D(Device game, byte[] shaderByteCode, BoundingBox dimension, Color color)
        {
            short[] indices = CreateHexagonIndices3D();
            VertexPositionColor[] verts = new VertexPositionColor[12];
            Vector3[] positions = CreateHexagonPoints3D(dimension);
            for(int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionColor(positions[i], color.ToVector4());               
            return BuildMesh<VertexPositionColor>(game, shaderByteCode, verts, indices, VertexPositionColor.InputElements, VertexPositionColor.ElementSize, dimension);
        }

        public static Mesh BuildBasicLitHexagon3D(Device game, byte[] shaderByteCode, BoundingBox dimension, Color color)
        {
            short[] indices = CreateHexagonIndices3D();
            VertexPositionNormalColor[] verts = new VertexPositionNormalColor[12];
            Vector3[] positions = CreateHexagonPoints3D(dimension);
            Vector3[] normals = GenerateNormals(positions, indices);
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionNormalColor(positions[i], normals[i], color.ToVector4());
            return BuildMesh<VertexPositionNormalColor>(game, shaderByteCode, verts, indices, VertexPositionNormalColor.InputElements, VertexPositionNormalColor.ElementSize, dimension);
        }

        public static Mesh BuildBasicLitHexagon3DNI(Device game, byte[] shaderByteCode, BoundingBox dimension, Color color)
        {            
            VertexPositionNormalColor[] verts = new VertexPositionNormalColor[60];
            Vector3[] positions = CreateHexagonPoints3DNI(dimension);
            Vector3[] normals = GenerateNormals(positions, null);
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionNormalColor(positions[i], normals[i], color.ToVector4());
            return BuildMesh<VertexPositionNormalColor>(game, shaderByteCode, verts, VertexPositionNormalColor.InputElements, VertexPositionNormalColor.ElementSize, dimension);
        }

        public static Mesh BuildBasicTexturedHexagon3D(Device game, byte[] shaderByteCode, BoundingBox dimension, Vector2[] texCoords = null)
        {
            short[] indices = CreateHexagonIndices3D();
            VertexPositionTexture[] verts = new VertexPositionTexture[12];
            Vector3[] positions = CreateHexagonPoints3D(dimension);
            Vector2[] _texCoords = (texCoords != null) ? texCoords : CreateHexagonTextureCoords3D();
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionTexture(positions[i], _texCoords[i]);
            return BuildMesh<VertexPositionTexture>(game, shaderByteCode, verts, indices, VertexPositionTexture.InputElements, VertexPositionTexture.ElementSize, dimension);
        }

        public static Mesh BuildBasicLitTexturedHexagon3D(Device game, byte[] shaderByteCode, BoundingBox dimension, Vector2[] texCoords = null)
        {
            short[] indices = CreateHexagonIndices3D();
            VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[12];
            Vector3[] positions = CreateHexagonPoints3D(dimension);
            Vector2[] _texCoords = (texCoords != null) ? texCoords : CreateHexagonTextureCoords3D();
            Vector3[] normals = GenerateNormals(positions, indices);
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionNormalTexture(positions[i], normals[i], _texCoords[i]);
            return BuildMesh<VertexPositionNormalTexture>(game, shaderByteCode, verts, indices, VertexPositionNormalTexture.InputElements, VertexPositionNormalTexture.ElementSize, dimension);
        }

        public static Mesh BuildBasicLitTexturedHexagon3DNI(Device game, byte[] shaderByteCode, BoundingBox dimension, Vector2[] texCoords = null)
        {    
            VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[60];
            Vector3[] positions = CreateHexagonPoints3DNI(dimension);
            Vector2[] _texCoords = (texCoords != null) ? texCoords : CreateHexagonTextureCoords3D();
            Vector3[] normals = GenerateNormals(positions, null);
            for (int i = 0; i < positions.Length; i++)
                verts[i] = new VertexPositionNormalTexture(positions[i], normals[i], _texCoords[i]);
            return BuildMesh<VertexPositionNormalTexture>(game, shaderByteCode, verts, VertexPositionNormalTexture.InputElements, VertexPositionNormalTexture.ElementSize, dimension);
        }

        public static Vector2[] CreateHexagonTextureCoords3D()
        {
            throw new NotImplementedException();
        }

        public static Vector3[] CreateHexagonPoints3D(BoundingBox dimension)
        {
            float quaterWidth = dimension.GetLengthX() * 0.25f;
            float halfHeight = -dimension.GetLengthY() * 0.5f;            
            return new Vector3[12]
            {          
                new Vector3(dimension.Minimum.X + quaterWidth, dimension.Maximum.Y, dimension.Maximum.Z),
                new Vector3(dimension.Maximum.X - quaterWidth, dimension.Maximum.Y, dimension.Maximum.Z),
                new Vector3(dimension.Maximum.X, dimension.Maximum.Y, dimension.Maximum.Z + halfHeight), 
                new Vector3(dimension.Maximum.X - quaterWidth, dimension.Maximum.Y, dimension.Minimum.Z),
                new Vector3(dimension.Minimum.X + quaterWidth, dimension.Maximum.Y, dimension.Minimum.Z),
                new Vector3(dimension.Minimum.X, dimension.Maximum.Y, dimension.Maximum.Z + halfHeight),
                new Vector3(dimension.Minimum.X + quaterWidth, dimension.Minimum.Y, dimension.Maximum.Z),
                new Vector3(dimension.Maximum.X - quaterWidth, dimension.Minimum.Y, dimension.Maximum.Z),
                new Vector3(dimension.Maximum.X, dimension.Minimum.Y, dimension.Maximum.Z + halfHeight), 
                new Vector3(dimension.Maximum.X - quaterWidth, dimension.Minimum.Y, dimension.Minimum.Z),
                new Vector3(dimension.Minimum.X + quaterWidth, dimension.Minimum.Y, dimension.Minimum.Z),
                new Vector3(dimension.Minimum.X, dimension.Minimum.Y, dimension.Maximum.Z + halfHeight)
            };   
        }

        private static short[] CreateHexagonIndices3D()
        {
            return new short[60] { 0, 4, 5, 0, 3, 4, 0, 1, 3, 1, 2, 3,  //TOP
                                              7, 9, 8, 7, 10, 9, 6, 10, 7, 6, 11, 10, //BOTTOM
                                              //STARTING FROM LEFT-MOST SIDE, GOING COUNTER CLOCKWISE
                                              0, 5, 11, 11, 6, 0, //SIDE 1
                                              1, 0, 6, 6, 7, 1, //SIDE 2
                                              2, 1, 7, 7, 8, 2, //SIDE 3
                                              3, 2, 8, 8, 9, 3, //SIDE 4
                                              4, 3, 9, 9, 10, 4, //SIDE 5
                                              5, 4, 10, 10, 11, 5 //SIDE 6
        };
        }

        private static Vector3[] CreateHexagonPoints3DNI(BoundingBox dimension)
        {
            Vector3[] positions = CreateHexagonPoints3D(dimension);
            short[] indices = CreateHexagonIndices3D();
            return indices.Select((i) => positions[i]).ToArray();
        }

        #endregion
      
        #region ReferenceGrid
        public static Mesh BuildReferenceGrid(Device game, byte[] shaderByteCode, DrawingSizeF gridSize, Vector2 gridSteps, Color gridColor)
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
            VertexPositionColor[] vertices = new VertexPositionColor[nVertices];
            Vector3 vOrigin = new Vector3(-halfWidth, 0f, halfHeight);
            Vector3 vBackRight = new Vector3(gridSize.Width - halfWidth, 0f, halfHeight);
            Vector3 vFrontLeft = new Vector3(-halfWidth, 0f, -gridSize.Height + halfHeight);
            Vector3 vx1 = vOrigin;
            Vector3 vx2 = vFrontLeft;
            for (int i = 0; i < xSteps; i++)
            {
                vertices[i * 2] = new VertexPositionColor(vx1, gridColor.ToVector4());
                vertices[i * 2 + 1] = new VertexPositionColor(vx2, gridColor.ToVector4());
                vx1 += new Vector3(xStepSize, 0f, 0f);
                vx2 += new Vector3(xStepSize, 0f, 0f);
            }
            int k = zSteps * 2;
            Vector3 vz1 = vOrigin;
            Vector3 vz2 = vBackRight;
            for (int i = 0; i < zSteps; i++)
            {
                vertices[i * 2 + k] = new VertexPositionColor(vz1, gridColor.ToVector4());
                vertices[i * 2 + 1 + k] = new VertexPositionColor(vz2, gridColor.ToVector4());
                vz1 += new Vector3(0f, 0f, -zStepSize);
                vz2 += new Vector3(0f, 0f, -zStepSize);
            }
            //NOTE: We must get the shader byte code
            MeshDescription meshDesc = new MeshDescription();
            BufferDescription vertexBufferDesc = new BufferDescription();
            vertexBufferDesc.BindFlags = BindFlags.VertexBuffer;
            vertexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            vertexBufferDesc.SizeInBytes = vertices.Length * VertexPositionColor.ElementSize;
            vertexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            vertexBufferDesc.StructureByteStride = 0;
            meshDesc.VertexCount = vertices.Length;
            meshDesc.VertexBuffer = DXBuffer.Create<VertexPositionColor>(game, vertices, vertexBufferDesc);
            meshDesc.VertexLayout = new InputLayout(game, shaderByteCode, VertexPositionColor.InputElements);
            meshDesc.VertexStride = VertexPositionColor.ElementSize;
            meshDesc.Topology = PrimitiveTopology.LineList;
            Vector3[] positions = (from v in vertices select new Vector3(v.Position.X, v.Position.Y, v.Position.Z)).ToArray();
            BoundingBox boundingBox = BoundingBox.FromPoints(positions); 
            return new Mesh(game, meshDesc); 
        }
        #endregion

        #region Skybox
        public static Mesh[] CreateSkyBox(Device game, byte[] shaderByteCode, float distanceFromViewPoint)
        {
            VertexPositionTexture[] front = new VertexPositionTexture[4];
            VertexPositionTexture[] back = new VertexPositionTexture[4];
            VertexPositionTexture[] left = new VertexPositionTexture[4];
            VertexPositionTexture[] right = new VertexPositionTexture[4];
            VertexPositionTexture[] top = new VertexPositionTexture[4];
            VertexPositionTexture[] bottom = new VertexPositionTexture[4];
            Vector2[] _textureCoords = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            
            //front quad...
            front[0] = new VertexPositionTexture(new Vector3(distanceFromViewPoint, distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[0]);
            front[1] = new VertexPositionTexture(new Vector3(-distanceFromViewPoint, distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[1]);
            front[2] = new VertexPositionTexture(new Vector3(-distanceFromViewPoint, -distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[2]);
            front[3] = new VertexPositionTexture(new Vector3(distanceFromViewPoint, -distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[3]);
            
            //back quad...
            back[0] = new VertexPositionTexture(new Vector3(-distanceFromViewPoint, distanceFromViewPoint, distanceFromViewPoint), _textureCoords[0]);
            back[1] = new VertexPositionTexture(new Vector3(distanceFromViewPoint, distanceFromViewPoint, distanceFromViewPoint), _textureCoords[1]);
            back[2] = new VertexPositionTexture(new Vector3(distanceFromViewPoint, -distanceFromViewPoint, distanceFromViewPoint), _textureCoords[2]);
            back[3] = new VertexPositionTexture(new Vector3(-distanceFromViewPoint, -distanceFromViewPoint, distanceFromViewPoint), _textureCoords[3]);
            
            //left quad...
            left[0] = new VertexPositionTexture(new Vector3(distanceFromViewPoint, distanceFromViewPoint, distanceFromViewPoint), _textureCoords[0]);
            left[1] = new VertexPositionTexture(new Vector3(distanceFromViewPoint, distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[1]);
            left[2] = new VertexPositionTexture(new Vector3(distanceFromViewPoint, -distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[2]);
            left[3] = new VertexPositionTexture(new Vector3(distanceFromViewPoint, -distanceFromViewPoint, distanceFromViewPoint), _textureCoords[3]);
            
            //right quad...
            right[0] = new VertexPositionTexture(new Vector3(-distanceFromViewPoint, distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[0]);
            right[1] = new VertexPositionTexture(new Vector3(-distanceFromViewPoint, distanceFromViewPoint, distanceFromViewPoint), _textureCoords[1]);
            right[2] = new VertexPositionTexture(new Vector3(-distanceFromViewPoint, -distanceFromViewPoint, distanceFromViewPoint), _textureCoords[2]);
            right[3] = new VertexPositionTexture(new Vector3(-distanceFromViewPoint, -distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[3]);
            
            //top quad...
            top[0] = new VertexPositionTexture(new Vector3(-distanceFromViewPoint, distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[0]);
            top[1] = new VertexPositionTexture(new Vector3(distanceFromViewPoint, distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[1]);
            top[2] = new VertexPositionTexture(new Vector3(distanceFromViewPoint, distanceFromViewPoint, distanceFromViewPoint), _textureCoords[2]);
            top[3] = new VertexPositionTexture(new Vector3(-distanceFromViewPoint, distanceFromViewPoint, distanceFromViewPoint), _textureCoords[3]);
            
            //bottom quad...
            bottom[0] = new VertexPositionTexture(new Vector3(-distanceFromViewPoint, -distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[0]);
            bottom[1] = new VertexPositionTexture(new Vector3(-distanceFromViewPoint, -distanceFromViewPoint, distanceFromViewPoint), _textureCoords[1]);
            bottom[2] = new VertexPositionTexture(new Vector3(distanceFromViewPoint, -distanceFromViewPoint, distanceFromViewPoint), _textureCoords[2]);
            bottom[3] = new VertexPositionTexture(new Vector3(distanceFromViewPoint, -distanceFromViewPoint, -distanceFromViewPoint), _textureCoords[3]);

            BoundingBox boundingBox = new BoundingBox(new Vector3(-distanceFromViewPoint, -distanceFromViewPoint, -distanceFromViewPoint), new Vector3(distanceFromViewPoint, distanceFromViewPoint, distanceFromViewPoint));
            short[] indices = new short[] { 0, 1, 2, 0, 2, 3 };
            Mesh frontQuad = BuildMesh<VertexPositionTexture>(game, shaderByteCode, front, indices, VertexPositionTexture.InputElements, VertexPositionTexture.ElementSize, boundingBox);
            Mesh backQuad = BuildMesh<VertexPositionTexture>(game, shaderByteCode, back, indices, VertexPositionTexture.InputElements, VertexPositionTexture.ElementSize, boundingBox);
            Mesh leftQuad = BuildMesh<VertexPositionTexture>(game, shaderByteCode, left, indices, VertexPositionTexture.InputElements, VertexPositionTexture.ElementSize, boundingBox);
            Mesh rightQuad = BuildMesh<VertexPositionTexture>(game, shaderByteCode, right, indices, VertexPositionTexture.InputElements, VertexPositionTexture.ElementSize, boundingBox);
            Mesh topQuad = BuildMesh<VertexPositionTexture>(game, shaderByteCode, top, indices, VertexPositionTexture.InputElements, VertexPositionTexture.ElementSize, boundingBox);
            Mesh bottomQuad = BuildMesh<VertexPositionTexture>(game, shaderByteCode, bottom, indices, VertexPositionTexture.InputElements, VertexPositionTexture.ElementSize, boundingBox);

            return new Mesh[] { frontQuad, backQuad, leftQuad, rightQuad, topQuad, bottomQuad };
        }
        #endregion        

        #region Ring
        public static Mesh BuildBasicRing(Device game, byte[] shaderByteCode, Color color, float radius, float width, int segments, bool isDynamic = false)
        {          
            Vector3[] positions = CreateRingPoints(radius, width, segments);
            short[] indices = CreateRingIndices(segments);
            BoundingBox boundingBox = BoundingBox.FromPoints(positions);
            VertexPositionColor[] verts = new VertexPositionColor[positions.Length];
            for (int i = 0; i < verts.Length; i++)
                verts[i] = new VertexPositionColor(positions[i], color.ToVector4());
            if (!isDynamic)
                return BuildMesh<VertexPositionColor>(game, shaderByteCode, verts, indices, VertexPositionColor.InputElements, VertexPositionColor.ElementSize, boundingBox);
            else
                return BuildDynamicMesh<VertexPositionColor>(game, shaderByteCode, verts, verts.Length, indices, indices.Length, VertexPositionColor.InputElements, VertexPositionColor.ElementSize, boundingBox);
        }      

        private static Vector3[] CreateRingPoints(float radius, float width, int segments)
        {
            float angle = MathUtil.Pi / 2.0f;
            float angle_stepsize = MathUtil.TwoPi / (float)segments;
            float innerRadius = Math.Max(radius - width, 0);
            Vector3[] points = new Vector3[segments * 2];
            for (int i = 0; i < segments; i++)
            {             
                int k = i * 2;
                points[k] = new Vector3(radius * (float)Math.Cos(-angle), radius * (float)Math.Sin(-angle), 0);
                points[k + 1] = new Vector3(innerRadius * (float)Math.Cos(-angle), innerRadius * (float)Math.Sin(-angle), 0);
                angle -= angle_stepsize;             
            }
            return points;
        }

        private static short[] CreateRingIndices(int segments)
        {
            int[] indices = new int[segments * 6];

            for (int i = 0; i < segments; i++)
            {                
                int k = i * 2;
                int m = i * 6;
               
                indices[m + 0] = k + 0;
                indices[m + 1] = k + 1;
                
                if (i < (segments - 1))
                    indices[m + 2] = k + 2;
                else
                    indices[m + 2] = 0;

                indices[m + 3] = k + 1;
                if (i < (segments - 1))
                {
                    indices[m + 4] = k + 3;
                    indices[m + 5] = k + 2;
                }
                else
                {
                    indices[m + 4] = 1;
                    indices[m + 5] = 0;
                }               
            }

            return indices.Select(e => (short)e).ToArray();
        }
        #endregion

        #region Mesh
        public static Mesh BuildMesh<T>(Device device, byte[] shaderByteCode, T[] verts, InputElement[] inputElements, int vertexSize, BoundingBox boundingBox) where T : struct
        {
            return BuildMesh<T>(device, shaderByteCode, verts, null, inputElements, vertexSize, boundingBox);
        }

        public static Mesh BuildMesh<T>(Device device, byte[] shaderByteCode, T[] verts, short[] indices, InputElement[] inputElements, int vertexSize, BoundingBox boundingBox) where T : struct
        {            
            MeshDescription meshDesc = new MeshDescription();

            if (verts == null)
                throw new ArgumentNullException("verts");

            //Vertices...
            BufferDescription vertexBufferDesc = new BufferDescription();
            vertexBufferDesc.BindFlags = BindFlags.VertexBuffer;
            vertexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            vertexBufferDesc.SizeInBytes = verts.Length * vertexSize;
            vertexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            vertexBufferDesc.StructureByteStride = 0;
            DXBuffer vBuffer = DXBuffer.Create<T>(device, verts, vertexBufferDesc);
            meshDesc.VertexBuffer = vBuffer;
            meshDesc.VertexCount = verts.Length;
            meshDesc.VertexLayout = new InputLayout(device, shaderByteCode, inputElements);
            meshDesc.VertexStride = vertexSize;           

            //Optional Indices...
            if (indices != null)
            {
                BufferDescription indexBufferDesc = new BufferDescription();
                indexBufferDesc.BindFlags = BindFlags.IndexBuffer;
                indexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
                indexBufferDesc.SizeInBytes = indices.Length * sizeof(short);
                indexBufferDesc.OptionFlags = ResourceOptionFlags.None;
                DXBuffer iBuffer = DXBuffer.Create<short>(device, indices, indexBufferDesc);
                meshDesc.IndexCount = indices.Length;
                meshDesc.IndexBuffer = iBuffer;
            }

            meshDesc.Topology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            meshDesc.BoundingBox = boundingBox;
            
            return new Mesh(device, meshDesc);
        }

        public static Mesh BuildDynamicMesh<T>(Device device, byte[] shaderByteCode, T[] verts, int maxVertices, short[] indices, int maxIndices, InputElement[] inputElements, int vertexSize, BoundingBox boundingBox, PrimitiveTopology topology = PrimitiveTopology.TriangleList) where T : struct
        {
            MeshDescription meshDesc = new MeshDescription();

            if (maxVertices <= 0)
                throw new ArgumentOutOfRangeException("maxVertices");

            if (indices != null && maxIndices <= 0)
                throw new ArgumentOutOfRangeException("maxIndices");

            //Vertices...
            BufferDescription vertexBufferDesc = new BufferDescription();
            vertexBufferDesc.BindFlags = BindFlags.VertexBuffer;
            vertexBufferDesc.CpuAccessFlags = CpuAccessFlags.Write;
            vertexBufferDesc.SizeInBytes = maxVertices * vertexSize;
            vertexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            vertexBufferDesc.StructureByteStride = 0;
            vertexBufferDesc.Usage = ResourceUsage.Dynamic;
            DXBuffer vBuffer = (verts != null) ? DXBuffer.Create<T>(device, verts, vertexBufferDesc) : new DXBuffer(device, vertexBufferDesc);
            meshDesc.VertexCount = (verts != null) ? verts.Length : 0;
            meshDesc.VertexBuffer = vBuffer;            
            meshDesc.VertexLayout = new InputLayout(device, shaderByteCode, inputElements);
            meshDesc.VertexStride = vertexSize;

            //Optional Indices...
            if (maxIndices != 0)
            {                
                BufferDescription indexBufferDesc = new BufferDescription();
                indexBufferDesc.BindFlags = BindFlags.IndexBuffer;
                indexBufferDesc.CpuAccessFlags = CpuAccessFlags.Write;
                indexBufferDesc.SizeInBytes = maxIndices * sizeof(short);
                indexBufferDesc.OptionFlags = ResourceOptionFlags.None;
                indexBufferDesc.Usage = ResourceUsage.Dynamic;
                DXBuffer iBuffer = (indices != null) ? DXBuffer.Create<short>(device, indices, indexBufferDesc) : new DXBuffer(device, indexBufferDesc);
                meshDesc.IndexCount = (indices != null) ? indices.Length : 0;
                meshDesc.IndexBuffer = iBuffer;
            }

            meshDesc.Topology = topology;
            meshDesc.BoundingBox = boundingBox;
            return new Mesh(device, meshDesc);
        }      

        public static Mesh BuildInstancedMesh<Tv, Ti>(Device device, byte[] shaderByteCode, Tv[] verts, short[] indices, 
            InputElement[] vertexInputElements, int vertexSize, Ti[] instances,  
            int instanceSize) where Ti : struct where Tv : struct
        {
            MeshDescription meshDesc = new MeshDescription();
            
            //Vertices..
            BufferDescription vertexBufferDesc = new BufferDescription();
            vertexBufferDesc.BindFlags = BindFlags.VertexBuffer;
            vertexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            vertexBufferDesc.SizeInBytes = verts.Length * vertexSize;
            vertexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            vertexBufferDesc.StructureByteStride = 0;
            DXBuffer vBuffer = DXBuffer.Create<Tv>(device, verts, vertexBufferDesc);
            meshDesc.VertexBuffer = vBuffer;
            meshDesc.VertexCount = verts.Length;
            meshDesc.VertexLayout = new InputLayout(device, shaderByteCode, vertexInputElements);
            meshDesc.VertexStride = vertexSize;
            
            //Instances...
            //NOTE: We always make the instance buffer a dynamic/writable one.
            BufferDescription instanceBufferDesc = new BufferDescription();
            instanceBufferDesc.BindFlags = BindFlags.VertexBuffer;
            instanceBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            instanceBufferDesc.SizeInBytes = instanceSize * instances.Length;
            instanceBufferDesc.OptionFlags = ResourceOptionFlags.None;            
            instanceBufferDesc.StructureByteStride = 0;
            DXBuffer nBuffer = DXBuffer.Create<Ti>(device, instances, instanceBufferDesc);
            meshDesc.InstanceBuffer = nBuffer;
            meshDesc.InstanceCount = instances.Length;
            meshDesc.InstanceStride = instanceSize;
            
            //Optional Indices...
            if (indices != null)
            {
                BufferDescription indexBufferDesc = new BufferDescription();
                indexBufferDesc.BindFlags = BindFlags.IndexBuffer;
                indexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
                indexBufferDesc.SizeInBytes = indices.Length * sizeof(short);
                indexBufferDesc.OptionFlags = ResourceOptionFlags.None;
                DXBuffer iBuffer = DXBuffer.Create<short>(device, indices, indexBufferDesc);                            
                meshDesc.IndexCount = indices.Length;
                meshDesc.IndexBuffer = iBuffer;
            }
            
            meshDesc.Topology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            return new Mesh(device, meshDesc);
        }

        public static Mesh BuildDynamicInstancedMesh<Tv, Ti>(Device device, byte[] shaderByteCode, Tv[] verts, short[] indices,
            int maxIndices, InputElement[] vertexInputElements, int vertexSize, Ti[] instances, int maxInstances,
            int instanceSize, BoundingBox boundingBox)
            where Ti : struct
            where Tv : struct
        {
            MeshDescription meshDesc = new MeshDescription();

            if (maxInstances <= 0)
                throw new ArgumentOutOfRangeException("maxVertices");

            if (indices != null && maxIndices <= 0)
                throw new ArgumentOutOfRangeException("maxIndices");

            //Vertices...
            BufferDescription vertexBufferDesc = new BufferDescription();
            vertexBufferDesc.BindFlags = BindFlags.VertexBuffer;
            vertexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            vertexBufferDesc.SizeInBytes = verts.Length * vertexSize;
            vertexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            vertexBufferDesc.StructureByteStride = 0;
            DXBuffer vBuffer = DXBuffer.Create<Tv>(device, verts, vertexBufferDesc);
            meshDesc.VertexBuffer = vBuffer;
            meshDesc.VertexCount = verts.Length;
            meshDesc.VertexLayout = new InputLayout(device, shaderByteCode, vertexInputElements);
            meshDesc.VertexStride = vertexSize;

            //Instances...
            //NOTE: We always make the instance buffer a dynamic/writable one.
            BufferDescription instanceBufferDesc = new BufferDescription();
            instanceBufferDesc.BindFlags = BindFlags.VertexBuffer;
            instanceBufferDesc.CpuAccessFlags = CpuAccessFlags.Write;
            instanceBufferDesc.SizeInBytes = instanceSize * maxInstances;
            instanceBufferDesc.OptionFlags = ResourceOptionFlags.None;
            instanceBufferDesc.Usage = ResourceUsage.Dynamic;
            instanceBufferDesc.StructureByteStride = 0;
            DXBuffer nBuffer = (instances != null) ? DXBuffer.Create<Ti>(device, instances, instanceBufferDesc)  : new DXBuffer(device, instanceBufferDesc);           
            meshDesc.InstanceCount = (instances != null) ? instances.Length : 0;
            meshDesc.InstanceBuffer = nBuffer;
            meshDesc.InstanceStride = instanceSize;

            //Optional Indices...
            if (indices != null)
            {
                BufferDescription indexBufferDesc = new BufferDescription();
                indexBufferDesc.BindFlags = BindFlags.IndexBuffer;
                indexBufferDesc.CpuAccessFlags = CpuAccessFlags.Write;
                indexBufferDesc.SizeInBytes = maxIndices * sizeof(short);
                indexBufferDesc.OptionFlags = ResourceOptionFlags.None;
                indexBufferDesc.Usage = ResourceUsage.Dynamic;
                DXBuffer iBuffer = (indices != null) ? DXBuffer.Create<short>(device, indices, indexBufferDesc) : new DXBuffer(device, indexBufferDesc);
                meshDesc.IndexCount = (indices != null) ? indices.Length : 0;
                meshDesc.IndexBuffer = iBuffer;
            }

            meshDesc.Topology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            meshDesc.BoundingBox = boundingBox;

            return new Mesh(device, meshDesc);
        }
        #endregion

        public static Vector3[] GenerateNormals(Vector3[] positions, short[] indices)
        {
            Vector3[] normals = new Vector3[positions.Length];
            short[] _indices = (indices != null) ? indices : Enumerable.Range(0, positions.Length).Select((r) => (short)r).ToArray();
            if (_indices != null)
            {
                List<Vector3>[] positionNormals = new List<Vector3>[positions.Length];
                for (int i = 0; i < positionNormals.Length; i++)
                    positionNormals[i] = new List<Vector3>();
                for (int i = 0; i < _indices.Length; i += 3)
                {
                    Vector3 v1 = positions[_indices[i + 1]] - positions[_indices[i + 2]];
                    Vector3 v2 = positions[_indices[i + 1]] - positions[_indices[i]];
                    v1.Normalize();
                    v2.Normalize();
                    Vector3 n = Vector3.Cross(v1, v2);
                    for (int j = 0; j < 3; j++)
                        if (!positionNormals[_indices[i + j]].Contains(n))
                            positionNormals[_indices[i + j]].Add(n);
                }
                for (int i = 0; i < positionNormals.Length; i++)
                {
                    Vector3 v = Vector3.Zero;
                    for (int j = 0; j < positionNormals[i].Count; j++)
                        v += positionNormals[i][j];
                    normals[i] = v / positionNormals[i].Count;
                    normals[i].Normalize();
                }
                return normals;
            }
            else
                throw new NotSupportedException();
        }
    }    
}
