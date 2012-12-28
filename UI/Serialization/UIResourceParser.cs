using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CipherPark.AngelJacket.Core.UI.Components;

namespace CipherPark.AngelJacket.Core.UI.Design
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
