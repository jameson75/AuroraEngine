using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Animation
{
    public class PointAnimation : KeyframeAnimation
    {
        public Vector2 GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            AnimationKeyFrame f1 = GetNextKeyFrame(f0);
            Vector2 frameVal0 = (f0.Value != null) ? (Vector2)f0.Value : Utils.Vector2Extension.Zero;
            Vector2 frameVal1 = (f1 != null && f1.Value != null) ? (Vector2)f1.Value : Utils.Vector2Extension.Zero;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.InterpolationCurve == null || f0.InterpolationCurve.Length == 0)
                {
                    float x = (float)Lerp(frameVal0.X, frameVal1.Y, pctT);
                    float y = (float)Lerp(frameVal0.Y, frameVal1.Y, pctT);
                    return new Vector2(x, y);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}
