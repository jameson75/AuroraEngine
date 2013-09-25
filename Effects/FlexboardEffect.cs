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
    public class FlexboardEffect : Effect
    {
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private byte[] _vertexShaderByteCode = null;   
        private SharpDX.Direct3D11.Buffer _constantBuffer = null;

        public FlexboardEffect(Device graphicsDevice)
            : base(graphicsDevice)        
        {      
            World = Matrix.Identity;
            View = Matrix.Identity;
            Projection = Matrix.Identity;

            string psFileName = "Content\\Shaders\\flexboard-ps.cso";
            string vsFileName = "Content\\Shaders\\flexboard-vs.cso";
            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);           
            _vertexShader = new VertexShader(GraphicsDevice, _vertexShaderByteCode);            
            _pixelShader = new PixelShader(GraphicsDevice, System.IO.File.ReadAllBytes(psFileName));
            int bufferSize = sizeof(float) * 16  * 3; //size of WorldViewProj + View + Proj
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
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            SetShaderConstants();
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer);
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantBuffer);
            //GraphicsDevice.ImmediateContext.OutputMerger.BlendState = oldBlendState;
        }

        public override byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;
        }

        private void SetShaderConstants()
        {
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Matrix worldViewProjection = this.World * this.View;
            worldViewProjection.Transpose();
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref worldViewProjection);
            Matrix view = this.View;
            view.Transpose();
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref view);
            Matrix projection = this.Projection;
            projection.Transpose();
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref projection);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer, 0);
        }
    }
}
