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
    public class UIContentAnimationController<T> : UIAnimationControllerBase where T : UIContent
    { 
        public T Target { get; set; }

        public UIContentAnimationController()
        {

        }   

        protected override void OnUpdateTarget(ulong timeT)
        {
            if (TargetPropertyExists(UIContentPropertyNames.Opacity))
                Target.Opacity = GetPropertyFloatValueAtT(UIContentPropertyNames.Opacity, timeT);

            base.OnUpdateTarget(timeT);
        }
        
        public float GetOpacityAtT(ulong timeT)
        {
            if (TargetPropertyExists(UIContentPropertyNames.Opacity))
                return GetPropertyFloatValueAtT(UIContentPropertyNames.Opacity, timeT);
            else
                return Target.Opacity;
        }

        public void SetOpacityAtT(ulong timeT, float value)
        {
            if (!TargetPropertyExists(UIContentPropertyNames.Opacity))
                AddPropertyAnimation(UIContentPropertyNames.Opacity, new FloatAnimation());
            SetPropertyKeyFrame(UIContentPropertyNames.Opacity, new AnimationKeyFrame(timeT, value));
        }

        private static class UIContentPropertyNames
        {
            public const string Opacity = "Opacity";
        }
    }

    public class ColorContentAnimationController : UIContentAnimationController<ColorContent>
    {
        protected override void OnUpdateTarget(ulong timeT)
        {
            if (TargetPropertyExists(ColorContentPropertyNames.Color))
                Target.Opacity = GetPropertyFloatValueAtT(ColorContentPropertyNames.Color, timeT);

            base.OnUpdateTarget(timeT);
        }

        public Color4 GetColorAT(ulong timeT)
        {
            if (TargetPropertyExists(ColorContentPropertyNames.Color))
                return GetPropertyColorValueAtT(ColorContentPropertyNames.Color, timeT);
            else
                return Target.Color;
        }

        public void SetColorAtT(ulong timeT, Color4 value)
        {
            if (!TargetPropertyExists(ColorContentPropertyNames.Color))
                AddPropertyAnimation(ColorContentPropertyNames.Color, new Color4Animation());
            SetPropertyKeyFrame(ColorContentPropertyNames.Color, new AnimationKeyFrame(timeT, value));
        }

        private static class ColorContentPropertyNames
        {
            public const string Color = "Color";
        }  
    }
}
