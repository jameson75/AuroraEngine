using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.AngelJacket.Core.Animation
{
    public class BoneAnimation : KeyframeAnimation
    {
        public BoneAnimation()
        { }

        public BoneAnimation(IEnumerable<AnimationKeyFrame> keyFrames) : base(keyFrames)
        {
           
        }
    }    
}
