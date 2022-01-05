using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public class StreakEffect : SurfaceEffect
    {
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private byte[] _vertexShaderByteCode = null;
        private SharpDX.Direct3D11.Buffer _vertexConstantBuffer = null;
        private SharpDX.Direct3D11.Buffer _pixelConstantBuffer = null;
        private const int VertexConstantsBufferSize = SizeOfMatrix * 2;
        private const int PixelConstantsBufferSize = SizeOfMatrix * 2;

        public StreakEffect(IGameApp game)
            : base(game)
        {
            CreateShaders();
        }

        private void CreateShaders()
        {
            _vertexShaderByteCode = LoadVertexShader("Resources\\Shaders\\streak_vs.cso", out _vertexShader);
            LoadPixelShader("Resources\\Shaders\\streak_ps.cso", out _pixelShader);
            _vertexConstantBuffer = new SharpDX.Direct3D11.Buffer(Game.GraphicsDevice, VertexConstantsBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _pixelConstantBuffer = new SharpDX.Direct3D11.Buffer(Game.GraphicsDevice, PixelConstantsBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        public override void Apply()
        {
            Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);           
            SetShaderConstants();
            Game.GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _vertexConstantBuffer);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _pixelConstantBuffer);         
        }

        private void SetShaderConstants()
        {
            Matrix worldViewProjection = this.World * this.View * this.Projection;
            worldViewProjection.Transpose();
            
            Matrix view = this.View;
            view.Transpose();

            //Vertex shader constants
            //-----------------------
            DataBox dataBox = Game.GraphicsDevice.ImmediateContext.MapSubresource(_vertexConstantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);                        
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref worldViewProjection);                        
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref view);
            Game.GraphicsDevice.ImmediateContext.UnmapSubresource(_vertexConstantBuffer, 0);

            //Pixel shader constants
            //----------------------
            dataBox = Game.GraphicsDevice.ImmediateContext.MapSubresource(_pixelConstantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);         
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref worldViewProjection);       
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref view);
            Game.GraphicsDevice.ImmediateContext.UnmapSubresource(_pixelConstantBuffer, 0);
        }

        public override byte[] GetVertexShaderByteCode()
        {
            return _vertexShaderByteCode;
        }
    }
}