using System;
using System.Collections.Generic;
using SharpDX;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
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
    public class Label : UIControl
    {
        private TextContent _textContent = null;
        //private ColorContent _backgroundContent = null;

        public Label(Components.IUIRoot visualRoot)
            : base(visualRoot)
        {
            _textContent = new TextContent();
            _textContent.Container = this;
        }

        //public Label(Components.IUIRoot visualRoot, TextContent content)
        //    : base(visualRoot)
        //{
        //    _textContent = content;
        //    _textContent.Container = this;
        //    _backgroundContent = new ColorContent();
        //    _backgroundContent.Container = this;
        //    this.Size = visualRoot.Theme.Label.Size.Value;
        //}
        
        public Label(Components.IUIRoot visualRoot, TextContent content)
            : base(visualRoot)
        {
            _textContent = content;
            _textContent.Container = this;
        }

        public Label(Components.IUIRoot visualRoot, string text, SpriteFont font, Color4 fontColor, Color4 backgroundColor)
            : base(visualRoot)
        {
            _textContent = new TextContent(text, font, fontColor);
            _textContent.Container = this;
            //_backgroundContent = new ColorContent(backgroundColor);
            //_backgroundContent.Container = this;           
        }
    

        public static Label FromTemplate(IUIRoot visualRoot, LabelTemplate template)
        {
            Label label = new Label(visualRoot);
            label.ApplyTemplate(template);
            return label;
        }

        public TextContent Text { get { return _textContent; } set { _textContent = value; } }

        //public ColorContent Background { get { return _backgroundContent; } set { _backgroundContent = value; }  }

        protected override void OnDraw(long gameTime)
        {
            //Background.Draw(gameTime);
           // if (Background != null)
            //    Background.Draw(gameTime);
            if(Text != null)
                Text.Draw(gameTime);
            base.OnDraw(gameTime);
        }

        public override void ApplyTemplate(Components.UIControlTemplate template)
        {
            Components.LabelTemplate labelTemplate = template as Components.LabelTemplate;
            if (labelTemplate == null)
                throw new ArgumentException("template was not of type LabelTemplate");

            if (labelTemplate.CaptionStyle != null)
                Text.ApplyStyle(labelTemplate.CaptionStyle);

            //if (labelTemplate.BackgroundColor != null)
                //Background.ApplyStyle(labelTemplate.BackgroundColor);

            base.ApplyTemplate(template);
        }

        public void SizeToContent()
        {
            //RectangleF smallestBounds = Content.CalculateSmallestBoundingRect();
            //Vector2 smallestSize = smallestBounds.Size();
            //this.Size = new DrawingSizeF(smallestSize.X, smallestSize.Y);
            if (Text != null)
                this.Size = Text.CalculateSmallestBoundingRect().Size();
            else
                this.Size = DrawingSizeFExtension.Zero;
        }
    }
}
