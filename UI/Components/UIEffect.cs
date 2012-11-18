
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.Utils.Interop;

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public abstract class UIEffect
    {
        private TargetAnimation targetAnimation = new TargetAnimation();
        private TargetAnimation TargetAnimation { get { return targetAnimation; } }
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
        private PropertyKeyFrame GetActiveKeyFrameAtT(string propertyName, ulong timeT)
        {
            if (!TargetPropertyExists(propertyName))
                throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");

            TargetPropertyKeyFrames propertyKeyFrames = TargetAnimation[propertyName];
            for (int i = propertyKeyFrames.Count - 1; i >= 0; i--)
            {
                if (propertyKeyFrames[propertyKeyFrames.Keys[i]].Time > timeT)
                    return propertyKeyFrames[propertyKeyFrames.Keys[i-1]];
            }

            return propertyKeyFrames[DefaultKeyFrameTime];
        }

        private PropertyKeyFrame GetNextKeyFrame(string propertyName, PropertyKeyFrame keyFrame)
        {
            if (!TargetPropertyExists(propertyName))
                throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");

            TargetPropertyKeyFrames propertyKeyFrames = TargetAnimation[propertyName];
            int i = propertyKeyFrames.IndexOfValue(keyFrame);
            if (i == -1 )
                throw new ArgumentException("keyFrame is not an element of the specified property's animation", "keyFrame");
            else if(i == propertyKeyFrames.Count - 1)
                return null;
            else
                return propertyKeyFrames[propertyKeyFrames.Keys[i + 1]];                            
        }

        private PropertyKeyFrame GetPreviousKeyFrame(string propertyName, PropertyKeyFrame keyFrame)
        {
            if (!TargetPropertyExists(propertyName))
                throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");

            TargetPropertyKeyFrames propertyKeyFrames = TargetAnimation[propertyName];
            int i = propertyKeyFrames.IndexOfValue(keyFrame);
            if (i == -1)
                throw new ArgumentException("keyFrame is not an element of the specified property's animation", "keyFrame");
            else if (i == 0)
                return null;
            else
                return propertyKeyFrames[propertyKeyFrames.Keys[i-1]];
        }

        protected void AddProperty(string propertyName, object valueAtKeyFrameZero) 
        {
            if (TargetPropertyExists(propertyName))
                throw new InvalidOperationException("The target property already exists for this effect");

            TargetAnimation.Add(propertyName, new TargetPropertyKeyFrames());
            
            TargetAnimation[propertyName].Add(DefaultKeyFrameTime, new PropertyKeyFrame(DefaultKeyFrameTime, valueAtKeyFrameZero));
        }

        protected void SetPropertyKeyFrame(string propertyName, PropertyKeyFrame keyFrame)
        {
            if (!TargetPropertyExists(propertyName))
                AddProperty(propertyName, keyFrame.Value);

            if (TargetAnimation[propertyName].ContainsKey(keyFrame.Time))
                TargetAnimation[propertyName][keyFrame.Time] = keyFrame;
            else
                TargetAnimation[propertyName].Add(keyFrame.Time, keyFrame);
        }

        protected void RemovePropertyKeyFrame(string propertyName, ulong tF, bool removePropertyOnLingeringDefault = false)
        {
            if (!TargetPropertyExists(propertyName))
               throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");

            if (!TargetAnimation[propertyName].ContainsKey(tF))
                throw new ArgumentException("No key frame exists with the specified time.", "tF");

            if (tF == DefaultKeyFrameTime)
                throw new InvalidOperationException("Removing the default key frame from a property animation is not allowed.");
            
            TargetAnimation[propertyName].Remove(tF);

            if (TargetAnimation[propertyName].Count == 1 && removePropertyOnLingeringDefault)
                RemoveProperty(propertyName);
        }

        protected void RemoveProperty(string propertyName)
        {
            if(!TargetPropertyExists(propertyName))
                throw new ArgumentException("The specified property is not a target property of this effect.", "propertyName");

            TargetAnimation.Remove(propertyName);
        }

        protected float GetPropertyFloatValueAtT(string propertyName, ulong t)
        {
            PropertyKeyFrame f0 = GetActiveKeyFrameAtT(propertyName, t);
            PropertyKeyFrame f1 = GetNextKeyFrame(propertyName, f0);
            float frameVal0 = (f0.Value != null) ? Convert.ToSingle(f0.Value) : 0.0f;
            float frameVal1 = (f1 != null && f1.Value != null) ? Convert.ToSingle(f1.Value) : 0.0f;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.Curve != null && f0.Curve.Length != 0)
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

        protected bool GetPropertyBooleanValueAtT(string propertyName, ulong t)
        {
            PropertyKeyFrame f0 = GetActiveKeyFrameAtT(propertyName, t);
            return (f0.Value != null) ? Convert.ToBoolean(f0.Value) : false;
        }

        protected DrawingPointF GetPropertyDrawingPointValueAtT(string propertyName, ulong t)
        {
            PropertyKeyFrame f0 = GetActiveKeyFrameAtT(propertyName, t);
            PropertyKeyFrame f1 = GetNextKeyFrame(propertyName, f0);
            DrawingPointF frameVal0 = (f0.Value != null) ? (DrawingPointF)f0.Value : Utils.DrawingPointFExtension.Zero;
            DrawingPointF frameVal1 = (f1 != null && f1.Value != null) ? (DrawingPointF)f0.Value : Utils.DrawingPointFExtension.Zero;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.Curve != null && f0.Curve.Length != 0)
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

        protected DrawingSizeF GetPropertyDrawingSizeValueAtT(string propertyName, ulong t)
        {
            PropertyKeyFrame f0 = GetActiveKeyFrameAtT(propertyName, t);
            PropertyKeyFrame f1 = GetNextKeyFrame(propertyName, f0);
            DrawingSizeF frameVal0 = (f0.Value != null) ? (DrawingSizeF)f0.Value : Utils.DrawingSizeFExtension.Zero;
            DrawingSizeF frameVal1 = (f1 != null && f1.Value != null) ? (DrawingSizeF)f0.Value : Utils.DrawingSizeFExtension.Zero;
            if (f1 == null)
                return frameVal0;
            else
            {
                float pctT = (float)(t - f0.Time) / (float)(f1.Time - f0.Time);
                if (f0.Curve != null && f0.Curve.Length != 0)
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

        protected string GetPropertyStringValueAtT(string propertyName, ulong t)
        {
            PropertyKeyFrame f0 = GetActiveKeyFrameAtT(propertyName, t);
            return (f0.Value != null) ? Convert.ToString(f0.Value) : null;
        }

        protected bool TargetPropertyExists(string propertyName)
        {
            return (TargetAnimation.ContainsKey(propertyName) &&
                TargetAnimation[propertyName].Count > 0);
        }

        private double Lerp(double v0, double v1, float percentage)
        {
            return v0 + (percentage * (v1 - v0));
        }
    }

    public abstract class ControlEffect : UIEffect
    {
        ControlEffectCollection _children = null;

        public ControlEffect()
        {
            _children = new ControlEffectCollection();
        }

        public ControlEffectCollection Children
        {
            get
            {
                return _children;
            }
        }       

        public virtual void UpdateControl(long gameTime, UIControl control)
        {
            ulong timeT = 0;

            if (TargetPropertyExists(UIControlPropertyNames.Enabled))
                control.Enabled = GetPropertyBooleanValueAtT(UIControlPropertyNames.Enabled, timeT);

            if (TargetPropertyExists(UIControlPropertyNames.Position))
                control.Position = GetPropertyDrawingPointValueAtT(UIControlPropertyNames.Position, timeT);

            if (TargetPropertyExists(UIControlPropertyNames.Size))
                control.Size = GetPropertyDrawingSizeValueAtT(UIControlPropertyNames.Size, timeT);

            if (TargetPropertyExists(UIControlPropertyNames.Visible))
                control.Visible = GetPropertyBooleanValueAtT(UIControlPropertyNames.Visible, timeT);

            if (TargetPropertyExists(UIControlPropertyNames.ZOrder))
                control.ZOrder = GetPropertyFloatValueAtT(UIControlPropertyNames.ZOrder, timeT);
        }

        public static class UIControlPropertyNames
        {
            public const string Enabled = "Enabled";
            public const string Position = "Position";
            public const string Size = "Size";
            public const string Visible = "Visible";
            public const string ZOrder = "ZOrder";
        }
    }

    public abstract class ContentEffect : UIEffect
    {
        public SpriteSortMode? SpriteSortMode { get; set; }
        public BlendState BlendState { get; set; }
        public SamplerState SamplerState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public RasterizerState RasterizerState { get; set; }
        public Action CustomShaderCallback { get; set; }
        public Matrix? TransformationMatrix { get; set; }
    }

    public class ListControlItemEffect : UIEffect
    {

    }

    public class ControlEffectCollection : List<ControlEffect>
    { }

    public class TargetAnimation : SortedList<string, TargetPropertyKeyFrames>
    { }

    public class TargetPropertyKeyFrames : SortedList<ulong, PropertyKeyFrame>
    { }

    public class PropertyKeyFrame
    {
        private ulong _time = 0;
        public PropertyKeyFrame(ulong time)
        {
            _time = time;
        }
        public PropertyKeyFrame(ulong time, object value)
        {
            _time = time;
            Value = value;
        }
        public ulong Time { get { return _time; } }
        public object Value { get; set; }
        public PropertyKeyFrameCurvePoint[] Curve { get; set; }
    }

    public class PropertyKeyFrameCurvePoint
    {
         private float _timeOffset = 0;
        public PropertyKeyFrameCurvePoint(float timeOffset)
        {
            _timeOffset = timeOffset;
        }
        public PropertyKeyFrameCurvePoint(float timeOffset, object value)
        {
            _timeOffset = timeOffset;
            Value = value;
        }
        public float TimeOffset { get { return _timeOffset; } }
        public object Value { get; set; }
    }
}