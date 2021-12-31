using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class FlexboardEffect : SurfaceEffect
    {
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private byte[] _vertexShaderByteCode = null;
        private SharpDX.Direct3D11.Buffer _constantBuffer = null;

        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }

        public FlexboardEffect(IGameApp game)
            : base(game)
        {
            ForegroundColor = Color.Transparent;
            BackgroundColor = Color.Transparent;
            World = Matrix.Identity;
            View = Matrix.Identity;
            Projection = Matrix.Identity;

            string psFileName = "Content\\Shaders\\flexboard-ps.cso";
            string vsFileName = "Content\\Shaders\\flexboard-vs.cso";
            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            _vertexShader = new VertexShader(Game.GraphicsDevice, _vertexShaderByteCode);
            _pixelShader = new PixelShader(Game.GraphicsDevice, System.IO.File.ReadAllBytes(psFileName));
            int bufferSize = sizeof(float) * 16 * 3; //size of WorldViewProj + ForegroundColor + BackgroundColor
            _constantBuffer = new SharpDX.Direct3D11.Buffer(Game.GraphicsDevice, bufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        public override void Apply()
        {
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            SetShaderConstants();
            Game.GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantBuffer);
            //Game.GraphicsDevice.ImmediateContext.OutputMerger.BlendState = oldBlendState;
        }

        public override byte[] GetVertexShaderByteCode()
        {
            return _vertexShaderByteCode;
        }

        private void SetShaderConstants()
        {
            DataBox dataBox = Game.GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Matrix worldViewProjection = this.World * this.View * this.Projection;
            worldViewProjection.Transpose();
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref worldViewProjection);
            Matrix view = this.View;
            view.Transpose();
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref view);
            Vector4 vForegroundColor = ForegroundColor.ToVector4();
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector4>(dataBox.DataPointer, ref vForegroundColor);
            Vector4 vBackgroundColor = BackgroundColor.ToVector4();
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector4>(dataBox.DataPointer, ref vBackgroundColor);
            Game.GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer, 0);
        }
    }
}
