using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Interop;
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
        
        public SpriteFont Font { get; set; }
        
        public Color4 FontColor { get; set; }

        public VerticalAlignment VAlignment { get; set; }

        public HorizontalAlignment HAlignment { get; set; }

        public override void Draw(long gameTime)
        {
            base.Draw(gameTime);            
            if (Container == null)
                throw new InvalidOperationException("No container for this content was specified.");
            
            DrawingPointF contentSurfacePosition = Container.PositionToSurface(Container.Position);
            if (!HasDrawParameters)
                Container.ControlSpriteBatch.Begin();
            else
                Container.ControlSpriteBatch.Begin(SpriteSortMode == null ? CipherPark.AngelJacket.Core.Utils.Interop.SpriteSortMode.Deferred : SpriteSortMode.Value, BlendState, SamplerState, DepthStencilState, RasterizerState, CustomShaderCallback, TransformationMatrix);           
            Container.ControlSpriteBatch.DrawString(Font, Text == null ? string.Empty : Text, contentSurfacePosition.ToVector2(), FontColor);
            Container.ControlSpriteBatch.End();                        
        }

        public override Rectangle CalculateSmallestBoundingRect()
        {
            if (Font == null || string.IsNullOrEmpty(Text))
                return Rectangle.Empty;
            else
            {
                DrawingSizeF textSize = Font.MeasureString(Text);
                return RectangleExtension.CreateLTWH(0, 0, (int)Math.Ceiling(textSize.Width),(int) Math.Ceiling(textSize.Height));
            }
        }

        public float GetTextLength(int startIndex, int count)
        {
            string subText = this.Text.Substring(startIndex, count);
            return Font.MeasureString(subText).Width;
        }

        public override void ApplyTemplate(Components.UIContentTemplate template)
        {
            Components.TextContentTemplate textTemplate = template as Components.TextContentTemplate;
            if (textTemplate == null)
                throw new ArgumentException("Template is not of type TextContentTemplate", "textTemplate");

            if (textTemplate.Text != null)
                this.Text = textTemplate.Text;

            if (textTemplate.Font != null)
                this.Font = textTemplate.Font;

            if (textTemplate.FontColor != null)
                this.FontColor = textTemplate.FontColor.Value;

            base.ApplyTemplate(template);
        }
    }   
}
