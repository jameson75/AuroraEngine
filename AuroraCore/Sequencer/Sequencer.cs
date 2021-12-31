using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using CipherPark.KillScript.Core.Module;
using CipherPark.KillScript.Core.World;
using CipherPark.KillScript.Core.World.Scene;
using CipherPark.KillScript.Core.Utils;
using CipherPark.KillScript.Core.UI.Components;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.Sequencer
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