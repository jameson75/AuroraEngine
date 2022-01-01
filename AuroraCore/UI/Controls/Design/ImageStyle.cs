using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.UI.Controls;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Components
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
