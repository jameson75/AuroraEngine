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
    public class FlexboardEffect : SurfaceEffect
    {
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private byte[] _vertexShaderByteCode = null;   
        private SharpDX.Direct3D11.Buffer _constantBuffer = null;

        public FlexboardEffect(IGameApp game)
            : base(game)        
        {      
            World = Matrix.Identity;
            View = Matrix.Identity;
            Projection = Matrix.Identity;
            string psFileName = "Content\\Shaders\\flexboard-ps.cso";
            string vsFileName = "Content\\Shaders\\flexboard-vs.cso";
            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            _vertexShader = new VertexShader(Game.GraphicsDevice, _vertexShaderByteCode);
            _pixelShader = new PixelShader(Game.GraphicsDevice, System.IO.File.ReadAllBytes(psFileName));
            int bufferSize = sizeof(float) * 16  * 3; //size of WorldViewProj + View + Proj
            _constantBuffer = new SharpDX.Direct3D11.Buffer(game.GraphicsDevice, bufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        public override void Apply()
        {
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            SetShaderConstants();
            Game.GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantBuffer);
        }

        public override byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;
        }

        private void SetShaderConstants()
        {
            DataBox dataBox = Game.GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Matrix worldViewProjection = this.World * this.View;
            worldViewProjection.Transpose();
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref worldViewProjection);
            Matrix view = this.View;
            view.Transpose();
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref view);
            Matrix projection = this.Projection;
            projection.Transpose();
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref projection);
            Game.GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer, 0);
        }
    }
}
