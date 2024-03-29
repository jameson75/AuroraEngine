﻿using System;
using System.Collections.Specialized;
using CipherPark.Aurora.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Scene
{
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

        public IGameApp Game 
        { 
            get { return _game; } 
        }

        public virtual string Name { get; set; }

        public SceneGraph Scene
        {
            get { return _scene; }
            set 
            { 
                _scene = value; 
                HandleSceneChanged(); 
            }
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

        public SceneNodePipeline Pipeline { get; set; } = SceneNodePipeline.Standard;

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

        public string[] Tags { get; set; }

        public bool Visible { get; set; }

        public bool IsVisibleInTree
        {
            get
            {
                return Visible && (Parent == null || Parent.IsVisibleInTree);
            }
        }

        public virtual void Draw() 
        { 
        }

        public virtual void Update(GameTime gameTime)
        {
            Behaviour?.Update(this);
        }

        public void Dispose()
        {
            Dispose(false);
        }

        public void Dispose(bool disposeChildren)
        {
            if (disposeChildren)
            {
                foreach (var child in Children)
                    child.Dispose(true);
            }

            OnDisposed();
        }

        protected virtual void OnChildAdded(SceneNode child)
        {
            if (child.Parent != this)
                child.Parent = this;

            if (child.Scene != this.Scene)
                child.Scene = this.Scene;
        }

        protected virtual void OnChildRemoved(SceneNode child)
        {
            child.Parent = null;
            child.Scene = null;
        }

        protected virtual void OnChildReset()
        {
        }     

        protected virtual void OnDisposed()
        {
        }

        private void HandleSceneChanged()
        {
            foreach (SceneNode child in Children)
                child.Scene = this._scene;
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
    }
}
