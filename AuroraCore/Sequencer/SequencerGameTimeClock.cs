using CipherPark.Aurora.Core.Services;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Sequencer
{
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
}