using System;
using System.Linq;
using System.Collections.Generic;
using CipherPark.KillScript.Core.World.Geometry;
using System.Collections.Specialized;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.World.Scene
{
    public class SceneGraph
    {
        private IGameApp _game = null;
        private SceneNodes _nodes = null;
        private List<SpriteBatchContext> _spriteBatchContexts = null;

        public SceneGraph(IGameApp game)
        {
            _game = game;
            _nodes = new SceneNodes();            
            _nodes.CollectionChanged += OnCollectionChanged;
            _spriteBatchContexts = new List<SpriteBatchContext>();
        }

        public IGameApp Game { get { return _game; } }
        
        public SceneNodes Nodes { get { return _nodes; } }
        
        public CameraSceneNode CameraNode { get; set; }

        public Model SkyModel { get; set; }

        public List<SpriteBatchContext> SpriteBatchContexts { get { return _spriteBatchContexts; } }

        public void Update(GameTime gameTime)
        {
            OnBeginUpdate();
            foreach (SceneNode node in Nodes.ToList())
                _UpdateNodeHierarchy(gameTime, node);
            OnEndUpdate();
        }

        public void Draw()
        {
            OnBeginDraw();
            //CameraNode.Camera.PostEffectChain.Begin(gameTime);            
            _DrawSkyModel();
            foreach (SceneNode node in Nodes)
                _DrawNodeHierarchy(node);            
            //CameraNode.Camera.PostEffectChain.End(gameTime);
            
            CameraNode.Camera.PostEffectChain.InputTexture = Game.RenderTargetShaderResource;
            CameraNode.Camera.PostEffectChain.OutputView = Game.RenderTargetView;
            CameraNode.Camera.PostEffectChain.Apply();

            _DrawSprites();
            OnEndDraw();            
        }

        private void _DrawSprites()
        {
            foreach (SpriteBatchContext context in _spriteBatchContexts)
                context.Draw();
        }

        private void _DrawSkyModel()
        {
            if (SkyModel != null)
            {               
                SkyModel.Effect.View = CameraNode.Camera.ViewMatrix;
                SkyModel.Effect.Projection = CameraNode.Camera.ProjectionMatrix;
                SkyModel.Draw();
            }
        }

        private void _UpdateNodeHierarchy(GameTime gameTime, SceneNode node)
        {       
            node.Update(gameTime);            
            foreach (SceneNode child in node.Children.ToList())
                _UpdateNodeHierarchy(gameTime, child);           
        }

        private void _DrawNodeHierarchy(SceneNode node)
        {
            if (node.Visible)
            {
                node.Draw();
                foreach (SceneNode child in node.Children)
                    _DrawNodeHierarchy(child);
            }
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

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (SceneNode child in e.OldItems)
                        OnChildRemoved(child);
                    foreach (SceneNode child in e.NewItems)
                        OnChildAdded(child);
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

    public enum SpatialReference
    {
        BoundingBoxTop,
        BoundingBoxBottom,
        BoundingBoxLeft,
        BoundingBoxRight,
        BoundingBoxFront,
        BoundingBoxBack
    }
}
