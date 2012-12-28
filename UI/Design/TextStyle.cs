using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using CipherPark.AngelJacket.Core.Module;
using SharpDX;

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public class TextStyle : ColorStyle
    {
        public TextStyle(IGameApp game) : base(game)
        { }

        public SpriteFont Font { get; set; }

        public Color4 FontColor { get; set; }

        public override UIContent GenerateContent()
        {
            TextContent content = new TextContent(null, Font, FontColor);
            return content;
        }
    }
}
