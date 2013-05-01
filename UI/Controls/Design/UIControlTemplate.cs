using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using CipherPark.AngelJacket.Core.UI.Controls;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public abstract class UIControlTemplate
    {
        public DrawingSizeF? Size { get; set; }    
        //**************************************************************************************
        //TODO: Implement so that controls with abstract children can create themselves from 
        //a theme... (ie: see ListControlItem() constructor).
        //****************************************************************************************
        //public abstract UIControl CreateControl(IUIRoot visualRoot);
    }

    public class ListControlItemTemplate : UIControlTemplate
    {       
        public ListControlItemTemplate()
        { }
        public UIControlTemplate Content { get; set; }
        public UIControlTemplate ItemTemplate { get; set; }
        public UIControlTemplate SelectTemplate { get; set; }
    }

    public class LabelTemplate : UIControlTemplate
    {
        public LabelTemplate()
        { }

        public LabelTemplate(ColorStyle backgroundColor, TextStyle text)
        {
            BackgroundColor = backgroundColor;
            CaptionStyle = text;
        }

        public LabelTemplate(string caption, SpriteFont font, Color4? fontColor, Color4? backgroundColor)
        {
            if (caption != null || font != null || fontColor != null)
                CaptionStyle = new TextStyle() { Text = caption, Font = font, FontColor = fontColor.HasValue ? fontColor.Value : Color.Transparent };
        
            if (backgroundColor != null)
                BackgroundColor = new ColorStyle() { Color = backgroundColor.Value };
        }

        public ColorStyle BackgroundColor{ get; set; }
        public TextStyle CaptionStyle { get; set; }
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

        public ButtonTemplate(Texture2D image)
        {
            if (image != null)
                ForegroundStyle = new ImageStyle() { Texture = image };
        }
    }

    public class CheckBoxTemplate : UIControlTemplate
    {
        public ContentControlTemplate CheckContentTemplate { get; set; }
        public ContentControlTemplate UncheckContentTemplate { get; set; }

        public CheckBoxTemplate()
        { }

        public CheckBoxTemplate(Texture2D checkTexture, Texture2D uncheckTexture)
        {
            if (checkTexture != null)
                CheckContentTemplate = new ContentControlTemplate() { ContentStyle = new ImageStyle() { Texture = checkTexture } };

            if (uncheckTexture != null)
                UncheckContentTemplate = new ContentControlTemplate() { ContentStyle = new ImageStyle() { Texture = uncheckTexture } };
        }
    }

    public class ContentControlTemplate : UIControlTemplate
    {
        public UIStyle ContentStyle { get; set; }
    }

    public class ImageControlTemplate : UIControlTemplate
    {
        public ImageStyle ImageStyle { get; set; }

        public ImageControlTemplate(SharpDX.Direct3D11.Texture2D texture)
        {
            if (texture != null)
                ImageStyle = new ImageStyle() { Texture = texture };
        }
    }

    public class PanelControlTemplate : UIControlTemplate
    {

    }

    public class SliderTemplate : UIControlTemplate
    {
        public ContentControlTemplate TrackContent { get; set; }
        public ContentControlTemplate HandleContent { get; set; }
    }

    public class SpinnerControlTemplate : UIControlTemplate
    {

    }

    public class TextBoxTemplate : UIControlTemplate
    {
        public TextStyle TextStyle { get; set; }

        public TextBoxTemplate(string text, SpriteFont font, Color? fontColor, Color? backgroundColor)
        {
            TextStyle = new Components.TextStyle() 
            { 
                Text = text,
                Font = font,
                FontColor = fontColor,
                Color = backgroundColor
            };
        }
    }

    public class DropListTemplate : UIControlTemplate
    {
        public ButtonTemplate DropDownButton { get; set; }
        public TextBoxTemplate TextBox { get; set; }
        public ListControlTemplate ListControl { get; set; }
    }

    public class ListControlTemplate : UIControlTemplate
    {
        public UIStyle BackgroundStyle { get; set; }

        public ListControlTemplate(Color4 backgroundColor)
        {
            BackgroundStyle = new ColorStyle(backgroundColor);
        }
    }
}
