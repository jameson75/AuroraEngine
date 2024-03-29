﻿using System;
using System.Collections.Generic;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.UI.Components;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Controls
{
    public class Slider : UIControl
    {
        ContentControl trackContentControl = null;
        ContentControl handleContentControl = null;
        RangeF _range = RangeF.Empty;
        float _interval = 0.0f;

        private Slider(IUIRoot visualRoot) 
           : base(visualRoot)
        {
            trackContentControl = new ContentControl(VisualRoot);
            handleContentControl = new ContentControl(VisualRoot);
            Children.Add(trackContentControl);
            Children.Add(handleContentControl);
            UpdateLayout(LayoutUpdateReason.ChildSizeChanged);
        }

        public Slider(IUIRoot visualRoot, UIContent trackRendering, float trackWidth, UIContent handleRendering, float handleWidth)
            : base(visualRoot)
        {
            trackContentControl = new ContentControl(VisualRoot, trackRendering);            
            handleContentControl = new ContentControl(VisualRoot, handleRendering); 
            Children.Add(trackContentControl);
            Children.Add(handleContentControl);
            UpdateLayout(LayoutUpdateReason.ChildSizeChanged);
        }

        public static Slider FromTemplate(IUIRoot visualRoot, SliderTemplate template)
        {
            Slider slider = new Slider(visualRoot);
            slider.ApplyTemplate(template);
            return slider;
        }

        public override void ApplyTemplate(UIControlTemplate template)
        {
            SliderTemplate sliderTemplate = (SliderTemplate)template;
            
            if (sliderTemplate.TrackContent != null)
                trackContentControl.ApplyTemplate(sliderTemplate.TrackContent);

            if (sliderTemplate.HandleContent != null)
                handleContentControl.ApplyTemplate(sliderTemplate.HandleContent);

            base.ApplyTemplate(template);
        }

        protected virtual void OnRangeChanged()
        {
           
        }

        protected virtual void OnIntervalChanged()
        {
            
        }

        public RangeF Range
        {
            get { return _range; }
            set
            {
                _range = value;
                OnRangeChanged();
            }
        }

        public float Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                OnIntervalChanged();
            }
        }

        protected override void OnDraw()
        {
            trackContentControl.Draw();
            handleContentControl.Draw();
        }
    }
}
