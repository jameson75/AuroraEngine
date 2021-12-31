using System;
using System.Collections.Generic;
using CipherPark.KillScript.Core.UI.Components;
using CipherPark.KillScript.Core.Utils;
using CipherPark.KillScript.Core.Utils.Toolkit;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.UI.Controls
{
    public class MenuItem : ItemControl
    {
        //public static readonly Size2F DefaultItemTextMargin = new Size2F(10f, 10f);
        private UIContent _itemContent = null;
        private UIContent _selectContent = null;

        public MenuItem(IUIRoot visualRoot) : base(visualRoot)
        {
            this.Size = new Size2F(100, 20);
        }

        public MenuItem(Components.IUIRoot visualRoot, string name, string text, SpriteFont font, Color4 itemFontColor, Color4? selectFontColor = null, string commandName = null, ContextMenu subMenu = null)
            : base(visualRoot)
        {
            Name = name;
            CommandName = (commandName != null) ? commandName : name;
            SubMenu = subMenu;           
            ItemContent = new TextContent(text, font, itemFontColor);            
            if(selectFontColor.HasValue)
                SelectContent = new TextContent(text, font, selectFontColor.Value);
            Size = font.MeasureString(text); //.Add(DefaultItemTextMargin);
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

        public ContextMenu SubMenu { get; set; }

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

        protected override void OnDraw()
        {
            if (ActiveContent != null)
                ActiveContent.Draw();
            base.OnDraw();
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
