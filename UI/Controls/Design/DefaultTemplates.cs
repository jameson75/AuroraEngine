using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;


namespace CipherPark.AngelJacket.Core.UI.Components
{
    public static class DefaultTemplates
    {
        private static ButtonTemplate _button = null;

        public static ButtonTemplate Button
        {
            get
            {
                if (_button == null)
                {
                    _button = new ButtonTemplate(null, ControlFont, ControlFontColor, ButtonColor);
                    _button.Size = new DrawingSizeF(30, 10);
                    return _button;
                }
            }                
        }

        public static CheckBoxTemplate CheckBox
        {

        }
    }
}
