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
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Animation
{
    public abstract class PropertiesAnimationController : SimulatorController
    {
        private PropertyAnimations _animatedProperties = new PropertyAnimations();
        private PropertyAnimations AnimatedProperties { get { return _animatedProperties; } }
        private const ulong DefaultKeyFrameTime = 0;

        public ulong RunningTime
        {
            get
            {
                ulong runningTime = 0;
                foreach (string key in _animatedProperties.Keys)
                {
                    if (_animatedProperties[key].RunningTime > runningTime)
                        runningTime = _animatedProperties[key].RunningTime;
                }
                return runningTime;
            }
        }

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
            return AnimatedProperties[propertyName].GetActiveKeyFrameAtT(timeT);
        }

        private AnimationKeyFrame GetNextKeyFrame(string propertyName, AnimationKeyFrame keyFrame)
        {
            if (!TargetPropertyExists(propertyName))
                throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");
            return AnimatedProperties[propertyName].GetNextKeyFrame(keyFrame);
        }

        private AnimationKeyFrame GetPreviousKeyFrame(string propertyName, AnimationKeyFrame keyFrame)
        {
            if (!TargetPropertyExists(propertyName))
                throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");
            return AnimatedProperties[propertyName].GetPreviousKeyFrame(keyFrame);
        }

        protected void AddPropertyAnimation(string propertyName, KeyframeAnimation animation)
        {
            if (TargetPropertyExists(propertyName))
                throw new InvalidOperationException("The target property already exists for this effect");
            AnimatedProperties.Add(propertyName, animation);
        }

        protected void SetPropertyKeyFrame(string propertyName, AnimationKeyFrame keyFrame)
        {
            if (!TargetPropertyExists(propertyName))
                throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");
            AnimatedProperties[propertyName].SetKeyFrame(keyFrame);
        }

        protected void RemovePropertyKeyFrame(string propertyName, ulong tF, bool removeEmptyAnimation = false)
        {
            if (!TargetPropertyExists(propertyName))
                throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");

            AnimatedProperties[propertyName].RemoveKeyFrame(tF);

            if (removeEmptyAnimation && AnimatedProperties[propertyName].FrameCount == 0)
                AnimatedProperties.Remove(propertyName);
        }

        protected void RemoveProperty(string propertyName)
        {
            if (!TargetPropertyExists(propertyName))
                throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");

            AnimatedProperties.Remove(propertyName);
        }


        protected float GetPropertyIntValueAtT(string propertyName, ulong t)
        {
            return ((IntAnimation)AnimatedProperties[propertyName]).GetValueAtT(t);
        }
        
        protected float GetPropertyFloatValueAtT(string propertyName, ulong t)
        {
            return ((FloatAnimation)AnimatedProperties[propertyName]).GetValueAtT(t);
        }

        protected bool GetPropertyBooleanValueAtT(string propertyName, ulong t)
        {
            return ((BooleanAnimation)AnimatedProperties[propertyName]).GetValueAtT(t);
        }

        protected Vector2 GetPropertyPointValueAtT(string propertyName, ulong t)
        {
            return ((PointAnimation)AnimatedProperties[propertyName]).GetValueAtT(t);
        }

        protected Size2F GetPropertySize2ValueAtT(string propertyName, ulong t)
        {
            return ((Size2Animation)AnimatedProperties[propertyName]).GetValueAtT(t);
        }
        
        protected RectangleF GetPropertyRectangleValueAtT(string propertyName, ulong t)
        {
            return ((RectangleAnimation)AnimatedProperties[propertyName]).GetValueAtT(t);
        }      

        protected Color4 GetPropertyColorValueAtT(string propertyName, ulong t)
        {
            return ((Color4Animation)AnimatedProperties[propertyName]).GetValueAtT(t);
        }

        protected Vector3 GetPropertyVector3AtT(string propertyName, ulong t)
        {
            return ((Vector3Animation)AnimatedProperties[propertyName]).GetValueAtT(t);
        }       

        protected bool TargetPropertyExists(string propertyName)
        {
            return (AnimatedProperties.ContainsKey(propertyName));
        }
       
        private class PropertyAnimations : SortedList<string, KeyframeAnimation>
        { }    
    }
}
