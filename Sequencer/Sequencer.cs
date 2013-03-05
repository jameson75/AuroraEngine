using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.World;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.UI.Components;

namespace CipherPark.AngelJacket.Core.Sequencer
{
    public class Sequencer
    {
        private IGameApp _game = null;     

        public IGameApp Game { get { return _game; } }

        public Sequence Sequence { get; set; }
 
        public Sequencer(IGameApp game)
        {
            _game = game;
        }

        public void Update(long gameTime, SequencerContext context)
        {

        }
    }

    public class Sequence : List<Trigger>
    {

    }

    public abstract class Trigger
    {
        public long Time { get; set; }

        public Trigger() { }

        public Trigger(long time)
        {
            Time = time;
        }

        public abstract void Fire(long gameTime, SequencerContext context);
    }

    public struct SequencerContext
    {
        public SequencerContext(Scene scene, WorldSimulator simulator, UITree ui) : this()
        {            
            Scene = scene;
            Simulator = simulator;
            UI = ui;
        } 
        public Scene Scene { get; set; }
        public WorldSimulator Simulator { get; set; }
        public UITree UI { get; set; }
    }
}
