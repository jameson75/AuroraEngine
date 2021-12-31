using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.World.Collision;
using CipherPark.AngelJacket.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World
{
    public class WorldSimulator
    {
        private IGameApp _game = null;
        private List<IAnimationController> _animationControllers = new List<IAnimationController>();
        private CollisionDetector _collisionDetector = null;
        private LifetimeManager _lifetimeManager = null;

        public IGameApp Game { get { return _game; } }    
        public List<IAnimationController> AnimationControllers { get { return _animationControllers; } }
        public CollisionDetector CollisionDetector { get { return _collisionDetector; } } 
        public LifetimeManager LifetimeManager { get { return _lifetimeManager; } }

        public WorldSimulator(IGameApp game)
        {
            _game = game;
            _collisionDetector = new CollisionDetector(game);
            _lifetimeManager = new LifetimeManager();
        }

        public void Update(GameTime gameTime)
        {
            //I. Update animation controllers
            //-------------------------------
            //**********************************************************************************
            //NOTE: We use an auxilary controller collection to enumerate through, in 
            //the event that an updated controller alters this Simulator's Animation Controllers
            //collection.
            //**********************************************************************************
            List<IAnimationController> auxAnimationControllers = new List<IAnimationController>(_animationControllers);
            foreach (IAnimationController controller in auxAnimationControllers)
            {
                controller.UpdateAnimation(gameTime);
                if (controller.IsAnimationComplete)
                    _animationControllers.Remove(controller);
            }

            //II. Update collision detection
            //------------------------------
            CollisionDetector.Update(gameTime);

            //III. Remove entities (discarded by either animation controller or collision detector)
            //-------------------------------------------------------------------------------------
            LifetimeManager.Collect();
        }       
    }  
}
