using System;

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
        private Sequence _sequence = new Sequence();

        public IGameApp Game { get { return _game; } }

        public Sequence Sequence { get { return _sequence; } }

        public Sequencer(IGameApp game)
        {
            _game = game;
        }

        public ISequencerClock Clock { get; set; }

        public void Update(GameTime gameTime)
        {
            if (Clock == null)
            {
                throw new InvalidOperationException("No clock specified");
            }

            Clock.Update();

            float elapsedSequencerTime = Clock.Time;
            //**************************************************************************
            //NOTE: We use an auxilary sequence to enumerate through, in the event that
            //an executed sequence alters this sequencer's Sequence collection.
            //**************************************************************************
            Sequence auxSequence = new Sequence();
            auxSequence.AddRange(this.Sequence);
            foreach (TimedEvent timedEvent in auxSequence)
            {
                if (timedEvent.Time <= elapsedSequencerTime)
                {
                    timedEvent.Execute(new TimedEventStamp(timedEvent.Time, elapsedSequencerTime));
                    Sequence.Remove(timedEvent);
                }
            }
        } 
    }
}