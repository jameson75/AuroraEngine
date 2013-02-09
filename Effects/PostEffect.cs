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
    public abstract class PostEffect
    {
        public Device GraphicsDevice { get; protected set; }

        public PostEffect(Device graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }

        public ShaderResourceView Texture { get; set; }
        public ShaderResourceView Depth { get; set; }

        public abstract void Apply();
    }
}
