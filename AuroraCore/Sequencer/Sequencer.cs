using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.Utils.Toolkit;
using System;
using System.Collections.Generic;

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

    public class Sequence : List<TimedEvent>
    { }

    public abstract class TimedEvent
    {
        public float Time { get; set; }
        public abstract void Execute(TimedEventStamp time);
    }

    public class TimedEventStamp
    {
        public TimedEventStamp(float eventTime, float clockTime)
        {
            EventTime = eventTime;
            ClockTime = clockTime;
        }
        public float EventTime { get; }
        public float ClockTime { get; }
    }

    public interface ISequencerClock
    {
        float Time { get; }
        void Update();
    }

    public class SequencerGameTimeClock : ISequencerClock
    {
        private const float SecondsInAMillisecond = 1000f;
        private bool _isStarted = false;
        private long _startTime = 0;
        private readonly IGameApp gameClockSource;

        public SequencerGameTimeClock(IGameApp clockSource)
        {
            gameClockSource = clockSource;
        }
        
        public float Time { get; private set; }

        public void Update()
        {
            var gameStateService = gameClockSource.Services.GetService<IGameStateService>();
            var gameTime = gameStateService.GameTime;
            
            if (!_isStarted)
                Start(gameTime);

            Time = CalculateElapsedGameTime(gameTime);
        }

        private void Start(GameTime gameTime)
        {
            _startTime = gameTime.GetTotalSimtime();
            _isStarted = true;
        }

        private float CalculateElapsedGameTime(GameTime gameTime)
        { 
            return ToFractionalSeconds(gameTime.GetTotalSimtime() - _startTime); 
        }

        private static float ToFractionalSeconds(long time)
            => time / SecondsInAMillisecond;
    }

    public class SequencerAudioStreamClock : ISequencerClock
    {
        private const float SecondsIn100NanoSecond = 10000000f;
        private readonly XAudio2StreamingManager audioClockSource;

        public SequencerAudioStreamClock(XAudio2StreamingManager clockSource)
        {
            audioClockSource = clockSource;
        }

        public float Time { get; private set; }

        public void Update()
        {
            Time = ToFractionalSeconds(audioClockSource.CurrentSampleTime);
        }

        private static float ToFractionalSeconds(long time)
            => time / SecondsIn100NanoSecond;
    }
}