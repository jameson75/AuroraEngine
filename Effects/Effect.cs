using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;

namespace CipherPark.AngelJacket.Core.Effects
{
    public abstract class Effect
    {
        private Device _graphicsDevice = null;
        
        protected Effect(Device graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }
        
        public Device GraphicsDevice { get { return _graphicsDevice; } }

        public virtual Matrix World { get; set; }

        public virtual Matrix View { get; set; }

        public virtual Matrix Projection { get; set; }

        public virtual byte[] SelectShaderByteCode()
        {
            return null;
        }

        public virtual void Apply()
        { }

        protected byte[] LoadVertexShader(string fileName, out VertexShader shader)
        {
            byte[] shaderByteCode = System.IO.File.ReadAllBytes(fileName);
            shader = new VertexShader(GraphicsDevice, shaderByteCode);
            return shaderByteCode;
        }

        protected byte[] LoadPixelShader(string fileName, out PixelShader shader)
        {
            byte[] shaderByteCode = System.IO.File.ReadAllBytes(fileName);
            shader = new PixelShader(GraphicsDevice, shaderByteCode);
            return shaderByteCode;
        }
    }
}
