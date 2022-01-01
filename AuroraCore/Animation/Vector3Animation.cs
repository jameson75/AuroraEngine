using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Animation
{
    public class Vector3Animation : KeyframeAnimation
    {
        public Vector3 GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            AnimationKeyFrame f1 = GetNextKeyFrame(f0);
            Vector3 frameVal0 = (f0.Value != null) ? (Vector3)f0.Value : Vector3.Zero;
            Vector3 frameVal1 = (f1 != null && f1.Value != null) ? (Vector3)f1.Value : Vector3.Zero;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.InterpolationCurve == null || f0.InterpolationCurve.Length == 0)
                {
                    return Vector3.Lerp(frameVal0, frameVal1, pctT);
                }
                else
                {                   
                    throw new NotSupportedException();
                }
            }
        }
    }
}
