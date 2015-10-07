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
        const int N_CYLINDER_SIDES = 8;
        const int VERTS_PER_SIDE = 6;
        const int INDICES_PER_SIDE = 4;

        private Mesh _mesh = null;
        private Vector3[] _meshGeometry = null;
        private Path _path = null;
        private bool _geometryRequiresUpdate = true;
        private List<PathNode> _prevPathNodes = null;
        private Vector3 prev_up = Vector3.UnitY;        

        private StreakRenderer()
        { }
        
        public static StreakRenderer Create(IGameApp game)
        {           
            StreakEffect effect = new StreakEffect(game);
            //effect.EnableVertexColor = true;            
            return new StreakRenderer()
            {          
                Color = Color.Transparent,
                Radius = 5.0f,
                StepSize = 10.0f,
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

        public bool PreserveGeometry { get; set; }      

        public ITransformable PathParent { get; set; }

        public Color Color { get; set; }

        public SurfaceEffect Effect { get; private set; }       

        public Path Path 
        {
            get { return _path; }
            set 
            {
                if (_path != null)
                    _path.PathChanged += Path_Changed;
                _path = value;
                if (value != null)
                    value.PathChanged += Path_Changed;
            }
        }

        private void Path_Changed(object sender, EventArgs e)
        {
            _geometryRequiresUpdate = true;
        }  

        public void Update(GameTime gameTime)
        {
            if (_geometryRequiresUpdate)
            {
                Path pathNewGeometry = null;                
                //Vector3[] preservedGeometry = null;
                List<PathNode> currentPathNodes = Path.GetNodes();

                //Step 1.
                //If we're preserving geometry and we have a snapshot of the path
                //from a previous update, then we attempt to preserve as much of 
                //the previously-constructed geometry as possible (instead of 
                //rebuilding the entire geometry from scratch). Preserving geometry
                //avoids unwanted animation of segment joints.       
                /*
                /* COMMENTED OUT UNTIL I GET THE MATH STRAIGHTEND OUT.
                if (PreserveGeometry && _prevPathNodes != null)
                {                  
                    pathNewGeometry = new Path();

                    //Compare the prevPathSnapshot with Path to determine the newPath (new portion of path).                                            
                    PathNode last = _prevPathNodes.LastOrDefault();
                    int li = currentPathNodes.IndexOf(last);
                    if (li != -1 && last != currentPathNodes.LastOrDefault())
                    {                        
                        for (int i = li; i < currentPathNodes.Count(); i++)
                            pathNewGeometry.AddNode(currentPathNodes[i]);   
                        pathNewGeometry.GenerateLinearApproximation();                    
                    }                   
                    
                    //Compare the prevPathSnapshot with Path to determine how much geometry to remove from the current geometry.
                    if (_meshGeometry != null)
                    {
                        PathNode first = currentPathNodes.FirstOrDefault();
                        int nRemovedNodes = _prevPathNodes.IndexOf(first);
                        if (nRemovedNodes != -1)
                        {
                            Path removedPath = new Path();
                            removedPath.AddNodes(_prevPathNodes.Take(nRemovedNodes + 1), true);
                            int nRemovedSteps = this.EvaluatePathNodes(removedPath).Count(); 
                            preservedGeometry = _meshGeometry.Skip(nRemovedSteps * N_CYLINDER_SIDES).ToArray();
                        }
                    }
                }
                else
                */
                {
                    prev_up = Vector3.UnitY;
                    pathNewGeometry = Path;
                }

                //Step 2.
                Vector3[] newGeometry = null;
                Vector3[] newPathPoints = this.EvaluatePathNodes(pathNewGeometry).Select(n => n.Transform.Translation).ToArray(); 
                newGeometry = new Vector3[(newPathPoints.Length) * N_CYLINDER_SIDES];
                    
                for (int i = 0; i < newPathPoints.Length; i++)
                {
                    //Store the current point.
                    Vector3 p = newPathPoints[i];

                    //Get the direction vector from the current point to the next point.
                    Vector3 d = (i < newPathPoints.Length - 1) ? newPathPoints[i + 1] - p : p - newPathPoints[i - 1];

                    //Get the normalized direction from the current point to the next point.
                    Vector3 n = Vector3.Normalize(d);

                    //Draw tube segment in container space where the top of the tube has normal of "projected_p_cam".
                    //This way, each tube segment drawn has a "top" that always faces the view ray of the camera.
                    float step = 2.0f * (float)Math.PI / N_CYLINDER_SIDES;

                    //To prevent the cylinder from twisting (and creating unwanted sharp edges) we preserve the relative
                    //direction of the up vector.
                    Vector3 up = prev_up;

                    //Calculate new up vector based on the previous up vector and change in orientation of the path normal.
                    if (i > 0)
                    {
                        Vector3 prev_p = newPathPoints[i - 1];
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
                        Vector3 mp = Vector3.TransformCoordinate(new Vector3()
                        {
                            X = Radius * (float)Math.Cos(angle),
                            Y = Radius * (float)Math.Sin(angle)
                        }, nodeMatrix);
                        newGeometry[i * N_CYLINDER_SIDES + j] = mp;
                    }
                    prev_up = up;
                }      

                //Step 3.
                /* COMMENTED OUT UNTIL I GET THE MATH STRAIGHTEND OUT.
                if (preservedGeometry != null)
                {
                    //Attach new geometry to preserved mesh geometry.                                       
                    _meshGeometry = preservedGeometry.Concat(newGeometry).ToArray();
                }
                else 
                */
                    _meshGeometry = newGeometry;

                //Step 4.               
                int nCylinders = (_meshGeometry.Length / N_CYLINDER_SIDES) - 1;
                VertexPositionColor[] meshVertices = new VertexPositionColor[nCylinders * N_CYLINDER_SIDES * VERTS_PER_SIDE];
                int vi = 0;
                for (int i = 0; i < nCylinders; i++)
                {
                    for (int j = 0; j < N_CYLINDER_SIDES; j++)
                    {
                        int[] qi = new int[INDICES_PER_SIDE];
                        qi[0] = i * N_CYLINDER_SIDES + j;
                        qi[1] = (i + 1) * N_CYLINDER_SIDES + j;
                        if (j < N_CYLINDER_SIDES - 1)
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
                            meshVertices[vi] = new VertexPositionColor(_meshGeometry[gi[k]], Color.ToVector4());
                            vi++;
                        }
                    }
                }

                _mesh.UpdateVertexStream<VertexPositionColor>(meshVertices);           

                //Step 5.                
                _prevPathNodes = _path.GetNodes();
                
                _geometryRequiresUpdate = false;
            }
        }  

        public void Draw(GameTime gameTime)
        {
            Effect.World = Matrix.Identity;
            _mesh.Draw(gameTime);
        }

        private IEnumerable<PathNode> EvaluatePathNodes(Path p)
        {
            float stepSize_ = this.PreserveGeometry ? -1 : StepSize;
            if (stepSize_ < 0)
                return p.GetNodes();
            else
                return p.EvaluateEquidistantNodes(stepSize_);
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
                Renderer.Effect.Restore();
            }
        }
    }   
}
