using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
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
    public class BloomPostEffect : PostEffect
    {
        private const int SampleCount = 15;
        private const int BloomExtractConstantBufferSize = 16;
        private const int BloomCombineConstantBufferSize = 16;
        private readonly int GuassianConstantBufferSize;

        private IGameApp _game = null;
        private byte[] _vertexShaderByteCode = null;
        private Mesh _quad = null;
        private SharpDX.Direct3D11.Buffer _bloomExtractConstantBuffer = null;
        private SharpDX.Direct3D11.Buffer _guassianHorizontalConstantBuffer = null;
        private SharpDX.Direct3D11.Buffer _guassianVerticalConstantBuffer = null;
        private SharpDX.Direct3D11.Buffer _bloomCombineConstantBuffer = null;
        private VertexShader _passThruVertexShader = null;
        private PixelShader _gaussianBlurPixelShader = null;
        private PixelShader _bloomExtractPixelShader = null;
        private PixelShader _bloomCombinePixelShader = null;

        public float Threshold { get; set; }
        public float BlurAmount { get; set; }
        public float BloomIntensity { get; set; }
        public float BaseIntensity { get; set; }
        public float BloomSaturation { get; set; }
        public float BaseSaturation { get; set; }

        public BloomPostEffect(Device graphicsDevice, IGameApp app)
            : base(graphicsDevice)
        {
            GuassianConstantBufferSize = Effect.CalculateRequiredConstantBufferSize((Vector2.SizeInBytes * SampleCount) + (sizeof(float) * SampleCount));
            _game = app;
            CreateConstantBuffers();
            CreateShaders();
            CreateResources();
        }

        public void InitializeWithPreset(BloomPreset preset)
        {
            switch (preset)
            {
                case BloomPreset.Soft:
                    Threshold = 0; BlurAmount = 3; BloomIntensity = 1; BaseIntensity = 1; BloomSaturation = 1; BaseSaturation = 1;
                    break;
                case BloomPreset.Desaturated:
                    Threshold = 0.5f; BlurAmount = 8; BloomIntensity = 2; BaseIntensity = 1; BloomSaturation = 0; BaseSaturation = 1;
                    break;
                case BloomPreset.Saturated:
                    Threshold = 0.25f; BlurAmount = 4; BloomIntensity = 2; BaseIntensity = 1; BloomSaturation = 2; BaseSaturation = 0;
                    break;
                case BloomPreset.Blurry:
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

        public override void Apply()
        {
            //TODO: This method is incomplete. Needs finishing.
            //-------------------------------------------------

            /////////////
            //Pass0
            /////////////
            //Input: Scene Texture
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _bloomExtractConstantBuffer);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_bloomExtractPixelShader);
            _quad.Draw(0);

            /////////////
            //Pass1
            /////////////
            //Input: 
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _guassianHorizontalConstantBuffer);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_gaussianBlurPixelShader);
            _quad.Draw(0);

            /////////////
            //Pass2
            /////////////
            //Input: 
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _guassianVerticalConstantBuffer);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_gaussianBlurPixelShader);
            _quad.Draw(0);

            /////////////
            //Pass3
            /////////////
            //Input: 
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _bloomCombineConstantBuffer);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_bloomCombinePixelShader);
            _quad.Draw(0);   
        }

        private void CreateConstantBuffers()
        {
            _bloomExtractConstantBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, BloomExtractConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _guassianHorizontalConstantBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, GuassianConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _guassianVerticalConstantBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, GuassianConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _bloomCombineConstantBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, BloomCombineConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        private void CreateShaders()
        {
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\passthru-vs.cso", out _passThruVertexShader);
            LoadPixelShader("Content\\Shaders\\msbloom-guassian-ps.cso", out _gaussianBlurPixelShader);
            LoadPixelShader("Content\\Shaders\\msbloom-extract-ps.cso", out _bloomExtractPixelShader);
            LoadPixelShader("Content\\Shaders\\msbloom-combine-ps.cso", out _bloomCombinePixelShader);
        }

        private void CreateResources()
        {
            _quad = ContentBuilder.BuildViewportQuad(_game, _vertexShaderByteCode);

            Texture2DDescription textureDesc = new Texture2DDescription();
            textureDesc.ArraySize = 1;
            textureDesc.BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource;
            textureDesc.CpuAccessFlags = CpuAccessFlags.None;
            textureDesc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            textureDesc.Height = _game.RenderTarget.ResourceAs<Texture2D>().Description.Height;
            textureDesc.Width = _game.RenderTarget.ResourceAs<Texture2D>().Description.Width;
            textureDesc.MipLevels = 1;
            textureDesc.OptionFlags = ResourceOptionFlags.None;
            textureDesc.Usage = ResourceUsage.Default;
            textureDesc.SampleDescription.Count = 1;

            ShaderResourceViewDescription resourceViewDesc = new ShaderResourceViewDescription();
            resourceViewDesc.Format = textureDesc.Format;
            resourceViewDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            resourceViewDesc.Texture2D.MostDetailedMip = 0;
            resourceViewDesc.Texture2D.MipLevels = 1;

            RenderTargetViewDescription renderTargetViewDesc = new RenderTargetViewDescription();
            renderTargetViewDesc.Format = textureDesc.Format;
            renderTargetViewDesc.Dimension = RenderTargetViewDimension.Texture2D;
            renderTargetViewDesc.Texture2D.MipSlice = 0;

            SamplerStateDescription samplerStateDesc = SamplerStateDescription.Default();
            samplerStateDesc.Filter = Filter.MinMagMipLinear;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;
            samplerStateDesc.AddressV = TextureAddressMode.Clamp;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;

            //TODO: Finish creating resources here.
        }

        private void WriteShaderConstants()
        {
            DataBox dataBox;
            int inputTextureWidth = InputTexture.ResourceAs<Texture2D>().Description.Width;
            int inputTextureHeight = InputTexture.ResourceAs<Texture2D>().Description.Height;

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
            //The code in this method is borrowed from the Microsoft XNA Bloom Sample
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

            for (int i = 0; i < sampleCount; i++)
            {
                Vector2 offset = sampleOffsets[i];
                pointer = Utilities.WriteAndPosition<Vector2>(pointer, ref offset);
            }
            for (int i = 0; i < sampleCount; i++)
            {
                float weight = sampleWeights[i];
                pointer = Utilities.WriteAndPosition<float>(pointer, ref weight);
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
            //The code in this method is borrowed from the Microsoft XNA Bloom Sample
            //Link: http://xbox.create.msdn.com/en-US/education/catalog/sample/bloom
            //************************************************************************

            float theta = BlurAmount;
            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
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
