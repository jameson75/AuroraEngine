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
    // a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
    ///////////////////////////////////////////////////////////////////////////////

    public class CodeBlockSequenceEvent : SequenceEvent
    {
        public override void Execute(GameTime gameTime, ModuleContext context)
        {
            if (CodeBlock != null)
                CodeBlock(gameTime, context);
        }

        public ExecuteSequenceDelegate CodeBlock { get; set; }
    }

    public delegate void ExecuteSequenceDelegate(GameTime gameTime, ModuleContext context);
}
