using System;
using System.Collections.Generic;

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

        //TODO: Move this up a level.
        public bool IsVisible { get; set; }
 
        public TextContent Content { get { return _textContent; } }

        public override void Draw(GameTime gameTime)
        {
            Content.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
