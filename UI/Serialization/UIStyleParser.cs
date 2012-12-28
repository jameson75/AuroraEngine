using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using CipherPark.AngelJacket.Core.UI.Components;

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

        public abstract UIStyle CreateStyle(IGameApp game);
    }
}
