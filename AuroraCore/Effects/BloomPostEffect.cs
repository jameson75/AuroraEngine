﻿using System;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Extensions;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public class BloomPostEffect : PostEffect
    {
        private const int SampleCount = 15;
        private const int BloomExtractConstantBufferSize = 32;
        private const int BloomCombineConstantBufferSize = 32;
        private readonly int GuassianConstantBufferSize;
        private IGameApp _game = null;
        SharpDX.Direct3D11.Buffer _bloomExtractConstantBuffer = null;
        SharpDX.Direct3D11.Buffer _guassianHorizontalConstantBuffer = null;
        SharpDX.Direct3D11.Buffer _guassianVerticalConstantBuffer = null;
        SharpDX.Direct3D11.Buffer _bloomCombineConstantBuffer = null;
        Mesh _quad = null;
        VertexShader _passThruVertexShader = null;
        PixelShader _gaussianBlurPixelShader = null;
        PixelShader _bloomExtractPixelShader = null;
        PixelShader _bloomCombinePixelShader = null;
        private SamplerState _inputTextureSamplerState;
        private Texture2D _bloomExtractTexture;
        private ShaderResourceView _bloomExtractShaderResourceView;
        private SamplerState _bloomExtractSamplerState;
        private RenderTargetView _bloomExtractRenderTargetView;
        private Texture2D _tempTexture;
        private ShaderResourceView _tempShaderResourceView;
        private SamplerState _tempSamplerState;
        private RenderTargetView _tempRenderTargetView;
        private Texture2D _blurTexture;
        private ShaderResourceView _blurShaderResourceView;
        private SamplerState _blurSamplerState;
        private RenderTargetView _blurRenderTargetView;
        private byte[] _vertexShaderByteCode;

        public float Threshold { get; set; }
        public float BlurAmount { get; set; }
        public float BloomIntensity { get; set; }
        public float BaseIntensity { get; set; }
        public float BloomSaturation { get; set; }
        public float BaseSaturation { get; set; }

        public BloomPostEffect(IGameApp app)
            : base(app)
        {            
            //Explanation: In HLSL, each element in an array is aligned to a 16-byte boundary... 
            //With 2 arrays of lenght "SampleCount" the calculation is 16 x SampleCount x 2.
            GuassianConstantBufferSize = BloomPostEffect.CalculateRequiredConstantBufferSize((Vector4.SizeInBytes * SampleCount * 2));
            _game = app;
            Initialize();
        }  

        public void SetPresetState(BloomPreset preset)
        {
            switch (preset)
            {
                case BloomPreset.Soft:
                    Threshold = 0; BlurAmount = 3; BloomIntensity = 1; BaseIntensity = 1; BloomSaturation = 1; BaseSaturation = 1;
                    break;
                 case BloomPreset.Blurry:
                    Threshold = 0.5f; BlurAmount = 8; BloomIntensity = 2; BaseIntensity = 1; BloomSaturation = 0; BaseSaturation = 1;
                    break;
                case BloomPreset.Saturated:
                    Threshold = 0.25f; BlurAmount = 4; BloomIntensity = 2; BaseIntensity = 1; BloomSaturation = 2; BaseSaturation = 0;
                    break;
               case BloomPreset.Desaturated:
                    Threshold = 0; BlurAmount = 2; BloomIntensity = 1; BaseIntensity = 0.1f; BloomSaturation = 1; BaseSaturation = 1;
                    break;
                case BloomPreset.Subtle:
                    Threshold = 0.25f; BlurAmount = 4; BloomIntensity = 1.25f; BaseIntensity = 1; BloomSaturation = 1; BaseSaturation = 1;
                    break;
                default:
                    Threshold = 0.5f; BlurAmount = 2; BloomIntensity = 1; BaseIntensity = 1; BloomSaturation = 1; BaseSaturation = 1;
                    break;
            }
        }

        protected override void OnBeginApply()
        {
            if (Passes.Count == 0)
            {
                CreatePasses();
            }
            WriteShaderConstants();
            base.OnBeginApply();
        }

        protected override void OnDispose()
        {
            _bloomExtractConstantBuffer.Dispose();
            _guassianHorizontalConstantBuffer.Dispose();
            _guassianVerticalConstantBuffer.Dispose();
            _bloomCombineConstantBuffer.Dispose();
            _quad.Dispose();
            _passThruVertexShader.Dispose();
            _gaussianBlurPixelShader.Dispose();
            _bloomExtractPixelShader.Dispose();
            _bloomCombinePixelShader.Dispose();
            _inputTextureSamplerState.Dispose();
            _bloomExtractTexture.Dispose();
            _bloomExtractShaderResourceView.Dispose();
            _bloomExtractSamplerState.Dispose();
            _bloomExtractRenderTargetView.Dispose();
            _tempTexture.Dispose();
            _tempShaderResourceView.Dispose();
            _tempSamplerState.Dispose();
            _tempRenderTargetView.Dispose();
            _blurTexture.Dispose();
            _blurShaderResourceView.Dispose();
            _blurSamplerState.Dispose();
            _blurRenderTargetView.Dispose();
            base.OnDispose();
        }

        private void CreatePasses()
        {
            PostEffectPass[] passes = new PostEffectPass[4];

            //Create Pass0
            //Input: Input Texture
            //Output: BloomExtract Texture
            //-----------------------------            
            passes[0] = new PostEffectPass(Game.GraphicsDevice);
            passes[0].Name = "Bloom Extract";
            passes[0].VertexShader = _passThruVertexShader;
            passes[0].PixelShader = _bloomExtractPixelShader;
            passes[0].PixelShaderConstantBuffers.Add(_bloomExtractConstantBuffer);
            passes[0].PixelShaderResources.Add(InputTexture);
            passes[0].PixelShaderSamplers.Add(_inputTextureSamplerState);
            passes[0].RenderTarget = _bloomExtractRenderTargetView;
            passes[0].ScreenQuad = _quad;

            //Create Pass1
            //Input: Input Texture
            //Output: Temp Texture
            //--------------------           
            passes[1] = new PostEffectPass(Game.GraphicsDevice);
            passes[1].Name = "Horizontal Blur";
            passes[1].VertexShader = _passThruVertexShader;
            passes[1].PixelShader = _gaussianBlurPixelShader;
            passes[1].PixelShaderConstantBuffers.Add(_guassianHorizontalConstantBuffer);
            passes[1].PixelShaderResources.Add(InputTexture);
            passes[1].PixelShaderSamplers.Add(_inputTextureSamplerState);
            passes[1].RenderTarget = _tempRenderTargetView;
            passes[1].ScreenQuad = _quad;

            //Create Pass2
            //Input: Temp Texture
            //Output: Blur Texture
            //--------------------
            passes[2] = new PostEffectPass(Game.GraphicsDevice);
            passes[2].Name = "Vertical Blur";
            passes[2].VertexShader = _passThruVertexShader;
            passes[2].PixelShader = _gaussianBlurPixelShader;
            passes[2].PixelShaderConstantBuffers.Add(_guassianVerticalConstantBuffer);
            passes[2].PixelShaderResources.Add(_tempShaderResourceView);
            passes[2].PixelShaderSamplers.Add(_tempSamplerState);
            passes[2].RenderTarget = _blurRenderTargetView;
            passes[2].ScreenQuad = _quad;

            //Create Pass3
            //Input: Bloom Extract Texture
            //       Blur Texture
            //Output: 
            //----------------------------
            passes[3] = new PostEffectPass(Game.GraphicsDevice);
            passes[3].Name = "Combine";
            passes[3].VertexShader = _passThruVertexShader;
            passes[3].PixelShader = _bloomCombinePixelShader;
            passes[3].PixelShaderConstantBuffers.Add(_bloomCombineConstantBuffer);
            passes[3].PixelShaderResources.Add(_bloomExtractShaderResourceView);
            passes[3].PixelShaderSamplers.Add(_bloomExtractSamplerState);
            passes[3].PixelShaderResources.Add(_blurShaderResourceView);
            passes[3].PixelShaderSamplers.Add(_blurSamplerState);
            passes[3].RenderTarget = OutputTexture;
            passes[3].ScreenQuad = _quad;

            //Add passes to the post effect
            //-----------------------------
            Passes.Clear();
            Passes.AddRange(passes);
        }

        private void Initialize()
        {
            //Create Screen Quad
            //------------------
            _quad = ContentBuilder.BuildViewportQuad(Game.GraphicsDevice, _vertexShaderByteCode);

            //Create Constant Buffers
            //------------------------
            _bloomExtractConstantBuffer = new SharpDX.Direct3D11.Buffer(Game.GraphicsDevice, BloomExtractConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _guassianHorizontalConstantBuffer = new SharpDX.Direct3D11.Buffer(Game.GraphicsDevice, GuassianConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _guassianVerticalConstantBuffer = new SharpDX.Direct3D11.Buffer(Game.GraphicsDevice, GuassianConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _bloomCombineConstantBuffer = new SharpDX.Direct3D11.Buffer(Game.GraphicsDevice, BloomCombineConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

            //Create descriptions for textures and samplers
            //---------------------------------------------
            Size2 gameScreenSize = new Size2(_game.RenderTargetView.GetTextureDescription().Width, _game.RenderTargetView.GetTextureDescription().Height);
            Texture2DDescription textureDesc = CreateCommonTextureDesc(gameScreenSize);
            ShaderResourceViewDescription shaderResourceViewDesc = CreateCommonShaderResourceViewDesc(textureDesc);
            RenderTargetViewDescription renderTargetViewDesc = CreateCommonRenderTargetViewDesc(textureDesc);
            SamplerStateDescription samplerStateDesc = CreateCommonSamplerStateDesc();
            
            //Input Texture...
            _inputTextureSamplerState = new SamplerState(Game.GraphicsDevice, samplerStateDesc);
            
            //Bloom Extract Texture...
            _bloomExtractTexture = new Texture2D(Game.GraphicsDevice, textureDesc);
            _bloomExtractShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, _bloomExtractTexture, shaderResourceViewDesc);
            _bloomExtractSamplerState = new SamplerState(Game.GraphicsDevice, samplerStateDesc);
            _bloomExtractRenderTargetView = new RenderTargetView(Game.GraphicsDevice, _bloomExtractTexture, renderTargetViewDesc); 
    
            //Temp Texture...
            _tempTexture = new Texture2D(Game.GraphicsDevice, textureDesc);
            _tempShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, _tempTexture, shaderResourceViewDesc);
            _tempSamplerState = new SamplerState(Game.GraphicsDevice, samplerStateDesc);         
            _tempRenderTargetView = new RenderTargetView(Game.GraphicsDevice, _tempTexture, renderTargetViewDesc);            
            
            //Blur Texture...
            _blurTexture = new Texture2D(Game.GraphicsDevice, textureDesc);
            _blurShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, _blurTexture, shaderResourceViewDesc);
            _blurSamplerState = new SamplerState(Game.GraphicsDevice, samplerStateDesc);
            _blurRenderTargetView = new RenderTargetView(Game.GraphicsDevice, _blurTexture, renderTargetViewDesc); 
            
            //Load Shaders
            //-------------
            _vertexShaderByteCode = LoadVertexShader("Assets\\Shaders\\postpassthru-vs.cso", out _passThruVertexShader);
            LoadPixelShader("Assets\\Shaders\\msbloom-guassian-ps.cso", out _gaussianBlurPixelShader);
            LoadPixelShader("Assets\\Shaders\\msbloom-extract-ps.cso", out _bloomExtractPixelShader);           
            LoadPixelShader("Assets\\Shaders\\msbloom-combine-ps.cso", out _bloomCombinePixelShader);                
        }

        private void WriteShaderConstants()
        {
            DataBox dataBox;
            int inputTextureWidth = InputTexture.GetTextureDescription().Width;
            int inputTextureHeight = InputTexture.GetTextureDescription().Height;

            //Write to BloomExtract Buffer
            dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_bloomExtractConstantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            float bloomThreshold = this.Threshold;           
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref bloomThreshold);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_bloomExtractConstantBuffer, 0);

            //Write to Guassian Horizontal Buffer
            dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_guassianHorizontalConstantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            WriteAndPositionGuassianConstants(dataBox.DataPointer, 1.0f / inputTextureWidth, 0);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_guassianHorizontalConstantBuffer, 0);
      
            //Write to Guassian Vertical Buffer
            dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_guassianVerticalConstantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            WriteAndPositionGuassianConstants(dataBox.DataPointer, 0, 1.0f / inputTextureHeight);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_guassianVerticalConstantBuffer, 0);

            //Write to BloomCombine Buffer
            dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_bloomCombineConstantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            float bloomIntensity = this.BloomIntensity;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref bloomIntensity);
            float baseIntensity = this.BaseIntensity;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref baseIntensity);
            float bloomSaturation = this.BloomSaturation;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref bloomSaturation);
            float baseSaturation = this.BaseSaturation;           
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref baseSaturation);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_bloomCombineConstantBuffer, 0);
        }

        private IntPtr WriteAndPositionGuassianConstants(IntPtr pointer, float dx, float dy)
        {
            //************************************************************************
            //CREDITS:
            //The code in this method was ported from the Microsoft XNA Bloom Sample
            //Link: http://xbox.create.msdn.com/en-US/education/catalog/sample/bloom
            //************************************************************************

            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = SampleCount;

            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            for (int i = 0; i < sampleOffsets.Length; i++)
            {
                // Vector2 offset = sampleOffsets[i]; Fix for this memory misalignment is below - in hlsl, each element is 4 bytes wide.
                Vector4 offset = new Vector4(sampleOffsets[i], 0, 0);
                pointer = Utilities.WriteAndPosition<Vector4>(pointer, ref offset);
            }

            for (int i = 0; i < sampleWeights.Length; i++)
            {
                // float weight = sampleWeights[i]; Fix for this memory misalignment is below - in hlsl, each element is 4 bytes wide.
                Vector4 weight = new Vector4(sampleWeights[i], 0, 0, 0);
                pointer = Utilities.WriteAndPosition<Vector4>(pointer, ref weight);
            }

            return pointer;
        }

        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>
        private float ComputeGaussian(float n)
        {
            //************************************************************************
            //CREDITS:
            //The code in this method was ported from the Microsoft XNA Bloom Sample
            //Link: http://xbox.create.msdn.com/en-US/education/catalog/sample/bloom
            //************************************************************************

            float theta = BlurAmount;
            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }

        private static int CalculateRequiredConstantBufferSize(int minimumSize)
        {
            return minimumSize + (32 - minimumSize % 32);
        }
    }

    public enum BloomPreset
    {
        Default,
        Soft,
        Saturated,
        Desaturated,
        Blurry,
        Subtle
    }
}
