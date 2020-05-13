using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.Content;
using System.Text.RegularExpressions;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Design
{
    public class ImageControlParser : UIControlParser 
    {
        private const string SourceAttributeName = "Source";

        public override void Parse(UITree tree, XElement element, UIControl control)
        {
            if (control is ImageControl == false)
                throw new ArgumentException("control is not of type ImageControl", "control");

            ImageControl imageControl = (ImageControl)control;            
            string resourcePattern = @"^\s*{\s*Resource\s*:\s*([A-Za-z0-9\-_]+)\s*}$";
            Match resourceMatch = Regex.Match(element.Attribute(SourceAttributeName).Value, resourcePattern);
            if (resourceMatch.Success)
            {
                Capture resourceCapture = resourceMatch.Captures[0];
                string resourceName = resourceCapture.Value;

                if (!tree.Resources.Contains(resourceName))
                    throw new InvalidOperationException("Resource not found.");

                if (tree.Resources[resourceName] is PathResource == false)
                    throw new InvalidOperationException("Resource is not of type PathResource");

                PathResource pr = (PathResource)tree.Resources[resourceName];
                imageControl.Content.Texture = ContentImporter.LoadTexture(tree.Game.GraphicsDeviceContext, element.Attribute(SourceAttributeName).Value);
            }
            base.Parse(tree, element, control);
        }

        public override UIControl CreateControl(IUIRoot visualRoot)
        {
            return new ImageControl(visualRoot);
        }
    }
}
