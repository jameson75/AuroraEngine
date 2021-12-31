using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.KillScript.Core.Module;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.Animation
{
    public class AnimationKeyFrame
    {
        private ulong _time = 0;
        public AnimationKeyFrame(ulong time)
        {
            _time = time;
        }
        public AnimationKeyFrame(ulong time, object value)
        {
            _time = time;
            Value = value;
        }
        public ulong Time { get { return _time; } }
        public object Value { get; set; }
        public AnimationInterpolationPoint[] InterpolationCurve { get; set; }
        public Vector2? EaseOutTangent { get; set; }
        public Vector2? EaseInTangent { get; set; }
    }

    public class AnimationKeyFrames : SortedList<ulong, AnimationKeyFrame>
    { }
}
