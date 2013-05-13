using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Scene
{
    [Obsolete]
    public interface ISceneObject
    {

    }

    public abstract class SceneNode : ITransformable
    {        
        private IGameApp _game = null;
        private SceneNode _parent = null;
        private SceneNodes _children = null;
        private Scene _scene = null;

        public SceneNode(IGameApp game)
        {
            _game = game;
            _children = new SceneNodes();
            _children.CollectionChanged += Children_CollectionChanged;
           // Transform = Transform.Identity;
        }

        public SceneNode(IGameApp game, string name)
            : this(game)
        {
            Name = name;
        }

        public virtual string Name { get; set; }

        public Scene Scene { get { return _scene; } set { _scene = value; OnSceneChanged(); } }

        public SceneNode Parent { get { return _parent; } set { _parent = value; } }

        public SceneNodes Children { get { return _children; } }
        
        public virtual Transform Transform { get; set; }

        public virtual BoundingBox Bounds { get { return new BoundingBox(Transform.Translation, Transform.Translation); } }

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

        public Transform WorldToLocal(Transform worldTransform)
        {
            TransformStack stack = new TransformStack();
            stack.Push(worldTransform);
            SceneNode node = this.Parent;
            while (node != null)
            {
                stack.Push(Animation.Transform.Invert(node.Transform));
                node = node.Parent;
            }
            return stack.ReverseTransform;
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

            if (child.Scene != this.Scene)
                child.Scene = this.Scene;
        }

        protected void OnChildRemoved(SceneNode child)
        {
            child.Parent = null;
            child.Scene = null;
        }

        protected void OnChildReset()
        { }

        protected void OnSceneChanged()
        {
            foreach (SceneNode child in Children)
                child.Scene = this._scene;
        }
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

        public SceneNode this[string name]
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                    if (this[i].Name == name)
                        return this[i];
                throw new IndexOutOfRangeException();
            }
        }
    }  
}
