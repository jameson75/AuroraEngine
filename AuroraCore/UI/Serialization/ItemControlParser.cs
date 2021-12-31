using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CipherPark.KillScript.Core.UI.Controls;
using CipherPark.KillScript.Core.UI.Components;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.UI.Design
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
