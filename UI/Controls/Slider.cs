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
            trackContentControl.ApplyTemplate(visualRoot.Theme.Slider.TrackContent); //new ColorContent(SharpDX.Color.Red)                   
            handleContentControl = new ContentControl(VisualRoot); //, new ColorContent(SharpDX.Color.Blue));
            handleContentControl.ApplyTemplate(this.VisualRoot.Theme.Slider.HandleContent);
            this.Children.Add(trackContentControl);
            this.Children.Add(handleContentControl);
            UpdateLayout(LayoutUpdateReason.ChildSizeChanged);
        }       

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

        protected override void OnDraw(long gameTime)
        {
            trackContentControl.Draw(gameTime);
            handleContentControl.Draw(gameTime);
        }
    }
}
