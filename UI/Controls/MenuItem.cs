using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class MenuItem : ItemControl
    {
        public static readonly DrawingSizeF DefaultItemTextMargin = new DrawingSizeF(10f, 10f);
        private UIContent _itemContent = null;
        private UIContent _selectContent = null;

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
            Size = font.MeasureString(text).Add(DefaultItemTextMargin); 
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
            Size = font.MeasureString(text).Add(DefaultItemTextMargin);
        }

        public MenuItem(Components.IUIRoot visualRoot, string name, string text, SpriteFont font, Color4 itemFontColor, Color4 selectFontColor, Submenu subMenu)
            : base(visualRoot)
        {
            Name = name;
            Submenu = subMenu;
            TextContent itemContent = new TextContent(text, font, itemFontColor);
            ItemContent = itemContent;
            TextContent selectContent = new TextContent(text, font, selectFontColor);
            SelectContent = selectContent;
            Size = font.MeasureString(text).Add(DefaultItemTextMargin);
        }

        public UIContent ItemContent
        {
            get { return _itemContent; }
            set
            {
                if (_itemContent != null)
                    _itemContent.Container = null;
                _itemContent = value;
                if (value != null)
                    value.Container = this;
            }
        }

        public UIContent SelectContent
        {
            get { return _selectContent; }
            set
            {
                if (_selectContent != null)
                    _selectContent.Container = null;
                _selectContent = value;
                if (value != null)
                    value.Container = this;
            }
        }

        public Submenu Submenu { get; set; }

        protected UIContent ActiveContent
        {
            get
            {
                if (this.IsSelected && SelectContent != null)
                    return SelectContent;
                else
                    return ItemContent;
            }
        }

        protected override void OnDraw(long gameTime)
        {
            if (ActiveContent != null)
                ActiveContent.Draw(gameTime);
            base.OnDraw(gameTime);
        }

        public override void ApplyTemplate(UIControlTemplate template)
        {
            MenuItemTemplate menuItemTemplte = (MenuItemTemplate)template;
            base.ApplyTemplate(template);
        }

        public static UIControl FromTemplate(IUIRoot visualRoot, MenuItemTemplate menuItemTemplate)
        {
            MenuItem menuItem = new MenuItem(visualRoot);
            menuItem.ApplyTemplate(menuItemTemplate);
            return menuItem;
        }
    }
}
