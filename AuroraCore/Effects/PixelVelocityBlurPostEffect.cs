using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.Content;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public class PixelVelocityBlurPostEffect : PostEffect
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

        private Matrix _previousWorld = Matrix.Identity;
        private Matrix _previousView = Matrix.Identity;
        private Matrix _previousProjection = Matrix.Identity;

        public virtual Matrix World { get; set; }

        public virtual Matrix View { get; set; }

        public virtual Matrix Projection { get; set; }
       
        public Color MaterialAmbient { get; set; }

        public Color MaterialDiffuse { get; set; }
         /// <summary>
        /// <remarks>Default: (1,1,1,1)</remarks>
        /// </summary>
        public Color LightAmbient { get; set; }
        /// <summary>
        /// <remarks>Default: (1,1,1,1)</remarks>
        /// </summary>
        public Color LightDiffuse { get; set; }
        /// <summary>
        /// <remarks>Default: (1,1,1)</remarks>
        /// </summary>
        public Vector3 LightDir { get; set; }

        public float BlurAmount { get; set; }

        public PixelVelocityBlurPostEffect(IGameApp game)
            : base(game)
        {
            _game = game;
            LightAmbient = new Color(1, 1, 1, 1);
            LightDiffuse = new Color(1, 1, 1, 1);
            LightDir = new Vector3(1, 1, 1);
            BlurAmount = 1.0f;
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
            _quad.Draw();
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
            _quad.Draw();
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
                _quad.Draw();
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
            _quad.Draw();
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.VertexShader.Set(null);
            GraphicsDevice.ImmediateContext.PixelShader.Set(null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);
            _lastVelocityTextureFilled = true;

            //////////////////////////////////
            //Restore State
            //////////////////////////////////
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_originalDepthStencilView, _originalRenderTargetView);

            /////////////////////////////////////////////////////////////////////////////////////
            //Cache world, view and projection matrices used (in WriteConstants()) for this call. 
            /////////////////////////////////////////////////////////////////////////////////////
            _previousWorld = this.World;
            _previousView = this.View;
            _previousProjection = this.Projection;

            //COM Release swap chain resources
            _originalDepthStencilView.Dispose();
            _originalRenderTargetView.Dispose();

            _originalRenderTargetView = null;
            _originalDepthStencilView = null;
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
            textureDesc.Height = _game.RenderTargetView.GetTextureDescription().Height;
            textureDesc.Width = _game.RenderTargetView.GetTextureDescription().Width;
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
            
            //MeshTexture
            //------------
            _meshTextureSampler = new SamplerState(GraphicsDevice, samplerStateDesc);

            //Color
            //-----
            Texture2D colorTexture = new Texture2D(_game.GraphicsDevice, textureDesc);
            _colorTextureShaderResourceView = new ShaderResourceView(GraphicsDevice, colorTexture, resourceViewDesc);
            _colorTextureRenderTargetView = new RenderTargetView(GraphicsDevice, colorTexture, renderTargetViewDesc);
            _colorTextureSampler = new SamplerState(GraphicsDevice, samplerStateDesc);

            //Currrent Velocity
            //-----------------
            Texture2D _currentVelocityTexture = new Texture2D(_game.GraphicsDevice, highPercisionTextureDesc);
            _currentVelocityTextureShaderResourceView = new ShaderResourceView(GraphicsDevice, _currentVelocityTexture, resourceViewDesc);
            _currentVelocityTextureRenderTargetView = new RenderTargetView(GraphicsDevice, _currentVelocityTexture, renderTargetViewDesc);
            _currentVelocityTextureSampler = new SamplerState(GraphicsDevice, samplerStateDesc);

            //Last Velocity
            //-------------
            Texture2D _lastVelocityTexture = new Texture2D(_game.GraphicsDevice, highPercisionTextureDesc);
            _lastVelocityTextureShaderResourceView = new ShaderResourceView(GraphicsDevice, _lastVelocityTexture, resourceViewDesc);
            _lastVelocityTextureRenderTargetView = new RenderTargetView(GraphicsDevice, _lastVelocityTexture, renderTargetViewDesc);
            _lastVelocityTextureSampler = new SamplerState(GraphicsDevice, samplerStateDesc);
        }

        private void CreateShaders()
        {
            _vertexShaderByteCode = LoadVertexShader("Resources\\Shaders\\pixel-velocity-blur-calc-vs.cso", out _worldVertexShader);
            LoadVertexShader("Resources\\Shaders\\postpassthru-vs.cso", out _passThruVertexShader);
            LoadPixelShader("Resources\\Shaders\\pixel-velocity-blur-clr-ps.cso", out _worldColorPixelShader);
            LoadPixelShader("Resources\\Shaders\\pixel-velocity-blur-vel-ps.cso", out _worldVelocityPixelShader);
            LoadPixelShader("Resources\\Shaders\\pixel-velocity-blur-blr-ps.cso", out _blurPixelShader);
            LoadPixelShader("Resources\\Shaders\\postpassthru-ps.cso", out _passThruPixelShader);
        }

        private void WriteConstants()
        {
            DataBox dataBox;

            dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_worldVertexConstantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            //mWorld
            Matrix world = this.World;
            world.Transpose();
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref world);
            //mWorldViewProjection
            Matrix worldViewProjection = this.World * this.View * this.Projection;
            worldViewProjection.Transpose();
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref worldViewProjection);
            //mWorldViewProjectionLast
            Matrix worldViewProjectionLast = this._previousWorld * this._previousView * this._previousProjection;
            worldViewProjection.Transpose();
            dataBox.DataPointer = Utilities.WriteAndPosition<Matrix>(dataBox.DataPointer, ref worldViewProjection);           
            //MaterialAmbientColor
            Vector4 materialAmbient = MaterialAmbient.ToVector4();
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector4>(dataBox.DataPointer, ref materialAmbient);
            //MaterialDiffuseColor
            Vector4 materialDiffuse = MaterialDiffuse.ToVector4();
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector4>(dataBox.DataPointer, ref materialDiffuse);
            //LightAmbient
            Vector4 lightAmbient = LightAmbient.ToVector4();
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector4>(dataBox.DataPointer, ref lightAmbient);
            //LightDiffuse
            Vector4 lightDiffuse = LightDiffuse.ToVector4();
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector4>(dataBox.DataPointer, ref lightDiffuse);
            //LightDir
            Vector3 lightDir = Vector3.Normalize(LightDir);
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector3>(dataBox.DataPointer, ref lightDir);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_worldVertexConstantBuffer, 0);
            

            dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_blurPixelConstantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            //PixelBlurConst
            float blurAmount = this.BlurAmount;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref blurAmount);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_blurPixelConstantBuffer, 0);         
        }
    }
}
