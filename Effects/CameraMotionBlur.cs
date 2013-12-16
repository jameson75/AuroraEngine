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
    public class CameraMotionBlur : PostEffect
    {
        IGameApp _game = null;
        private byte[] _vertexShaderByteCode = null;
        private VertexShader _passThruFixVertexShader = null;
        private PixelShader _combinePixelShader = null;   
        private PixelShader _passThruPixelShader = null;               
        private Mesh _quad = null;
        private SharpDX.Direct3D11.Buffer _passThruFixConstantsBuffer = null;
        private SamplerState _inputTextureSampler = null;
        private ShaderResourceView _sumTextureShaderResourceView = null;
        private SamplerState _sumTextureSampler = null;
        private RenderTargetView _tempTextureRenderTargetView = null;
        private ShaderResourceView _tempTextureShaderResourceView = null;
        private SamplerState _tempTextureSampler = null;
        private RenderTargetView _sumTextureRenderTargetView = null;
        private int ConstantsBufferSize = 16;

        public CameraMotionBlur(Device graphicsDevice, IGameApp game) : base(graphicsDevice)
        {
            _game = game;
            CreateConstantBuffers();
            CreateTextures();
            CreateShaders();            
        }

        public override void Apply()
        {
            ///////////////////////////////
            //Setup
            ///////////////////////////////
            WriteShaderConstants();            
            RenderTargetView originalRenderTarget = null;
            DepthStencilView originalDepthStencilView = null;
            originalRenderTarget = GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out originalDepthStencilView)[0];

            ////////////////////////////////
            //Pass0
            //Accumulate (Combine)
            //Input: Input (Scene) Texture, 
            //       Sum Texture
            //Ouput: Temp Texture
            ////////////////////////////////
            ShaderResourceView baseMap = InputTexture;           
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _inputTextureSampler);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, _sumTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, _sumTextureSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_tempTextureRenderTargetView);
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _passThruFixConstantsBuffer);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruFixVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_combinePixelShader);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);

            ///////////////////////////////////////////////
            //Pass1
            //Copy Back
            //Input: Temp Texture
            //Output: Sum Texture
            ///////////////////////////////////////////////
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _tempTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _tempTextureSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_sumTextureRenderTargetView);
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _passThruFixConstantsBuffer);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruFixVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_passThruPixelShader);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);

            ///////////////////////////////////////////////
            //Pass2
            //Output
            //Input: Sum Texture
            //Ouput: Render Target
            ///////////////////////////////////////////////
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, _sumTextureShaderResourceView);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _sumTextureSampler);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(originalRenderTarget);
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _passThruFixConstantsBuffer);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_passThruFixVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_passThruPixelShader);
            _quad.Draw(0);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);

            ///////////////////////////////////
            //Clean up
            ///////////////////////////////////
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(originalDepthStencilView, originalRenderTarget);
        }

        public override byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;   
        }

        private void CreateConstantBuffers()
        {
            _passThruFixConstantsBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, ConstantsBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        private void CreateShaders()
        {
            _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\postpassthru-fix-vs.cso", out _passThruFixVertexShader);
            LoadPixelShader("Content\\Shaders\\motionblur-combine-ps.cso", out _combinePixelShader);
            LoadPixelShader("Content\\Shaders\\postpassthru-ps.cso", out _passThruPixelShader);
        }

        private void CreateTextures()
        {
            _quad = ContentBuilder.BuildBasicViewportQuad(_game, _vertexShaderByteCode);

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

            RenderTargetViewDescription renderTargetViewDesc = new RenderTargetViewDescription();
            renderTargetViewDesc.Format = textureDesc.Format;
            renderTargetViewDesc.Dimension = RenderTargetViewDimension.Texture2D;
            renderTargetViewDesc.Texture2D.MipSlice = 0;

            SamplerStateDescription samplerStateDesc = SamplerStateDescription.Default();
            samplerStateDesc.Filter = Filter.MinMagMipLinear;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;
            samplerStateDesc.AddressV = TextureAddressMode.Clamp;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;

            //Scene (Input) Texture
            //---------------------
            Texture2D _sceneTexture = new Texture2D(GraphicsDevice, textureDesc);           
            _inputTextureSampler = new SamplerState(GraphicsDevice, samplerStateDesc);

            //Sum Texture
            //-----------
            Texture2D _sumTexture = new Texture2D(_game.GraphicsDevice, textureDesc);
            _sumTextureShaderResourceView = new ShaderResourceView(GraphicsDevice, _sumTexture, resourceViewDesc);
            _sumTextureRenderTargetView = new RenderTargetView(GraphicsDevice, _sumTexture, renderTargetViewDesc);
            _sumTextureSampler = new SamplerState(GraphicsDevice, samplerStateDesc);

            //Temp Texture
            //--------
            Texture2D _tempTexture = new Texture2D(_game.GraphicsDevice, textureDesc);
            _tempTextureShaderResourceView = new ShaderResourceView(GraphicsDevice, _tempTexture, resourceViewDesc);
            _tempTextureRenderTargetView = new RenderTargetView(GraphicsDevice, _tempTexture, renderTargetViewDesc);
            _tempTextureSampler = new SamplerState(GraphicsDevice, samplerStateDesc);
        }

        private void WriteShaderConstants()
        {
            int inputTextureWidth = InputTexture.ResourceAs<Texture2D>().Description.Width;
            int inputTextureHeight = InputTexture.ResourceAs<Texture2D>().Description.Height;          
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_passThruFixConstantsBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            float invWidth = 1 / inputTextureWidth;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref invWidth);
            float invHeight = 1 / inputTextureHeight;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref invHeight);
            GraphicsDevice.ImmediateContext.UnmapSubresource(_passThruFixConstantsBuffer, 0);        
        }
    }
}
