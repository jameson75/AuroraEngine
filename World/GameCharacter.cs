using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using SharpDX.XInput;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.Services;
using CipherPark.AngelJacket.Core.Content;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Sequencer;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.Kinetics;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class GameCharacter : IRigidBody
    {
        private IGameApp _game = null;

        public Transform Transform { get; set; }

        public ITransformable TransformableParent { get; set; }       

        public GameCharacter(IGameApp game)
        {
            _game = game;
        }

        public IGameApp Game { get { return _game; } }

        public abstract void Draw(GameTime gameTime);

        public Vector3 CenterOfMass { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BasicCharacter : GameCharacter
    {
        private Model _model = null;

        public BasicCharacter(Model model) : base(model.Game)
        {
            _model = model;
            //_model.TransformableParent = this;
        }

        public Model Model { get { return _model; } }

        public override void Draw(GameTime gameTime)
        {
            IGameContextService contextService = (IGameContextService)Game.Services.GetService(typeof(IGameContextService));

            if (contextService == null)
                throw new InvalidOperationException("Context service not registered.");

            if (contextService.Context == null)
                throw new InvalidOperationException("No game context is associated with active game module");            
           
            Scene.Scene scene = contextService.Context.Value.Scene;
            Model.Effect.World = this.WorldTransform().ToMatrix();
            Model.Effect.View = Camera.TransformToViewMatrix(scene.CameraNode.WorldTransform()); 
            Model.Effect.Projection = scene.CameraNode.Camera.ProjectionMatrix;
            Model.Draw(gameTime);
        }
    }      
}
