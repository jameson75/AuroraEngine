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
