using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CipherPark.AngelJacket.Core.Effects;

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