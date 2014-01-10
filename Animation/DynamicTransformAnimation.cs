﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class DynamicTransformableAnimation : KeyframeAnimation
    {
        public DynamicTransformableAnimation()
        { }

        public DynamicTransformableAnimation(ITransformable localConverter)
        {
            LocalConverter = localConverter;
        }

        public DynamicTransformableAnimation(ITransformable localConverter, IEnumerable<AnimationKeyFrame> keyFrames)
            : base(keyFrames)
        {
            LocalConverter = localConverter;
        }

        public bool SmoothingEnabled { get; set; }

        public ITransformable LocalConverter { get; set; }

        public Transform GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            AnimationKeyFrame f1 = GetNextKeyFrame(f0);
            Transform frameVal0 = (f0.Value != null) ? ((ITransformable)f0.Value).ParentToWorld(((ITransformable)f0.Value).Transform) : Transform.Identity;
            Transform frameVal1 = (f1 != null && f1.Value != null) ? ((ITransformable)f1.Value).ParentToWorld(((ITransformable)f1.Value).Transform) : Transform.Identity;        

            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                float pctStep = 0;
                if (f0.EaseOutTangent != null || f1.EaseInTangent != null)
                {
                    Vector2 eoTangent = (f0.EaseOutTangent != null) ? f0.EaseOutTangent.Value : new Vector2(1, 1);
                    Vector2 eiTangent = (f1.EaseInTangent != null) ? f1.EaseInTangent.Value : new Vector2(-1, -1);
                    Vector2 p1 = new Vector2(0, 0);
                    Vector2 p2 = new Vector2(1, 1);
                    Vector2 result = Vector2.Hermite(p1, eoTangent, p2, eiTangent, pctT);
                    pctStep = result.Y;
                }
                else
                    pctStep = pctT;

                Quaternion r = Quaternion.Lerp(frameVal0.Rotation, frameVal1.Rotation, pctStep);
                Vector3 x = Vector3.Zero;
                if (SmoothingEnabled)
                {
                    AnimationKeyFrame fa = GetPreviousKeyFrame(f0);
                    if (fa == null)
                        fa = f0;
                    AnimationKeyFrame fb = GetNextKeyFrame(f1);
                    if (fb == null)
                        fb = f1;
                    Transform frameVala = (fa.Value != null) ? (Transform)fa.Value : Transform.Identity;
                    Transform frameValb = (fb.Value != null) ? (Transform)fb.Value : Transform.Identity;
                    x = Vector3.CatmullRom(frameVala.Translation, frameVal0.Translation, frameVal1.Translation, frameValb.Translation, pctStep);
                }
                else
                    x = Vector3.Lerp(frameVal0.Translation, frameVal1.Translation, pctStep);

                Transform tx = new Transform { Rotation = r, Translation = x };
               
                //return (LocalConverter != null) ? LocalConverter.WorldToParent(tx) : tx;
                return (LocalConverter != null) ? LocalConverter.WorldToParent(tx) : ((ITransformable)f0.Value).WorldToParent(tx);
            }
        }
    }
}
