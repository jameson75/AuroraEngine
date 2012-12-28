using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public class PathResource : UIResource
    {
        public PathResource(IGameApp game) : base(game)
        { }
        public string Path { get; set; }
    }
}
