using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.UI.Components
{
    public class PathResource : UIResource
    {
        public PathResource(IGameApp game) : base(game)
        { }
        public string Path { get; set; }
    }
}
