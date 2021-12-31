using System;
using System.Collections.Generic;
using CipherPark.KillScript.Core.Utils;
using CipherPark.KillScript.Core.Utils.Toolkit;
using SharpDX;

namespace CipherPark.KillScript.Core.UI.Controls
{
    public class TextContent : ColorContent
    {
        public TextContent()
        {
            VAlignment = VerticalAlignment.Top;
            HAlignment = HorizontalAlignment.Left;
        }

        public TextContent(string text, SpriteFont font, Color4 fontColor, Color4? color = null)
        {
            Text = text;
            Font = font;
            FontColor = fontColor;
            VAlignment = VerticalAlignment.Top;
            HAlignment = HorizontalAlignment.Left;
            Color = color.GetValueOrDefault();
        }

        public string Text { get; set; }

        public string Format { get; set; }
        
        public SpriteFont Font { get; set; }
        
        public Color4 FontColor { get; set; }

        public VerticalAlignment VAlignment { get; set; }

        public HorizontalAlignment HAlignment { get; set; }

        public override void Draw()
        {                       
            if (Container == null)
                throw new InvalidOperationException("No container for this content was specified.");
            
            base.Draw(); 
           
            Vector2 contentSurfacePosition = Container.PositionToSurface(Container.Position);

            this.BeginDraw();
            string outputString = string.IsNullOrEmpty(Text) ? string.Empty : string.IsNullOrEmpty(Format) ? Text : string.Format(Format, Text);            
            Container.ControlSpriteBatch.DrawString(Font, outputString, contentSurfacePosition.ToVector2(), FontColor);               
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
            Components.TextStyle textStyle = style as Components.TextStyle;
            
            if (textStyle == null)
                throw new ArgumentException("Style is not of type TextStyle", "textStyle");           

            if (textStyle.Font != null)
                this.Font = textStyle.Font;

            if (textStyle.FontColor != null)
                this.FontColor = textStyle.FontColor.Value;

            if (textStyle.Color != null)
                this.Color = textStyle.Color.Value;

            base.ApplyStyle(style);
        }
    }   
}
