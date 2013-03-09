using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
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
    public class DofEffect : PostEffect
    {
        public const int ConstantsBufferSize = 16;
        public const int ConstantsBufferSize2 = 32;

        private SharpDX.Direct3D11.Buffer _constantsBuffer1 = null;
        private SharpDX.Direct3D11.Buffer _constantsBuffer2 = null;
        private VertexShader _downSampleVertexShader = null;
        private PixelShader _downSamplePixelShader = null;
        private VertexShader _gaussianVertexShader = null;
        private PixelShader _gaussianPixelShader = null;
        private PixelShader _gaussianPixelShader2 = null;
        private VertexShader _depthFinal1VertexShader = null;
        private PixelShader _depthFinal1PixelShader = null;
        private VertexShader _depthFinal2VertexShader = null;
        private PixelShader _depthFinal2PixelShader = null;
        private byte[] _vertexShaderByteCode = null;
        private Mesh _quad = null;
        private IGameApp _game = null;
        private SamplerState _screenImageSampler = null;
        private RenderTargetView _lowResTarget = null;
        private ShaderResourceView _lowResShaderResource = null;
        private SamplerState _lowResSampler = null;
        private RenderTargetView _tempTarget = null;
        private ShaderResourceView _tempShaderResource = null;
        private SamplerState _tempSampler = null;

        public DofEffect(Device graphicsDevice, IGameApp game)
            : base(graphicsDevice)
        {
            _game = game;
            CreateShaders();
            CreateShaderTargets();           
        }

        private void CreateShaders()
        {
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\dof_downsample_vs.cso", out _downSampleVertexShader);
            LoadPixelShader("Content\\Shaders\\dof_downsample_ps.cso", out _downSamplePixelShader);
            LoadVertexShader("Content\\Shaders\\dof_gaussian_vs.cso", out _gaussianVertexShader);
            LoadPixelShader("Content\\Shaders\\dof_gaussian_ps.cso", out _gaussianPixelShader);
            LoadPixelShader("Content\\Shaders\\dof_gaussian2_ps.cso", out _gaussianPixelShader2);
            LoadVertexShader("Content\\Shaders\\dof_depthfinal1_vs.cso", out _depthFinal1VertexShader);
            LoadPixelShader("Content\\Shaders\\dof_depthfinal1_ps.cso", out _depthFinal1PixelShader);
            LoadVertexShader("Content\\Shaders\\dof_depthfinal2_vs.cso", out _depthFinal2VertexShader);
            LoadPixelShader("Content\\Shaders\\dof_depthfinal2_ps.cso", out _depthFinal2PixelShader);
            _constantsBuffer1 = new SharpDX.Direct3D11.Buffer(GraphicsDevice, ConstantsBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _constantsBuffer2 = new SharpDX.Direct3D11.Buffer(GraphicsDevice, ConstantsBufferSize2, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        private void CreateShaderTargets()
        {
            _quad = ContentBuilder.BuildViewportQuad(_game, _vertexShaderByteCode);
            
            SamplerStateDescription samplerDesc = SamplerStateDescription.Default();
            samplerDesc.Filter = Filter.MinMagMipLinear;
            samplerDesc.AddressU = TextureAddressMode.Clamp;
            samplerDesc.AddressV = TextureAddressMode.Clamp;
            samplerDesc.AddressU = TextureAddressMode.Clamp;              
            _screenImageSampler = new SamplerState(GraphicsDevice, samplerDesc);

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
            
            //Create LowRes: texture, rendertarget, shaderresoure, and samplerstate.
            ///---------------------------------------------------------------------
            Texture2D lowResTexture = new Texture2D(_game.GraphicsDevice, textureDesc);
            RenderTargetViewDescription targetDesc = new RenderTargetViewDescription();
            targetDesc.Format = textureDesc.Format;
            targetDesc.Dimension = RenderTargetViewDimension.Texture2D;
            targetDesc.Texture2D.MipSlice = 0;
            _lowResTarget = new RenderTargetView(_game.GraphicsDevice, lowResTexture, targetDesc);
            ShaderResourceViewDescription resourceDesc = new ShaderResourceViewDescription();
            resourceDesc.Format = targetDesc.Format;
            resourceDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            resourceDesc.Texture2D.MostDetailedMip = 0;
            resourceDesc.Texture2D.MipLevels = 1;
            _lowResShaderResource = new ShaderResourceView(GraphicsDevice, lowResTexture, resourceDesc);
            _lowResSampler = new SamplerState(GraphicsDevice, samplerDesc);

            //Create Temp: texture, rendertarget, shaderresource and samplerstate.
            //--------------------------------------------------------------------
            Texture2D tempTexture = new Texture2D(_game.GraphicsDevice, textureDesc);
            targetDesc = new RenderTargetViewDescription();
            targetDesc.Format = textureDesc.Format;
            targetDesc.Dimension = RenderTargetViewDimension.Texture2D;
            targetDesc.Texture2D.MipSlice = 0;
            _tempTarget = new RenderTargetView(_game.GraphicsDevice, tempTexture, targetDesc);
            resourceDesc = new ShaderResourceViewDescription();
            resourceDesc.Format = targetDesc.Format;
            resourceDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            resourceDesc.Texture2D.MostDetailedMip = 0;
            resourceDesc.Texture2D.MipLevels = 1;
            _tempShaderResource = new ShaderResourceView(GraphicsDevice, tempTexture, resourceDesc);
            _tempSampler = new SamplerState(GraphicsDevice, samplerDesc);                                                       
        }
        
        public Vector2 ViewportSize { get; set; }
        public Vector2 MaxCoC { get; set; }
        public float RadiusScale { get; set; }

        private void WriteShaderConstants()
        {
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_constantsBuffer1, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, ConstantsBufferSize);
            int offset = 0;
            dataBuffer.Set(offset, ViewportSize.X);
            offset += sizeof(float);
            dataBuffer.Set(offset, ViewportSize.Y);           
            GraphicsDevice.ImmediateContext.UnmapSubresource(_constantsBuffer1, 0);
        }

        private void WriteShaderConstants2()
        {
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_constantsBuffer2, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, ConstantsBufferSize2);
            int offset = 0;
            dataBuffer.Set(offset, 1/ViewportSize.X);
            offset += sizeof(float);
            dataBuffer.Set(offset, 1/ViewportSize.Y);
            offset += sizeof(float);
            dataBuffer.Set(offset, MaxCoC);
            offset += sizeof(float);
            dataBuffer.Set(offset, RadiusScale);            
            GraphicsDevice.ImmediateContext.UnmapSubresource(_constantsBuffer2, 0);
        }

        public override void Apply()
        {
            WriteShaderConstants();
            WriteShaderConstants2();
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantsBuffer1);
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantsBuffer1);            
            RenderTargetView originalRenderTarget = null;
            DepthStencilView originalDepthStencilView = null;
            originalRenderTarget = GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out originalDepthStencilView)[0];
            
            ////////////////////////////
            //Pass0
            //-----
            //Name: DownSampling
            //Texture: ScreenImage
            //RenderTarget: LowRes
            /////////////////////////////            
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _screenImageSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_lowResTarget);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_downSampleVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_downSamplePixelShader);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            ////////////////////////////
            //Pass1
            //-----
            //Name: Guassian1
            //Texture: LowRes
            //RenderTarget: Temp
            /////////////////////////////
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _lowResShaderResource);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _lowResSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_tempTarget);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_gaussianVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_gaussianPixelShader);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            ////////////////////////////
            //Pass2
            //-----
            //Name: Guassian2
            //Texture: Temp
            //RenderTarget: LowRes
            /////////////////////////////
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _tempShaderResource);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _tempSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_lowResTarget);
            //*******************************************************************************
            //NOTE: We are reusing the Guassian vertex shader from previous pass.
            //*******************************************************************************
            GraphicsDevice.ImmediateContext.PixelShader.Set(_gaussianPixelShader2);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            /////////////////////////////////////////////////////////////////////////////////////
            //Pass3
            //-----
            //Name: DepthOfFieldPass1
            //Texture: ScreenImage, LowRes
            //RenderTarget: OriginalTarget
            //NOTE: The pixel shader used in this pass set the pixel with alpha of 0.5
            //so that it ends up being averaged with the shader from the next pass
            /////////////////////////////////////////////////////////////////////////////////////
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantsBuffer2);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _screenImageSampler);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, _lowResShaderResource);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, _lowResSampler);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _tempSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(originalRenderTarget);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_depthFinal1VertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_depthFinal1PixelShader);            
            _quad.Draw(0);
            //**********************************************************************************
            //NOTE: No need for clean-up, We'll be reusing the same resources in the next pass.
            //**********************************************************************************

            //////////////////////////////////////////////////////////////////////////////////
            //Pass4
            //-----
            //Name: DepthOfFieldPass2
            //Texture: ScreenImage, LowRes
            //RenderTarget: OriginalTarget
            //NOTE: The pixel shader used in this pass set the pixel with alpha of 0.5
            //so that it ends up being averaged with the shader from the previous pass
            /////////////////////////////////////////////////////////////////////////////////
            //************************************************************
            //NOTE: We're eusing the same resources from the previous pass.
            //************************************************************
            GraphicsDevice.ImmediateContext.VertexShader.Set(_depthFinal2VertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_depthFinal2PixelShader);
            BlendStateDescription blendDesc = BlendStateDescription.Default();
            //Enable alpha blending of pixel from previous stage.
            for (int i = 0; i < blendDesc.RenderTarget.Length; i++)
            {
                blendDesc.RenderTarget[i].IsBlendEnabled = true;
                blendDesc.RenderTarget[i].SourceBlend = BlendOption.SourceAlpha;
                blendDesc.RenderTarget[i].SourceAlphaBlend = BlendOption.SourceAlpha;
                blendDesc.RenderTarget[i].DestinationBlend = BlendOption.InverseSourceAlpha;
                blendDesc.RenderTarget[i].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
            }
            BlendState oldBlendState = GraphicsDevice.ImmediateContext.OutputMerger.BlendState;
            GraphicsDevice.ImmediateContext.OutputMerger.BlendState = new BlendState(GraphicsDevice, blendDesc);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.OutputMerger.BlendState = oldBlendState;
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(originalDepthStencilView, originalRenderTarget);
        }
    }

    public class DofLightingEffect : Effect
    {
        private byte[] _vertexShaderByteCode = null;
        private VertexShader _vertexShader = null;
        private PixelShader _pixelShader = null;
        private SharpDX.Direct3D11.Buffer _vsConstantsBuffer = null;
        private SharpDX.Direct3D11.Buffer _psConstantsBuffer = null;
        private const int VSConstantsBufferSize = 144;
        private const int PSConstantsBufferSize = 80;

        //public Matrix World { get; set; }
        //public Matrix View { get; set; }
        //public Matrix Projection { get; set; }
        public Matrix WorldView { get { return World * View; } }
        public Matrix WorldViewProjectionMatix { get { return World * View * Projection; } }        
        public Vector3 LightDirection { get; set; }
        public float FarClamp { get; set; }
        public float Far { get; set; }
        public float Focus { get; set; }
        public float Near { get; set; }
        public Color Ambient { get; set; }
        public Color Diffuse { get; set; }
        public Color Specular { get; set; }
        public float SpecularPower { get; set; }
        public  float Ka { get; set; }
        public float Ks { get; set; }
        public float Kd { get; set; }

        public DofLightingEffect(Device graphicsDevice) : base(graphicsDevice)
        {
            World = Matrix.Identity;
            View = Matrix.Identity;
            Projection = Matrix.Identity;
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\dof_lighting_vs.cso", out _vertexShader);
            LoadPixelShader("Content\\Shaders\\dof_lighting_ps.cso", out _pixelShader);
            _vsConstantsBuffer = new SharpDX.Direct3D11.Buffer(graphicsDevice, VSConstantsBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _psConstantsBuffer = new SharpDX.Direct3D11.Buffer(graphicsDevice, PSConstantsBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        public override byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;
        }

        public override void Apply()
        {
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);
            WriteVSShaderConstants();
            WritePSShaderConstants();
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _vsConstantsBuffer);
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _psConstantsBuffer);
        }

        private void WriteVSShaderConstants()
        {
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_vsConstantsBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, VSConstantsBufferSize);
            int offset = 0;
            dataBuffer.Set(offset, Matrix.Transpose(WorldView));            
            offset += sizeof(float) * 16;
            dataBuffer.Set(offset, Matrix.Transpose(WorldViewProjectionMatix));
            offset += sizeof(float) * 16;
            dataBuffer.Set(offset, LightDirection);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_vsConstantsBuffer, 0);
        }

        private void WritePSShaderConstants()
        {
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_psConstantsBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, PSConstantsBufferSize);
            int offset = 0;
            dataBuffer.Set(offset, FarClamp);
            offset += sizeof(float);
            dataBuffer.Set(offset, Far);
            offset += sizeof(float);
            dataBuffer.Set(offset, Focus);
            offset += sizeof(float);
            dataBuffer.Set(offset, Near);
            offset += sizeof(float);
            dataBuffer.Set(offset, Ambient.ToVector4());
            offset += sizeof(float) * 4;
            dataBuffer.Set(offset, Diffuse.ToVector4());
            offset += sizeof(float) * 4;
            dataBuffer.Set(offset, Specular.ToVector4());
            offset += sizeof(float) * 4;
            dataBuffer.Set(offset, SpecularPower);
            offset += sizeof(float);
            dataBuffer.Set(offset, Ka);
            offset += sizeof(float);
            dataBuffer.Set(offset, Ks);
            offset += sizeof(float);
            dataBuffer.Set(offset, Kd);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_psConstantsBuffer, 0);
        }
    }
}
