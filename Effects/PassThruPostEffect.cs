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
    public class PassThruPostEffect : PostEffect
    {
        private Mesh _quad = null;
        private byte[] _vertexShaderByteCode = null;
        public VertexShader VertexShader { get; protected set; }
        public PixelShader PixelShader { get; protected set; }

        public PassThruPostEffect(Device graphicsDevice, IGameApp game)
            : base(graphicsDevice)
        {
            string psFileName = "Content\\Shaders\\postpassthru-ps.cso";
            string vsFileName = "Content\\Shaders\\postpassthru-vs.cso";
            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            VertexShader = new VertexShader(GraphicsDevice, _vertexShaderByteCode);
            PixelShader = new PixelShader(GraphicsDevice, System.IO.File.ReadAllBytes(psFileName));
            _quad = ContentBuilder.BuildViewportQuad(game, _vertexShaderByteCode);
        }

        public override void Apply()
        {
            GraphicsDevice.ImmediateContext.VertexShader.Set(VertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(PixelShader);            
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, Texture);

            _quad.Draw(0);

            //Un-bind Texture from pixel shader input so it can possibly be used later as a render target.
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);            
        }

        public byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;
        }
    }  
}
