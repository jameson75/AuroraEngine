using System;
using System.Collections.Generic;
using SharpDX;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class Label : UIControl
    {
        private TextContent _textContent = null;
        private ColorContent _backgroundContent = null;

        public Label(Components.IUIRoot visualRoot)
            : base(visualRoot)
        {
            _textContent = new TextContent();
            _textContent.Container = this;
            _backgroundContent = new ColorContent();
            _backgroundContent.Container = this;
        }

        public Label(Components.IUIRoot visualRoot, TextContent content)
            : base(visualRoot)
        {
            _textContent = content;
            _textContent.Container = this;
            _backgroundContent = new ColorContent();
            _backgroundContent.Container = this;
        }
            
        public Label(Components.IUIRoot visualRoot, string text, SpriteFont font, Color4 fontColor, Color4 backgroundColor) : base(visualRoot)
        {
            _textContent = new TextContent(text, font, fontColor);
            _textContent.Container = this;
            _backgroundContent = new ColorContent(backgroundColor);
            _backgroundContent.Container = this;
        }
 
        public TextContent Text { get { return _textContent; } }

        public ColorContent BackgroundColor { get { return _backgroundContent; } }

        public override void Draw(long gameTime)
        {
            BackgroundColor.Draw(gameTime);
            Text.Draw(gameTime);
            base.Draw(gameTime);
        }

        public override void ApplyTemplate(Components.UIControlTemplate template)
        {
            Components.LabelTemplate labelTemplate = template as Components.LabelTemplate;
            if (labelTemplate == null)
                throw new ArgumentException("template was not of type LabelTemplate");

            if (labelTemplate.Text != null)
                Text.ApplyTemplate(labelTemplate.Text);

            if (labelTemplate.BackgroundColor != null)
                BackgroundColor.ApplyTemplate(labelTemplate.BackgroundColor);

            base.ApplyTemplate(template);
        }
    }
}
