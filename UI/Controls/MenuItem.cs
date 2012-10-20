using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Interop;
using SharpDX;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class MenuItem : ItemControl
    {
        public static readonly RectangleF DefaultItemTextMargin = new RectangleF(5, 5, 5, 5);

        public MenuItem(IUIRoot visualRoot) : base(visualRoot)
        {
            this.Size = new DrawingSizeF(100, 20);
        }

        public MenuItem(IUIRoot visualRoot, string name, string text, SpriteFont font, Color4 fontColor)
            : base(visualRoot)
        {
            Name = name;
            CommandName = name;
            TextContent textContent = new TextContent(text, font, fontColor);
            ItemContent = textContent;
            //SelectContent = textContent;
            Size = font.MeasureString(text).Add(DefaultItemTextMargin.GetSize()); 
        }

        public MenuItem(Components.IUIRoot visualRoot, string name, string text, SpriteFont font, Color4 itemFontColor, Color4 selectFontColor, string commandName = null)
            : base(visualRoot)
        {
            Name = name;
            CommandName = (commandName != null) ? commandName : name;
            TextContent itemContent = new TextContent(text, font, itemFontColor);
            ItemContent = itemContent;
            TextContent selectContent = new TextContent(text, font, selectFontColor);
            SelectContent = selectContent;
            Size = font.MeasureString(text).Add(DefaultItemTextMargin.GetSize());
        }       

        //public override bool CanFocus
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}
    }
}
