using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.UI.Components;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Design
{
    public class MenuItemControlParser : ItemControlParser
    {
        private string StyleAttributeName = "Style";
        private string SelectedStyleAttributeName = "SelectedStyle";

        public override void Parse(UITree tree, XElement element, UIControl control)
        {
            if (control is MenuItem == false)
                throw new ArgumentException("control parameters was not of type MenuItem", "control");

            MenuItem menuItemControl = (MenuItem)control;

            if (element.Attribute(StyleAttributeName) != null)
            {
                string styleName = element.Attribute(StyleAttributeName).Value;
                if (!tree.Styles.Contains(styleName))
                    throw new System.IO.InvalidDataException("Style not found.");
                UIContent content = tree.Styles[styleName].GenerateContent();
                //ContentControl contentControl = new ContentControl(tree, content);
                //itemControl.ItemTemplate = contentControl;                
                menuItemControl.ItemContent = content;
            }
            //else
            //{
            //    XElement itemTemplateChildElement = element.Elements("ItemTemplate").First().Elements().First();
            //    if (itemTemplateChildElement != null)
            //    {
            //        UIControlParser parser = tree.GetRegisteredControlParser(itemTemplateChildElement.Name.LocalName);
            //        UIControl itemTemplateControl = parser.CreateControl(tree);
            //        parser.Parse(tree, itemTemplateChildElement, itemTemplateControl);
            //        itemControl.ItemTemplate = itemTemplateControl;
            //    }
            //}

            if (element.Attribute(SelectedStyleAttributeName) != null)
            {
                string styleName = element.Attribute(SelectedStyleAttributeName).Value;
                if (!tree.Styles.Contains(styleName))
                    throw new System.IO.InvalidDataException("Style not found.");
                UIContent content = tree.Styles[styleName].GenerateContent();
                //ContentControl contentControl = new ContentControl(tree, content);
                //itemControl.SelectedItemTemplate = contentControl;
                menuItemControl.SelectContent = content;
            }
            //else 
            //{
            //    XElement selectedItemTemplateChildElement = element.Elements("SelectedItemTemplate").First().Elements().First();
            //    if (selectedItemTemplateChildElement != null)
            //    {
            //        UIControlParser parser = tree.GetRegisteredControlParser(selectedItemTemplateChildElement.Name.LocalName);
            //        UIControl selectedItemTemplateControl = parser.CreateControl(tree);
            //        parser.Parse(tree, selectedItemTemplateChildElement, selectedItemTemplateControl);
            //        itemControl.SelectedItemTemplate = selectedItemTemplateControl;
            //    }
            //}
            base.Parse(tree, element, control);
        }

        public override Controls.UIControl CreateControl(IUIRoot visualRoot)
        {
            return new MenuItem(visualRoot);
        }

        
    }
}
