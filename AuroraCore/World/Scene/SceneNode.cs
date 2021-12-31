using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SharpDX;
using CipherPark.KillScript.Core.Animation;
using CipherPark.KillScript.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.World.Scene
{
    [Obsolete]
    public interface ISceneObject
    {

    }

    public abstract class SceneNode : ITransformable, IDisposable
    {       
        private SceneNode _parent = null;
        private SceneNodes _children = null;
        private SceneGraph _scene = null;
        private ITransformable _transformableParent = null;
        private SceneNodeBehaviour _behaviour = null;
        private IGameApp _game = null;      

        public SceneNode(IGameApp game)
        {
            _game = game;
            _children = new SceneNodes();
            _children.CollectionChanged += Children_CollectionChanged;
            Transform = Transform.Identity;
            Visible = true;
        }

        public SceneNode(IGameApp game, string name)
            : this(game)
        {
            Name = name;
        }

        public IGameApp Game { get { return _game; } } 

        public virtual string Name { get; set; }

        public SceneGraph Scene 
        { 
            get { return _scene; } 
            set { _scene = value; OnSceneChanged(); } 
        }
        
        public SceneNode Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null && _parent.Children.Contains(this))
                    _parent.Children.Remove(this);
                _parent = value;
                if (_parent != null && !_parent.Children.Contains(this))
                    _parent.Children.Add(this);
                _transformableParent = value;
            }
        }
        
        public SceneNodes Children { get { return _children; } }

        public SceneNodeBehaviour Behaviour
        {
            get { return _behaviour; }
            set
            {
                if (value == null)
                {
                    var b = _behaviour;
                    _behaviour = null;
                    if (b.ContainerNode == this)
                        b.ContainerNode = null;                 
                }
                else
                {
                    _behaviour = value;
                    if (_behaviour.ContainerNode != this)
                        _behaviour.ContainerNode = this;
                }
            }
        }

        public ulong Flags { get; set; }

        public virtual Transform Transform { get; set; }        

        public virtual void Draw() { }

        public virtual void Update(GameTime gameTime)
        {
            Behaviour?.Update(this);
        }   

        /*
        public virtual BoundingBox CalcBoundingBox()
        {
            return BoundingBoxExtension.Empty;
        }

        public virtual BoundingBox  CalcWorldBoundingBox()
        {
            var localBoundingBox = CalcBoundingBox();
            if (localBoundingBox == BoundingBoxExtension.Empty)
                return BoundingBoxExtension.Empty;
            else
                return this.LocalToWorldBoundingBox(localBoundingBox);
        }
        */

        public virtual ITransformable TransformableParent
        {
            get { return this._transformableParent; }
            set
            {
                if (value is SceneNode)
                    this.Parent = (SceneNode)value;
                else
                {
                    this.Parent = null;
                    _transformableParent = value;
                }
            }
        }

        public bool Visible 
        { 
            get; set; 
        }

        public void Dispose()
        {
            OnDispose();
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
               
                case NotifyCollectionChangedAction.Replace:
                    foreach (SceneNode child in args.NewItems)
                        OnChildAdded(child);
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
        {
            
        }

        protected void OnSceneChanged()
        {
            foreach (SceneNode child in Children)
                child.Scene = this._scene;
        }

        protected virtual void OnDispose()
        {

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
