using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Effects
{
    public abstract class PostEffect : Effect
    {
        public bool Enabled { get; set; }

        public ShaderResourceView InputTexture { get; set; }

        protected PostEffect(Device graphicsDevice)
            : base(graphicsDevice)
        {
            Enabled = true;
        }

        [Obsolete]
        public ShaderResourceView Depth { get; set; }       
    }
}
