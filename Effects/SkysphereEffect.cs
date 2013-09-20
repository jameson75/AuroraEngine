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
    public class SkysphereEffect : Effect
    {
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private byte[] _vertexShaderByteCode = null;
        private SharpDX.Direct3D11.Buffer _constantBuffer = null;
        private IGameApp _game = null;
        private Mesh _quad = null;
        private ShaderResourceView _environmentMapShaderResourceView = null;
        private SamplerState _environmentMapSamplerState = null;
        private bool _isRestoreRequired = false;
        private RasterizerState _oldRasterizerState = null;
        private DepthStencilState _oldDepthStencilState = null;

        public SkysphereEffect(Device graphicsDevice, IGameApp game) : base(graphicsDevice)
        {
            _game = game;
            string psFileName = @"Content\Shaders\skysphere-ps.cso";
            string vsFileName = @"Content\Shaders\skysphere-vs.cso";
            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            _vertexShader = new VertexShader(GraphicsDevice, _vertexShaderByteCode);
            _pixelShader = new PixelShader(GraphicsDevice, System.IO.File.ReadAllBytes(psFileName));
            int bufferSize = sizeof(float) * 16 * 2; //Size of View and Projection matrices.
            _constantBuffer = new SharpDX.Direct3D11.Buffer(graphicsDevice, bufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        private void CreateResources()
        {
            _quad = ContentBuilder.BuildViewportQuad(_game, _vertexShaderByteCode);

            Texture2DDescription textureDesc = new Texture2DDescription();
            textureDesc.ArraySize = 1;
            textureDesc.BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource;
            textureDesc.CpuAccessFlags = CpuAccessFlags.None;
            textureDesc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            textureDesc.Height = _game.RenderTarget.ResourceAs<Texture2D>().Description.Height;
            textureDesc.Width = _game.RenderTarget.ResourceAs<Texture2D>().Description.Width;
            textureDesc.MipLevels = 1;
            textureDesc.OptionFlags = ResourceOptionFlags.None;
            textureDesc.Usage = ResourceUsage.Default;
            textureDesc.SampleDescription.Count = 1;          

            ShaderResourceViewDescription resourceViewDesc = new ShaderResourceViewDescription();
            resourceViewDesc.Format = textureDesc.Format;
            resourceViewDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            resourceViewDesc.Texture2D.MostDetailedMip = 0;
            resourceViewDesc.Texture2D.MipLevels = 1;
            
            SamplerStateDescription samplerStateDesc = SamplerStateDescription.Default();
            samplerStateDesc.Filter = Filter.MinMagMipLinear;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;
            samplerStateDesc.AddressV = TextureAddressMode.Clamp;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;

            Texture2D _environmentMapTexture = new Texture2D(_game.GraphicsDevice, textureDesc);
            _environmentMapShaderResourceView = new ShaderResourceView(GraphicsDevice, _environmentMapTexture, resourceViewDesc);            
            _environmentMapSamplerState = new SamplerState(GraphicsDevice, samplerStateDesc); 
        }

        public override void Apply()
        {
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

            /////////
            //Pass0
            /////////                     
            //Input: Environment Map
            //Output: Render Target
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _environmentMapShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _environmentMapSamplerState);
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantBuffer);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);            
        }

        public override void RestoreGraphicsState()
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
