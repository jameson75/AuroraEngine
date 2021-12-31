using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.KillScript.Core;
using CipherPark.KillScript.Core.World.Scene;
using CipherPark.KillScript.Core.World.Collision;
using CipherPark.KillScript.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.World
{
    public class WorldSimulator
    {
        private IGameApp _game = null;
        private List<ISimulatorController> _animationControllers = new List<ISimulatorController>();
        private CollisionDetector _collisionDetector = null;       

        public IGameApp Game { get { return _game; } }    
        public List<ISimulatorController> Controllers { get { return _animationControllers; } }
        public CollisionDetector CollisionDetector { get { return _collisionDetector; } } 
       

        public WorldSimulator(IGameApp game)
        {
            _game = game;
            _collisionDetector = new CollisionDetector(game);           
        }

        public void Update(GameTime gameTime)
        {
            //I. Update animation controllers
            //-------------------------------           
            List<ISimulatorController> auxAnimationControllers = new List<ISimulatorController>(_animationControllers);
            foreach (ISimulatorController controller in auxAnimationControllers)
            {
                if (!controller.IsSimulationSuspended)
                {
                    controller.Update(gameTime);
                    if (controller.IsSimulationFinal)
                        _animationControllers.Remove(controller);
                }
            }

            //II. Update collision detection
            //------------------------------
            CollisionDetector.Update(gameTime);           
        }       
    }  
}
