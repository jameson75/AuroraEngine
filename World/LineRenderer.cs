using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D;
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
        private SurfaceEffect _effect = null;
        private Mesh _mesh = null;
        private IGameApp _game = null;
        
        private StreakRenderer()
        { }
        
        public static StreakRenderer Create(IGameApp game)
        {           
            BasicEffectEx effect = new BasicEffectEx(game);
            effect.EnableVertexColor = true;            
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
                    BoundingBoxExtension.Empty,
                    PrimitiveTopology.LineStrip)                
            };
        }

        public float Width { get; set; }

        public float StepSize { get; set; }

        public Path Path { get; set; }

        public SurfaceEffect Effect { get { return _effect; } }       

        public void Draw(GameTime gameTime, ITransformable sceneCamera, ITransformable sceneRendererContainer)
        {
            //TODO:
            //Steps:
            //Transform point to camera space.
            //Project point to camera plane.
            //Get direction from point to camera.
            //Normalize.
            //Transform direction to camera vector/normal to path streak space.
            //Get cross product of slope and direction to camera vector.
            //Normalize Cross product.
            //Cross product is +X direction.          
            const int VERTICES_PER_POINT = 2;   
            float halfWidth = Width / 2.0f;         
            
            Vector3[] points = this.Path.ToPoints(StepSize);
            VertexPositionColor[] vertices = new VertexPositionColor[(points.Length) * VERTICES_PER_POINT];
            Vector2[] texCoords = Content.ContentBuilder.CreateQuadTextureCoords();            
            Vector3[] offsets = new Vector3[VERTICES_PER_POINT] { new Vector3(halfWidth, 0, 0), new Vector3(-halfWidth, 0, 0) };                                  
            for (int i = 0; i < points.Length; i++)
            {
                for (int j = 0; j < VERTICES_PER_POINT; j++)
                {
                    Vector3 v = points[i] + offsets[j];
                    //Transform point to camera space.
                    Vector3 vw = sceneRendererContainer.ParentToWorld(v);
                    Vector3 vcs = sceneCamera.WorldToParent(vw);
                    //Project point transformed point to camera z plane.
                    Vector3 vpcs = vcs - (Vector3.Dot(vcs, Vector3.UnitZ) * Vector3.UnitZ);
                    //Get direction to camera from projected point.
                    Vector3 npcs = sceneCamera.Transform.Translation - vpcs;
                    //Transform direction to container space.
                    Vector3 nw = sceneCamera.ParentToWorldNormal(npcs);
                    Vector3 n = sceneRendererContainer.WorldToParentNormal(nw);
                }
            }
            _mesh.UpdateVertexStream<VertexPositionColor>(vertices);
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
                Renderer.Draw(gameTime, Scene.CameraNode, this);     
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

    public static class StreakRendererTransformableExtension
    {
        public static Vector3 ParentToWorld(this ITransformable t, Vector3 p)
        {
            Matrix m = Matrix.Translation(p);
            return t.ParentToWorld(m).TranslationVector;
        }

        public static Vector3 WorldToParent(this ITransformable t, Vector3 p)
        {
            Matrix m = Matrix.Translation(p);
            return t.WorldToParent(m).TranslationVector;
        }
    }
}
