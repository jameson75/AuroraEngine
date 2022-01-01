using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using CipherPark.Aurora.Core.Module;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.UI.Components;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Sequencer
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

        public void Update(GameTime gameTime, ModuleContext context)
        {
            if (!_isStarted)
                Start(gameTime);

            long elapsedSequencerTime = CalculateElapsedSequencerTime(gameTime);
            //**************************************************************************
            //NOTE: We use an auxilary sequence to enumerate through, in the event that
            //an executed sequence alters this sequencer's Sequence collection.
            //**************************************************************************
            Sequence auxSequence = new Sequence();
            auxSequence.AddRange(this.Sequence);
            foreach (SequenceEvent sequenceEvent in auxSequence)
            {
                if (sequenceEvent.Time <= elapsedSequencerTime)
                {
                    sequenceEvent.Execute(gameTime, context);
                    Sequence.Remove(sequenceEvent);
                }
            }
        }

        private void Start(GameTime gameTime)
        {
            _startTime = gameTime.GetTotalSimtime();
            _isStarted = true;
        }

        private long CalculateElapsedSequencerTime(GameTime gameTime)
        { return gameTime.GetTotalSimtime() - _startTime; }
    }

    public class Sequence : List<SequenceEvent>
    { }

    public abstract class SequenceEvent
    {
        public long Time { get; set; }
        public abstract void Execute(GameTime gameTime, ModuleContext context);
    }
}