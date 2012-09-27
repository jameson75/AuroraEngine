using System;
using System.Collections.Generic;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class ImageControl : UIControl
    {
        private ImageContent _content = null;

        public ImageControl(Components.IUIRoot visualRoot) : base(visualRoot)
        {
            Content = new ImageContent();            
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
