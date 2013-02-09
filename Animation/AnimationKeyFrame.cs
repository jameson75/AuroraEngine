﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;

namespace CipherPark.AngelJacket.Core.Animation
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
