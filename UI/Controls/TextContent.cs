using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.Utils;
using SharpDX;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class TextContent : ColorContent
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

        public TextContent(string text, SpriteFont font, Color4 fontColor, Color4 bgColor)
            : base(bgColor)
        {
            Text = text;
            Font = font;
            FontColor = fontColor;
            VAlignment = VerticalAlignment.Top;
            HAlignment = HorizontalAlignment.Left;
        }

        public string Text { get; set; }
        
        public SpriteFont Font { get; set; }
        
        public Color FontColor { get; set; }

        public VerticalAlignment VAlignment { get; set; }

        public HorizontalAlignment HAlignment { get; set; }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            
            if (Container == null)
                throw new InvalidOperationException("No container for this content was specified.");          
            Container.ControlSpriteBatch.Begin();
            Vector2 contentSurfacePosition = Container.PositionToSurface(Container.Position);
            Container.ControlSpriteBatch.DrawString(Font, Text == null ? string.Empty : Text, contentSurfacePosition, FontColor);
            Container.ControlSpriteBatch.End();                        
        }

        public override Rectangle CalculateSmallestBoundingRect()
        {
            if (Font == null || string.IsNullOrEmpty(Text))
                return Rectangle.Empty;
            else
            {
                Vector2 textSize = Font.MeasureString(Text);
                return new Rectangle(0, 0, (int)Math.Ceiling(textSize.X),(int) Math.Ceiling(textSize.Y));
            }
        }
    }   
}
