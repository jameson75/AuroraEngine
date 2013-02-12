using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;

namespace CipherPark.AngelJacket.Core.Effects
{
    public abstract class PostEffect : Effect
    {
        public ShaderResourceView Texture { get; set; }

        protected PostEffect(Device graphicsDevice)
            : base(graphicsDevice)
        { }

        [Obsolete]
        public ShaderResourceView Depth { get; set; }

        public abstract void Apply();
    }
}
