using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.Module;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Animation
{
    public class AnimationInterpolationPoint
    {
        private float _timeOffset = 0;
        public AnimationInterpolationPoint(float timeOffset)
        {
            _timeOffset = timeOffset;
        }
        public AnimationInterpolationPoint(float timeOffset, object value)
        {
            _timeOffset = timeOffset;
            Value = value;
        }
        public float TimeOffset { get { return _timeOffset; } }
        public object Value { get; set; }
    }
}
