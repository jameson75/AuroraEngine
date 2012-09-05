using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils;
using SharpDX;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class MenuItem : ItemControl
    {
        public static readonly Vector2 DefaultItemTextMargin = new Vector2(5, 5);

        public MenuItem(IUIRoot visualRoot) : base(visualRoot)
        {
            this.Size = new Vector2(100, 20);
        }

        public MenuItem(IUIRoot visualRoot, string name, string text, SpriteFont font, Color fontColor)
            : base(visualRoot)
        {
            Name = name;
            CommandName = name;
            TextContent textContent = new TextContent(text, font, fontColor);
            ItemContent = textContent;
            //SelectContent = textContent;
            Size = font.MeasureString(text) + (DefaultItemTextMargin * 2);   
        }

        public MenuItem(Components.IUIRoot visualRoot, string name, string text, SpriteFont font, Color itemFontColor, Color selectFontColor, string commandName = null)
            : base(visualRoot)
        {
            Name = name;
            CommandName = (commandName != null) ? commandName : name;
            TextContent itemContent = new TextContent(text, font, itemFontColor);
            ItemContent = itemContent;
            TextContent selectContent = new TextContent(text, font, selectFontColor);
            SelectContent = selectContent;
            Size = font.MeasureString(text) + (DefaultItemTextMargin * 2);   
        }

        public string CommandName { get; set; }

        //public override bool CanFocus
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}
    }
}
