using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.Aurora.Core.UI.Controls;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.Utils.Toolkit;
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
    public class TextStyle : ColorStyle
    {
        public TextStyle()
        { }

        public TextStyle(SpriteFont font, Color4 fontColor, Color4? bgColor)
        {           
            Font = font;
            FontColor = fontColor;
            Color = bgColor;
        }      

        public SpriteFont Font { get; set; }

        public Color4? FontColor { get; set; }    

        public override UIContent GenerateContent()
        {
            TextContent content = new TextContent(null, 
                                                  Font, 
                                                  FontColor.GetValueOrDefault(),
                                                  Color.GetValueOrDefault());
            return content;
        }
    }
}
