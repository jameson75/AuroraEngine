using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;

namespace CipherPark.AngelJacket.Core.World.Scene
{
    [Obsolete]
    public interface ISceneObject
    {

    }

    public abstract class SceneNode : ITransformable
    {
        private Scene _scene = null;
        private SceneNode _parent = null;
        private SceneNodes _children = null;        
        
        public SceneNode(Scene scene)
        {
            _scene = scene;
            _children = new SceneNodes();
            _children.CollectionChanged += Children_CollectionChanged;
           // Transform = Transform.Identity;
        }

        public Scene Scene { get { return _scene; } }

        public SceneNode Parent { get { return _parent; } set { _parent = value; } }

        public SceneNodes Children { get { return _children; } }
        
        public virtual Transform Transform { get; set; }

        [Obsolete]
        public ISceneObject SceneObject { get; set; }

        public virtual void Draw(long gameTime) { }

        public virtual void Update(long gameTime) { }

        public Matrix LocalToWorld(Matrix localTransform)
        {
            MatrixStack stack = new MatrixStack();
            stack.Push(localTransform);
            SceneNode node = this.Parent;
            while (node != null)
            {
                stack.Push(node.Transform.ToMatrix());
                node = node.Parent;
            }
            return stack.Transform;
        }

        public Transform LocalToWorld(Transform localTransform)
        {
            TransformStack stack = new TransformStack();
            stack.Push(localTransform);
            SceneNode node = this.Parent;
            while (node != null)
            {
                stack.Push(node.Transform);
                node = node.Parent;
            }
            return stack.Transform;
        } 

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (SceneNode child in args.NewItems)
                        OnChildAdded(child);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (SceneNode child in args.OldItems)
                        OnChildRemoved(child);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    OnChildReset();
                    break;
            }
        }

        protected void OnChildAdded(SceneNode child)
        {
            if (child.Parent != this)
                child.Parent = this;
        }

        protected void OnChildRemoved(SceneNode child)
        {
            if (child.Parent != this)
                child.Parent = this;
        }

        protected void OnChildReset()
        { }
    }
     
    public class SceneNodes :  ObservableCollection<SceneNode>
    {   
        public SceneNodes()
        { }    
       
        public void AddRange(IEnumerable<SceneNode> nodes)
        {
            foreach( SceneNode node in nodes )
                this.Add(node);
        }
    }

    public class NullSceneNode : SceneNode
    {
        public NullSceneNode(Scene scene)
            : base(scene)
        { }
    }

    public class ModelSceneNode : SceneNode
    {
        public ModelSceneNode(Scene scene)
            : base(scene)
        {
            HitTestable = true;
        }

        public ModelSceneNode(Scene scene, Model model) : base(scene)
        {
            Model = model;
            HitTestable = true;
        }

        public Model Model { get; set; }

        public bool HitTestable { get; set; }

        public override Transform Transform { get { return Model.Transform; } set { Model.Transform = value; } }

        public override void Draw(long gameTime)
        {            
            if (Model != null)
            {
                Model.Effect.World = LocalToWorld(this.Transform.ToMatrix());
                Model.Effect.View = Scene.Camera.ViewMatrix;
                Model.Effect.Projection = Scene.Camera.ProjectionMatrix;
                Model.Effect.Apply();
                Model.Draw(gameTime);
            }
        }
    }

    public class CameraSceneNode : SceneNode
    {
        private Matrix _cachedViewMatrix = Matrix.Zero;

        public CameraSceneNode(Scene scene)
            : base(scene)
        {
            
        }
        
        public SceneNode LockOnTarget { get; set; }

        public CameraSceneNode(Scene scene, Camera camera) : base(scene)
        {
            Camera = camera; 
        }

        public Camera Camera { get; set; }

        public override void Update(long gameTime)
        {
            if (Camera != null)
            {                
                ////TODO: Figure out how to use
                //if (this.LockOnTarget != null)
                //{
                //    Matrix targetTransform = LockOnTarget.LocalToWorld(LockOnTarget.Transform.ToMatrix());
                //    Matrix cameraTransform = LocalToWorld(this.Transform.ToMatrix());
                //    Vector3 worldViewFrom = -(Camera.ViewMatrix * Matrix.Invert(Camera.ViewMatrix * Matrix.Translation(-Camera.ViewMatrix.TranslationVector))).TranslationVector + cameraTransform.TranslationVector;
                //    Vector3 targetWorldPosition = (targetTransform * Matrix.Invert(targetTransform * Matrix.Translation(-targetTransform.TranslationVector))).TranslationVector;
                //    Camera.ViewMatrix = Matrix.LookAtLH(worldViewFrom, targetWorldPosition, Vector3.UnitY);
                //}
                //else 
                //    Camera.ViewMatrix = Matrix.Translation(-LocalToWorld(Transform.ToMatrix()).TranslationVector) * _cachedViewMatrix;

                //if (this.LockOnTarget != null)
                //{   
                //    //Determine look at vector.
                //    Vector3 lookAt = LockOnTarget.LocalToWorld(LockOnTarget.Transform).Translation;
                //    //Determine look at 
                //    Vector3 location = LocalToWorld(this.Transform).Translation;
                //    Vector3 up = DetermineUpVector(location, lookAt);                  
                //    //Mat
                //    //-(Camera.ViewMatrix * Matrix.Invert(Camera.ViewMatrix * Matrix.Translation(-Camera.ViewMatrix.TranslationVector)))
                //    //Vector3 worldViewFrom = -(Camera.ViewMatrix * Matrix.Invert(Camera.ViewMatrix * Matrix.Translation(-Camera.ViewMatrix.TranslationVector))).TranslationVector + location.TranslationVector;
                //    //Vector3 targetWorldPosition = (lookAt * Matrix.Invert(lookAt * Matrix.Translation(-lookAt.TranslationVector))).TranslationVector;
                //    Camera.ViewMatrix = Matrix.LookAtLH(location, lookAt, up);
                //}
                
                //else
                //    Camera.ViewMatrix = Matrix.Translation(-LocalToWorld(Transform.ToMatrix()).TranslationVector) * _cachedViewMatrix;

            }
        }

        public override Transform Transform
        {
            get
            {
                return Camera.ViewToTransform(Camera.ViewMatrix);
            }
            set
            {
                if (LockOnTarget != null)
                {
                    //Vector3 oldZAxis = new Vector3(Camera.ViewMatrix.Column3.ToArray().Take(3).ToArray());
                    //Vector3 oldYAxis = new Vector3(Camera.ViewMatrix.Column2.ToArray().Take(3).ToArray());
                    //Vector3 oldLocation = (Camera.ViewMatrix * Matrix.Invert(Camera.ViewMatrix * Matrix.Translation(-Camera.ViewMatrix.TranslationVector))).TranslationVector;
                    ////Determine look at vector.
                    //Vector3 lookAt = LockOnTarget.LocalToWorld(LockOnTarget.Transform).Translation;
                    ////Determine look at 
                    //Vector3 location = LocalToWorld(value).Translation;
                    ////Vector3 up = DetermineUpVector(location, lookAt);                    
                    //Vector3 newZAxis = Vector3.Normalize(Vector3.Subtract(lookAt, location));
                    ////if (oldLocation != location)
                    ////{
                    //Vector3 rotAxis = Vector3.Cross(oldZAxis, newZAxis);
                    //float cosTheta = Vector3.Dot(oldZAxis, newZAxis);
                    //Vector3 up = new Vector3(Vector3.Transform(oldZAxis, Matrix.RotationAxis(rotAxis, cosTheta)).ToArray().Take(3).ToArray());
                    ////Mat
                    ////-(Camera.ViewMatrix * Matrix.Invert(Camera.ViewMatrix * Matrix.Translation(-Camera.ViewMatrix.TranslationVector)))
                    ////Vector3 worldViewFrom = -(Camera.ViewMatrix * Matrix.Invert(Camera.ViewMatrix * Matrix.Translation(-Camera.ViewMatrix.TranslationVector))).TranslationVector + location.TranslationVector;
                    ////Vector3 targetWorldPosition = (lookAt * Matrix.Invert(lookAt * Matrix.Translation(-lookAt.TranslationVector))).TranslationVector;
                    //Camera.ViewMatrix = Matrix.LookAtLH(location, lookAt, up);
                    ////}
                    ////else 
                    ////{
                    ////    Vector3 up = new Vector3(Vector3.Transform(

                    Matrix specifiedNewView = Camera.TransformToView(value);
                    Vector3 up = new Vector3(specifiedNewView.Column2.ToArray().Take(3).ToArray());
                    Vector3 lookAt = LockOnTarget.LocalToWorld(LockOnTarget.Transform).Translation;
                    Vector3 eye = LocalToWorld(value).Translation;
                    Camera.ViewMatrix = Matrix.LookAtLH(eye, lookAt, up);
                }
                else
                    Camera.ViewMatrix = Camera.TransformToView(LocalToWorld(value));
            }
        }

        //private Vector3 DetermineUpVector(Vector3 camLocation, Vector3 camLookAt)
        //{
        //    Vector4 r3 = LocalToWorld(this.Transform).ToMatrix().Row3;
        //    Vector4 r2  = LocalToWorld(this.Transform).ToMatrix().Row2;
        //    Vector3 orientationZ = Vector3.Normalize(new Vector3(r3.X, r3.Y, r3.Z));
        //    Vector3 orientationY = Vector3.Normalize(new Vector3(r2.X, r2.Y, r3.Z));          
        //    Vector3 u = Vector3.Normalize(camLookAt - camLocation);
        //    Vector3 v = orientationZ;
        //    Vector3 n = Vector3.Cross(u, v);
        //    float cosTheta = Vector3.Dot(u, v);           
        //    float theta = (float)Math.Acos(cosTheta);
        //    float thetaDegrees = MathUtil.RadiansToDegrees(theta); //for debugging purposes.            
        //    Vector3 up = Vector3.Normalize(Vector3.Transform(orientationY, Quaternion.RotationAxis(n, theta)));
        //    return up;
        //}
    }

    public class PlexSceneNode
    {

    }
}
