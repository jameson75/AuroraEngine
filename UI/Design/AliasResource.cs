using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.Module;

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public class AliasResource : UIResource
    {
        public AliasResource(IGameApp game)
            : base(game)
        { }

        public string Expansion { get; set; }
    }
}
