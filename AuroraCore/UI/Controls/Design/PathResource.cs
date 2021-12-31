using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Components
{
    public class PathResource : UIResource
    {
        public PathResource(IGameApp game) : base(game)
        { }
        public string Path { get; set; }
    }
}
