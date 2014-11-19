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
                _mesh = Content.ContentBuilder.BuildDynamicMesh<VertexPositionColor>(
                    game.GraphicsDevice,
                    effect.SelectShaderByteCode(),
                    null,
                    1000,
                    null,
                    0,
                    VertexPositionColor.InputElements,
                    VertexPositionColor.ElementSize,
                    BoundingBoxExtension.Empty, PrimitiveTopology.TriangleList)                
            };
        }

        public float Width { get; set; }

        public float StepSize { get; set; }

        public Path Path { get; set; }

        public SurfaceEffect Effect { get { return _effect; } }       

        public void Update(GameTime gameTime, Scene.CameraSceneNode sceneCamera, ITransformable pathParentSpace)
        {                     
            const int VERTICES_PER_POINT = 2;   
            float halfWidth = Width / 2.0f;                     
            Vector3[] points = this.Path.ToPoints(StepSize);
            VertexPositionColor[] vertices = new VertexPositionColor[(points.Length) * VERTICES_PER_POINT];                                         
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 nSlope = Vector3.Normalize((i < points.Length - 1) ? points[i + 1] - points[i] : points[i] - points[i - 1]);
                Vector3 p = points[i];
                //Transform point to camera space.
                Vector3 vw = ((pathParentSpace != null) ? pathParentSpace.ParentToWorldCoordinate(p) : p);
                Vector3 vw_camPos = vw - sceneCamera.Transform.Translation;
                Vector3 camDir = Vector3.Normalize(sceneCamera.Camera.ViewMatrix.Column3.ToVector3());
               //Project point transformed point to camera z plane.                
                //(See tmpearce's explanation at http://stackoverflow.com/questions/9605556/how-to-project-a-3d-point-to-a-3d-plane#comment12185786_9605695)
                Vector3 vpcs = vw - (Vector3.Dot(vw_camPos, camDir) * camDir);
                //Get direction to camera from projected point.
                Vector3 npcs = Vector3.Normalize(sceneCamera.Transform.Translation - vpcs);
                //Transform direction to container space.
                Vector3 nw = npcs;
                Vector3 n = (pathParentSpace != null) ? pathParentSpace.WorldToParentNormal(nw) : nw;
                //Get cross product of slope and direction to camera.
                Vector3 xDir = Vector3.Normalize(Vector3.Cross(n, nSlope));
                vertices[i * VERTICES_PER_POINT] = new VertexPositionColor(p + xDir * halfWidth, Color.White.ToVector4());
                vertices[i * VERTICES_PER_POINT + 1] = new VertexPositionColor(p - xDir * halfWidth, Color.White.ToVector4());
            }

            VertexPositionColor[] meshVertices = new VertexPositionColor[(points.Length - 1) * 6];

            Color[] colors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow };
            int colorIndex = 0;

            for (int i = 0; i < vertices.Length - 2; i += 2)
            {
                int j = i * 3;
                meshVertices[j] = vertices[i];
                meshVertices[j].Color = colors[colorIndex].ToVector4();
                meshVertices[j + 1] = vertices[i + 1];
                meshVertices[j + 1].Color = colors[colorIndex].ToVector4();
                meshVertices[j + 2] = vertices[i + 2];
                meshVertices[j + 2].Color = colors[colorIndex].ToVector4();
                colorIndex = (colorIndex == colors.Length - 1) ? 0 : colorIndex + 1;

                meshVertices[j + 3] = vertices[i + 2];
                meshVertices[j + 3].Color = colors[colorIndex].ToVector4();
                meshVertices[j + 4] = vertices[i + 1];
                meshVertices[j + 4].Color = colors[colorIndex].ToVector4();
                meshVertices[j + 5] = vertices[i + 3];
                meshVertices[j + 5].Color = colors[colorIndex].ToVector4();
                colorIndex = (colorIndex == colors.Length - 1) ? 0 : colorIndex + 1;
            }
             
            _mesh.UpdateVertexStream<VertexPositionColor>(meshVertices);                        
        }

        public void Draw(GameTime gameTime)
        {
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Renderer != null)
            {              
                Renderer.Update(gameTime, Scene.CameraNode, null);
            }
        }

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
