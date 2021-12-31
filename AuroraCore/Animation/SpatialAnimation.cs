using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.KillScript.Core.Module;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.Animation
{
    public class SpatialAnimation : TransformAnimation
    {
        public SpatialAnimation()
        { }
       

        public SpatialAnimation(IEnumerable<AnimationKeyFrame> keyFrames, ITransformable localChild = null)
            : base(keyFrames)
        {
            LocalChild = localChild;
        }      

        public ITransformable LocalChild { get; set; }

        public override Transform GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            AnimationKeyFrame f1 = GetNextKeyFrame(f0);
            Transform frameValTx0 = (f0.Value != null) ? ((SpatialNode)f0.Value).ParentToWorld(((SpatialNode)f0.Value).Transform) : Transform.Identity;
            Transform frameValTx1 = (f1 != null && f1.Value != null) ? ((SpatialNode)f1.Value).ParentToWorld(((SpatialNode)f1.Value).Transform) : Transform.Identity;        

            if (f1 == null)
                return (LocalChild != null) ? LocalChild.WorldToParent(frameValTx0) : frameValTx0;
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

                Quaternion r = Quaternion.Lerp(frameValTx0.Rotation, frameValTx1.Rotation, pctStep);
                Vector3 x = Vector3.Zero;
                if (SmoothingEnabled)
                {
                    AnimationKeyFrame fa = GetPreviousKeyFrame(f0);
                    if (fa == null)
                        fa = f0;
                    AnimationKeyFrame fb = GetNextKeyFrame(f1);
                    if (fb == null)
                        fb = f1;
                    Transform frameValTxa = (fa.Value != null) ? ((SpatialNode)fa.Value).ParentToWorld(((SpatialNode)fa.Value).Transform) : Transform.Identity;
                    Transform frameValTxb = (fb.Value != null) ? ((SpatialNode)fb.Value).ParentToWorld(((SpatialNode)fb.Value).Transform) : Transform.Identity;
                    x = Vector3.CatmullRom(frameValTxa.Translation, frameValTx0.Translation, frameValTx1.Translation, frameValTxb.Translation, pctStep);
                }
                else
                    x = Vector3.Lerp(frameValTx0.Translation, frameValTx1.Translation, pctStep);

                Transform tx = new Transform { Rotation = r, Translation = x };  
            
                
                
                return (LocalChild != null) ? LocalChild.WorldToParent(tx) : tx;
            }
        }
    }

    public class SpatialNode : ITransformable
    {
        public Transform Transform { get; set; }

        public ITransformable TransformableParent { get; set; }
    }
}
