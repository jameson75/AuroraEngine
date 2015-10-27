using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
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
    public interface IWeapon
    {
        void Initialize();
        void Fire();
        void Uninitialize();
    }

    public abstract class BasicWeapon : BasicWorldObject, IWeapon
    {
        public BasicWeapon(IGameApp game)
            : base(game)
        { }

        public abstract void Fire();
        public abstract void Initialize();
        public abstract void Uninitialize();
    }

    public class PlayerGun : BasicWeapon
    {
        private bool _isInitialized = false;
        private CompositeAnimationController _animationController = null;
        GameContext? _gameContext = null;
        public Model Projectile { get; set; }
        public ModelSceneNode ContainerNode { get; set; }
        public SceneNode ActionNode { get; set; }
        public Vector3 DischargeLocation { get; set; }
        public double DischargeLatency { get; set; }

        public PlayerGun(IGameApp game  )
            : base(game)
        { }
       

        public override void Fire()
        {
            if (!_isInitialized)
                throw new InvalidOperationException();

            BasicWorldObject projectileWO = new BasicWorldObject(this.Game)
            {
                Model = Projectile
            };
            WorldObjectSceneNode projectileNode = new WorldObjectSceneNode(projectileWO);
            
            //TODO: Find more elegant design than referencing the container scene node from inside this world object.                     
            //TODO: Use the following code to implemement ITransformable.LocalToLocal extension method.

            Vector3 actionFrameDischargeLocation = ActionNode.WorldToLocalCoordinate(ContainerNode.LocalToWorldCoordinate(DischargeLocation));
            ActionNode.Children.Add(projectileNode);
            projectileNode.Transform = new Transform(actionFrameDischargeLocation);
            RigidBodyAnimationController animationController = new RigidBodyAnimationController();
            animationController.Target = projectileWO;
            animationController.Motion = new Motion()
            {
                Direction = Vector3.UnitZ,
                LinearVelocity = 10
            };
            _gameContext.Value.Simulator.AnimationControllers.Add(animationController);           
        }

        public override void Initialize()
        {
            //Register our animation controller.
            //----------------------------------
            _animationController = new CompositeAnimationController();
            IGameContextService service = Game.Services.GetService<IGameContextService>();
            _gameContext = service.Context;
            _gameContext.Value.Simulator.AnimationControllers.Add(_animationController);
            _isInitialized = true;
        }

        public override void Uninitialize()
        {
            _animationController.SetComplete();
        }       
    }   
}
