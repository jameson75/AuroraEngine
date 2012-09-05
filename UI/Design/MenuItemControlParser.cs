using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.UI.Components;

namespace CipherPark.AngelJacket.Core.UI.Design
{
    public class MenuItemControlParser : ItemControlParser
    {
        public override void Parse(UITree tree, XElement element, UIControl control)
        {            
            base.Parse(tree, element, control);
        }

        public override Controls.UIControl CreateControl(IUIRoot visualRoot)
        {
            return new MenuItem(visualRoot);
        }

        
    }
}
