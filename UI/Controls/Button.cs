using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils;
using SharpDX;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class Button : UIControl
    {
        UIContent _content = null;        

        public Button(IUIRoot visualRoot)
            : base(visualRoot)
        { }

        public Button(IUIRoot visualRoot, string text, SpriteFont font, Color4 fontColor, Color4 bgColor)
            : base(visualRoot)
        {
            Content = new TextContent(text, font, fontColor, bgColor);
        }

        public Button(IUIRoot visualRoot, Texture2D texture)
            : base(visualRoot)
        {
            Content = new ImageContent(texture);
        }

        public UIContent Content
        {
            get { return _content; }
            set
            {
                if (value == null && _content != null)
                    _content.Container = null;
                _content = value;
                if (_content != null)
                    _content.Container = this;
                OnContentChanged();
            }
        }

        public override void Draw(long gameTime)
        {
            if( Content != null )            
                Content.Draw(gameTime);
            base.Draw(gameTime);
        }

        protected virtual void OnContentChanged()
        {
            EventHandler handler = ContentChanged;
            if( handler != null )
                handler(this, EventArgs.Empty);
        }

        public EventHandler ContentChanged;
    }
}
