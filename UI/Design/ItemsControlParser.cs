using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.UI.Controls;

namespace CipherPark.AngelJacket.Core.UI.Design
{
    public abstract class ItemsControlParser : UIControlParser
    { 
        public override void Parse(UITree tree, XElement element, UIControl control)
        {
            if (control is ItemsControl == false)
                throw new ArgumentException("control parameters was not of type itemscontrol", "control");

            ItemsControl itemsControl = (ItemsControl)control;          
           
            var _menuitems = element.Descendants(ItemElementName);
            ItemControlParser childItemParser = (ItemControlParser)tree.GetRegisteredControlParser(ItemElementName);
            foreach (var _item in _menuitems)
            {              
                ItemControl itemControl = (ItemControl)childItemParser.CreateControl(tree);
                itemsControl.Items.Add(itemControl);
            }

            base.Parse(tree, element, control);
        }

        protected virtual string ItemsElementName
        {
            get { return "Items"; }
        }

        protected virtual string ItemElementName
        {
            get { return "Item"; }
        }
    }
}
