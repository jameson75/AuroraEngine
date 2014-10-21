using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Float4 = SharpDX.Vector4;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Effects;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World
{
    public class LineRenderer
    {
        private Vector3 _p1;
        private Vector3 _p2;
        private Vector3[] _allPoints;
        private LineEffect _effect = null;
        private Mesh _lineMesh = null;

        public LineRenderer(IGameApp game)
        {
            LineWidth = 10.0f;
            _p1 = Vector3.Zero;
            _p2 = Vector3.Zero;
            _allPoints = new Vector3[] { Vector3.Zero };
            _effect = new LineEffect(game);
            _lineMesh = new Mesh(game.GraphicsDevice, new MeshDescription()
            {
                
            });
        }

        public float LineWidth { get; set; }

        public Vector3 P1
        {
            get { return _p1; }
            set
            {
                _p1 = value;
                OnLengthChanged();
            }
        }

        public Vector3 P2
        {
            get { return _p2; }
            set
            {
                _p2 = value;
                OnLengthChanged();
            }
        }      

        public void Draw()
        {
            const int VERTICES_PER_QUAD = 4;
            LineVertex[] vertices = new LineVertex[(_allPoints.Length - 1) * VERTICES_PER_QUAD];
            Vector2[] texCoords = Content.ContentBuilder.CreateQuadTextureCoords();
            Vector2[] offsets = new Vector2[] { new Vector2(halfWidth, 0), 
                                                new Vector2(halfWidth, stepSize), 
                                                new Vector2(-halfWdith, stepSize), 
                                                new Vector2(-halfWidth, 0}; 
            for (int i = 0; i < _allPoints.Length - 1; i++)
            {
                Vector3 slopeDir = Vector3.Normalize(_allPoints[i+1] - _allPoints[i]);
                for(int j = 0; j < VERTICES_PER_QUAD; j++)                
                    vertices[i * VERTICES_PER_QUAD + j] = new LineVertex(_allPoints[i], texCoords[j], offsets[j], slopeDir);                     
            }
            _lineMesh.UpdateVertexStream<LineVertex>(vertices);
        }

        private void OnLengthChanged()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct LineVertex
    {
        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        
        public Vector4 Position;
        public Vector4 TextureCoord;
        public Vector4 TextureCoord2;   
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }

        static LineVertex()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, 16, 0),
                 new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, 32)
             };
            _elementSize = 48;
        }

        public LineVertex(Vector3 position, Vector2 textureCoords, Vector2 offset, Vector3 slopeDir)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = new Vector4(textureCoords.X, textureCoords.Y, offset.X, offset.Y);
            TextureCoord2 = new Vector4(slopeDir, 1.0f);
        }
    }
}
