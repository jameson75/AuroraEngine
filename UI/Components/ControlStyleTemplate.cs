using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using CipherPark.AngelJacket.Core.Utils.Interop;

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public abstract class UIControlTemplate
    {
        public UIControlTemplate[] ChildrenTemplates { get; set; }
    }

    public class ListControlItemTemplate : UIControlTemplate
    {
        public ListControlItemTemplate()
        { }

        public ListControlItemTemplate(UIControlTemplate[] childStyleTemplates)
        {
            ChildrenTemplates = childStyleTemplates;
        }       
    }

    public class LabelTemplate : UIControlTemplate
    {
        public LabelTemplate()
        { }

        public LabelTemplate(ColorContentTemplate backgroundColor, TextContentTemplate text)
        {
            BackgroundColor = backgroundColor;
            Text = text;
        }

        public LabelTemplate(string caption, SpriteFont font, Color4? fontColor, Color4? backgroundColor)
        {
            if (caption != null || font != null || fontColor != null)
                Text = new TextContentTemplate(caption, font, fontColor);
            if (backgroundColor != null)
                BackgroundColor = new ColorContentTemplate(backgroundColor);
        }

        public ColorContentTemplate BackgroundColor{ get; set; }
        public TextContentTemplate Text { get; set; }
    }

    public abstract class UIContentTemplate
    { }

    public class ColorContentTemplate : UIContentTemplate
    {
        public ColorContentTemplate()
        {}
        public ColorContentTemplate(Color4? color)
        {            
            Color = color;
        }
        public Color4? Color { get; set; }
    }

    public class TextContentTemplate : UIContentTemplate
    {
        public TextContentTemplate()
        {}
        public TextContentTemplate(string text, SpriteFont font, Color4? fontColor )
        {
            Text = text;
            Font = font;
            FontColor = fontColor;
        }
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public Color4? FontColor { get; set; }
    }
}
