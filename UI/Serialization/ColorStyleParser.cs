using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.UI.Components;
using System.Xml.Linq;
using CipherPark.AngelJacket.Core.Module;

namespace CipherPark.AngelJacket.Core.UI.Design
{
    public class ColorStyleParser : UIStyleParser
    {
        public const string ColorAttributeName = "Color";

        public override void Parse(UITree tree, XElement element, UIStyle style)
        {
            if( style is ColorStyle == false )
                throw new ArgumentException("Style is not of type ColorStyle", "style");

            if (element.Attribute(ColorAttributeName) != null)
                ((ColorStyle)style).Color = UIControlPropertyParser.ParseColor(element.Attribute(ColorAttributeName).Value);

            base.Parse(tree, element, style);
        }

        public override UIStyle CreateStyle(IGameApp game)
        {
            return new ColorStyle(game);
        }
    }
}
