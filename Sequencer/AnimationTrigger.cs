using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CipherPark.AngelJacket.Core.Animation;

namespace CipherPark.AngelJacket.Core.Sequencer
{
    public class AnimationTrigger : Trigger
    {
        private List<Animation.Animation> _animations = new List<Animation.Animation>();       

        public AnimationTrigger() { }

        public AnimationTrigger(long time, IEnumerable<Animation.Animation> animations, AnimationTriggerAction action = AnimationTriggerAction.Start) : base(time)
        {
            _animations.AddRange(animations);
            Action = action;
        }
        
        public AnimationTriggerAction Action { get; set; }
    }

    public enum AnimationTriggerAction
    {
        None = 0,
        Start,
        Stop
    }
}
