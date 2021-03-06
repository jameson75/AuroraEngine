using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using CipherPark.KillScript.Core.UI.Components;
using CipherPark.KillScript.Core.UI.Controls;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.UI.Design
{    
    public class MenuControlParser : ItemsControlParser 
    {
        private const string SelectedIndexAttributeName = "SelectedIndex";
        private const string OrientationAttributeName = "Orientation";
        //private static readonly MenuOrientation DefaultOrientation = MenuOrientation.Vertical;
        private MenuItemControlParser menuItemControlParser = new MenuItemControlParser();      
        
        public override void Parse(UITree tree, XElement element, UIControl control)
        {
            if (control is Menu == false)
                throw new ArgumentException("control parameter was not of type menu", "control");

            Menu newMenu = (Menu)control;
            
            if (element.Attribute(SelectedIndexAttributeName) != null)
                newMenu.SelectedItemIndex = int.Parse(element.Attribute(SelectedIndexAttributeName).Value);

            if (element.Attribute(OrientationAttributeName) != null)
                newMenu.Orientation = UIControlPropertyParser.ParseEnum<MenuOrientation>(element.Attribute(OrientationAttributeName).Value);
            
            base.Parse(tree, element, newMenu);
        }   
    
        protected override string ItemsElementName
        {
            get { return "MenuItems"; }
        }

        protected override string ItemElementName
        {
            get
            {
                return "MenuItem"; 
            }
        }

        public override UIControl CreateControl(IUIRoot visualRoot)
        {
            return new Menu(visualRoot);
        }
    }
}
