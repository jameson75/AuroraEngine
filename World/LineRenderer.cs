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
    public class StreakRenderer
    {       
        private StreakEffect _effect = null;
        private Mesh _mesh = null;
        private IGameApp _game = null;

        private StreakRenderer()
        {}
        
        public static StreakRenderer Create(IGameApp game)
        {           
            StreakEffect effect = new StreakEffect(game);
            return new StreakRenderer()
            {               
                Width = 10.0f,
                StepSize = 10.0f,
                _effect = effect,
                _mesh = Content.ContentBuilder.BuildDynamicMesh<FlexboardVertex>(
                    game.GraphicsDevice,
                    effect.SelectShaderByteCode(),
                    null,
                    1000,
                    null,
                    0,
                    FlexboardVertex.InputElements,
                    FlexboardVertex.ElementSize,
                    BoundingBoxExtension.Empty)                
            };
        }

        public float Width { get; set; }

        public float StepSize { get; set; }

        public Path Path { get; set; }

        public StreakEffect Effect { get { return _effect; } }

        public void Draw(GameTime gameTime)
        {
            const int VERTICES_PER_QUAD = 4;
            Vector3[] points = this.Path.ToPoints(StepSize);

            FlexboardVertex[] vertices = new FlexboardVertex[(points.Length - 1) * VERTICES_PER_QUAD];
            Vector2[] texCoords = Content.ContentBuilder.CreateQuadTextureCoords();
            float halfWidth = Width / 2.0f;
            Vector2[] offsets = new Vector2[] { new Vector2(halfWidth, 0), 
                                                new Vector2(halfWidth, StepSize), 
                                                new Vector2(-halfWidth, StepSize), 
                                                new Vector2(-halfWidth, 0) }; 
            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector3 slopeDir = Vector3.Normalize(points[i+1] - points[i]);
                for(int j = 0; j < VERTICES_PER_QUAD; j++)                
                    vertices[i * VERTICES_PER_QUAD + j] = new FlexboardVertex(points[i], texCoords[j], offsets[j], slopeDir);                     
            }
            _mesh.UpdateVertexStream<FlexboardVertex>(vertices);
            _effect.World = Matrix.Identity;
            _mesh.Draw(gameTime);
        }      
    }

    public class StreakRendererSceneNode : Scene.SceneNode
    {
        public StreakRenderer Renderer { get; set; }

        public StreakRendererSceneNode(IGameApp game)
            : base(game)
        { }

        public override void Draw(GameTime gameTime)
        {
            if (Renderer != null)
            {                
                Renderer.Effect.View = Camera.TransformToViewMatrix(Scene.CameraNode.ParentToWorld(Scene.CameraNode.Transform)); //ViewMatrix;
                Renderer.Effect.Projection = Scene.CameraNode.Camera.ProjectionMatrix;
                Renderer.Effect.Apply();
                Renderer.Draw(gameTime);     
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FlexboardVertex
    {
        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        
        public Vector4 Position;
        public Vector4 TextureCoord;
        public Vector4 TextureCoord2;   
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }

        static FlexboardVertex()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, 16, 0),
                 new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, 32, 0)
             };
            _elementSize = 48;
        }

        public FlexboardVertex(Vector3 position, Vector2 textureCoords, Vector2 offset, Vector3 slopeDir)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = new Vector4(textureCoords.X, textureCoords.Y, offset.X, offset.Y);
            TextureCoord2 = new Vector4(slopeDir, 1.0f);
        }
    }
}
