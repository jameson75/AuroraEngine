using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;

using CipherPark.AngelJacket.Core.Module;

namespace CipherPark.AngelJacket.Core.Animation
{
    public class DrawingSizeAnimation : Animation
    {
        public DrawingSizeF GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            AnimationKeyFrame f1 = GetNextKeyFrame(f0);
            DrawingSizeF frameVal0 = (f0.Value != null) ? (DrawingSizeF)f0.Value : Utils.DrawingSizeFExtension.Zero;
            DrawingSizeF frameVal1 = (f1 != null && f1.Value != null) ? (DrawingSizeF)f1.Value : Utils.DrawingSizeFExtension.Zero;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.InterpolationCurve == null || f0.InterpolationCurve.Length == 0)
                {
                    float w = (float)Lerp(frameVal0.Width, frameVal1.Height, pctT);
                    float h = (float)Lerp(frameVal0.Width, frameVal1.Height, pctT);
                    return new DrawingSizeF(w, h);
                }
                else
                {
                    //TODO: Implement either ease-in/ease-out with hermite control points.
                    //or implement Curve with catmull-rom.
                    //    for (int i = 0; i < f0.Curve.Length; i++)
                    //    {

                    //    }
                    return Utils.DrawingSizeFExtension.Zero;
                }
            }
        }
    }
}