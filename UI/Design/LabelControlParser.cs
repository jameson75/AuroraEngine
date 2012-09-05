using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.UI.Controls;

namespace CipherPark.AngelJacket.Core.UI.Design
{
    public class LabelControlParser : UIControlParser
    {
        public override Controls.UIControl CreateControl(IUIRoot visualRoot)
        {
            return new Label(visualRoot);
        }
    }
}
