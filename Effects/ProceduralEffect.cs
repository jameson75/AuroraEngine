using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Content;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Effects
{
    public abstract class ProceduralEffect : PostEffect
    {
        public ProceduralEffect(IGameApp game) : base(game)
        {
            
        }
    }

    public class SpectrumAnalyzerProcEffect : ProceduralEffect
    {
        private VertexShader _vertexShader = null;        
        private PixelShader _pixelShader = null;
        private byte[] _vertexShaderByteCode = null;       
        private SharpDX.Direct3D11.Buffer _vsConstantsBuffer = null;
        private SharpDX.Direct3D11.Buffer _psConstantsBuffer = null; 
        private Mesh _quad = null;

        private const int MaxBandCount = 0;

        /// <summary>
        /// 
        /// </summary>
        public int BandCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int LevelCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float[] Values { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        public SpectrumAnalyzerProcEffect(IGameApp game)
            : base(game)        
        {
            CreateShaders();
            CreateShaderTargets();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Apply()
        {
            WriteVSConstants();                  
            WritePSConstants();
           
            RenderTargetView originalRenderTarget = null;
            DepthStencilView originalDepthStencilView = null;
            originalRenderTarget = Game.GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out originalDepthStencilView)[0];

            ////////////////////////////
            //Pass0
            //-----           
            ////////////////////////////           
            
            //Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            //Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _inputTextureSampler);   
            Game.GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _vsConstantsBuffer); 
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _psConstantsBuffer);        
            Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(OutputTexture);
            
            _quad.Draw(null);
            
            //Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            //Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            Game.GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, null);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(originalDepthStencilView, originalRenderTarget);

            base.Apply();
        }

        /// <summary>
        /// 
        /// </summary>
        private void WritePSConstants()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        private void WriteVSConstants()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateShaders()
        {
            string psFileName = "Content\\Shaders\\spectrumanalyzer-ps.cso";
            string vsFileName = "Content\\Shaders\\spectrumanalyzer-vs.cso";
            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            _vertexShader = new VertexShader(Game.GraphicsDevice, _vertexShaderByteCode);
            _pixelShader = new PixelShader(Game.GraphicsDevice, System.IO.File.ReadAllBytes(psFileName));
            //int bufferSize = sizeof(float) * 16  * 3; //size of WorldViewProj + ForegroundColor + BackgroundColor
            //_constantBuffer = new SharpDX.Direct3D11.Buffer(Game.GraphicsDevice, bufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);       
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateShaderTargets()
        {
            _quad = ContentBuilder.BuildBasicViewportQuad(Game.GraphicsDevice, _vertexShaderByteCode);
        }
    }
}
