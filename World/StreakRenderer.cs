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
    public interface IRenderer
    {
        SurfaceEffect Effect { get; }
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }

    public class StreakRenderer : IRenderer
    {       
        private Mesh _mesh = null;
                
        private StreakRenderer()
        { }
        
        public static StreakRenderer Create(IGameApp game)
        {           
            BasicEffectEx effect = new BasicEffectEx(game);
            effect.EnableVertexColor = true;            
            return new StreakRenderer()
            {          
                Color = Color.Transparent,
                Radius = 5.0f,
                StepSize = 20.0f,
                Effect = effect,
                _mesh = Content.ContentBuilder.BuildDynamicMesh<VertexPositionColor>(
                    game.GraphicsDevice,
                    effect.SelectShaderByteCode(),
                    null,
                    6000,
                    null,
                    0,
                    VertexPositionColor.InputElements,
                    VertexPositionColor.ElementSize,
                    BoundingBoxExtension.Empty, PrimitiveTopology.TriangleList)                
            };
        }

        public float Radius { get; set; }

        public float StepSize { get; set; }

        public Path Path { get; set; }

        public ITransformable PathParent { get; set; }

        public Color Color { get; set; }

        public SurfaceEffect Effect { get; private set; }

        //public Scene.CameraSceneNode SceneCamera { get; set; }

        public void Update(GameTime gameTime)
        {                     
            const int N_CYLINDER_SIDES = 8;
            Vector3[] pathPoints = this.Path.EvaluateEquidistantNodes(StepSize).Select(n => n.Transform.Translation).ToArray();
            Vector3[] meshGeometry = new Vector3[(pathPoints.Length) * N_CYLINDER_SIDES];
            Vector3 prev_up = Vector3.UnitY;                        
            
            for (int i = 0; i < pathPoints.Length; i++)
            {
                //Store the current point.
                Vector3 p = pathPoints[i];              
                
                //Get the direction vector from the current point to the next point.
                Vector3 d = (i < pathPoints.Length - 1) ? pathPoints[i + 1] - p : p - pathPoints[i - 1];

                //Get the normalized direction from the current point to the next point.
                Vector3 n = Vector3.Normalize(d);
                
                /*
                //Get the distance from the current point to the next point.
                float l = d.Length();                
               
                //Transform point to world space.
                Vector3 p_ws = ((PathParent != null) ? PathParent.ParentToWorldCoordinate(p) : p);

                //Store camera location in world space.
                Vector3 camPos_ws = p_ws - SceneCamera.Transform.Translation;

                //get view normal in world space.
                Vector3 camDirection_ws = Vector3.Normalize(SceneCamera.Camera.ViewMatrix.Column3.ToVector3());
                
                //Project point transformed point to camera z plane (defined in world space).                
                //(See tmpearce's explanation at http://stackoverflow.com/questions/9605556/how-to-project-a-3d-point-to-a-3d-plane#comment12185786_9605695)
                Vector3 projected_p_ws = p_ws - (Vector3.Dot(camPos_ws, camDirection_ws) * camDirection_ws);
                
                //Get direction to camera from projected point.
                Vector3 projected_p_cam_ws = Vector3.Normalize(SceneCamera.Transform.Translation - projected_p_ws);
                
                //Transform direction to container space.               
                Vector3 projected_p_cam = (PathParent != null) ? PathParent.WorldToParentNormal( projected_p_cam_ws) : projected_p_cam_ws;                                                
                */

                //Draw tube segment in container space where the top of the tube has normal of "projected_p_cam".
                //This way, each tube segment drawn has a "top" that always faces the view ray of the camera.
                float step = 2.0f * (float)Math.PI / N_CYLINDER_SIDES;

                //To prevent the cylinder from twisting (and creating unwanted sharp edges) we preserve the relative
                //direction of the up vector.
                Vector3 up = prev_up;
                if(i > 0)
                {
                    Vector3 prev_p = pathPoints[i - 1]; 
                    Vector3 prev_d = p - prev_p;
                    Vector3 prev_n = Vector3.Normalize(prev_d);
                    //http://forums.cgsociety.org/archive/index.php/t-741227.html
                    float angle = (float)Math.Acos(Vector3.Dot(prev_n, n));
                    if (angle > float.Epsilon)
                    {
                        Vector3 rotationVector = Vector3.Normalize(Vector3.Cross(prev_n, n));
                        if (rotationVector != Vector3.Zero)
                            up = Vector3.TransformNormal(prev_up, Matrix.RotationAxis(rotationVector, angle));
                    }                                      
                }
                Matrix parentMatrix = PathParent != null ? PathParent.WorldTransform().ToMatrix() : Matrix.Identity;
                Matrix nodeMatrix = Camera.ViewMatrixToTransform(Matrix.LookAtLH(p, p + n, up)).ToMatrix() * parentMatrix;
                
                for (int j = 0; j < N_CYLINDER_SIDES; j++)
                {                  
                    float angle = -step * j;
                    Vector3 mp =  Vector3.TransformCoordinate(new Vector3()
                    {
                        X = Radius * (float)Math.Cos(angle),
                        Y = Radius * (float)Math.Sin(angle)
                    }, nodeMatrix);                   
                    meshGeometry[i * N_CYLINDER_SIDES + j] = mp;
                }
                prev_up = up;
            }

            const int VERTS_PER_SIDE = 6;
            const int INDICES_PER_SIDE = 4;
            int nCylinders = pathPoints.Length - 1;
           
            VertexPositionColor[] meshVertices = new VertexPositionColor[nCylinders * N_CYLINDER_SIDES * VERTS_PER_SIDE];
       
            int vi = 0;

            for (int i = 0; i < nCylinders; i++)
            {
                for(int j = 0; j < N_CYLINDER_SIDES; j++)
                {
                    int[] qi = new int[INDICES_PER_SIDE];
                    qi[0] = i * N_CYLINDER_SIDES + j;
                    qi[1] = (i + 1) * N_CYLINDER_SIDES + j;
                    if( j < N_CYLINDER_SIDES - 1)
                    {
                        qi[2] = qi[1] + 1;
                        qi[3] = qi[0] + 1;
                    }
                    else 
                    {
                        qi[2] = (i + 1) * N_CYLINDER_SIDES;
                        qi[3] = i * N_CYLINDER_SIDES;
                    }

                    int[] gi = new int[VERTS_PER_SIDE] { qi[0], qi[1], qi[2], qi[0], qi[2], qi[3] };
                    for (int k = 0; k < gi.Length; k++)
                    {                         
                        meshVertices[vi] = new VertexPositionColor(meshGeometry[gi[k]], Color.ToVector4());
                        vi++;                        
                    }                   
                }               
            }
            Console.WriteLine("Vertex Count {0}", meshVertices.Count());
            _mesh.UpdateVertexStream<VertexPositionColor>(meshVertices);                        
        }

        /*********************************************************************
         * OBSOLETE
         * *******************************************************************
         
        public void Update(GameTime gameTime)
        {
            const int VERTICES_PER_POINT = 2;
            float halfWidth = Width / 2.0f;
            Vector3[] points = this.Path.ToPoints(StepSize);
            VertexPositionColor[] vertices = new VertexPositionColor[(points.Length) * VERTICES_PER_POINT];
            for (int i = 0; i < points.Length; i++)
            {
                //*****************************************************************************
                //TODO: BELOW. WE'RE PERFORMING AN ORTHOGONAL PROJECTION. 
                //REPLACE THIS AND PERFORM A PERSEPCTIVE PROJECTION.
                //*****************************************************************************

                Vector3 nSlope = Vector3.Normalize((i < points.Length - 1) ? points[i + 1] - points[i] : points[i] - points[i - 1]);
                Vector3 p = points[i];
                //Transform point to camera space.
                Vector3 vw = ((PathParent != null) ? PathParent.ParentToWorldCoordinate(p) : p);
                Vector3 vw_camPos = vw - SceneCamera.Transform.Translation;
                Vector3 camDir = Vector3.Normalize(SceneCamera.Camera.ViewMatrix.Column3.ToVector3());
                //Project point transformed point to camera z plane.                
                //(See tmpearce's explanation at http://stackoverflow.com/questions/9605556/how-to-project-a-3d-point-to-a-3d-plane#comment12185786_9605695)
                Vector3 vpcs = vw - (Vector3.Dot(vw_camPos, camDir) * camDir);
                //Get direction to camera from projected point.
                Vector3 npcs = Vector3.Normalize(SceneCamera.Transform.Translation - vpcs);
                //Transform direction to container space.
                Vector3 nw = npcs;
                Vector3 n = (PathParent != null) ? PathParent.WorldToParentNormal(nw) : nw;
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
        */

        public void Draw(GameTime gameTime)
        {
            Effect.World = Matrix.Identity;
            _mesh.Draw(gameTime);
        }
    }

    public class RendererSceneNode : Scene.SceneNode
    {
        public IRenderer Renderer { get; set; }

        public RendererSceneNode(IGameApp game)
            : base(game)
        { }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Renderer != null)
            {         
                Renderer.Update(gameTime);
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

    public class LightTrail : IRenderer
    {
        private StreakRenderer _streakRenderer = null;
        private PathTracker _tracker = null;

        private LightTrail()
        { }

        public static LightTrail Create(IGameApp game)
        {
            LightTrail trail = new LightTrail();
            trail._streakRenderer = StreakRenderer.Create(game); 
            trail.Color = Color.Transparent;          
            return trail;
        }

        public void Update(GameTime gameTime)
        {
            if (_tracker != null)
            {
                _tracker.Update(gameTime);
                if (_tracker.Path.Nodes.Count > 1)
                {
                    _streakRenderer.Path = _tracker.Path;
                    _streakRenderer.Update(gameTime);
                }
            }
        }

        public ITransformable Anchor
        {
            get
            {
                if (_tracker != null)
                    return _tracker.Target;
                else
                    return null;
            }
            set
            {
                if (value == null)
                    _tracker = null;
                else
                    _tracker = new PathTracker()
                    {
                        PathNodeMinDistance = 20.0f,
                        Target = value
                    };            
            }
        }

        public Color Color
        {
            get { return _streakRenderer.Color; }
            set { _streakRenderer.Color = value; }
        }

        public void Draw(GameTime gameTime)
        {
            if (_streakRenderer.Path != null)
                _streakRenderer.Draw(gameTime);
        }

        public SurfaceEffect Effect
        {
            get { return _streakRenderer.Effect; }
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

    public static class Vector3Extension2
    {
        public static bool IsAnyComponentNaN(this Vector3 v)
        {
            return float.IsNaN(v.X) || float.IsNaN(v.Y) || float.IsNaN(v.Z);
        }
    }
}
