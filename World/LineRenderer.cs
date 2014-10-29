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
            const int VERTICES_PER_POINT = 2;   
            float halfWidth = Width / 2.0f;                     
            Vector3[] points = this.Path.ToPoints(StepSize);
            VertexPositionColor[] vertices = new VertexPositionColor[(points.Length) * VERTICES_PER_POINT];                                         
            for (int i = 0; i < points.Length; i+= VERTICES_PER_POINT)
            {
                Vector3 nSlope = Vector3.Normalize((i < points.Length - 1) ? points[i + 1] - points[i] : points[i] - points[i - 1]);          
                Vector3 v = points[i];
                //Transform point to camera space.
                Vector3 vw = sceneRendererContainer.ParentToWorldCoordinate(v);
                Vector3 vcs = sceneCamera.WorldToParentCoordinate(vw);
                //Project point transformed point to camera z plane.
                Vector3 vpcs = vcs - (Vector3.Dot(vcs, Vector3.UnitZ) * Vector3.UnitZ);
                //Get direction to camera from projected point.
                Vector3 npcs = sceneCamera.Transform.Translation - vpcs;
                //Transform direction to container space.
                Vector3 nw = sceneCamera.ParentToWorldNormal(npcs);
                Vector3 n = sceneRendererContainer.WorldToParentNormal(nw);
                //Get cross product of slope and direction to camera.
                Vector3 xDir = Vector3.Normalize(Vector3.Cross(nSlope, n));
                vertices[i * VERTICES_PER_POINT] = new VertexPositionColor(v + xDir * halfWidth, Color.White.ToVector4());
                vertices[i * VERTICES_PER_POINT + 1] = new VertexPositionColor(v + xDir * halfWidth, Color.White.ToVector4());
            }

            //TODO: Create mesh quads from vertices.

            _mesh.UpdateVertexStream<VertexPositionColor>(meshVertices);            
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
}
