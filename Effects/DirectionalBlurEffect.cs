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
    public class DirectionalBlurEffect : PostEffect
    {
        private IGameApp _game = null;
        private readonly int GuassianConstantBufferSize;

        public DirectionalBlurEffect(Device graphicsDevice, IGameApp app) : base(graphicsDevice)
        {
            _game = app;
        }

        private void CreateShaders()
        {
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\passthru-vs.cso", out _passThruVertexShader);
            LoadPixelShader("Content\\Shaders\\msbloom-guassian-ps.cso", out _gaussianBlurPixelShader);
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
}
