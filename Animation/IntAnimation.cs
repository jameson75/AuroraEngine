using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class IntAnimation : KeyframeAnimation
    {
        public int GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            AnimationKeyFrame f1 = GetNextKeyFrame(f0);
            int frameVal0 = (f0.Value != null) ? Convert.ToInt32(f0.Value) : 0;
            int frameVal1 = (f1 != null && f1.Value != null) ? Convert.ToInt32(f1.Value) : 0;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.InterpolationCurve == null || f0.InterpolationCurve.Length == 0)
                    return (int)Lerp(frameVal0, frameVal1, pctT);
                else
                {
                    //TODO: Implement either ease-in/ease-out with hermite control points.
                    //or implement Curve with catmull-rom.
                    //    for (int i = 0; i < f0.Curve.Length; i++)
                    //    {

                    //    }
                    return 0;
                }
            }
        }
    }
}
