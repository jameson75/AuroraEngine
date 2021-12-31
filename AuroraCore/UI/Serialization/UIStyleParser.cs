using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
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
    public abstract class UIStyleParser
    {
        private const string NameAttributeName = "Name";

        public virtual void Parse(UITree tree, XElement element, UIStyle style)
        {
            if (element.Attribute(NameAttributeName) != null)
                style.Name = element.Attribute(NameAttributeName).Value;
        }

        public abstract UIStyle CreateStyle();
    }
}
