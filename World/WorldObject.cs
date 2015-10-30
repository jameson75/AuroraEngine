using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.World.Collision;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Animation.Controllers;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.Kinetics;
using CipherPark.AngelJacket.Core.Services;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World
{
    public abstract class WorldObject : ITransformable
    {
        private Collider _collider = null;
        private IGameApp _game = null;        

        protected WorldObject(IGameApp game)
        {
            _game = game;
        }

        public IGameApp Game { get { return _game; } }

        public abstract void Draw(GameTime gameTime);

        public abstract void Update(GameTime gameTime);

        public Transform Transform { get; set; }

        public ITransformable TransformableParent { get; set; }    

        public abstract BoundingBox BoundingBox { get; }

        public SceneNode ContainerNode
        {
            get { return this.TransformableParent as SceneNode; }
        }    

        public Collider Collider
        {
            get { return _collider; }
            set
            {
                if (_collider != null)
                    _collider.TransformableParent = null;
                if (value != null)
                    value.TransformableParent = this;
                _collider = value;
            }
        }
    }

    public class BasicWorldObject : WorldObject, IRigidBody
    {
        public BasicWorldObject(IGameApp game) : base(game)
        { }      

        public Model Model { get; set; }

        public Vector3 CenterOfMass { get; set; }

        public float Velocity { get; set; }
        
        public override BoundingBox BoundingBox
        {
            get
            {
                if (Model != null)
                    return Model.BoundingBox;
                else
                    return BoundingBoxExtension.Empty;
            }
        }     

        public override void Update(GameTime gameTime)
        { }

        public override void Draw(GameTime gameTime)
        {
            IGameContextService contextService = (IGameContextService)Game.Services.GetService(typeof(IGameContextService));

            if (contextService == null)
                throw new InvalidOperationException("Context service not registered.");

            if (contextService.Context == null)
                throw new InvalidOperationException("No game context is associated with active game module");

            if (Model != null)
            {
                Scene.Scene scene = contextService.Context.Value.Scene;
                Model.Effect.World = this.WorldTransform().ToMatrix();
                Model.Effect.View = Camera.TransformToViewMatrix(scene.CameraNode.WorldTransform());
                Model.Effect.Projection = scene.CameraNode.Camera.ProjectionMatrix;
                Model.Draw(gameTime);
            }
        }         
    }
}
