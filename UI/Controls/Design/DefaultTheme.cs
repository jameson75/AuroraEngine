using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Utils;
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
        private ImageControlTemplate _imageControl = null;
        private LabelTemplate _label = null;
        //private DropListTemplate _dropList = null;
        private TextBoxTemplate _textBox = null;
        private SliderTemplate _slider = null;
        private ListControlItemTemplate _listControlItem = null;
        private ListControlTemplate _listControl = null;
        private MenuTemplate _menu = null;
        private MenuItemTemplate _menuItem = null;

        private DefaultTheme()
        {
        }

        public static DefaultTheme Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new DefaultTheme();
                }
                return _instance;
            }
        }

        public Color ControlColor { get; set; }
        public SpriteFont ControlFont { get; set; }
        public Color ControlFontColor { get; set; }
        public Color ControlFontHightlightColor { get; set; }
        public Color ButtonColor { get; set; }
        public Texture2D DefaultCheckTexture { get; set; }
        public Texture2D DefaultUncheckTexture { get; set; }
        public Texture2D EmptyImageTexture { get; set; }
        public Texture2D DropDownImageTexture { get; set; }
        public SpriteFont EditorFont { get; set; }
        public Color EditorFontColor { get; set; }
        public Color EditorColor { get; set; }

        public ButtonTemplate Button
        {
            get
            {
                if (_button == null)
                {
                    _button = new ButtonTemplate(null, ControlFont, ControlFontColor, ButtonColor);
                    _button.Size = new DrawingSizeF(30, 10);
                }
                return _button;                
            }                
        }

        public CheckBoxTemplate CheckBox
        {
            get
            {
                if (_checkbox == null)
                {
                    _checkbox = new CheckBoxTemplate()
                    {
                        CaptionTemplate = this.Label,
                        CheckContentTemplate = new ContentControlTemplate() { ContentStyle = new ImageStyle() { Texture = DefaultCheckTexture } },
                        UncheckContentTemplate = new ContentControlTemplate() { ContentStyle = new ImageStyle() { Texture = DefaultUncheckTexture } },
                        Size = new DrawingSizeF(30, 10)
                    };       
                }
                return _checkbox;
            }
        }

        public ContentControlTemplate ContentControl
        {
            get
            {
                if (_contentControl == null)
                {
                    _contentControl = new ContentControlTemplate();
                    _contentControl.Size = new DrawingSizeF(20, 20);
                }
                return _contentControl;
            }
        }

        public ImageControlTemplate ImageControl
        {
            get
            {
                if (_imageControl == null)
                {
                    _imageControl = new ImageControlTemplate(EmptyImageTexture);
                    _imageControl.Size = new DrawingSizeF(20, 20);
                }
                return _imageControl;
            }
        }

        public LabelTemplate Label
        {
            get
            {
                if (_label == null)
                {
                    _label = new LabelTemplate(null, ControlFont, ControlFontColor, null);
                    _label.Size = new DrawingSizeF(30, 10);
                }
                return _label;
            }
        }

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
        //            _dropList.Size = new DrawingSizeF(50, 20);
        //        }
        //        return _dropList;
        //    }
        //}

        public TextBoxTemplate TextBox
        {
            get 
            {
                if(_textBox == null)
                {
                    _textBox = new TextBoxTemplate(null, EditorFont, EditorFontColor, EditorColor)
                    {
                       Size = new DrawingSizeF(40, 20)
                    };                    
                }
                return _textBox;
            }
        }

        public SliderTemplate Slider
        {
            get
            {
                if (_slider == null)
                {
                    _slider = new SliderTemplate()
                    {
                        TrackContent = new ContentControlTemplate() 
                        { 
                            ContentStyle = new ColorStyle(Color.Gray),
                            Size = new DrawingSizeF(50, 20)                                    
                        },
                        HandleContent = new ContentControlTemplate()
                        { 
                            ContentStyle = new ColorStyle(Color.LightGray),
                            Size = new DrawingSizeF(20, 20)
                        },
                        Size = new DrawingSizeF(50, 20)
                    };
                    
                }
                return _slider;
            }
        }

        public ListControlItemTemplate ListControlItem
        {
            get
            {
                if (_listControlItem == null)
                {
                    _listControlItem = new ListControlItemTemplate()
                    {
                        Content = new LabelTemplate(null, ControlFont, ControlFontColor, null),
                        ItemTemplate = new LabelTemplate(null, ControlFont, ControlFontColor, null),
                        SelectTemplate = new LabelTemplate(null, ControlFont, ControlFontHightlightColor, null)
                    };
                    _listControlItem.Size = new DrawingSizeF(30, 20);
                }
                return _listControlItem;
            }
        }

        public ListControlTemplate ListControl
        {
            get
            {
                if (_listControl == null)
                {
                    _listControl = new ListControlTemplate(ControlColor)
                    {
                    };
                    _listControl.Size = new DrawingSizeF(30, 20);
                }
                return _listControl;
            }
        }

        public MenuTemplate Menu
        {
            get
            {
                if (_menu == null)
                {
                    _menu = new MenuTemplate();                    {
                    };
                    _menu.Size = new DrawingSizeF(30, 20);
                }
                return _menu;
            }
        }

        public MenuItemTemplate MenuItem
        {
            get
            {
                if (_menuItem == null)
                {
                    _menuItem = new MenuItemTemplate();
                    {
                    };
                    _menuItem.Size = new DrawingSizeF(30, 20);
                }
                return _menuItem;
            }
        }
    }
}
