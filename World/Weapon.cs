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
        private GameContext? _gameContext = null;
        private long? _lastFireTime = null;
        
        public Model Projectile { get; set; }        
        public SceneNode ProjectileRootNode { get; set; }
        public Vector3 DischargeLocation { get; set; }
        public double DischargeLatency { get; set; }
        public long EmissionDelay { get; set; }
        public float ProjectileVelocity { get; set; }

        public PlayerGun(IGameApp game  )
            : base(game)
        { }       

        public override void Fire()
        {
            if (!_isInitialized)
                throw new InvalidOperationException();

            IGameStateService gameStateService =_gameContext.Value.Game.Services.GetService<IGameStateService>();            
            long gameSimTime = gameStateService.GameTime.GetTotalSimtime();
            if (EmissionDelay == 0 ||
                _lastFireTime == null ||
                gameSimTime - _lastFireTime.Value >= EmissionDelay)
            {                
                BasicWorldObject projectileWO = new BasicWorldObject(this.Game)
                {
                    Model = Projectile
                };
                WorldObjectSceneNode projectileNode = new WorldObjectSceneNode(projectileWO);
                projectileNode.Transform = new Transform(Matrix.RotationX(MathUtil.DegreesToRadians(90f)));

                //TODO: Use the following code to implemement ITransformable.LocalToLocal extension method.              
               
                Vector3 actionFrameDischargeLocation = ProjectileRootNode.WorldToLocalCoordinate(ContainerNode.LocalToWorldCoordinate(DischargeLocation));
                ProjectileRootNode.Children.Add(projectileNode);
                projectileNode.Transform = new Transform(projectileNode.Transform.ToMatrix() * Matrix.Translation(actionFrameDischargeLocation));
                RigidBodyAnimationController animationController = new RigidBodyAnimationController();
                animationController.Target = projectileWO;
                animationController.Motion = new Motion()
                {
                    Direction = Vector3.UnitZ,
                    LinearVelocity = ProjectileVelocity,
                    TimeDomainUnits = TimeDomainUnits.Seconds
                };
                _gameContext.Value.Simulator.AnimationControllers.Add(animationController);
                _lastFireTime = gameSimTime;
            }
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
