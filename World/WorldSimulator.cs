﻿using System;
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
        IGameApp _game = null;
        private List<IAnimationController> _animationControllers = new List<IAnimationController>();

        public IGameApp Game { get { return _game; } }    

        public WorldSimulator(IGameApp game)
        {
            _game = game;
        }

        public WorldSimulatorSettings Settings { get; set; }

        public void Update(long gameTime, SimulationContext context)
        {

        }

        public List<IAnimationController> AnimationControllers { get { return _animationControllers; } }
    }

    public class WorldSimulatorSettings
    {
        
    }

    public struct SimulationContext
    {
        public Scene.Scene Scene { get; set; }
        public UI.Components.UITree UI { get; set; }

        public SimulationContext(Scene.Scene scene, UI.Components.UITree ui) : this()
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
