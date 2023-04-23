using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Utils;
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
        private uint _maxStandardPipelineDrawPasses;

        public SceneGraph(IGameApp game)
        {
            _game = game;
            _nodes = new SceneNodes();            
            _nodes.CollectionChanged += OnCollectionChanged;
            _spriteBatchContexts = new List<SpriteBatchContext>();
            MaxStandardPipelineDrawPasses = 1;
        }

        public IGameApp Game { get { return _game; } }
        
        public SceneNodes Nodes { get { return _nodes; } }
        
        public CameraSceneNode CameraNode { get; set; }

        public Model SkyModel { get; set; }
        
        public uint MaxStandardPipelineDrawPasses
        {
            get => _maxStandardPipelineDrawPasses;
            set
            {
                if (value < 1)
                {
                    throw new InvalidOperationException($"{nameof(MaxStandardPipelineDrawPasses)} value must be greater than 0");
                }

                _maxStandardPipelineDrawPasses = value;
            }
        }

        public List<SpriteBatchContext> SpriteBatchContexts { get { return _spriteBatchContexts; } }        

        public void Update(GameTime gameTime)
        {
            OnBeginUpdate();
            foreach (SceneNode node in Nodes.ToList())
                UpdateNodeHierarchy(gameTime, node);
            OnEndUpdate();
        }

        public void Draw()
        {
            OnBeginDraw();   
            DrawSkyModel();
            DrawStandardNodePipline();
            DrawTransparentNodePipline();
            ProcessNodePostEffects();
            DrawSprites();
            OnEndDraw();            
        }

        private void DrawStandardNodePipline()
        {
            for (int i = 0; i < MaxStandardPipelineDrawPasses; i++)
            {
                OnBeginDrawPass(i);
                foreach (SceneNode node in Nodes)
                    DrawStandardNodeHierarchy(node, i);
                OnEndDrawPass(i);
            }
        }

        private void DrawTransparentNodePipline()
        {
            var transparencyNodes = Nodes.SelectNodes(n => n.Pipeline == SceneNodePipeline.Transparency && n.IsVisibleInTree);
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

        private void DrawStandardNodeHierarchy(SceneNode node, int pass)
        {
            if (node.Visible && 
                node.RenderPass == pass && 
                node.Pipeline == SceneNodePipeline.Standard)
            {
                node.Draw();
                foreach (SceneNode child in node.Children)
                {
                    DrawStandardNodeHierarchy(child, pass);
                }
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
        
        protected virtual void OnBeginDrawPass(int pass)
        {
            EventHandler<int> handler = BeginDrawPass;
            if (handler != null)
                handler(this, pass);
        }

        protected virtual void OnEndDrawPass(int pass)
        {
            EventHandler<int> handler = EndDrawPass;
            if (handler != null)
                handler(this, pass);
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

        public event EventHandler BeginDraw;
        public event EventHandler EndDraw;
        public event EventHandler BeginUpdate;
        public event EventHandler EndUpdate;
        public event EventHandler<int> BeginDrawPass;
        public event EventHandler<int> EndDrawPass;
        public event EventHandler BeginPostEffects;
        public event EventHandler EndPostEffects;
    }

    public enum SceneNodePipeline
    {
        Standard =     0,
        Transparency = 1
    }
}
