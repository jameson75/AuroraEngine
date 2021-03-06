using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.Aurora.Core.Sequencer
{
    ///////////////////////////////////////////////////////////////////////////////
    // Developer: Eugene Adams
    //  LLC
    // Copyright © 2010-2013
    // Aurora Engine is licensed under 
    // MIT License.
    ///////////////////////////////////////////////////////////////////////////////

    public class CodeBlockSequenceEvent : SequenceEvent
    {
        public override void Execute(GameTime gameTime)
        {
            if (CodeBlock != null)
                CodeBlock(gameTime);
        }

        public ExecuteSequenceDelegate CodeBlock { get; set; }
    }

    public delegate void ExecuteSequenceDelegate(GameTime gameTime);
}
