using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Systems.Animation;
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
            Transform = Transform.Identity;
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
            SceneNode node = this;
            while (node != null)
            {
                stack.Push(node.Transform.ToMatrix());
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
        { }

        public ModelSceneNode(Scene scene, Model model) : base(scene)
        {
            Model = model;
        }

        public Model Model { get; set; }

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
            scene.BeginUpdate += Scene_BeginUpdate;
            scene.EndDraw += Scene_EndDraw;
        }
        
        public SceneNode LockOnTarget { get; set; }

        public CameraSceneNode(Scene scene, Camera camera) : base(scene)
        {
            Camera = camera;           
            scene.EndDraw += Scene_EndDraw; scene.BeginUpdate += Scene_BeginUpdate;
            scene.EndDraw += Scene_EndDraw;
        }

        public Camera Camera { get; set; }

        public override void Update(long gameTime)
        {
            if (Camera != null)
            {                
                //TODO: Figure out how to use
                if (this.LockOnTarget != null)
                {
                    Matrix targetTransform = LockOnTarget.LocalToWorld(LockOnTarget.Transform.ToMatrix());
                    Matrix cameraTransform = LocalToWorld(this.Transform.ToMatrix());
                    Vector3 worldViewFrom = -(Camera.ViewMatrix * Matrix.Invert(Camera.ViewMatrix * Matrix.Translation(-Camera.ViewMatrix.TranslationVector))).TranslationVector + cameraTransform.TranslationVector;
                    Vector3 targetWorldPosition = (targetTransform * Matrix.Invert(targetTransform * Matrix.Translation(-targetTransform.TranslationVector))).TranslationVector;
                    Camera.ViewMatrix = Matrix.LookAtLH(worldViewFrom, targetWorldPosition, Vector3.UnitY);
                }
                else 
                    Camera.ViewMatrix = Matrix.Translation(-LocalToWorld(Transform.ToMatrix()).TranslationVector) * _cachedViewMatrix;
            }
        }

        private void Scene_EndDraw(object sender, EventArgs e)
        {
            if (Camera != null)
            {
                Camera.ViewMatrix = _cachedViewMatrix;
            }
        }

        private void Scene_BeginUpdate(object sender, EventArgs e)
        {
            if (Camera != null)
            {
                _cachedViewMatrix = Camera.ViewMatrix;
            }
        }
    }
}
