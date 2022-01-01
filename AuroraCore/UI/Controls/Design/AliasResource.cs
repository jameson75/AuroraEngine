using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.Aurora.Core.Module;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Components
{
    public class AliasResource : UIResource
    {
        public AliasResource(IGameApp game)
            : base(game)
        { }

        public string Expansion { get; set; }
    }
}
