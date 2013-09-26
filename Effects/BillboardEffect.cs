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
    public class BillboardEffect : Effect
    {
        private VertexShader _vertexShader = null;
        private VertexShader _ivertexShader = null;
        private PixelShader _pixelShader = null;
        private byte[] _vertexShaderByteCode = null;
        private byte[] _ivertexShaderByteCode = null;
        private SharpDX.Direct3D11.Buffer _constantBuffer = null;

        public bool IsBillboardInstanced { get; set; }
        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }

        public BillboardEffect(Device graphicsDevice) : base(graphicsDevice)        
        {      
            ForegroundColor = Color.Transparent;
            BackgroundColor = Color.Transparent;
            World = Matrix.Identity;
            View = Matrix.Identity;
            Projection = Matrix.Identity;

            string psFileName = "Content\\Shaders\\billboard-ps.cso";
            string vsFileName = "Content\\Shaders\\billboard-vs.cso";
            string ivsFileName = "Content\\Shaders\\billboard-i-vs.cso";
            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            _ivertexShaderByteCode = System.IO.File.ReadAllBytes(ivsFileName);
            _vertexShader = new VertexShader(GraphicsDevice, _vertexShaderByteCode);
            _ivertexShader = new VertexShader(GraphicsDevice, _ivertexShaderByteCode);
            _pixelShader = new PixelShader(GraphicsDevice, System.IO.File.ReadAllBytes(psFileName));
            int bufferSize = sizeof(float) * 16  * 3; //size of WorldViewProj + ForegroundColor + BackgroundColor
            _constantBuffer = new SharpDX.Direct3D11.Buffer(graphicsDevice, bufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        public override void Apply()
        {
            //BlendStateDescription blendDesc = BlendStateDescription.Default();
            //for (int i = 0; i < blendDesc.RenderTarget.Length; i++)
            //{
            //    blendDesc.RenderTarget[i].IsBlendEnabled = true;
            //    blendDesc.RenderTarget[i].SourceBlend = BlendOption.SourceAlpha;
            //    blendDesc.RenderTarget[i].SourceAlphaBlend = BlendOption.SourceAlpha;
            //    blendDesc.RenderTarget[i].DestinationBlend = BlendOption.InverseSourceAlpha;
            //    blendDesc.RenderTarget[i].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
            //}
            //BlendState newBlendState = new BlendState(GraphicsDevice, blendDesc);
            //Game.GraphicsDeviceContext.OutputMerger.BlendFactor = Color.Zero;
            //BlendState oldBlendState = GraphicsDevice.ImmediateContext.OutputMerger.BlendState;
            //GraphicsDevice.ImmediateContext.OutputMerger.BlendState = newBlendState;           
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            if (IsBillboardInstanced)
                GraphicsDevice.ImmediateContext.VertexShader.Set(_ivertexShader);
            else
                GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            SetShaderConstants();
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer);
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantBuffer);
            //GraphicsDevice.ImmediateContext.OutputMerger.BlendState = oldBlendState;
        }

        public override byte[] SelectShaderByteCode()
        {
            return (IsBillboardInstanced) ? _ivertexShaderByteCode : _vertexShaderByteCode;
        }

        private void SetShaderConstants()
        {
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
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
            GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer, 0);
        }
    }
}
