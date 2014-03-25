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
        private Sequence _sequence = new Sequence();

        public IGameApp Game { get { return _game; } }

        public Sequence Sequence { get { return _sequence; } }
 
        public Sequencer(IGameApp game)
        {
            _game = game;
        }

        public void Update(GameTime gameTime, SequencerContext context)
        {
            if (!_isStarted)
                Start();
            
            long elapsedSequencerTime = CalculateElapsedSequencerTime();
            //**************************************************************************
            //NOTE: We use an auxilary sequence to enumerate through, in the event that
            //an executed sequence alters this sequencer's Sequence collection.
            //**************************************************************************
            Sequence auxSequence = new Sequence();
            auxSequence.AddRange(this.Sequence);
            foreach ( SequenceEvent sequenceEvent in auxSequence )
            {
                if (sequenceEvent.Time <= elapsedSequencerTime)
                {
                    sequenceEvent.Execute(gameTime, context);
                    Sequence.Remove(sequenceEvent);                   
                }
            }          
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
        public abstract void Execute(GameTime gameTime, SequencerContext context);
    }     

    public struct SequencerContext
    {
        public SequencerContext(GameAssets assets, Scene scene, WorldSimulator simulator, IUIRoot ui) : this()
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
        public IUIRoot UI { get; private set; }
        public IGameApp Game { get; private set; }
    }
}
