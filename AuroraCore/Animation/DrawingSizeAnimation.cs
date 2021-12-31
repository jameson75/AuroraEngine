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
    public class Size2Animation : KeyframeAnimation
    {
        public Size2F GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            AnimationKeyFrame f1 = GetNextKeyFrame(f0);
            Size2F frameVal0 = (f0.Value != null) ? (Size2F)f0.Value : Utils.Size2FExtension.Zero;
            Size2F frameVal1 = (f1 != null && f1.Value != null) ? (Size2F)f1.Value : Utils.Size2FExtension.Zero;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.InterpolationCurve == null || f0.InterpolationCurve.Length == 0)
                {
                    float w = (float)Lerp(frameVal0.Width, frameVal1.Height, pctT);
                    float h = (float)Lerp(frameVal0.Width, frameVal1.Height, pctT);
                    return new Size2F(w, h);
                }
                else
                {
                    //TODO: Implement either ease-in/ease-out with hermite control points.
                    //or implement Curve with catmull-rom.
                    //    for (int i = 0; i < f0.Curve.Length; i++)
                    //    {

                    //    }
                    return Utils.Size2FExtension.Zero;
                }
            }
        }
    }
}