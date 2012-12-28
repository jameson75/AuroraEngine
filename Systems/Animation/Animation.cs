using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;

using CipherPark.AngelJacket.Core.Module;

namespace CipherPark.AngelJacket.Core.Systems.Animation
{
    public class AnimationController
    {
        private IGameApp _game = null;

        public AnimationController(IGameApp game)
        {
            _game = game;    
        }

        public void Start(Animation animation)
        { }

        public void Stop(Animation animation)
        { }       
    }

    public abstract class Animation
    {
        private AnimationKeyFrames _keyFrames = new AnimationKeyFrames();

         /// <summary>
        /// Gets the nearest key frame whose time is less than or equal to timeT.
        /// </summary>       
        /// <param name="timeT"></param>
        /// <returns>The nearest key frame whose time is less than or equal to timeT.</returns>       
        /// <example>If there are keyframes, at t=0, t=2 and t=4, respectively, then the keyframe
        /// at t=2 would be returned for a timeT of 3.9.</example>
        public AnimationKeyFrame GetActiveKeyFrameAtT(ulong timeT)
        {            
            for (int i = _keyFrames.Count - 1; i >= 0; i--)
            {
                if (_keyFrames[_keyFrames.Keys[i]].Time > timeT)
                    return _keyFrames[_keyFrames.Keys[i - 1]];
            }
            if (_keyFrames.Count != 0 &&
                _keyFrames[0].Time <= timeT)
                return _keyFrames[0];
            else
                return null;
        }

        public AnimationKeyFrame GetNextKeyFrame(AnimationKeyFrame keyFrame)
        {
            int i = _keyFrames.IndexOfValue(keyFrame);
            if (i == -1)
                throw new ArgumentException("keyFrame is not an element of the specified property's animation", "keyFrame");
            else if (i == _keyFrames.Count - 1)
                return null;
            else
                return _keyFrames[_keyFrames.Keys[i + 1]];
        }

        public AnimationKeyFrame GetPreviousKeyFrame(AnimationKeyFrame keyFrame)
        {
            int i = _keyFrames.IndexOfValue(keyFrame);
            if (i == -1)
                throw new ArgumentException("keyFrame is not an element of the specified property's animation", "keyFrame");
            else if (i == 0)
                return null;
            else
                return _keyFrames[_keyFrames.Keys[i - 1]];
        }

        public void SetKeyFrame(AnimationKeyFrame keyFrame)
        {
            if (_keyFrames.ContainsKey(keyFrame.Time))
                _keyFrames[keyFrame.Time] = keyFrame;
            else
                _keyFrames.Add(keyFrame.Time, keyFrame);
        }

        public void RemoveKeyFrame(ulong timeT)
        {
            if (!_keyFrames.ContainsKey(timeT))
                throw new ArgumentException("No key frame exists with the specified time.", "timeT");
            _keyFrames.Remove(timeT);
        }

        public int FrameCount
        {
            get { return _keyFrames.Count; }
        }
        
        protected static double Lerp(double v0, double v1, float percentage)
        {
            return v0 + (percentage * (v1 - v0));
        }
    }

    public class DrawingSizeAnimation : Animation
    {
        public DrawingSizeF GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            AnimationKeyFrame f1 = GetNextKeyFrame(f0);
            DrawingSizeF frameVal0 = (f0.Value != null) ? (DrawingSizeF)f0.Value : Utils.DrawingSizeFExtension.Zero;
            DrawingSizeF frameVal1 = (f1 != null && f1.Value != null) ? (DrawingSizeF)f0.Value : Utils.DrawingSizeFExtension.Zero;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.InterpolationCurve != null && f0.InterpolationCurve.Length != 0)
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

    public class DrawingPointAnimation : Animation
    {
        public DrawingPointF GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            AnimationKeyFrame f1 = GetNextKeyFrame(f0);
            DrawingPointF frameVal0 = (f0.Value != null) ? (DrawingPointF)f0.Value : Utils.DrawingPointFExtension.Zero;
            DrawingPointF frameVal1 = (f1 != null && f1.Value != null) ? (DrawingPointF)f0.Value : Utils.DrawingPointFExtension.Zero;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.InterpolationCurve != null && f0.InterpolationCurve.Length != 0)
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

