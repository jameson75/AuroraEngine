using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;


namespace CipherPark.AngelJacket.Core.Animation
{
    public abstract class PropertyGroupAnimation
    {
        private PropertyAnimations _animations = new PropertyAnimations();
        private PropertyAnimations Animations { get { return _animations; } }
        private const ulong DefaultKeyFrameTime = 0;

        /// <summary>
        /// Gets the nearest key frame whose time is less than or equal to timeT.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="timeT"></param>
        /// <returns>The nearest key frame whose time is less than or equal to timeT.</returns>
        /// <remarks>Note, each target property always has a default key frame at t=0</remarks>
        /// <example>If there are keyframes, at t=0, t=2 and t=4, respectively, then the keyframe
        /// at t=2 would be returned for a timeT of 3.9.</example>
        private AnimationKeyFrame GetActiveKeyFrameAtT(string propertyName, ulong timeT)
        {
            if (!TargetPropertyExists(propertyName))
                throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");
            return Animations[propertyName].GetActiveKeyFrameAtT(timeT);
        }

        private AnimationKeyFrame GetNextKeyFrame(string propertyName, AnimationKeyFrame keyFrame)
        {
            if (!TargetPropertyExists(propertyName))
                throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");
            return Animations[propertyName].GetNextKeyFrame(keyFrame);
        }

        private AnimationKeyFrame GetPreviousKeyFrame(string propertyName, AnimationKeyFrame keyFrame)
        {
            if (!TargetPropertyExists(propertyName))
                throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");
            return Animations[propertyName].GetPreviousKeyFrame(keyFrame);
        }

        protected void AddPropertyAnimation(string propertyName, Animation animation)
        {
            if (TargetPropertyExists(propertyName))
                throw new InvalidOperationException("The target property already exists for this effect");
            Animations.Add(propertyName, animation);
        }

        protected void SetPropertyKeyFrame(string propertyName, AnimationKeyFrame keyFrame)
        {
            if (!TargetPropertyExists(propertyName))
                throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");
            Animations[propertyName].SetKeyFrame(keyFrame);
        }

        protected void RemovePropertyKeyFrame(string propertyName, ulong tF, bool removeEmptyAnimation = false)
        {
            if (!TargetPropertyExists(propertyName))
                throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");

            Animations[propertyName].RemoveKeyFrame(tF);

            if (removeEmptyAnimation && Animations[propertyName].FrameCount == 0)
                Animations.Remove(propertyName);
        }

        protected void RemoveProperty(string propertyName)
        {
            if (!TargetPropertyExists(propertyName))
                throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");

            Animations.Remove(propertyName);
        }

        protected float GetPropertyFloatValueAtT(string propertyName, ulong t)
        {
            return ((FloatAnimation)Animations[propertyName]).GetValueAtT(t);
        }

        protected bool GetPropertyBooleanValueAtT(string propertyName, ulong t)
        {
            return ((BooleanAnimation)Animations[propertyName]).GetValueAtT(t);
        }

        protected DrawingPointF GetPropertyDrawingPointValueAtT(string propertyName, ulong t)
        {
            return ((DrawingPointAnimation)Animations[propertyName]).GetValueAtT(t);
        }

        protected DrawingSizeF GetPropertyDrawingSizeValueAtT(string propertyName, ulong t)
        {
            return ((DrawingSizeAnimation)Animations[propertyName]).GetValueAtT(t);
        }

        protected string GetPropertyStringValueAtT(string propertyName, ulong t)
        {
            return ((StringAnimation)Animations[propertyName]).GetValueAtT(t);
        }

        protected bool TargetPropertyExists(string propertyName)
        {
            return (Animations.ContainsKey(propertyName));
        }

        private class PropertyAnimations : SortedList<string, Animation>
        { }
    }
}
