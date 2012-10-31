using System;
using System.Collections.Generic;
using SharpDX;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Interop;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class Label : UIControl
    {
        private TextContent _textContent = null;

        public Label(Components.IUIRoot visualRoot)
            : base(visualRoot)
        {
            _textContent = new TextContent();
            _textContent.Container = this;
        }

        public Label(Components.IUIRoot visualRoot, TextContent content)
            : base(visualRoot)
        {
            _textContent = content;
            _textContent.Container = this;
        }
            
        public Label(Components.IUIRoot visualRoot, string text, SpriteFont font, Color4 fontColor, Color4 backgroundColor) : base(visualRoot)
        {
            _textContent = new TextContent(text, font, fontColor, backgroundColor);
            _textContent.Container = this;
        }
 
        public TextContent Content { get { return _textContent; } }

        public override void Draw(long gameTime)
        {
            Content.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
