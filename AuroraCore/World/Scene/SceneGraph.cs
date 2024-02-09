using CipherPark.Aurora.Core.Extensions;
using CipherPark.Aurora.Core.World.Geometry;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Scene
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
                UpdateNodeHierarchy(gameTime, node);
            OnEndUpdate();
        }

        public void Draw(SceneRenderContext context = null)
        {
            OnBeginDraw(context);   
            DrawSkyModel();
            DrawStandardNodePipline(context);
            DrawTransparentNodePipline(context);
            ProcessNodePostEffects();
            DrawSprites();
            OnEndDraw(context);            
        }

        private void DrawStandardNodePipline(SceneRenderContext context)
        {
            foreach (SceneNode node in Nodes)
            {
                DrawStandardNodeHierarchy(node, context);
            }          
        }

        private void DrawTransparentNodePipline(SceneRenderContext context)
        {
            var transparencyNodes = Nodes.SelectNodes(n => 
                                        n.Pipeline == SceneNodePipeline.Transparency && 
                                        (n.IsVisibleInTree || context.ShowAll) &&
                                        n.MatchesFilter(context.NodeFilter));
            var orderedTransparencyNodes = transparencyNodes.OrderByDescending(n => Vector3.DistanceSquared(CameraNode.WorldPosition(), n.WorldPosition()));
            foreach (var transparentNode in orderedTransparencyNodes)
            {
                transparentNode.Draw();
            }
        }

        private void ProcessNodePostEffects()
        {
            OnBeginPostEffects();
            CameraNode.Camera.PostEffectChain.InputTexture = Game.RenderTargetShaderResource;
            CameraNode.Camera.PostEffectChain.OutputView = Game.RenderTargetView;
            CameraNode.Camera.PostEffectChain.Apply();
            OnEndPostEffects();
        }

        private void DrawSprites()
        {
            foreach (SpriteBatchContext context in _spriteBatchContexts)
            {
                context.Draw();
            }
        }

        private void DrawSkyModel()
        {
            if (SkyModel != null)
            {               
                SkyModel.Effect.View = CameraNode.RiggedViewMatrix;
                SkyModel.Effect.Projection = CameraNode.Camera.ProjectionMatrix;
                SkyModel.Draw();
            }
        }

        private void UpdateNodeHierarchy(GameTime gameTime, SceneNode node)
        {       
            node.Update(gameTime);
            foreach (SceneNode child in node.Children.ToList())
            {
                UpdateNodeHierarchy(gameTime, child);
            }
        }

        private void DrawStandardNodeHierarchy(SceneNode node, SceneRenderContext context)
        {
            if (node.Pipeline == SceneNodePipeline.Standard &&
                (node.Visible || context.ShowAll) &&
                node.MatchesFilter(context.NodeFilter))
            {
                node.Draw();
                foreach (SceneNode child in node.Children)
                {
                    DrawStandardNodeHierarchy(child, context);
                }
            }
        }

        protected virtual void OnBeginDraw(SceneRenderContext context)
        {
            EventHandler<SceneRenderContext> handler = BeginDraw;
            if (handler != null)
                handler(this, context);
        }

        protected virtual void OnEndDraw(SceneRenderContext context)
        {
            EventHandler<SceneRenderContext> handler = EndDraw;
            if (handler != null)
                handler(this, context);
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
      
        protected virtual void OnBeginPostEffects()
        {
            EventHandler handler = BeginPostEffects;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnEndPostEffects()
        {
            EventHandler handler = EndPostEffects;
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

        public event EventHandler<SceneRenderContext> BeginDraw;
        public event EventHandler<SceneRenderContext> EndDraw;
        public event EventHandler BeginUpdate;
        public event EventHandler EndUpdate;
        public event EventHandler BeginPostEffects;
        public event EventHandler EndPostEffects;
    }
}
