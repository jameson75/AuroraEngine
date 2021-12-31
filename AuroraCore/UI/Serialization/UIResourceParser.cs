using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
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
    public abstract class UIResourceParser
    {
        private string NameAttributeName = "Name";

        public virtual void Parse(UITree tree, XElement element, UIResource resource)
        {
            if (element.Attribute(NameAttributeName) != null)
                resource.Name = element.Attribute(NameAttributeName).Value;
        }

        public abstract UIResource CreateResource(IGameApp game);
    }
}
