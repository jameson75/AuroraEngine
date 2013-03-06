using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.Animation;

namespace CipherPark.AngelJacket.Core.World
{
    public class WorldSimulator
    {
        IGameApp _game = null;       
        private 
        public IGameApp Game { get { return _game; } }    

        public WorldSimulator(IGameApp game)
        {
            _game = game;
        }

        public WorldSimulatorSettings Settings { get; set; }

        public void Update(long gameTime, SimulationContext context)
        {

        }
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

    public class TransformAnimationTask : SimulatorTask
    {
        public ITransformable Target { get; set; }

        public TransformAnimation Animation { get; set; }

        public TransformAnimationTask(TransformAnimation animation, ITransformable target)
        {
            Animation = animation;
            Target = target;
        }

        public override void Step(long gameTime, SimulationContext context)
        {
            Animation.UpdateAnimation(gameTime, Target);
        }

        public override bool IsComplete
        {
            get { return false; }
        }
    }
}
