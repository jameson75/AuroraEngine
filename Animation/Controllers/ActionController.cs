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
    public class ActionController : AnimationController
    {
        public IReaction Action { get; set; }

        public override void Reset()
        {  }

        public override void UpdateAnimation(GameTime gameTime)
        {
            if (Action != null)
            {
                Action.React();

                if (Action.IsComplete || !IsAnimationComplete)
                    OnAnimationComplete();
            }
        }
    }
}
