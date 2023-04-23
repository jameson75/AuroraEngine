using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.World.Geometry;

/////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
/////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public class TransitionEffect : PostEffect
    {
        private const int ConstantsBufferSize = 16;
        private Buffer constantsBuffer;
        private SamplerState inputTextureSamplerState;
        private Texture2D wipeTexture;
        private ShaderResourceView wipeTextureShaderResourceView;
        private SamplerState wipeTextureSamplerState;
        private RenderTargetView wipeTextureRenderTargetView;
        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private byte[] vertexShaderByteCode;
        private Mesh screenQuad;

        public float TransitionStep { get; set; }
        public Color WipeColor { get; set; }

        public TransitionEffect(IGameApp game)
            : base(game)
        {            
            CreateConstantBuffers();
            CreateTextureResources();
            CreateShaders();
            CreateMesh();
        }

        public RenderTargetView WipeTextureRenderTargetView
        {
            get => wipeTextureRenderTargetView;
        }

        protected override void OnBeginApply()
        {          
            WriteConstants();
            base.OnBeginApply();
            CreatePasses();            
        }

        private void CreateMesh()
        {
            screenQuad = ContentBuilder.BuildViewportQuad(GraphicsDevice, vertexShaderByteCode);
        }

        private void CreatePasses()
        {            
            PostEffectPass pass = new PostEffectPass(GraphicsDevice);
            pass.Name = "Main";
            pass.VertexShader = vertexShader;
            pass.PixelShader = pixelShader;
            pass.PixelShaderConstantBuffers.Add(constantsBuffer);
            pass.PixelShaderResources.Add(InputTexture);
            pass.PixelShaderResources.Add(wipeTextureShaderResourceView);
            pass.PixelShaderSamplers.Add(inputTextureSamplerState);
            pass.PixelShaderSamplers.Add(wipeTextureSamplerState);
            pass.RenderTarget = OutputTexture;
            pass.ScreenQuad = screenQuad;
            Passes.Clear();
            Passes.Add(pass);
        }

        private void CreateConstantBuffers()
        {
            constantsBuffer = new Buffer(GraphicsDevice, ConstantsBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);                
        }

        private void CreateTextureResources()
        {
            Texture2DDescription textureDesc = CreateCommonTextureDesc(Game.RenderTargetView.GetTexture2DSize());
            ShaderResourceViewDescription shaderResourceViewDesc = CreateCommonShaderResourceViewDesc(textureDesc);
            RenderTargetViewDescription renderTargetViewDesc = CreateCommonRenderTargetViewDesc(textureDesc);
            SamplerStateDescription samplerStateDesc = CreateCommonSamplerStateDesc();          
            
            inputTextureSamplerState = new SamplerState(GraphicsDevice, samplerStateDesc);

            wipeTexture = new Texture2D(Game.GraphicsDevice, textureDesc);
            wipeTextureShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, wipeTexture, shaderResourceViewDesc);
            wipeTextureSamplerState = new SamplerState(Game.GraphicsDevice, samplerStateDesc);
            wipeTextureRenderTargetView = new RenderTargetView(Game.GraphicsDevice, wipeTexture, renderTargetViewDesc);
        }

        private void CreateShaders()
        {            
            vertexShaderByteCode = LoadVertexShader("Assets\\Shaders\\postpassthru-vs.cso", out vertexShader);
            LoadPixelShader("Assets\\Shaders\\transition-wipe.cso", out pixelShader);
        }

        private void WriteConstants()
        {
            DataBox dataBox;
            dataBox = GraphicsDevice.ImmediateContext.MapSubresource(constantsBuffer, 0, MapMode.WriteDiscard, MapFlags.None); 
            
            //Step
            float transitionStep = this.TransitionStep;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref transitionStep);

            //WipeTextureSize
            Vector2 textureSize = this.WipeTextureRenderTargetView.GetTexture2DSize().ToVector2();
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector2>(dataBox.DataPointer, ref textureSize);

            //Wipe Color
            Vector4 wipeColor = this.WipeColor.ToVector4();
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector4>(dataBox.DataPointer, ref wipeColor);

            GraphicsDevice.ImmediateContext.UnmapSubresource(constantsBuffer, 0);         
        }

        protected override void OnDispose()
        {
            constantsBuffer.Dispose();
            inputTextureSamplerState.Dispose();
            wipeTexture.Dispose();
            wipeTextureShaderResourceView.Dispose();
            wipeTextureSamplerState.Dispose();
            wipeTextureRenderTargetView.Dispose();
            vertexShader.Dispose();
            pixelShader.Dispose();
            screenQuad.Dispose();
            base.OnDispose();
        }
    }
}
