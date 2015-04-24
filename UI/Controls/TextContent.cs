using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using SharpDX;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class TextContent : UIContent
    {
        public TextContent()
        {
            VAlignment = VerticalAlignment.Top;
            HAlignment = HorizontalAlignment.Left;
        }

        public TextContent(string text, SpriteFont font, Color4 fontColor)
        {
            Text = text;
            Font = font;
            FontColor = fontColor;
            VAlignment = VerticalAlignment.Top;
            HAlignment = HorizontalAlignment.Left;
        }

        public string Text { get; set; }

        public string Format { get; set; }
        
        public SpriteFont Font { get; set; }
        
        public Color4 FontColor { get; set; }

        public VerticalAlignment VAlignment { get; set; }

        public HorizontalAlignment HAlignment { get; set; }

        public override void Draw(GameTime gameTime)
        {                       
            if (Container == null)
                throw new InvalidOperationException("No container for this content was specified.");
            
            base.Draw(gameTime); 
           
            Vector2 contentSurfacePosition = Container.PositionToSurface(Container.Position);
            this.BeginDraw();
            //if (!HasDrawParameters)
            //    Container.ControlSpriteBatch.Begin();
            //else
            //    Container.ControlSpriteBatch.Begin(SpriteSortMode == null ? CipherPark.AngelJacket.Core.Utils.Toolkit.SpriteSortMode.Deferred : SpriteSortMode.Value, BlendState, SamplerState, DepthStencilState, RasterizerState, CustomShaderCallback, TransformationMatrix);
            string outputString = string.IsNullOrEmpty(Text) ? string.Empty : string.IsNullOrEmpty(Format) ? Text : string.Format(Format, Text);            
            Container.ControlSpriteBatch.DrawString(Font, outputString, contentSurfacePosition.ToVector2(), FontColor);
            
            //Container.ControlSpriteBatch.End();     
            this.EndDraw();                   
        }

        public override RectangleF CalculateSmallestBoundingRect()
        {
            if (Font == null || string.IsNullOrEmpty(Text))
                return RectangleF.Empty;
            else
            {
                Size2F textSize = Font.MeasureString(Text);
                return new RectangleF(0, 0, (int)Math.Ceiling(textSize.Width),(int) Math.Ceiling(textSize.Height));
            }
        }

        public float GetTextLength(int startIndex, int count)
        {
            string subText = this.Text.Substring(startIndex, count);
            return Font.MeasureString(subText).Width;
        }

        public override void ApplyStyle(Components.UIStyle style)
        {
            Components.TextStyle textTemplate = style as Components.TextStyle;
            
            if (textTemplate == null)
                throw new ArgumentException("Template is not of type TextContentTemplate", "textTemplate");

            if (textTemplate.Text != null)
                this.Text = textTemplate.Text;

            if (textTemplate.Font != null)
                this.Font = textTemplate.Font;

            this.FontColor = textTemplate.FontColor;

            base.ApplyStyle(style);
        }
    }   
}
