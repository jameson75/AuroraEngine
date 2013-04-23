using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public abstract class UIControlTemplate
    {
       public DrawingSizeF? Size { get; set; }    
    }

    public class ListControlItemTemplate : UIControlTemplate
    {
        public ListControlItemTemplate()
        { }

        public ListControlItemTemplate(UIControlTemplate childTemplate)
        {
            ChildTemplate = childTemplate;
        }

        UIControlTemplate ChildTemplate { get; set; }
    }

    public class LabelTemplate : UIControlTemplate
    {
        public LabelTemplate()
        { }

        public LabelTemplate(ColorStyle backgroundColor, TextStyle text)
        {
            BackgroundColor = backgroundColor;
            Text = text;
        }

        public LabelTemplate(string caption, SpriteFont font, Color4? fontColor, Color4? backgroundColor)
        {
            if (caption != null || font != null || fontColor != null)
                Text = new TextStyle() { Text = caption, Font = font, FontColor = fontColor.HasValue ? fontColor.Value : Color.Transparent };
        
            if (backgroundColor != null)
                BackgroundColor = new ColorStyle() { Color = backgroundColor.Value };
        }

        public ColorStyle BackgroundColor{ get; set; }
        public TextStyle Text { get; set; }
    }

   // public abstract class UIContentTemplate
   // { }

    //public class ColorContentTemplate : UIContentTemplate
    //{
    //    public ColorContentTemplate()
    //    {}
    //    public ColorContentTemplate(Color4? color)
    //    {            
    //        Color = color;
    //    }
    //    public Color4? Color { get; set; }
    //}

    //public class TextContentTemplate : UIContentTemplate
    //{
    //    public TextContentTemplate()
    //    {}
    //    public TextContentTemplate(string text, SpriteFont font, Color4? fontColor )
    //    {
    //        Text = text;
    //        Font = font;
    //        FontColor = fontColor;
    //    }
    //    public string Text { get; set; }
    //    public SpriteFont Font { get; set; }
    //    public Color4? FontColor { get; set; }
    //}

    public class ButtonTemplate : UIControlTemplate
    {
        public UIStyle ForegroundStyle { get; set; }
        public UIStyle BackgroundStyle { get; set; }

        public ButtonTemplate()
        { }

        public ButtonTemplate(string text, SpriteFont font, Color4? fontColor, Color4? bgColor)
        {
            if ( text != null || font != null || fontColor != null )
                ForegroundStyle = new TextStyle() { Text = text, Font = font, FontColor = fontColor.HasValue ? Color.Transparent : fontColor };

            if (bgColor != null)
                BackgroundStyle = new ColorStyle() { Color = bgColor.Value };
        }
    }

    public class CheckBoxTemplate
}
