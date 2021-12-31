
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Animation
{
    public abstract class UIAnimationControllerBase : PropertiesAnimationController
    { 
        private long? _animationStartTime = null;
        protected long? ElapsedTime 
        {
             get { return _animationStartTime; }
        }

        public override void Reset()
        {
            _animationStartTime = null;            
        }

        public override void UpdateAnimation(GameTime gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime.GetTotalSimtime();
            
            ulong timeT = (ulong)(gameTime.GetTotalSimtime() - _animationStartTime.Value);

            OnUpdateTarget(timeT);

            if (timeT >= RunningTime)
            {
                if (Loop)
                    _animationStartTime = null;
                else
                    this.OnAnimationComplete();
            }
        }

        public bool Loop { get; set; }

        protected virtual void OnUpdateTarget(ulong timeT)
        { }
    }

    public abstract class UIControlAnimationController<T> : UIAnimationControllerBase where T : UIControl
    {       
        public T Target { get; set; }

        public UIControlAnimationController()
        {
        }

        protected override void OnUpdateTarget(ulong timeT)
        {            
            if (TargetPropertyExists(UIControlPropertyNames.Enabled))
                Target.Enabled = GetPropertyBooleanValueAtT(UIControlPropertyNames.Enabled, timeT);

            if (TargetPropertyExists(UIControlPropertyNames.Position))
                Target.Position = GetPropertyPointValueAtT(UIControlPropertyNames.Position, timeT);

            if (TargetPropertyExists(UIControlPropertyNames.Size))
                Target.Size = GetPropertySize2ValueAtT(UIControlPropertyNames.Size, timeT);

            if (TargetPropertyExists(UIControlPropertyNames.Visible))
                Target.Visible = GetPropertyBooleanValueAtT(UIControlPropertyNames.Visible, timeT);

            if (TargetPropertyExists(UIControlPropertyNames.ZOrder))
                Target.ZOrder = GetPropertyFloatValueAtT(UIControlPropertyNames.ZOrder, timeT);
       
            if (TargetPropertyExists(UIControlPropertyNames.PositionAndSize))
            {
                RectangleF rect = GetPropertyRectangleValueAtT(UIControlPropertyNames.PositionAndSize, timeT);
                Target.Position = rect.Position();
                Target.Size = rect.Size();
            }

            base.OnUpdateTarget(timeT);
        }

        public bool GetEnabledAtT(ulong timeT)
        {
            if (TargetPropertyExists(UIControlPropertyNames.Enabled))
                return GetPropertyBooleanValueAtT(UIControlPropertyNames.Enabled, timeT);
            else
                return Target.Enabled;
        }

        public void SetEnabledAtT(ulong timeT, bool value)
        {
            if (!TargetPropertyExists(UIControlPropertyNames.Enabled))
                AddPropertyAnimation(UIControlPropertyNames.Enabled, new BooleanAnimation());
            SetPropertyKeyFrame(UIControlPropertyNames.Enabled, new AnimationKeyFrame(timeT, value));
        }

        public Vector2 GetPositionAtT(ulong timeT)
        {
            if (TargetPropertyExists(UIControlPropertyNames.Position))
                return GetPropertyPointValueAtT(UIControlPropertyNames.Position, timeT);
            else
                return Target.Position;
        }

        public void SetPositionAtT(ulong timeT, Vector2 value)
        {
            if (!TargetPropertyExists(UIControlPropertyNames.Position))
                AddPropertyAnimation(UIControlPropertyNames.Position, new PointAnimation());
            SetPropertyKeyFrame(UIControlPropertyNames.Position, new AnimationKeyFrame(timeT, value));
        }

        public Size2F GetSizeAtT(ulong timeT)
        {
            if (TargetPropertyExists(UIControlPropertyNames.Size))
                return GetPropertySize2ValueAtT(UIControlPropertyNames.Size, timeT);
            else
                return Target.Size;
        }

        public void SetEnabledAtT(ulong timeT, Size2F value)
        {
            if (!TargetPropertyExists(UIControlPropertyNames.Size))
                AddPropertyAnimation(UIControlPropertyNames.Size, new Size2Animation());
            SetPropertyKeyFrame(UIControlPropertyNames.Size, new AnimationKeyFrame(timeT, value));
        }


        public bool GetVisibleAtT(ulong timeT)
        {
            if (TargetPropertyExists(UIControlPropertyNames.Visible))
                return GetPropertyBooleanValueAtT(UIControlPropertyNames.Visible, timeT);
            else
                return Target.Visible;
        }

        public void SetVisibleAtT(ulong timeT, bool value)
        {
            if (!TargetPropertyExists(UIControlPropertyNames.Visible))
                AddPropertyAnimation(UIControlPropertyNames.Visible, new BooleanAnimation());
            SetPropertyKeyFrame(UIControlPropertyNames.Visible, new AnimationKeyFrame(timeT, value));
        }

        public float GetZOrderAtT(ulong timeT)
        {
            if (TargetPropertyExists(UIControlPropertyNames.ZOrder))
                return GetPropertyFloatValueAtT(UIControlPropertyNames.ZOrder, timeT);
            else
                return Target.ZOrder;
        }

        public void SetZOrderAtT(ulong timeT, float value)
        {
            if (!TargetPropertyExists(UIControlPropertyNames.ZOrder))
                AddPropertyAnimation(UIControlPropertyNames.ZOrder, new FloatAnimation());
            SetPropertyKeyFrame(UIControlPropertyNames.ZOrder, new AnimationKeyFrame(timeT, value));
        }

        public RectangleF GetPositionAndSizeAtT(ulong timeT)
        {
            if (TargetPropertyExists(UIControlPropertyNames.PositionAndSize))
                return GetPropertyRectangleValueAtT(UIControlPropertyNames.PositionAndSize, timeT);
            else
                return new RectangleF(Target.Position.X, Target.Position.Y, Target.Size.Width, Target.Size.Height);
        }

        public void SetPositionAndSizeAtT(ulong timeT, RectangleF value)
        {
            if (!TargetPropertyExists(UIControlPropertyNames.PositionAndSize))
                AddPropertyAnimation(UIControlPropertyNames.PositionAndSize, new RectangleAnimation());
            SetPropertyKeyFrame(UIControlPropertyNames.PositionAndSize, new AnimationKeyFrame(timeT, value));
        }
        
        private static class UIControlPropertyNames
        {
            public const string Enabled = "Enabled";
            public const string Position = "Position";
            public const string Size = "Size";
            public const string Visible = "Visible";
            public const string ZOrder = "ZOrder";
            public const string PositionAndSize = "PositionAndSize";
        }      
    }

    public class PanelAnimationController : UIControlAnimationController<Panel>
    {
        protected override void OnUpdateTarget(ulong timeT)
        {
            base.OnUpdateTarget(timeT);
        }
    }

    public class ContentControlAnimationController : UIControlAnimationController<ContentControl>
    {
        protected override void OnUpdateTarget(ulong timeT)
        {
            base.OnUpdateTarget(timeT);
        }
    }

    public class MenuAnimationController : UIControlAnimationController<Menu>
    {
        protected override void OnUpdateTarget(ulong timeT)
        {
            base.OnUpdateTarget(timeT);
        }
    }

    public class MenuItemAnimationController : UIControlAnimationController<MenuItem>
    {
        protected override void OnUpdateTarget(ulong timeT)
        {
            base.OnUpdateTarget(timeT);
        }
    }

    //public abstract class UIContentAnimation : PropertyGroupAnimation
    //{
    //    public SpriteSortMode? SpriteSortMode { get; set; }
    //    public BlendState BlendState { get; set; }
    //    public SamplerState SamplerState { get; set; }
    //    public DepthStencilState DepthStencilState { get; set; }
    //    public RasterizerState RasterizerState { get; set; }
    //    public Action CustomShaderCallback { get; set; }
    //    public Matrix? TransformationMatrix { get; set; }
    //}

    //public class ListControlItemEffect : PropertyGroupAnimation
    //{

    //}   
}