using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;

namespace CipherPark.AngelJacket.Core.Animation
{
    public class StringAnimation : Animation<string>
    {
        public string GetValueAtT(ulong t)
        {
            AnimationKeyFrame<string> f0 = GetActiveKeyFrameAtT(t);
            return (f0.Value != null) ? Convert.ToString(f0.Value) : null;
        }
    }
}
