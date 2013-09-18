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
    public class SkysphereEffect : Effect
    {
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private byte[] _vertexShaderByteCode = null;
        private SharpDX.Direct3D11.Buffer _constantBuffer = null;

        public SkysphereEffect(Device graphicsDevice) : base(graphicsDevice)
        {
            string psFileName = "Content\\Shaders\\skysphere-ps.cso";
            string vsFileName = "Content\\Shaders\\skysphere-vs.cso";
            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            _vertexShader = new VertexShader(GraphicsDevice, _vertexShaderByteCode);
            _pixelShader = new PixelShader(GraphicsDevice, System.IO.File.ReadAllBytes(psFileName));
            int bufferSize = sizeof(float) * 16 * 2; //Size of View + Projection matrices.
            _constantBuffer = new SharpDX.Direct3D11.Buffer(graphicsDevice, bufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        public override void Apply()
        {
            /////////
            //Pass0
            /////////                     
            //Input: SceneTexture
            //Output: GlowMap1
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _sceneTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _sceneTextureSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_glowMap1RenderTargetView);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderP0);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShaderP0P1);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

        }

        public override byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;
        }

        protected void WriteShaderConstants()
        {
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_constantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Matrix tView = this.View;
            tView.Transpose(); 
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref tView);
            Matrix tProjection = this.Projection;
            tProjection.Transpose();
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref tProjection);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_constantBuffer, 0);   
        }
    }
}
