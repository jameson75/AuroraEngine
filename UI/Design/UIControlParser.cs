using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.UI.Controls;

namespace CipherPark.AngelJacket.Core.UI.Design
{
    public abstract class UIControlParser
    {
        public const string PositionAttributeName = "Position";
        public const string SizeAttributeName = "Size";
        public const string NameAttributeName = "Name";
        public const string EnabledAttributeName = "Enabled";
        public const string BoundsAttributeName = "Bounds";
        public const string PaddingAttributeName = "Padding";
        public const string MarginAttributeName = "Margin";
        public const string VerticleAlignmentAttributeName = "VerticleAlignment";
        public const string HortizontalAlignmentAttributeName = "HorizontalAlignment";
        
        public virtual void Parse(UITree tree, XElement element, UIControl control)
        {
            if (element.Attribute(PositionAttributeName) != null)
                control.Position = UIControlPropertyParser.ParseVector2(element.Attribute(PositionAttributeName).Value);

            if (element.Attribute(SizeAttributeName) != null)
                control.Size = UIControlPropertyParser.ParseVector2(element.Attribute(SizeAttributeName).Value);

            if (element.Attribute(NameAttributeName) != null)
                control.Name = element.Attribute(NameAttributeName).Value;

            if (element.Attribute(EnabledAttributeName) != null)
            {
                bool result = false;
                if (!bool.TryParse(element.Attribute(EnabledAttributeName).Value, out result))
                    throw new InvalidDataException("Property value not equal to boolean.");
                control.Enabled = result;
            }

            if (element.Attribute(PaddingAttributeName) != null)
                control.Padding = UIControlPropertyParser.ParseVector2(element.Attribute(PaddingAttributeName).Value);

            if (element.Attribute(MarginAttributeName) != null)
                control.Margin = UIControlPropertyParser.ParseVector2(element.Attribute(MarginAttributeName).Value);

            if (element.Attribute(VerticleAlignmentAttributeName) != null)
                control.VerticalAlignment = UIControlPropertyParser.ParseEnum<VerticalAlignment>(element.Attribute(VerticleAlignmentAttributeName).Value);

            if (element.Attribute(HortizontalAlignmentAttributeName) != null)
                control.HorizontalAlignment = UIControlPropertyParser.ParseEnum<HorizontalAlignment>(element.Attribute(HortizontalAlignmentAttributeName).Value);
        }

        public abstract UIControl CreateControl(IUIRoot visualRoot);
    }
}
