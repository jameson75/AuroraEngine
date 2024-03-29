﻿using System;
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
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Animation
{
    public class FloatAnimation : KeyframeAnimation
    {
        public float GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            AnimationKeyFrame f1 = GetNextKeyFrame(f0);
            float frameVal0 = (f0.Value != null) ? Convert.ToSingle(f0.Value) : 0.0f;
            float frameVal1 = (f1 != null && f1.Value != null) ? Convert.ToSingle(f1.Value) : 0.0f;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.InterpolationCurve == null || f0.InterpolationCurve.Length == 0)
                    return (float)Lerp(frameVal0, frameVal1, pctT);
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}
