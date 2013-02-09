using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;

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
}
