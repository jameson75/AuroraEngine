using System;
using System.Collections.Generic;
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
            Content = (ImageContent)DefaultTemplates.ImageControl.ImageStyle.GenerateContent();
            Size = DefaultTemplates.ImageControl.Size.Value;
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

        public override void Draw(long gameTime)
        {
            if(_content != null)
                _content.Draw(gameTime);
            base.Draw(gameTime);
        }         
    }
}
