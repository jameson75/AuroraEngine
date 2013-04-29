using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

namespace CipherPark.AngelJacket.Core.UI.Components
{  
    public class DefaultTheme : IUITheme
    {
        private ButtonTemplate _button = null;
        private CheckBoxTemplate _checkbox = null;
        private ContentControlTemplate _contentControl = null;
        private ImageControlTemplate _imageControl = null;
        private LabelTemplate _label = null;

        public ButtonTemplate Button
        {
            get
            {
                if (_button == null)
                {
                    _button = new ButtonTemplate(null, DefaultControlFont, DefaultControlFontColor, DefaultButtonColor);
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
                    _checkbox = new CheckBoxTemplate(DefaultCheckTexture, DefaultUncheckTexture);
                    _checkbox.Size = new DrawingSizeF(30, 10);
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
                    _imageControl = new ImageControlTemplate(DefaultImage);
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
                    _label = new LabelTemplate(null, DefaultControlFont, DefaultControlFontColor, null);
                    _label.Size = new DrawingSizeF(30, 10);
                }
                return _label;
            }
        }                    
    }
}
