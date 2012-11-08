using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.Module;
using SharpDX;

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public class ColorStyle : UIStyle
    {
        public ColorStyle(IGameApp game)
            : base(game)
        {
            Color = SharpDX.Color.Transparent;
        }

        public Color4 Color { get; set; }

        public override UIContent GenerateContent()
        {
            ColorContent content = new ColorContent(Color);
            return content;
        }
    }
}
