using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.Aurora.Core.UI.Controls;
using CipherPark.Aurora.Core.Module;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Components
{
    public class ColorStyle : UIStyle
    {
        public ColorStyle()
        {
            Color = SharpDX.Color.Transparent;
        }

        public ColorStyle(Color4 color)
        {
            Color = color;
        }

        public Color4? Color { get; set; }

        public override UIContent GenerateContent()
        {
            ColorContent content = new ColorContent(Color.Value);
            return content;
        }
    }
}
