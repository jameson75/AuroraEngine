﻿using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.Utils;
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
        UIControl clientTemplate = null;
        UIControl cursorTemplate = null;
        Range _range = Range.Empty;
        float _interval = 0.0f;

        public Slider(Components.IUIRoot visualRoot)
            : base(visualRoot)
        {
            InitializeControl();
        }

        private void InitializeControl()
        {
            GrooveTemplate = new ContentControl(VisualRoot, new ColorContent(SharpDX.Color.Red));
            GrooveTemplate.Size = new DrawingSizeF(0, 3);
            GrooveTemplate.HorizontalAlignment = HorizontalAlignment.Stretch;
            GrooveTemplate.VerticalAlignment = VerticalAlignment.Center;
            HandleTemplate = new ContentControl(VisualRoot, new ColorContent(SharpDX.Color.Blue));
            HandleTemplate.Size = new DrawingSizeF(5, 0);
            HandleTemplate.VerticalAlignment = VerticalAlignment.Stretch;
            HandleTemplate.Margin = DrawingSizeFExtension.Zero;
            UpdateLayout(LayoutUpdateReason.ChildSizeChanged);
        }

        public UIControl GrooveTemplate
        {
            get
            {
                return clientTemplate;
            }
            set
            {
                clientTemplate = value;
                this.Children.Add(clientTemplate);               
            }
        }

        public UIControl HandleTemplate
        {
            get
            {
                return cursorTemplate;
            }
            set
            {
                cursorTemplate = value;
                this.Children.Add(cursorTemplate);
            }
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

        public override void Draw(long gameTime)
        {
            GrooveTemplate.Draw(gameTime);
            HandleTemplate.Draw(gameTime);
        }
    }
}
