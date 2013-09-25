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
        private IGameApp _game = null;
        private byte[] _vertexShaderByteCode = null;
        private Mesh _quad = null;
        private SharpDX.Direct3D11.Buffer _constantBuffer0 = null;
        private const int ConstantBufferSize0 = 16;
        private SharpDX.Direct3D11.Buffer _constantBuffer1 = null;
        private readonly int ConstantBufferSize1;
        private const int SampleCount = 15;

        public BloomPostEffect(Device graphicsDevice, IGameApp app)
            : base(graphicsDevice)
        {
            int actualRequiredBuffer1Size = (Vector2.SizeInBytes * SampleCount) + (sizeof(float) * SampleCount);
            ConstantBufferSize1 = Effect.CalculateRequiredConstantBufferSize(actualRequiredBuffer1Size); 
            _game = app;
            _constantBuffer0 = new SharpDX.Direct3D11.Buffer(GraphicsDevice, ConstantBufferSize0, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _constantBuffer1 = new SharpDX.Direct3D11.Buffer(GraphicsDevice, ConstantBufferSize1, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            CreateShaders();
            CreateResources();
        }

        public override void Apply()
        {
            base.Apply();
        }

          private void CreateShaders()
        {
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\postglowfs-x1-vs.cso", out _vertexShaderP0);                       
            LoadPixelShader("Content\\Shaders\\postglowfs-x1x2-ps.cso", out _pixelShaderP0P1);
            LoadVertexShader("Content\\Shaders\\postglowfs-x2-vs.cso", out _vertexShaderP1);
            LoadVertexShader("Content\\Shaders\\postglowfs-x3-vs.cso", out _vertexShaderP2);
            LoadPixelShader("Content\\Shaders\\postglowfs-x3-ps.cso", out _pixelShaderP2);           
        }

          private void CreateResources()
          {
              _quad = ContentBuilder.BuildViewportQuad(_game, _vertexShaderByteCode);
          }
    }
}
