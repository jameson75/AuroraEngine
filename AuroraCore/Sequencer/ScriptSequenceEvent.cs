using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.KillScript.Core.Sequencer
{
    public class ScriptSequenceEvent : SequenceEvent
    {
        public string Script { get; set; }

        public override void Execute(GameTime gameTime, ModuleContext context)
        {
            
        }
    }
}
