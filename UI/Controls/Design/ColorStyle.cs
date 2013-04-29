using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.Module;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public class ColorStyle : UIStyle
    {
        public ColorStyle()
        {
            Color = SharpDX.Color.Transparent;
        }

        public Color4? Color { get; set; }

        public override UIContent GenerateContent()
        {
            ColorContent content = new ColorContent(Color.Value);
            return content;
        }
    }
}
