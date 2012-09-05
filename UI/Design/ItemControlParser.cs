using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.UI.Components;

namespace CipherPark.AngelJacket.Core.UI.Design
{
    public abstract class ItemControlParser : UIControlParser
    {
        private string IsSelectedAttributeName = "IsSelected";
        private string StyleAttributeName = "Style";
        private string SelectedStyleAttributeName = "SelectedStyle";

        public override void Parse(UITree tree, XElement element, UIControl control)
        {
            if( control is ItemControl == false )
                throw new ArgumentException("control parameters was not of type itemscontrol", "control");

            ItemControl itemControl = (ItemControl)control;
            if (element.Attribute(IsSelectedAttributeName) != null)
                itemControl.IsSelected = bool.Parse(element.Attribute(IsSelectedAttributeName).Value);

            if (element.Attribute(StyleAttributeName) != null)
            {
                string styleName = element.Attribute(StyleAttributeName).Value;
                if (!tree.Styles.Contains(styleName))
                    throw new System.IO.InvalidDataException("Style not found.");
                UIContent content = tree.Styles[styleName].GenerateContent();
                //ContentControl contentControl = new ContentControl(tree, content);
                //itemControl.ItemTemplate = contentControl;                
                itemControl.ItemContent = content;
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

            if( element.Attribute(SelectedStyleAttributeName) != null)
            {
                string styleName = element.Attribute(SelectedStyleAttributeName).Value;
                if (!tree.Styles.Contains(styleName))
                    throw new System.IO.InvalidDataException("Style not found.");
                UIContent content = tree.Styles[styleName].GenerateContent();
                //ContentControl contentControl = new ContentControl(tree, content);
                //itemControl.SelectedItemTemplate = contentControl;
                itemControl.SelectContent = content;
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
    }
}
