﻿using System;
using SharpDX;
using SharpDX.Direct3D11;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public class FlatEffect : SurfaceEffect, ISkinEffect
    {
        private SharpDX.Direct3D11.Buffer _vertexConstantsBufferCommon = null;
        private SharpDX.Direct3D11.Buffer _vertexConstantsBufferSkin = null;
        private SharpDX.Direct3D11.Buffer _pixelConstantsBuffer = null;
        private byte[] _vertexShaderByteCode = null;
        private PixelShader _pixelShader = null;
        private VertexShader _vertexShader = null;

        private const int VertexConstantBufferCommonSize = 64;
        private const int VertexConstantBufferSkinSize = 3520;
        private const int PixelConstantBufferSize = 32;
        private const int MaxBones = 72;

        private RasterizerState _oldRasterizerState = null;
        private BlendState _oldBlendState = null;
        private DepthStencilState _oldDepthStencilState = null;
        private Color4 _oldBlendFactor = Color.Zero;
        private bool _restoreRastrizerState = false;
        private bool _restoreBlendState = false;
        private bool _restoreDepthStencilState = false;

        public SamplerState _textureSamplerState = null;

        SurfaceVertexType _surfaceVertexType = SurfaceVertexType.None;

        public FlatEffect(IGameApp game, SurfaceVertexType svt) : base(game)
        {
            _surfaceVertexType = svt;
            
            //Create constant buffer
            _vertexConstantsBufferCommon = new SharpDX.Direct3D11.Buffer(GraphicsDevice, VertexConstantBufferCommonSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _vertexConstantsBufferSkin = new SharpDX.Direct3D11.Buffer(GraphicsDevice, VertexConstantBufferSkinSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _pixelConstantsBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, PixelConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

            //Create texture sampler state
            SamplerStateDescription samplerStateDesc = SamplerStateDescription.Default();
            samplerStateDesc.Filter = Filter.MinMagMipLinear;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;
            samplerStateDesc.AddressV = TextureAddressMode.Clamp;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;
            _textureSamplerState = new SamplerState(GraphicsDevice, samplerStateDesc);
           
            switch (svt)
            {
                case SurfaceVertexType.SkinTexture:
                    _vertexShaderByteCode = LoadVertexShader("Assets\\Shaders\\flat-skint-vs.cso", out _vertexShader);
                    break;
                case SurfaceVertexType.SkinColor:
                    _vertexShaderByteCode = LoadVertexShader("Assets\\Shaders\\flat-skinc-vs.cso", out _vertexShader);
                    break;
                case SurfaceVertexType.PositionTexture:
                    _vertexShaderByteCode = LoadVertexShader("Assets\\Shaders\\flat-pt-vs.cso", out _vertexShader);
                    break;
                case SurfaceVertexType.PositionColor:
                    _vertexShaderByteCode = LoadVertexShader("Assets\\Shaders\\flat-pc-vs.cso", out _vertexShader);
                    break;
                case SurfaceVertexType.InstancePositionTexture:
                    _vertexShaderByteCode = LoadVertexShader("Assets\\Shaders\\flat-i-pt-vs.cso", out _vertexShader);
                    break;
                case SurfaceVertexType.InstancePositionColor:
                    _vertexShaderByteCode = LoadVertexShader("Assets\\Shaders\\flat-i-pc-vs.cso", out _vertexShader);
                    break;
                case SurfaceVertexType.PositionNormalColor:
                    _vertexShaderByteCode = LoadVertexShader("Assets\\Shaders\\flat-pnc-vs.cso", out _vertexShader);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported surface vertex type {svt} specified");
            }
            LoadPixelShader("Assets\\Shaders\\flat-ps.cso", out _pixelShader);
        }

        public Matrix[] BoneTransforms { get; set; }

        public ShaderResourceView Texture { get; set; }

        public BlendState BlendState { get; set; } 

        public DepthStencilState DepthStencilState { get; set; }

        public bool EnableBackFace { get; set; }

        private Matrix WorldViewProjection
        {
            get { return World * View * Projection; }
        }

        public override void Apply()
        {
            //Setup Constants
            //---------------
            if (_surfaceVertexType == SurfaceVertexType.SkinColor ||
                _surfaceVertexType == SurfaceVertexType.SkinTexture)
            {
                WriteVertexConstantsSkin();
                GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _vertexConstantsBufferSkin);
            }
            else
            {
                WriteVertexConstantsCommon();
                GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _vertexConstantsBufferCommon);
            }
            WritePixelConstants();
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _pixelConstantsBuffer);

            //Setup Vertex Shader
            //-------------------
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);

            //Setup Pixel Shader
            //------------------  
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);

            //Setup Shader Texture
            //--------------------
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _textureSamplerState);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, Texture);

            //If back face rendering is enabled, cache old rasterizer state and setup a new one.
            //Configure Rasterizer.
            if (EnableBackFace)
            {
                _oldRasterizerState = GraphicsDevice.ImmediateContext.Rasterizer.State;
                RasterizerStateDescription newRasterizerStateDesc = (_oldRasterizerState != null) ? _oldRasterizerState.Description : RasterizerStateDescription.Default();
                newRasterizerStateDesc.CullMode = CullMode.None;
                GraphicsDevice.ImmediateContext.Rasterizer.State = new RasterizerState(GraphicsDevice, newRasterizerStateDesc);
                _restoreRastrizerState = true;
            }

            if (BlendState != null)
            {
                _oldBlendState = GraphicsDevice.ImmediateContext.OutputMerger.BlendState;
                _oldBlendFactor = GraphicsDevice.ImmediateContext.OutputMerger.BlendFactor;
                GraphicsDevice.ImmediateContext.OutputMerger.BlendState = BlendState;
                _restoreBlendState = true;
            }

            if (DepthStencilState != null)
            {
                _oldDepthStencilState = GraphicsDevice.ImmediateContext.OutputMerger.DepthStencilState;
                GraphicsDevice.ImmediateContext.OutputMerger.DepthStencilState = DepthStencilState;
                _restoreDepthStencilState = true;
            }
        }

        public override void Restore()
        {
            if (_restoreRastrizerState)
            {
                GraphicsDevice.ImmediateContext.Rasterizer.State = _oldRasterizerState;
                _oldRasterizerState = null;
                _restoreRastrizerState = false;
            }

            if (_restoreBlendState)
            {
                GraphicsDevice.ImmediateContext.OutputMerger.BlendState = _oldBlendState;
                GraphicsDevice.ImmediateContext.OutputMerger.BlendFactor = _oldBlendFactor;
                _oldBlendState = null;
                _oldBlendFactor = Color.Zero;
                _restoreBlendState = false;
            }

            if (_restoreDepthStencilState)
            {
                GraphicsDevice.ImmediateContext.OutputMerger.DepthStencilState = _oldDepthStencilState;
                _oldDepthStencilState = null;
                _restoreDepthStencilState = false;
            }

            base.Restore();
        }

        public override byte[] GetVertexShaderByteCode()
        {
            return _vertexShaderByteCode;
        }

        public override EffectDataChannels GetDataChannels()
        {
            return GetDataChannelsForSurfaceVertexType(_surfaceVertexType);
        }

        private void WriteVertexConstantsSkin()
        {
            //Open Access to Buffer
            //---------------------
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_vertexConstantsBufferSkin, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, VertexConstantBufferSkinSize);
            int offset = 0;
          
            //WvpXf
            //-----
            Matrix wvpXf = this.WorldViewProjection;
            wvpXf.Transpose();
            dataBuffer.Set(offset, wvpXf);
            offset += sizeof(float) * 16;

            if (BoneTransforms != null)
            {
                if (BoneTransforms.Length > MaxBones)
                    throw new InvalidOperationException("Max bones exceeded.");
                for (int i = 0; i < BoneTransforms.Length; i++)
                {
                    Matrix boneTranspose = Matrix.Transpose(BoneTransforms[i]);
                    dataBuffer.Set(offset, boneTranspose.Row1);
                    offset += sizeof(float) * 4;
                    dataBuffer.Set(offset, boneTranspose.Row2);
                    offset += sizeof(float) * 4;
                    dataBuffer.Set(offset, boneTranspose.Row3);
                    offset += sizeof(float) * 4;
                }
            }

            //Close Access to Buffer
            //----------------------
            GraphicsDevice.ImmediateContext.UnmapSubresource(_vertexConstantsBufferSkin, 0);
        }

        private void WriteVertexConstantsCommon()
        {
            //Open Access to Buffer
            //---------------------
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_vertexConstantsBufferCommon, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, VertexConstantBufferCommonSize);
            int offset = 0;

            //WvpXf
            //-----
            Matrix wvpXf = this.WorldViewProjection;
            wvpXf.Transpose();
            dataBuffer.Set(offset, wvpXf);            

            //Close Access to Buffer
            //----------------------
            GraphicsDevice.ImmediateContext.UnmapSubresource(_vertexConstantsBufferCommon, 0);
        }

        private void WritePixelConstants()
        {
            //Open access to buffer
            //---------------------
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_pixelConstantsBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, PixelConstantBufferSize);            
            int offset = 0;
            
            //Write EnableVertexColor
            //-----------------------           
            dataBuffer.Set(offset, _surfaceVertexType == SurfaceVertexType.InstancePositionColor ||
                                   _surfaceVertexType == SurfaceVertexType.PositionColor ||
                                   _surfaceVertexType == SurfaceVertexType.BillboardInstancePositionColor);

            offset += sizeof(Int32); //A bool in HLSL is 32 bytes

            GraphicsDevice.ImmediateContext.UnmapSubresource(_pixelConstantsBuffer, 0);
        }

        protected override void OnDispose()
        {           
            _vertexConstantsBufferCommon?.Dispose();
            _vertexConstantsBufferSkin?.Dispose();
            _pixelConstantsBuffer?.Dispose();
            _textureSamplerState?.Dispose();
            _vertexShader?.Dispose();
            _pixelShader?.Dispose();
            base.OnDispose();
        }
    }
}



