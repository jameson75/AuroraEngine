using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CipherPark.AngelJacket.Core.UI.Components;

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
