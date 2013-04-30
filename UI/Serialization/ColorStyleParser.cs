using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.UI.Components;
using System.Xml.Linq;
using CipherPark.AngelJacket.Core.Module;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

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

        public override UIStyle CreateStyle()
        {
            return new ColorStyle();
        }
    }
}
