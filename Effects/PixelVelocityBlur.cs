using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
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
    public class PixelVelocityBlur : PostEffect
    {
        private const int WorldVertexConstantBufferSize = 80;
        private const int BlurVertexConstantBufferSize = 16;

        private IGameApp _game = null;
        private byte[] _vertexShaderByteCode = null;
        private VertexShader _worldVertexShader = null;
        private VertexShader _passThruVertexShader = null;
        private PixelShader _worldVelocityPixelShader = null;
        private PixelShader _worldColorPixelShader = null;
        private PixelShader _blurPixelShader = null;
        private PixelShader _passThruPixelShader = null;
        private SharpDX.Direct3D11.Buffer _worldVertexConstantBuffer = null;
        private SharpDX.Direct3D11.Buffer _blurPixelConstantBuffer = null;   
        private SamplerState _meshTextureSampler = null;
        private ShaderResourceView _colorTextureShaderResourceView = null;
        private SamplerState _colorTextureSampler = null;
        private RenderTargetView _colorTextureRenderTargetView = null;
        private RenderTargetView _currentVelocityTextureRenderTargetView = null;
        private ShaderResourceView _currentVelocityTextureShaderResourceView = null;
        private SamplerState _currentVelocityTextureSampler = null;
        private RenderTargetView _lastVelocityTextureRenderTargetView = null;
        private ShaderResourceView _lastVelocityTextureShaderResourceView = null;
        private SamplerState _lastVelocityTextureSampler = null;
        private RenderTargetView _originalRenderTargetView = null;
        private DepthStencilView _originalDepthStencilView = null;
        private Mesh _quad = null;
        private bool _lastVelocityTextureFilled = false;

        public PixelVelocityBlur(Device graphicsDevice, IGameApp game)
            : base(graphicsDevice)
        {
            _game = game;
            CreateConstantBuffers();
            CreateTextures();
            CreateShaders();
        }

        public override void Apply()
        {            
            //////////////////////////////////
            //Cache State
            //////////////////////////////////
             _originalRenderTargetView = GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(0, out _originalDepthStencilView)[0];

            //////////////////////////////////
            //Populate Constant Buffers
            //////////////////////////////////
            WriteConstants();

            //////////////////////////////////
            //Pass 0
            //Input: Scene (Input) Texture
            //Output: Color Texture
            //////////////////////////////////           
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _worldVertexConstantBuffer);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _meshTextureSampler);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_worldVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_worldColorPixelShader);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_colorTextureRenderTargetView);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            GraphicsDevice.ImmediateContext.VertexShader.Set(null);
            GraphicsDevice.ImmediateContext.PixelShader.Set(null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            //////////////////////////////////
            //Pass 1
            //Input: Scene (Input) Texture
            //Output: Velocity Texture
            //////////////////////////////////
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _worldVertexConstantBuffer);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _meshTextureSampler);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_worldVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_worldVelocityPixelShader);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_currentVelocityTextureRenderTargetView);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            GraphicsDevice.ImmediateContext.VertexShader.Set(null);
            GraphicsDevice.ImmediateContext.PixelShader.Set(null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            //NOTE: We only execute this pass and attempt to render a blurred image if we have already
            //stored a "previous/last" velocity texture.
            if (_lastVelocityTextureFilled)
            {
                //////////////////////////////////
                //Pass 2
                //Input: Color Texture,
                //       Last Velocity Texture,
                //       Current Velocity Texture
                //Output: Render Target         
                //////////////////////////////////      
                
                GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _blurPixelConstantBuffer);
                GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _colorTextureShaderResourceView);
                GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _colorTextureSampler);
                GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, _currentVelocityTextureShaderResourceView);
                GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, _currentVelocityTextureSampler);
                GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(2, _lastVelocityTextureShaderResourceView);
                GraphicsDevice.ImmediateContext.PixelShader.SetSampler(2, _lastVelocityTextureSampler);
                GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruVertexShader);
                GraphicsDevice.ImmediateContext.PixelShader.Set(_blurPixelShader);
                GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_originalRenderTargetView);
                _quad.Draw(0);
                GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);
                GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
                GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
                GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, null);
                GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, null);
                GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(2, null);
                GraphicsDevice.ImmediateContext.PixelShader.SetSampler(2, null);
                GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruVertexShader);
                GraphicsDevice.ImmediateContext.PixelShader.Set(_blurPixelShader);
                GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_originalRenderTargetView);
            }

            ///////////////////////////////////
            //Pass 3
            //Input: Current Velocity Texture
            //Output: Last Velocity Texture
            ///////////////////////////////////
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _currentVelocityTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_passThruPixelShader);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_lastVelocityTextureRenderTargetView);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.VertexShader.Set(null);
            GraphicsDevice.ImmediateContext.PixelShader.Set(null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);
            _lastVelocityTextureFilled = true;

            //////////////////////////////////
            //Restore State
            //////////////////////////////////
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_originalDepthStencilView, _originalRenderTargetView);
        }

        private void CreateConstantBuffers()
        {
            _worldVertexConstantBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, WorldVertexConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _blurPixelConstantBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice,  BlurVertexConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);     
        }

        private void CreateTextures()
        {
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

            Texture2DDescription highPercisionTextureDesc = textureDesc;
            highPercisionTextureDesc.Format = SharpDX.DXGI.Format.R16G16_Float;

            ShaderResourceViewDescription resourceViewDesc = new ShaderResourceViewDescription();
            resourceViewDesc.Format = textureDesc.Format;
            resourceViewDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            resourceViewDesc.Texture2D.MostDetailedMip = 0;
            resourceViewDesc.Texture2D.MipLevels = 1;

            RenderTargetViewDescription renderTargetViewDesc = new RenderTargetViewDescription();
            renderTargetViewDesc.Format = textureDesc.Format;
            renderTargetViewDesc.Dimension = RenderTargetViewDimension.Texture2D;
            renderTargetViewDesc.Texture2D.MipSlice = 0;

            SamplerStateDescription samplerStateDesc = SamplerStateDescription.Default();
            samplerStateDesc.Filter = Filter.MinMagMipLinear;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;
            samplerStateDesc.AddressV = TextureAddressMode.Clamp;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;

            //TODO: Create shader resources here...
        }

        private void CreateShaders()
        {
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\pixel-velocity-blur-calc-vs.cso", out _worldVertexShader);
            LoadVertexShader("Content\\Shaders\\postpassthru-vs.cso", out _passThruVertexShader);
            LoadPixelShader("Content\\Shaders\\pixel-velocity-blur-clr-ps.cso", out _worldColorPixelShader);
            LoadPixelShader("Content\\Shaders\\pixel-velocity-blur-vel-ps.cso", out _worldVelocityPixelShader);
            LoadPixelShader("Content\\Shaders\\pixel-velocity-blur-blr-ps.cso", out _blurPixelShader);
            LoadPixelShader("Content\\Shaders\\postpassthru-ps.cso", out _passThruPixelShader);
        }

        private void WriteConstants()
        {
            //TODO: Write shader constants here...
        }
    }
}
