using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.UI.Components;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class ImageControl : UIControl
    {
        private ImageContent _content = null;

        public ImageControl(Components.IUIRoot visualRoot) : base(visualRoot)
        {
           
        }

        public ImageControl(IUIRoot visualRoot, Texture2D image)
            : base(visualRoot)
        {
            Content = new ImageContent(image);
        }

        public ImageControl(IUIRoot visualRoot, ImageContent image)
            : base(visualRoot)
        {
            Content = image;
        }

        public static ImageControl FromTemplate(IUIRoot visualRoot, ImageControlTemplate template)
        {
            ImageControl imageControl = new ImageControl(visualRoot);
            imageControl.ApplyTemplate(template);
            return imageControl;
        }

        public ImageContent Content 
        {
            get { return _content; }
            set
            {
                if (value == null && _content != null)
                    _content.Container = null;
                _content = value;
                if (_content != null)
                    _content.Container = this;
            }
        }

        protected override void OnDraw(long gameTime)
        {
            if(_content != null)
                _content.Draw(gameTime);
            base.OnDraw(gameTime);
        }

        public override void ApplyTemplate(UIControlTemplate template)
        {
            ImageControlTemplate imageControlTemplate = (ImageControlTemplate)template;
            if (imageControlTemplate.ImageStyle != null)
                this.Content = (ImageContent)imageControlTemplate.ImageStyle.GenerateContent();
            base.ApplyTemplate(template);
        }
    }
}
