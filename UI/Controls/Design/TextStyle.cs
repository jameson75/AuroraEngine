﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
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
    public class TextStyle : UIStyle
    {
        public TextStyle()
        { }

        public TextStyle(SpriteFont font, Color4 fontColor)
        {           
            Font = font;
            FontColor = fontColor;
        }      

        public SpriteFont Font { get; set; }

        public Color4 FontColor { get; set; }

        public override UIContent GenerateContent()
        {
            TextContent content = new TextContent(null, Font, FontColor);
            return content;
        }
    }
}
