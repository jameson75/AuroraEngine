using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CipherPark.KillScript.Core.UI.Components;
using CipherPark.KillScript.Core.Content;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.UI.Design
{
    public class TextStyleParser : UIStyleParser
    {
        private const string FontAttributeName = "Font";
        private const string FontColorAttributeName = "FontColor";

        public override void Parse(UITree tree, XElement element, UIStyle style)
        {
            if( style is TextStyle == false )
                throw new ArgumentException("Style is not of type TextStyle", "style");
            
            TextStyle textStyle = (TextStyle)style;
           
            if (element.Attribute(FontAttributeName) != null)
            {
                string resourcePattern = @"^\s*{\s*Resource\s*:\s*([A-Za-z0-9\-_]+)\s*}$";                 
                //string pathPattern = @"^([A-Za-z0-9\-_]+(\\[A-Za-z0-9\-_]+)*)(\.[A-Za-z0-9\-]{3,4})?$";
                Match resourceMatch = Regex.Match(element.Attribute(FontAttributeName).Value, resourcePattern);
                if (resourceMatch.Success)
                {
                    Capture resourceCapture = resourceMatch.Captures[0];
                    string resourceName = resourceCapture.Value;
                    
                    if (!tree.Resources.Contains(resourceName))
                        throw new InvalidOperationException("Resource not found.");

                    if (tree.Resources[resourceName] is PathResource == false)
                        throw new InvalidOperationException("Resource is not of type PathResource");

                    PathResource pr = (PathResource)tree.Resources[resourceName];
                    textStyle.Font = ContentImporter.LoadFont(tree.Game.GraphicsDevice, pr.Path);
                }
                else
                    textStyle.Font = ContentImporter.LoadFont(tree.Game.GraphicsDevice, (element.Attribute(FontAttributeName).Value));
            }

            if (element.Attribute(FontColorAttributeName) != null)
                textStyle.FontColor = UIControlPropertyParser.ParseColor(element.Attribute(FontColorAttributeName).Value);
                
            base.Parse(tree, element, style);
        }

        public override UIStyle CreateStyle()
        {
            TextStyle style = new TextStyle();
            return style;
        }
    }
}
