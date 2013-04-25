using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.UI.Components;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class Slider : UIControl
    {
        ContentControl trackContentControl = null;
        ContentControl handleContentControl = null;
        Range _range = Range.Empty;
        float _interval = 0.0f;

        public Slider(IUIRoot visualRoot)
            : base(visualRoot)
        {
            trackContentControl = new ContentControl(VisualRoot);
            trackContentControl.ApplyTemplate(DefaultTemplates.Slider //new ColorContent(SharpDX.Color.Red)
            TrackTemplate.Size = new DrawingSizeF(0, 3);           
            HandleTemplate = new ContentControl(VisualRoot, new ColorContent(SharpDX.Color.Blue));
            HandleTemplate.Size = new DrawingSizeF(5, 0);
            HandleTemplate.VerticalAlignment = VerticalAlignment.Stretch;
            HandleTemplate.Margin = DrawingSizeFExtension.Zero;
            UpdateLayout(LayoutUpdateReason.ChildSizeChanged);
        }

        //public UIControl TrackTemplate
        //{
        //    get
        //    {
        //        return trackContentControl;
        //    }
        //    set
        //    {
        //        trackContentControl = value;
        //        this.Children.Add(trackContentControl);               
        //    }
        //}

        //public UIControl HandleTemplate
        //{
        //    get
        //    {
        //        return handleContentControl;
        //    }
        //    set
        //    {
        //        handleContentControl = value;
        //        this.Children.Add(handleContentControl);
        //    }
        //}         

        protected virtual void OnRangeChanged()
        {
           
        }

        protected virtual void OnIntervalChanged()
        {
            
        }

        public Range Range
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

        public override void Draw(long gameTime)
        {
            TrackTemplate.Draw(gameTime);
            HandleTemplate.Draw(gameTime);
        }
    }
}