    public class StringAnimation : Animation
    {
        public string GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            return (f0.Value != null) ? Convert.ToString(f0.Value) : null;
        }
    }

    public class FloatAnimation : Animation
    {
        public float GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            AnimationKeyFrame f1 = GetNextKeyFrame(f0);
            float frameVal0 = (f0.Value != null) ? Convert.ToSingle(f0.Value) : 0.0f;
            float frameVal1 = (f1 != null && f1.Value != null) ? Convert.ToSingle(f1.Value) : 0.0f;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.InterpolationCurve != null && f0.InterpolationCurve.Length != 0)
                    return (float)Lerp(frameVal0, frameVal1, pctT);
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

    public class BooleanAnimation : Animation
    {
        public bool GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            return (f0.Value != null) ? Convert.ToBoolean(f0.Value) : false;
        }
    }

    public class TransformAnimation : Animation
    {
        private long? _lastGameTime = null;
 
        public Transform GetValueAtT(ulong t)
        {
            AnimationKeyFrame f0 = GetActiveKeyFrameAtT(t);
            AnimationKeyFrame f1 = GetNextKeyFrame(f0);
            Transform frameVal0 = (f0.Value != null) ? (Transform)f0.Value : Transform.Zero;
            Transform frameVal1 = (f1 != null && f1.Value != null) ? (Transform)f0.Value : Transform.Zero;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.InterpolationCurve != null && f0.InterpolationCurve.Length != 0)
                {
                    Quaternion r = Quaternion.Lerp(frameVal0.Rotation, frameVal1.Rotation, pctT);
                    Vector3 x = Vector3.Lerp(frameVal0.Translation, frameVal1.Translation, pctT);
                    return new Transform { Rotation = r, Translation = x };
                }
                else
                {
                    //TODO: Implement either ease-in/ease-out with hermite control points.
                    //or implement Curve with catmull-rom.
                    //    for (int i = 0; i < f0.Curve.Length; i++)
                    //    {

                    //    }
                    return Transform.Zero;
                }
            }
        }

        public void Start()
        {
            _lastGameTime = null;
        }

        public void UpdateAnimation(long gameTime, ITransformable transformable)
        {
            if (_lastGameTime == null)
                _lastGameTime = gameTime;

            ulong timeT = (ulong)(gameTime - _lastGameTime.Value);

            transformable.Transform = this.GetValueAtT(timeT);
        }
    }

    public struct Transform
    {
        private static Transform _zero = new Transform { Rotation = Quaternion.Zero, Translation = Vector3.Zero };
        private static Transform _identity = new Transform { Rotation = Quaternion.Identity, Translation = Vector3.Zero };
        public Quaternion Rotation { get; set; }
        public Vector3 Translation { get; set; }
        public static Transform Zero { get { return _zero; } }
        public static Transform Identity { get { return _identity; } }
    }

    public interface ITransformable
    {
        Transform Transform { get; set; }
    }

    public class AnimationKeyFrames : SortedList<ulong, AnimationKeyFrame>
    { }

    public class AnimationKeyFrame
    {
        private ulong _time = 0;
        public AnimationKeyFrame(ulong time)
        {
            _time = time;
        }
        public AnimationKeyFrame(ulong time, object value)
        {
            _time = time;
            Value = value;
        }
        public ulong Time { get { return _time; } }
        public object Value { get; set; }
        public AnimationInterpolationPoint[] InterpolationCurve { get; set; }
    }

    public class AnimationInterpolationPoint
    {
        private float _timeOffset = 0;
        public AnimationInterpolationPoint(float timeOffset)
        {
            _timeOffset = timeOffset;
        }
        public AnimationInterpolationPoint(float timeOffset, object value)
        {
            _timeOffset = timeOffset;
            Value = value;
        }
        public float TimeOffset { get { return _timeOffset; } }
        public object Value { get; set; }
    }

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
