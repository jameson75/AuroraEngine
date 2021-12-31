using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Content;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public class SkysphereEffect : SurfaceEffect
    {
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private byte[] _vertexShaderByteCode = null;
        private SharpDX.Direct3D11.Buffer _constantBuffer = null;
        private IGameApp _game = null;      
        private ShaderResourceView _environmentMapShaderResourceView = null;
        private SamplerState _environmentMapSamplerState = null;
        private bool _isRestoreRequired = false;
        private RasterizerState _oldRasterizerState = null;
        private DepthStencilState _oldDepthStencilState = null;

        public SkysphereEffect(IGameApp game) : base(game)
        {
            _game = game;
            string psFileName = @"Content\Shaders\skysphere-ps.cso";
            string vsFileName = @"Content\Shaders\skysphere-vs.cso";
            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            _vertexShader = new VertexShader(game.GraphicsDevice, _vertexShaderByteCode);
            _pixelShader = new PixelShader(game.GraphicsDevice, System.IO.File.ReadAllBytes(psFileName));
            int bufferSize = sizeof(float) * 16 * 2; //Size of View and Projection matrices.
            _constantBuffer = new SharpDX.Direct3D11.Buffer(game.GraphicsDevice, bufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            SamplerStateDescription samplerStateDesc = SamplerStateDescription.Default();
            samplerStateDesc.Filter = Filter.MinMagMipLinear;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;
            samplerStateDesc.AddressV = TextureAddressMode.Clamp;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;
            _environmentMapSamplerState = new SamplerState(game.GraphicsDevice, samplerStateDesc);
        }

        public override void Apply()
        {

            if (_environmentMapShaderResourceView == null)
                throw new InvalidOperationException("Environment map not set.");

            ///////////
            //Setup
            ///////////
            //Write shader varibles to constant buffer.
            WriteShaderConstants();
            //Cache render state and setup a new one.
            _oldRasterizerState = GraphicsDevice.ImmediateContext.Rasterizer.State;
            RasterizerStateDescription newRasterizerStateDesc = (_oldRasterizerState != null) ? _oldRasterizerState.Description : RasterizerStateDescription.Default();
            newRasterizerStateDesc.CullMode = CullMode.None;
            GraphicsDevice.ImmediateContext.Rasterizer.State = new RasterizerState(GraphicsDevice, newRasterizerStateDesc);         
            _oldDepthStencilState = GraphicsDevice.ImmediateContext.OutputMerger.DepthStencilState;
            DepthStencilStateDescription newDepthStencilStateDesc = (_oldDepthStencilState != null) ? _oldDepthStencilState.Description : DepthStencilStateDescription.Default();
            newDepthStencilStateDesc.IsDepthEnabled = true;
            newDepthStencilStateDesc.DepthWriteMask = 0;
            GraphicsDevice.ImmediateContext.OutputMerger.DepthStencilState = new DepthStencilState(GraphicsDevice, newDepthStencilStateDesc);
            _isRestoreRequired = true;

            /////////
            //Pass0
            /////////                     
            //Input: Environment Map
            //Output: Render Target
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _environmentMapShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _environmentMapSamplerState);
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantBuffer);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);            
        }     

        public override void Restore()
        {
            if (_isRestoreRequired)
            {
                GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
                GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
                GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);
                GraphicsDevice.ImmediateContext.VertexShader.Set(null);
                GraphicsDevice.ImmediateContext.PixelShader.Set(null);
                GraphicsDevice.ImmediateContext.Rasterizer.State = _oldRasterizerState;
                GraphicsDevice.ImmediateContext.OutputMerger.DepthStencilState = _oldDepthStencilState;
            }
        }

        public override byte[] GetVertexShaderByteCode()
        {
            return _vertexShaderByteCode;
        }

        public void SetTexture(ShaderResourceView texture)
        {
            _environmentMapShaderResourceView = texture;
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

        protected override void OnDispose()
        {
            _vertexShader?.Dispose();
            _pixelShader?.Dispose();
            _constantBuffer?.Dispose();
            _environmentMapSamplerState?.Dispose();
            _environmentMapShaderResourceView?.Dispose();
            base.OnDispose();
        }
    }
}
