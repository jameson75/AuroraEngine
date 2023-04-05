namespace CipherPark.Aurora.Core.Sequencer
{
    ///////////////////////////////////////////////////////////////////////////////
    // Developer: Eugene Adams  
    // Copyright © 2010-2013
    // Aurora Engine is licensed under 
    // MIT License.
    ///////////////////////////////////////////////////////////////////////////////

    public class CodeBlockTimedEvent : TimedEvent
    {
        public override void Execute(TimedEventStamp time)
        {
            if (CodeBlock != null)
                CodeBlock(time);
        }

        public ExecuteSequenceDelegate CodeBlock { get; set; }
    }

    public delegate void ExecuteSequenceDelegate(TimedEventStamp time);
}
