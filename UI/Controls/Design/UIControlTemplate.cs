﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.KillScript.Core.Utils.Toolkit;
using CipherPark.KillScript.Core.UI.Controls;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.UI.Components
{
    public abstract class UIControlTemplate
    {
        public Size2F? Size { get; set; }
        public VerticalAlignment? VerticalAlignment { get; set; }
        public HorizontalAlignment? HorizontalAlignment { get; set; }
        public abstract UIControl CreateControl(IUIRoot visualRoot);
    }

    public class ListControlItemTemplate : UIControlTemplate
    {       
        public ListControlItemTemplate()
        { }
        public UIControlTemplate Content { get; set; }
        public UIControlTemplate ItemTemplate { get; set; }
        public UIControlTemplate SelectTemplate { get; set; }
        public override UIControl CreateControl(IUIRoot visualRoot)
        {
            return ListControlItem.FromTemplate(visualRoot, this);
        }
    }

    /*
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

        public override UIControl CreateControl(IUIRoot visualRoot)
        {
            return Label.FromTemplate(visualRoot, this);
        }
    }
    */

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

    public class ButtonTemplate : ContentControlTemplate
    {
        public ButtonTemplate()
        { }

        public ButtonTemplate(SpriteFont font, Color4 fontColor, Color4? bgColor)
        {
            if (font != null || fontColor != null )
                ContentStyle = new TextStyle() { Font = font, FontColor = fontColor, Color = bgColor };            
        }

        public ButtonTemplate(Texture2D image)
        {
            if (image != null)
                ContentStyle = new ImageStyle() { Texture = image };
        }
        
        public override UIControl CreateControl(IUIRoot visualRoot)
        {
            return Button.FromTemplate(visualRoot, this);
        }
        
    }

    public class CheckBoxTemplate : UIControlTemplate
    {
        public ContentControlTemplate CaptionTemplate { get; set; }
        public ContentControlTemplate CheckContentTemplate { get; set; }
        public ContentControlTemplate UncheckContentTemplate { get; set; }

        public CheckBoxTemplate()
        { }

        public CheckBoxTemplate(string caption, SpriteFont font, Color? fontColor, Texture2D checkTexture, Texture2D uncheckTexture)
        {
            if (caption != null || font != null || fontColor != null)
            {
                CaptionTemplate = ContentControlTemplate.CreateLabelTemplate(caption, font, fontColor.Value);
                if (caption != null && font != null)
                    CaptionTemplate.Size = font.MeasureString(caption);
            }
               
            if (checkTexture != null)
            {
                CheckContentTemplate = new ContentControlTemplate() { ContentStyle = new ImageStyle() { Texture = checkTexture } };
                CheckContentTemplate.Size = new Size2F(checkTexture.Description.Width, checkTexture.Description.Height);
            }

            if (uncheckTexture != null)
            {
                UncheckContentTemplate = new ContentControlTemplate() { ContentStyle = new ImageStyle() { Texture = uncheckTexture } };
                UncheckContentTemplate.Size = new Size2F(uncheckTexture.Description.Width, uncheckTexture.Description.Height);
            }
        }

        public override UIControl CreateControl(IUIRoot visualRoot)
        {
            return CheckBox.FromTemplate(visualRoot, this);
        }
    }

    public class ContentControlTemplate : UIControlTemplate
    {
        public UIStyle ContentStyle { get; set; }

        public ContentControlTemplate()
        { }

        public ContentControlTemplate(UIStyle contentStyle)
        {
            ContentStyle = contentStyle;
        }
       
        public override UIControl CreateControl(IUIRoot visualRoot)
        {
            return ContentControl.FromTemplate(visualRoot, this);
        }       

        public static ContentControlTemplate CreateLabelTemplate(string text, SpriteFont font, Color fontColor, Color? backgroundColor = null)
        {
            Color backgroundColor_ = backgroundColor != null ? backgroundColor.Value : Color.Transparent;
            ContentControlTemplate template = new ContentControlTemplate
            {
                ContentStyle = new TextStyle(font, fontColor, backgroundColor_),               
            };
            return template;
        }
    }

    /*
    public class ImageControlTemplate : UIControlTemplate
    {
        public ImageStyle ImageStyle { get; set; }

        public ImageControlTemplate(SharpDX.Direct3D11.Texture2D texture)
        {
            if (texture != null)
                ImageStyle = new ImageStyle() { Texture = texture };
        }

        public override UIControl CreateControl(IUIRoot visualRoot)
        {
            return ImageControl.FromTemplate(visualRoot, this);
        }
    }
   */

    public class SliderTemplate : UIControlTemplate
    {
        public ContentControlTemplate TrackContent { get; set; }
        public ContentControlTemplate HandleContent { get; set; }
        public override UIControl CreateControl(IUIRoot visualRoot)
        {
            return Slider.FromTemplate(visualRoot, this);
        }
    }

    public class SpinnerTemplate : UIControlTemplate
    {
        public override UIControl CreateControl(IUIRoot visualRoot)
        {
            return Spinner.FromTemplate(visualRoot, this);
        }
    }

    public class TextBoxTemplate : UIControlTemplate
    {
        public TextStyle TextStyle { get; set; }
        public ColorStyle EditorStyle { get; set; }

        public TextBoxTemplate(SpriteFont font, Color fontColor, Color? editorColor = null)
        {
            TextStyle = new Components.TextStyle() 
            {                 
                Font = font,
                FontColor = fontColor                
            };

            if (editorColor != null)
                EditorStyle = new ColorStyle(editorColor.Value);
        }

        public override UIControl CreateControl(IUIRoot visualRoot)
        {
            return TextBox.FromTemplate(visualRoot, this);
        }
    }

    //public class DropListTemplate : UIControlTemplate
    //{
    //    public ButtonTemplate DropDownButton { get; set; }
    //    public TextBoxTemplate TextBox { get; set; }
    //    public ListControlTemplate ListControl { get; set; }
    //    public override UIControl CreateControl(IUIRoot visualRoot)
    //    {
    //        return DropList.FromTemplate(visualRoot, this);
    //    }
    //}

    public class ListControlTemplate : UIControlTemplate
    {
        public UIStyle BackgroundStyle { get; set; }

        public ListControlTemplate(Color4 backgroundColor)
        {
            BackgroundStyle = new ColorStyle(backgroundColor);
        }

        public override UIControl CreateControl(IUIRoot visualRoot)
        {
            return ListControl.FromTemplate(visualRoot, this);
        }
    }

    public class MenuTemplate : UIControlTemplate
    {
        public override UIControl CreateControl(IUIRoot visualRoot)
        {
            return Menu.FromTemplate(visualRoot, this);
        }
    }

    public class MenuItemTemplate : UIControlTemplate
    {
        public override UIControl CreateControl(IUIRoot visualRoot)
        {
            return MenuItem.FromTemplate(visualRoot, this);
        }
    }
}
