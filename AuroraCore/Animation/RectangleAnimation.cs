using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class RectangleAnimation : KeyframeAnimation
    {
        public RectangleF GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            AnimationKeyFrame f1 = GetNextKeyFrame(f0);
            RectangleF frameVal0 = (f0.Value != null) ? (RectangleF)f0.Value : Utils.RectangleFExtension.Empty;
            RectangleF frameVal1 = (f1 != null && f1.Value != null) ? (RectangleF)f1.Value : Utils.RectangleFExtension.Empty;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.InterpolationCurve == null || f0.InterpolationCurve.Length == 0)
                {
                    float left = (float)Lerp(frameVal0.Left, frameVal1.Left, pctT);
                    float top = (float)Lerp(frameVal0.Top, frameVal1.Top, pctT);
                    float width = (float)Lerp(frameVal0.Right, frameVal1.Width, pctT);
                    float height = (float)Lerp(frameVal0.Bottom, frameVal1.Height, pctT);
                    return new RectangleF(left, top, width, height);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}
