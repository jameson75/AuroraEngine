using CipherPark.Aurora.Core.Toolkit;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Sequencer
{
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