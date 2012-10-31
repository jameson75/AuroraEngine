﻿using System;
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

        public override void Parse(UITree tree, XElement element, UIControl control)
        {
            if( control is ItemControl == false )
                throw new ArgumentException("control parameters was not of type ItemControl", "control");

            ItemControl itemControl = (ItemControl)control;
            
            if (element.Attribute(IsSelectedAttributeName) != null)
                itemControl.IsSelected = bool.Parse(element.Attribute(IsSelectedAttributeName).Value);           

            base.Parse(tree, element, control);
        }       
    }
}
