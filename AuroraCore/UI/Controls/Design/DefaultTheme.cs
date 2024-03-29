﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Content;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

namespace CipherPark.AngelJacket.Core.UI.Components
{  
    public class DefaultTheme : IUITheme
    {
        private static DefaultTheme _instance = null;
        private static readonly object _instanceLock = new object();

        private ButtonTemplate _button = null;
        private CheckBoxTemplate _checkbox = null;
        private ContentControlTemplate _contentControl = null;
        //private ImageControlTemplate _imageControl = null;
        //private LabelTemplate _label = null;
        //private DropListTemplate _dropList = null;
        private TextBoxTemplate _textBox = null;
        private SliderTemplate _slider = null;
        private ListControlItemTemplate _listControlItem = null;
        private ListControlTemplate _listControl = null;
        private MenuTemplate _menu = null;
        private MenuItemTemplate _menuItem = null;
        private bool isInitialized = false;

        /// <summary>
        /// Background color for all applicable control templates in this theme.
        /// </summary>
        public Color ControlBackgroundColor { get; private set; }
        /// <summary>
        /// Font for all control applicable templates in this theme.
        /// </summary>
        public SpriteFont ControlFont { get; private set; }
        /// <summary>
        /// Font color for all applicable control templates in this theme.
        /// </summary>        
        public Color ControlFontColor { get; private set; }
        /// <summary>
        /// "Selected Item" font color for all applicable control templates in this theme.
        /// </summary>
        public Color ControlFontSelectedColor { get; private set; }
        /// <summary>
        /// Foreground color for all applicable control templates in this theme.
        /// </summary>
        public Color ControlForegroundColor { get; private set; }
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
        public SpriteFont EditorFont { get; private set; }
        /// <summary>
        /// The font color for all editor control templates in this theme.
        /// </summary>
        public Color EditorFontColor { get; private set; }
        /// <summary>
        /// The background color for all editor control templates in this theme.
        /// </summary>
        public Color EditorColor { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private DefaultTheme()
        { }

        public ButtonTemplate Button
        {
            get
            {
                if (!isInitialized)
                    throw new InvalidOperationException("Theme not initialized.");

                if (_button == null)
                {
                    _button = new ButtonTemplate(null, ControlFont, ControlFontColor, ControlForegroundColor);
                    _button.Size = new Size2F(30, 10);
                }
                return _button;                
            }                
        }

        public CheckBoxTemplate CheckBox
        {
            get
            {
                if (!isInitialized)
                    throw new InvalidOperationException("Theme not initialized.");

                if (_checkbox == null)
                {
                    _checkbox = new CheckBoxTemplate()
                    {
                        CaptionTemplate = new ContentControlTemplate() { ContentStyle = new TextStyle() { Font = ControlFont, FontColor = ControlFontColor } }, 
                        CheckContentTemplate = new ContentControlTemplate() { ContentStyle = new ImageStyle() { Texture = DefaultCheckTexture } },
                        UncheckContentTemplate = new ContentControlTemplate() { ContentStyle = new ImageStyle() { Texture = DefaultUncheckTexture } },
                        Size = new Size2F(30, 10)
                    };       
                }
                return _checkbox;
            }
        }

        public ContentControlTemplate ContentControl
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

        /*
        public ImageControlTemplate ImageControl
        {
            get
            {
                if (!isInitialized)
                    throw new InvalidOperationException("Theme not initialized.");

                if (_imageControl == null)
                {
                    _imageControl = new ImageControlTemplate(NoImageTexture);
                    _imageControl.Size = new Size2F(20, 20);
                }
                return _imageControl;
            }
        }
        */

        /*
        public LabelTemplate Label
        {
            get
            {
                if (!isInitialized)
                    throw new InvalidOperationException("Theme not initialized.");

                if (_label == null)
                {
                    _label = new LabelTemplate(null, ControlFont, ControlFontColor, null);
                    _label.Size = new Size2F(30, 10);
                }
                return _label;
            }
        }
        */

        //public DropListTemplate DropList
        //{
        //    get
        //    {
        //        if (_dropList == null)
        //        {
        //            _dropList = new DropListTemplate()
        //            {
        //                DropDownButton = new ButtonTemplate(DropDownImageTexture),
        //                TextBox = new TextBoxTemplate(null, EditorFont, EditorFontColor, EditorColor),
        //                ListControl = new ListControlTemplate(ControlColor)
        //            };
        //            _dropList.Size = new Size2F(50, 20);
        //        }
        //        return _dropList;
        //    }
        //}

        public TextBoxTemplate TextBox
        {
            get 
            {
                if (!isInitialized)
                    throw new InvalidOperationException("Theme not initialized.");

                if(_textBox == null)
                {
                    _textBox = new TextBoxTemplate(null, EditorFont, EditorFontColor, EditorColor)
                    {
                       Size = new Size2F(40, 20)
                    };                    
                }
                return _textBox;
            }
        }

        public SliderTemplate Slider
        {
            get
            {
                if (!isInitialized)
                    throw new InvalidOperationException("Theme not initialized.");

                if (_slider == null)
                {
                    _slider = new SliderTemplate()
                    {
                        TrackContent = new ContentControlTemplate() 
                        { 
                            ContentStyle = new ColorStyle(Color.Gray),
                            Size = new Size2F(50, 20)                                    
                        },
                        HandleContent = new ContentControlTemplate()
                        { 
                            ContentStyle = new ColorStyle(Color.LightGray),
                            Size = new Size2F(20, 20)
                        },
                        Size = new Size2F(50, 20)
                    };
                    
                }
                return _slider;
            }
        }

        public ListControlItemTemplate ListControlItem
        {
            get
            {
                if (!isInitialized)
                    throw new InvalidOperationException("Theme not initialized.");

                if (_listControlItem == null)
                {
                    _listControlItem = new ListControlItemTemplate()
                    {
                        Content = ContentControlTemplate.CreateLabelTemplate(null, ControlFont, ControlFontColor, null),
                        ItemTemplate = ContentControlTemplate.CreateLabelTemplate(null, ControlFont, ControlFontColor, null),
                        SelectTemplate = ContentControlTemplate.CreateLabelTemplate(null, ControlFont, ControlFontSelectedColor, null)
                    };
                    _listControlItem.Size = new Size2F(30, 20);
                }
                return _listControlItem;
            }
        }

        public ListControlTemplate ListControl
        {
            get
            {
                if (!isInitialized)
                    throw new InvalidOperationException("Theme not initialized.");

                if (_listControl == null)
                {
                    _listControl = new ListControlTemplate(ControlBackgroundColor)
                    {
                    };
                    _listControl.Size = new Size2F(30, 20);
                }
                return _listControl;
            }
        }

        public MenuTemplate Menu
        {
            get
            {
                if (!isInitialized)
                    throw new InvalidOperationException("Theme not initialized.");

                if (_menu == null)
                {
                    _menu = new MenuTemplate();                    
                    {
                    };
                    _menu.Size = new Size2F(30, 20);
                }
                return _menu;
            }
        }

        public MenuItemTemplate MenuItem
        {
            get
            {
                if (!isInitialized)
                    throw new InvalidOperationException("Theme not initialized.");

                if (_menuItem == null)
                {
                    _menuItem = new MenuItemTemplate();
                    {
                    };
                    _menuItem.Size = new Size2F(30, 20);
                }
                return _menuItem;
            }
        }

        /// <summary>
        /// Returns an instance of the DefaultTheme singleton.
        /// </summary>
        /// <remarks>
        /// The DefaultTheme instance is created on the first call.
        /// </remarks>
        /// <param name="graphicsDevice"></param>
        /// <returns>An instance of the DefaultTheme singleton.</returns>
        public static void Initialize(Device graphicsDevice)
        {
            lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new DefaultTheme();
                        _instance.ControlBackgroundColor = Color.DarkGray;
                        _instance.ControlFont = ContentImporter.LoadFont(graphicsDevice, @"Content\UI\DefaultTheme\ControlFont10.font");
                        _instance.ControlFontColor = Color.White;
                        _instance.ControlFontSelectedColor = Color.Orange;
                        _instance.ControlForegroundColor = Color.Gray;
                        _instance.DefaultCheckTexture = ContentImporter.LoadTexture(graphicsDevice.ImmediateContext, @"Contnet\UI\DefaultTheme\Check.png");
                        _instance.DefaultUncheckTexture = ContentImporter.LoadTexture(graphicsDevice.ImmediateContext, @"Content\UI\DefaultTheme\Uncheck.png");
                        _instance.EditorColor = Color.White;
                        _instance.EditorFont = ContentImporter.LoadFont(graphicsDevice, @"Content\UI\DefaultTheme\EditorFont10.font");
                        _instance.EditorFontColor = Color.DarkGray;
                        _instance.isInitialized = true;
                    }
                }
        }

        public static DefaultTheme Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("Default theme was not initialized");

                return _instance;
            }
        }
    }
}
