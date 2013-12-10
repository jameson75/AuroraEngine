using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.World.Geometry;
using System.Collections.Specialized;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Scene
{
    public class Scene
    {
        private IGameApp _game = null;
        private SceneNodes _nodes = null;      

        public Scene(IGameApp game)
        {
            _game = game;
            _nodes = new SceneNodes();            
            _nodes.CollectionChanged += OnCollectionChanged;
        }

        public IGameApp Game { get { return _game; } }
        
        public SceneNodes Nodes { get { return _nodes; } }
        
        public CameraSceneNode CameraNode { get; set; }

        public Model SkyModel { get; set; }

        public void Update(long gameTime)
        {
            OnBeginUpdate();
            foreach (SceneNode node in Nodes)
                _UpdateNodeHierarchy(gameTime, node);
            OnEndUpdate();
        }       

        public void Draw(long gameTime)
        {
            //CameraNode.Camera.PostEffectChain.Begin(gameTime);
            OnBeginDraw();
            _DrawSkyModel(gameTime);
            foreach (SceneNode node in Nodes)
                _DrawNodeHierarchy(gameTime, node);
            OnEndDraw();
            //CameraNode.Camera.PostEffectChain.End(gameTime);
        }

        private void _DrawSkyModel(long gameTime)
        {
            if (SkyModel != null)
            {
                //TODO: Allow Models to access camera so the view and projection matrices of the effect can be set up
                //locally.
                SkyModel.Effect.View = CameraNode.Camera.ViewMatrix;
                SkyModel.Effect.Projection = CameraNode.Camera.ProjectionMatrix;
                SkyModel.Draw(gameTime);
            }
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

        public void Emplace(SceneNode sourceNode, SpatialReference sourceNodeSpatialReference, SceneNode targetNode, SpatialReference targetSpatialReference)
        {
            //Transform sourceWorldBounds = sourceNode.LocalToWorld(sourceNode.Bounds);
            //Transform targetWorldBounds = targetNode.LocalToWorld(targetNode.Bounds);            
        }
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
