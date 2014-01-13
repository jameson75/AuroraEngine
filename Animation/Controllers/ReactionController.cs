using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.AngelJacket.Core.Animation.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IReaction
    {
        void React();
        bool IsComplete { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ReactionController : AnimationController
    {
        public IReaction Reaction { get; set; }

        public override void Start()
        {  }

        public override void UpdateAnimation(long gameTime)
        {
            if (Reaction != null)
            {
                Reaction.React();

                if (Reaction.IsComplete || !IsAnimationComplete)
                    OnAnimationComplete();
            }
        }
    }
}
