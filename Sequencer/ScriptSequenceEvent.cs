using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.AngelJacket.Core.Sequencer
{
    public class ScriptSequenceEvent : SequenceEvent
    {
        public string Script { get; set; }

        public override void Execute(long gameTime, SequencerContext context)
        {
            
        }
    }
}
