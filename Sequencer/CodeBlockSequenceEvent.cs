using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.AngelJacket.Core.Sequencer
{
    ///////////////////////////////////////////////////////////////////////////////
    // Developer: Eugene Adams
    // Company: Cipher Park LLC
    // Copyright © 2010-2013
    // Angel Jacket by Cipher Park is licensed under 
    // a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
    ///////////////////////////////////////////////////////////////////////////////

    public class CodeBlockSequenceEvent : SequenceEvent
    {
        public override void Execute(long gameTime, SequencerContext context)
        {
            if (CodeBlock != null)
                CodeBlock(gameTime, context);
        }

        public ExecuteSequenceDelegate CodeBlock { get; set; }
    }

    public delegate void ExecuteSequenceDelegate(long gameTime, SequencerContext context);
}
