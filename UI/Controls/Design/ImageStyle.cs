using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.UI.Controls;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public class ImageStyle : UIStyle
    {
        public Texture2D Texture { get; set; }

        public ImageStyle()
        { }

        public ImageStyle(Texture2D texture)
        {
            Texture = texture;
        }

        public override UIContent GenerateContent()
        {
            ImageContent content = new ImageContent(Texture);
            return content;
        }
    }
}
