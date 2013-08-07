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
    public class DrawingPointAnimation : KeyframeAnimation
    {
        public DrawingPointF GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            AnimationKeyFrame f1 = GetNextKeyFrame(f0);
            DrawingPointF frameVal0 = (f0.Value != null) ? (DrawingPointF)f0.Value : Utils.DrawingPointFExtension.Zero;
            DrawingPointF frameVal1 = (f1 != null && f1.Value != null) ? (DrawingPointF)f1.Value : Utils.DrawingPointFExtension.Zero;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.InterpolationCurve == null || f0.InterpolationCurve.Length == 0)
                {
                    float x = (float)Lerp(frameVal0.X, frameVal1.Y, pctT);
                    float y = (float)Lerp(frameVal0.Y, frameVal1.Y, pctT);
                    return new DrawingPointF(x, y);
                }
                else
                {
                    //TODO: Implement either ease-in/ease-out with hermite control points.
                    //or implement Curve with catmull-rom.
                    //    for (int i = 0; i < f0.Curve.Length; i++)
                    //    {

                    //    }
                    return Utils.DrawingPointFExtension.Zero;
                }
            }
        }
    }
}
