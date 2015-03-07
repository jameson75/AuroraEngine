using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.UI.Controls;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

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
        public const string DivContainerIdAttributeName = "DivContainerId";
        
        public virtual void Parse(UITree tree, XElement element, UIControl control)
        {
            if (element.Attribute(PositionAttributeName) != null)
                control.Position = UIControlPropertyParser.ParseVector2(element.Attribute(PositionAttributeName).Value);

            if (element.Attribute(SizeAttributeName) != null)
                control.Size = UIControlPropertyParser.ParseSize2F(element.Attribute(SizeAttributeName).Value);

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
                control.Padding = UIControlPropertyParser.ParseBoundaryF(element.Attribute(PaddingAttributeName).Value);

            if (element.Attribute(MarginAttributeName) != null)
                control.Margin = UIControlPropertyParser.ParseBoundaryF(element.Attribute(MarginAttributeName).Value);

            if (element.Attribute(VerticleAlignmentAttributeName) != null)
                control.VerticalAlignment = UIControlPropertyParser.ParseEnum<VerticalAlignment>(element.Attribute(VerticleAlignmentAttributeName).Value);

            if (element.Attribute(HortizontalAlignmentAttributeName) != null)
                control.HorizontalAlignment = UIControlPropertyParser.ParseEnum<HorizontalAlignment>(element.Attribute(HortizontalAlignmentAttributeName).Value);

            if (element.Attribute(DivContainerIdAttributeName) != null)
            {
                Guid result = Guid.Empty;
                if (!Guid.TryParse(element.Attribute(DivContainerIdAttributeName).Value, out result))
                    throw new InvalidDataException("Property value not equal to Guid.");
                control.Id = result;
            }
        }

        public abstract UIControl CreateControl(IUIRoot visualRoot);
    }
}
