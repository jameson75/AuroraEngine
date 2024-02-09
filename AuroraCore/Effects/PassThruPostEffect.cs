using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Content;

/////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
/////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public class PassThruPostEffect : PostEffect
    {
        private Mesh _quad = null;
        private byte[] _vertexShaderByteCode = null;
        private SamplerState _samplerState = null;
        private VertexShader _vertexShaderNoFix = null;
        private PixelShader _pixelShader = null;
        private VertexShader _vertexShaderWithFix = null;       

        public bool EnableTexelFix { get; set; }

        public BlendState BlendState { get; set; }

        public PassThruPostEffect(IGameApp game)
            : base(game)
        {
            string psFileName = "Assets\\Shaders\\postpassthru-ps.cso";
            string vsFileName = "Assets\\Shaders\\postpassthru-vs.cso";
            string vsFixFileName = "Assets\\Shaders\\postpassthru-fix-vs.cso";
            
            //Load Shaders
            //------------
            _vertexShaderByteCode = ReadByteStream(vsFileName);
            _samplerState = new SamplerState(game.GraphicsDevice, SamplerStateDescription.Default());
            _vertexShaderNoFix = new VertexShader(Game.GraphicsDevice, _vertexShaderByteCode);
            _pixelShader = new PixelShader(Game.GraphicsDevice, ReadByteStream(psFileName));
            _vertexShaderWithFix = new VertexShader(Game.GraphicsDevice, ReadByteStream(vsFixFileName));
            
            //Create Scree Quad
            //------------------
            _quad = ContentBuilder.BuildViewportQuad(Game.GraphicsDevice, _vertexShaderByteCode);
        }

        public override void Apply()
        {
            BlendState oldBlendState = null;
            
            //Setup Shaders
            //-------------
            if (!EnableTexelFix)
                Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderNoFix);
            else
                Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderWithFix);
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);            
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _samplerState);
            
            //Setup states
            //------------
            if (BlendState != null)
            {
                oldBlendState = Game.GraphicsDevice.ImmediateContext.OutputMerger.BlendState;
                Game.GraphicsDevice.ImmediateContext.OutputMerger.BlendState = BlendState;
            }

            //Render Screen quad
            //------------------
            _quad.Draw();            
            
            //Clean Up Shader
            //---------------
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);            

            //Clean Up State
            //--------------
            if (BlendState != null)            
                Game.GraphicsDevice.ImmediateContext.OutputMerger.BlendState = oldBlendState;            
        }

        protected override void OnDispose()
        {            
            _samplerState.Dispose();
            _vertexShaderNoFix.Dispose();
            _pixelShader.Dispose();
            _vertexShaderWithFix.Dispose();
            _quad.Dispose();

            _vertexShaderByteCode = null;
            _samplerState = null;
            _vertexShaderNoFix = null;
            _pixelShader = null;
            _vertexShaderWithFix = null;            
            _quad = null;
        }
    }
}
