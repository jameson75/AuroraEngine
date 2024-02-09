using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Toolkit;
using CipherPark.Aurora.Core;
using CipherPark.Aurora.Core.UI.Controls;
using CipherPark.Aurora.Core.UI.Components;

namespace Aurora.Sample.Editor
{
    public class SampleGameAppTheme : IUITheme
    {
        private ButtonTemplate _button = null;        
        private ContentControlTemplate _contentControl = null;
        private bool isInitialized = false;

        /// <summary>
        /// Background color for all applicable control templates in this theme.
        /// </summary>
        public Color DefaultButtonColor { get; private set; }
        /// <summary>
        /// Font for all control applicable templates in this theme.
        /// </summary>
        public SpriteFont DefaultFont { get; private set; }
        /// <summary>
        /// Font color for all applicable control templates in this theme.
        /// </summary>        
        public Color DefaultFontColor { get; private set; }
        /// <summary>
        /// "Selected Item" font color for all applicable control templates in this theme.
        /// </summary>
        public Color DefaultFontSelectedColor { get; private set; }
        /// <summary>
        /// Foreground color for all applicable control templates in this theme.
        /// </summary>
        public Color DefaultContentColor { get; private set; }
        /// <summary>
        /// The "checked" image for all applicable control templates in this theme.
        /// </summary>
        public Texture2D DefaultCheckTexture { get; private set; }
        /// <summary>
        /// The "unchecked" image for all applicable control templates in this theme.
        /// </summary>
        public Texture2D DefaultUncheckTexture { get; private set; }
        /// <summary>
        /// The default background image for all applicable control templates in this theme.
        /// </summary>
        ///
        /// <remarks>
        /// Typically used by the image control, when no texture is specified.
        /// </remarks>
        public Texture2D NoImageTexture { get; private set; }
        /// <summary>
        /// The "checked" image for all applicable control templates in this theme.
        /// </summary>
        public Texture2D DropDownImageTexture { get; private set; }
        /// <summary>
        /// The font for all editor control templates in this theme.
        /// </summary>
        public SpriteFont DefaultEditorFont { get; private set; }
        /// <summary>
        /// The font color for all editor control templates in this theme.
        /// </summary>
        public Color DefaultEditorFontColor { get; private set; }
        /// <summary>
        /// The background color for all editor control templates in this theme.
        /// </summary>
        public Color DefaultEditorColor { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public ButtonTemplate ButtonTemplate
        {
            get
            {
                if (!isInitialized)
                    throw new InvalidOperationException("Theme not initialized.");

                if (_button == null)
                {
                    _button = new ButtonTemplate(DefaultFont, DefaultFontColor, DefaultButtonColor);
                    _button.Size = new Size2F(30, 10);
                }
                return _button;
            }
        }

        public ContentControlTemplate ContentControlTemplate
        {
            get
            {
                if (!isInitialized)
                    throw new InvalidOperationException("Theme not initialized.");

                if (_contentControl == null)
                {
                    _contentControl = new ContentControlTemplate();
                    _contentControl.Size = new Size2F(20, 20);
                }
                return _contentControl;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public SampleGameAppTheme(IGameApp gameApp)
        {
            Initialize(gameApp.GraphicsDevice);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visualRoot"></param>
        public void Apply(UITree visualRoot)
        {
            Apply(visualRoot.Controls);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controls">Top-level controls</param>
        public void Apply(IEnumerable<UIControl> controls)
        { 
            Stack<UIControl> controlStack = new Stack<UIControl>();
            
            //Initialize stack with top level controls.
            foreach (var control in controls)            
                controlStack.Push(control);
            
            //Apply theme in DFS order.
            while(controls.Any())
            {
                var control = controlStack.Pop();
                Apply(control);
                foreach (var child in control.Children)
                    controlStack.Push(child);
            }
        }

        public void Apply(UIControl control)
        {
            //Because a button control is essentially a content control with events, we apply the same styling heuristics
            //to both content and button controls.
            if (control is ContentControl || control is Button)
            {
                var contentControl = (ContentControl)control;

                ContentControlTemplate template = control is Button ? ButtonTemplate : ContentControlTemplate;
                
                //If the size of the control is not set, and the template specifies a style,
                //set it.
                if (contentControl.Size == Size2F.Zero && template.Size != null)
                    contentControl.Size = template.Size.Value;                   

                //If a style of the ContentControl's content is specified, apply it.
                if (template.ContentStyle != null)
                {
                    //If the control as no content, create one from the style.
                    if (contentControl.Content == null)
                        contentControl.Content = template.ContentStyle.GenerateContent();

                    //Otherwise, update the properties of the content which were not already set.
                    else if ((template.ContentStyle is TextStyle && contentControl.Content is TextContent) ||
                             (template.ContentStyle is ColorStyle && contentControl.Content is ColorContent) ||
                              template.ContentStyle is ImageStyle && contentControl.Content is ImageContent)
                        contentControl.Content.ApplyStyle(template.ContentStyle);                   
                    else
                        ApplyCustomContentStyle(contentControl, template.ContentStyle);                    
                }

                //Otherwise, if the ContentControl has a content, update the properties of the content
                //which were not already set to the theme's default style.
                else if(contentControl.Content != null)
                {
                    if(contentControl.Content is TextContent)
                    { 
                        TextStyle textStyle = new TextStyle(DefaultFont, DefaultFontColor, DefaultContentColor);
                        contentControl.Content.ApplyStyle(textStyle);
                    }
                    else if(contentControl.Content is ColorContent)
                    {
                        ColorStyle colorStyle = new ColorStyle(DefaultContentColor);
                        contentControl.Content.ApplyStyle(colorStyle);
                    }
                    else if(contentControl.Content is ImageContent)
                    {
                        ImageStyle imageStyle = new ImageStyle(NoImageTexture);
                        contentControl.Content.ApplyStyle(imageStyle);
                    }
                    else                   
                        ApplyCustomContentStyle(contentControl, contentControl.Content);
                }                
            }
            
            else      
                 ApplyCustom(control); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public virtual void ApplyCustom(UIControl control)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentControl"></param>
        public virtual void ApplyCustomContentStyle(UIControl control, UIContent content)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentControl"></param>
        public virtual void ApplyCustomContentStyle(UIControl control, UIStyle content)
        { }

        /// <summary>
        /// Returns an instance of the DefaultTheme singleton.
        /// </summary>
        /// <remarks>
        /// The DefaultTheme instance is created on the first call.
        /// </remarks>
        /// <param name="graphicsDevice"></param>
        /// <returns>An instance of the DefaultTheme singleton.</returns>
        private void Initialize(Device graphicsDevice)
        {
            DefaultButtonColor = Color.Gray;
            DefaultFont = ContentImporter.LoadFont(graphicsDevice, @"Assets\UI\DefaultTheme\ControlFont10.font");
            DefaultFontColor = Color.White;
            DefaultFontSelectedColor = Color.Orange;
            DefaultContentColor = Color.Transparent;
            DefaultCheckTexture = ContentImporter.LoadTexture(graphicsDevice.ImmediateContext, @"Assets\UI\DefaultTheme\Check.png");
            DefaultUncheckTexture = ContentImporter.LoadTexture(graphicsDevice.ImmediateContext, @"Assets\UI\DefaultTheme\Uncheck.png");
            DefaultEditorColor = Color.White;
            DefaultEditorFont = ContentImporter.LoadFont(graphicsDevice, @"Assets\UI\DefaultTheme\EditorFont10.font");
            DefaultEditorFontColor = Color.DarkGray;
            isInitialized = true;
        }        
    }
}
