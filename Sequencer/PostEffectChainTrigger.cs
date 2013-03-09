using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.World;
using CipherPark.AngelJacket.Core.World.Scene;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Sequencer
{
    public class PostEffectChainTrigger : Trigger
    {
        public PostEffectChainTrigger()
        { }

        public PostEffectChainTrigger(long time, PostEffectChain chain, PostEffect effect, PostEffectChainTriggerAction action = PostEffectChainTriggerAction.Add | PostEffectChainTriggerAction.Enable) : base(time)
        {
            Chain = chain;
            Effect = effect;
            Action = action;
        }

        public PostEffectChainTriggerAction Action { get; set; }

        public PostEffectChain Chain { get; set; }

        public PostEffect Effect { get; set; }

        public override void Fire(long gameTime, SequencerContext context)
        {
            if( (Action & PostEffectChainTriggerAction.Add) != 0 )
                Chain.Add(Effect);
            
            if ((Action & PostEffectChainTriggerAction.Enable) != 0)
                Effect.Enabled = true;
            
            if ((Action & PostEffectChainTriggerAction.Remove) != 0)
                Chain.Remove(Effect);
            
            if ((Action & PostEffectChainTriggerAction.Disable) != 0)
                Effect.Enabled = false;
        }
    }

    [Flags]
    public enum PostEffectChainTriggerAction
    {
        None =      0,
        Add =       0x01,
        Enable =    0x02,
        Remove =    0x04,
        Disable =   0x08
    }
}