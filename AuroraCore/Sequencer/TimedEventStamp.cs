///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Sequencer
{
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
}