using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Effects
{
    public class StreakEffect : SurfaceEffect
    {
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private byte[] _vertexShaderByteCode = null;
        private SharpDX.Direct3D11.Buffer _vertexConstantBuffer = null;
        private SharpDX.Direct3D11.Buffer _pixelConstantBuffer = null;

        public StreakEffect(IGameApp game)
            : base(game)
        {

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

        public override byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;
        }
    }
}