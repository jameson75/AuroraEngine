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
        private ColorContent _backgroundContent = null;

        public Label(Components.IUIRoot visualRoot)
            : base(visualRoot)
        {
           
        }
       
        public Label(Components.IUIRoot visualRoot, TextContent text, ColorContent background = null)
            : base(visualRoot)
        {
            _textContent = text;
            _textContent.Container = this;
        }

        public Label(Components.IUIRoot visualRoot, string text, SpriteFont font, Color4 fontColor, Color4? backgroundColor = null)
            : base(visualRoot)
        {
            _textContent = new TextContent(text, font, fontColor);
            _textContent.Container = this;
            if (backgroundColor != null)
            {
                _backgroundContent = new ColorContent(backgroundColor.Value);
                _backgroundContent.Container = this;
            }
        }
    

        public static Label FromTemplate(IUIRoot visualRoot, LabelTemplate template)
        {
            Label label = new Label(visualRoot);
            label.ApplyTemplate(template);
            return label;
        }

        public TextContent TextContent
        {
            get
            {
                return _textContent;
            }
            set
            {
                _textContent = value;
                if (value == null && _textContent != null)
                    _textContent.Container = null;
                _textContent = value;
                if (_textContent != null)
                    _textContent.Container = this;
            }
        }

        public ColorContent BackgroundContent
        {
            get
            {
                return _backgroundContent;
            }
            set
            { 
                _backgroundContent = value;
                if (value == null && _backgroundContent != null)
                    _backgroundContent.Container = null;
                _backgroundContent = value;
                if (_backgroundContent != null)
                    _backgroundContent.Container = this;
            }
        }

        protected override void OnDraw(GameTime gameTime)
        {          
            if (BackgroundContent != null)
                BackgroundContent.Draw(gameTime);
            
            if(TextContent != null)
                TextContent.Draw(gameTime);
            
            base.OnDraw(gameTime);
        }

        public override void ApplyTemplate(Components.UIControlTemplate template)
        {
            Components.LabelTemplate labelTemplate = template as Components.LabelTemplate;
            if (labelTemplate == null)
                throw new ArgumentException("template was not of type LabelTemplate");

            if (labelTemplate.CaptionStyle != null)
                TextContent.ApplyStyle(labelTemplate.CaptionStyle);

            if (labelTemplate.BackgroundColor != null)
                _backgroundContent.ApplyStyle(labelTemplate.BackgroundColor);

            base.ApplyTemplate(template);
        }

        public void SizeToContent()
        {            
            if (TextContent != null)
                this.Size = TextContent.CalculateSmallestBoundingRect().Size();
            else
                this.Size = Size2FExtension.Zero;
        }
    }
}
