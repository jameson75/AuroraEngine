using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Toolkit
{
    public class DirectText
    {
        public DirectText(
            string text,
            TextFormat textFormat,
            RectangleF layoutRect,
            SolidColorBrush brush)
        {
            Text = text;
            TextFormat = textFormat;
            LayoutRect = layoutRect;
            Brush = brush;
        }

        public DirectText(Vector2 layoutPosition, TextLayout textLayout, SolidColorBrush brush)
        {
            LayoutPosition = layoutPosition;
            TextLayout = textLayout;
            Brush = brush;
        }

        public string Text { get; }
        public TextFormat TextFormat { get; }
        public RectangleF LayoutRect { get; }
        public TextLayout TextLayout { get; }
        public SolidColorBrush Brush { get; }
        public Vector2 LayoutPosition { get; }
    }
}
