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

namespace CipherPark.AngelJacket.Core.Simulation
{
    public class Sequencer
    {
        private IGameApp _game = null;
        private bool _isStarted = false;
        private long _startTime = 0;
        private Sequence _sequence = new Sequence();

        public IGameApp Game { get { return _game; } }

        public Sequence Sequence { get { return _sequence; } }
 
        public Sequencer(IGameApp game)
        {
            _game = game;
        }

        public void Update(long gameTime, SequencerContext context)
        {
            if (!_isStarted)
                Start();

            List<SequenceEvent> executedSequenceEvents = new List<SequenceEvent>();

            long elapsedSequencerTime = CalculateElapsedSequencerTime();

            foreach (SequenceEvent sequenceEvent in Sequence)
            {
                if (sequenceEvent.Time <= elapsedSequencerTime)
                {
                    sequenceEvent.Execute(gameTime, context);
                    executedSequenceEvents.Add(sequenceEvent);
                }
            }

            foreach (SequenceEvent executedSequenceEvent in executedSequenceEvents)
                Sequence.Remove(executedSequenceEvent);
        }

        private void Start()
        {
            _startTime = Environment.TickCount;
            _isStarted = true;
        }

        private long CalculateElapsedSequencerTime()
        { return Environment.TickCount - _startTime; }
    }

    public class Sequence : List<SequenceEvent>
    { }

    public abstract class SequenceEvent 
    { 
        public long Time { get; set; }
        public abstract void Execute(long gameTime, SequencerContext context);
    }     

    public struct SequencerContext
    {
        public SequencerContext(GameAssets assets, Scene scene, WorldSimulator simulator, UITree ui) : this()
        {            
            Assets = assets;
            Scene = scene;
            Simulator = simulator;
            UI = ui;
            Game = scene.Game;
        } 
        public GameAssets Assets { get; private set; }
        public Scene Scene { get; private set; }
        public WorldSimulator Simulator { get; private set; }
        public UITree UI { get; private set; }
        public IGameApp Game { get; private set; }
    }
}
