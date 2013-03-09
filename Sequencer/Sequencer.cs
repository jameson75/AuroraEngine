using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.World;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.UI.Components;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Sequencer
{
    public class Sequencer
    {
        private IGameApp _game = null;
        private bool _isStarted = false;
        private long _startTime = 0;

        public IGameApp Game { get { return _game; } }

        public Sequence Sequence { get; set; }

        public GameAssets Assets { get; set; }

        public Sequencer(IGameApp game)
        {
            _game = game;
        }

        public void Update(long gameTime, SequencerContext context)
        {
            if (!_isStarted)
                Start();

            List<Trigger> firedTriggers = new List<Trigger>();

            long elapsedSequencerTime = CalculateElapsedSequencerTime();

            foreach (Trigger trigger in Sequence)
            {
                if (trigger.Time <= elapsedSequencerTime)
                {
                    trigger.Fire(gameTime, context);
                    firedTriggers.Add(trigger);
                }
            }

            foreach (Trigger firedTrigger in firedTriggers)
                Sequence.Remove(firedTrigger);
        }

        private void Start()
        {
            _startTime = Environment.TickCount;
            _isStarted = true;
        }

        private long CalculateElapsedSequencerTime()
        { return Environment.TickCount - _startTime; }
    }

    public class Sequence : List<Trigger>
    { }

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
            Game = scene.Game;
        } 
        public Scene Scene { get; set; }
        public WorldSimulator Simulator { get; set; }
        public UITree UI { get; set; }
        public IGameApp Game { get; set; ]
    }
}
