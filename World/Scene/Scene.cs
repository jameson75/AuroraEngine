using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Utils;
using System.Collections.Specialized;

namespace CipherPark.AngelJacket.Core.World.Scene
{
    public class Scene
    {
        private IGameApp _game = null;
        private SceneNodes _nodes = null;
        private MatrixStack _worldTransformStack = null;

        public Scene(IGameApp game)
        {
            _game = game;
            _nodes = new SceneNodes();
            _worldTransformStack = new MatrixStack();
            _nodes.CollectionChanged += OnCollectionChanged;
        }

        public IGameApp Game { get { return _game; } }
        
        public SceneNodes Nodes { get { return _nodes; } }
        
        public Camera Camera { get; set; }

        public void Update(long gameTime)
        {
            OnBeginUpdate();
            foreach (SceneNode node in Nodes)
                _UpdateNodeHierarchy(gameTime, node);
            OnEndUpdate();
        }       

        public void Draw(long gameTime)
        {
            Camera.PostEffectChain.Begin(gameTime);
            OnBeginDraw();
            foreach (SceneNode node in Nodes)
                _DrawNodeHierarchy(gameTime, node);
            OnEndDraw();
            Camera.PostEffectChain.End(gameTime);
        }
        
        private void _UpdateNodeHierarchy(long gameTime, SceneNode node)
        {       
            node.Update(gameTime);            
            foreach (SceneNode child in node.Children)
                _UpdateNodeHierarchy(gameTime, child);           
        }

        private void _DrawNodeHierarchy(long gameTime, SceneNode node)
        {            
            node.Draw(gameTime);           
            foreach (SceneNode child in node.Children)
                _DrawNodeHierarchy(gameTime, child);
        }

        protected virtual void OnBeginDraw()
        {
            EventHandler handler = BeginDraw;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnEndDraw()
        {
            EventHandler handler = EndDraw;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnBeginUpdate()
        {
            EventHandler handler = BeginUpdate;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnEndUpdate()
        {
            EventHandler handler = EndUpdate;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (SceneNode child in e.NewItems)
                        OnChildAdded(child);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (SceneNode child in e.OldItems)
                        OnChildRemoved(child);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    OnChildReset();
                    break;
            }
        }

        protected void OnChildAdded(SceneNode child)
        {
            if (child.Scene != this)
                child.Scene = this;
        }

        protected void OnChildRemoved(SceneNode child)
        {
            child.Scene = null;
        }

        protected void OnChildReset()
        {

        }

        public event EventHandler BeginDraw;
        public event EventHandler EndDraw;
        public event EventHandler BeginUpdate;
        public event EventHandler EndUpdate;
    }
}
