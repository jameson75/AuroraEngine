///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Sequencer
{
    public abstract class TimedEvent
    {
        public float Time { get; set; }
        public abstract void Execute(TimedEventStamp time);
    }
}