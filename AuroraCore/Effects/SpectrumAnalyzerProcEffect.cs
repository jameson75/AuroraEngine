using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.KillScript.Core.World.Geometry;
using CipherPark.KillScript.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.Effects
{
    public class SpectrumAnalyzerProcEffect : Effect
    {
        private byte[] _vertexShaderByteCode = null;
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private SharpDX.Direct3D11.Buffer _constantsBuffer = null;
        private const int ConstantsBufferSize = 464;
        private RenderTargetView _output = null;
        private Mesh _offscreenQuad = null;
        private Size2 _gameScreenSize = Size2.Zero;
        private float[] _bandPower = null;
        private const int MaxBands = 96;

        public Size2 GameScreenSize
        {
            get { return _gameScreenSize; }
            set
            {
                _gameScreenSize = value;
                CreateOffscreenQuad();
            }
        }

        public uint BandCount { get; set; }

        public uint LevelCount { get; set; }

        public float BandMarginSize { get; set; }

        public float LevelMarginSize { get; set; }

        public Color LitLEDColor { get; set; }

        public Color UnlitLEDColor { get; set; }

        public Color BackColor { get; set; }

        public float[] BandPower
        {
            get { return _bandPower; }
        }

        public RenderTargetView Output
        {
            get { return _output; }
            set
            {
                _output = value;
                CreateOffscreenQuad();
            }
        }

        public SpectrumAnalyzerProcEffect(IGameApp game) : base(game)
        {
            _bandPower = new float[MaxBands];
            LitLEDColor = Color.Transparent;
            BackColor = Color.Transparent;
            UnlitLEDColor = Color.Transparent;
            CreateShaders();           
        }

        private void CreateShaders()
        {
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\postpassthru-vs.cso", out _vertexShader);
            LoadPixelShader("Content\\Shaders\\spectrum-analyzer-ps.cso", out _pixelShader);
            _constantsBuffer = new SharpDX.Direct3D11.Buffer(Game.GraphicsDevice, ConstantsBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        private void WriteShaderConstants()
        {
            DataBox dataBox = Game.GraphicsDevice.ImmediateContext.MapSubresource(_constantsBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, ConstantsBufferSize);
            //TODO: Write data to buffer
            int offset = 0;
            dataBuffer.Set(offset, BandCount);
            offset += sizeof(uint);
            dataBuffer.Set(offset, LevelCount);
            offset += sizeof(uint);
            dataBuffer.Set(offset, MathUtil.Clamp(BandMarginSize, 0, 1.0f));
            offset += sizeof(float);
            dataBuffer.Set(offset, MathUtil.Clamp(LevelMarginSize, 0, 1.0f));
            offset += sizeof(float);            
            dataBuffer.Set(offset, LitLEDColor.ToVector4());
            offset += Vector4.SizeInBytes;
            dataBuffer.Set(offset, UnlitLEDColor.ToVector4());
            offset += Vector4.SizeInBytes;
            dataBuffer.Set(offset, BackColor.ToVector4());
            offset += Vector4.SizeInBytes;
            dataBuffer.Set(offset, BandPower);
            offset += (sizeof(float) * BandPower.Length);
            Game.GraphicsDevice.ImmediateContext.UnmapSubresource(_constantsBuffer, 0);
        }

        public override void Apply()
        {
            //Capture current render targets
            RenderTargetView originalRenderTarget = null;
            DepthStencilView originalDepthStencilView = null;
            originalRenderTarget = Game.GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out originalDepthStencilView)[0];

            //Write shader constants.
            WriteShaderConstants();
            //Set constant buffer.
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantsBuffer);
            //Set vertex shader.
            Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            //Set pixel shader.
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            //Set render target of offscreen texture.
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(Output);
            
            //Render spectrum analyzer to offscreen texture.
            _offscreenQuad.Draw();            
            
            //Clear.
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);
            //Clear.
            Game.GraphicsDevice.ImmediateContext.VertexShader.Set(null);
            //Clear.
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(null);
            
            //Set graphics context back to captured state.
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(originalDepthStencilView, originalRenderTarget);

            //COM Release() swap chain resource.
            originalRenderTarget.Dispose();
            originalDepthStencilView.Dispose();
        }
       
        private void CreateOffscreenQuad()
        {
            _offscreenQuad = null;
            if (_output != null &&
                GameScreenSize != Size2.Zero)
            {
                _offscreenQuad = Content.ContentBuilder.BuildOffscreenQuad(Game.GraphicsDevice,
                                                                                Output.GetTexture2DSize(),
                                                                                GameScreenSize,
                                                                                _vertexShaderByteCode);
            }
        }
    }
}
