using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.World.Scene;
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

        public IGameApp Game { get { return _game; } }    
        public List<IAnimationController> AnimationControllers { get { return _animationControllers; } }
        
        public WorldSimulator(IGameApp game)
        {
            _game = game;
        }

        public void Update(GameTime gameTime, SimulationContext context)
        {
            //Update animation controllers.
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

            //Update scene based on the physical state of the scene objects.
            foreach (SceneNode node in context.Scene.Nodes)
                SimulatePhysics(node);
        }

        private void SimulatePhysics(SceneNode node)
        {
            foreach (SceneNode childNode in node.Children)
                SimulatePhysics(childNode);
        }
    }  

    public struct SimulationContext
    {
        public Scene.Scene Scene { get; set; }
        public UI.Components.IUIRoot UI { get; set; }

        public SimulationContext(Scene.Scene scene, UI.Components.IUIRoot ui) : this()
        {
            Scene = scene;
            UI = ui;
        }
    }

    public abstract class SimulatorTask
    {
        public abstract void Step(long GameTime, SimulationContext context);
        public abstract bool IsComplete { get; }
    }
}
