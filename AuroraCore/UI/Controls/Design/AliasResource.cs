using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.KillScript.Core.Module;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.UI.Components
{
    public class AliasResource : UIResource
    {
        public AliasResource(IGameApp game)
            : base(game)
        { }

        public string Expansion { get; set; }
    }
}
