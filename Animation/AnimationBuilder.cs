using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;

namespace CipherPark.AngelJacket.Core.Animation
{
    public static class AnimationBuilder
    {
        public static AnimationKeyFrame[] Move(Vector3 direction, float distance, Vector3 rotationVector, float rotationAngle, int keyFrameCount, float totalAnimationTime )
        {
            List<AnimationKeyFrame> keyFrames = new List<AnimationKeyFrame>();
            for (int i = 0; i < keyFrameCount; i++)
            {
                if (i != (keyFrameCount - 1))
                {

                }
                //this is the last key frame.
                else
                {

                }
            }
            return keyFrames.ToArray();
        }

        public static AnimationKeyFrame[] ToPose(string name, RiggedModel model, int keyFrameCount, float totalAnimationTime)
        {
            List<AnimationKeyFrame> keyFrames = new List<AnimationKeyFrame>();


            return keyFrames.ToArray();
        }
    }
}
