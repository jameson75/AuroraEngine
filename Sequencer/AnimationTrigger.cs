using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CipherPark.AngelJacket.Core.Animation;
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
    public class TransformAnimationTrigger : Trigger
    {
        public TransformAnimationTrigger() { }

        public TransformAnimationTrigger(long time, TransformAnimation animation, ITransformable target, AnimationTriggerAction action = AnimationTriggerAction.Start) : base(time)
        {
            Animation = animation;
            Target = target;
            Action = action;
        }

        public TransformAnimation Animation { get; set; }

        public AnimationTriggerAction Action { get; set; }

        public ITransformable Target { get; set; }

        public override void Fire(long gameTime, SequencerContext context)
        {
            if (Action == AnimationTriggerAction.Start)
            {
                TransformAnimationController controller = new TransformAnimationController(context.Game) { Animation = Animation, Target = Target };
                context.Simulator.AnimationControllers.Add(controller);
            }
        }
    }

    public enum AnimationTriggerAction
    {
        None = 0,
        Start,
        Stop
    }
}
