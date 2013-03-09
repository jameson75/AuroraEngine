using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class PathResourceParser : UIResourceParser
    {
        private const string PathAttributeName = "Path";

        public override void Parse(UITree tree, XElement element, UIResource resource)
        {
            if( resource is PathResource == false )
                throw new ArgumentException("Resource was not of type PathResource.", "resource");
           
            PathResource pathResource = (PathResource)resource;

            if( element.Attribute(PathAttributeName) != null )
                pathResource.Path = element.Attribute(PathAttributeName).Value;

            base.Parse(tree, element, resource);
        }

        public override UIResource CreateResource(IGameApp game)
        {
            return new PathResource(game);
        }
    }
}
