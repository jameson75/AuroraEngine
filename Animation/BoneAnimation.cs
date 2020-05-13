using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

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
