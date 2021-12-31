using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Animation
{
    public class Color4Animation : KeyframeAnimation
    {
        public Color4 GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            AnimationKeyFrame f1 = GetNextKeyFrame(f0);
            Color4 frameVal0 = (f0.Value != null) ? (Color4)f0.Value : Color.Transparent;
            Color4 frameVal1 = (f1 != null && f1.Value != null) ? (Color4)f1.Value : Color.Transparent;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.InterpolationCurve == null || f0.InterpolationCurve.Length == 0)
                {
                    float a = (float)Lerp(frameVal0.Alpha, frameVal1.Alpha, pctT);
                    float r = (float)Lerp(frameVal0.Red, frameVal1.Red, pctT);
                    float g = (float)Lerp(frameVal0.Green, frameVal1.Green, pctT);
                    float b = (float)Lerp(frameVal0.Blue, frameVal1.Blue, pctT);
                    return new Color4(r, g, b, a);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}
