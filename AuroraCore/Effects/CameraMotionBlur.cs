using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
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
    public class CameraMotionBlur : PostEffect
    {
        //IGameApp _game = null;
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

        public CameraMotionBlur(IGameApp game) : base(game)
        {          
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
            _quad.Draw();
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
            _quad.Draw();
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
            _quad.Draw();
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, null);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, null);

            ///////////////////////////////////
            //Clean up
            ///////////////////////////////////
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, null);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(originalDepthStencilView, originalRenderTarget);
            originalRenderTarget.Dispose();
            originalDepthStencilView.Dispose();
        }
 
        private void CreateConstantBuffers()
        {
            _passThruFixConstantsBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, ConstantsBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
        }

        private void CreateShaders()
        {
            _vertexShaderByteCode = LoadVertexShader("Resources\\Shaders\\postpassthru-fix-vs.cso", out _passThruFixVertexShader);
            LoadPixelShader("Resources\\Shaders\\motionblur-combine-ps.cso", out _combinePixelShader);
            LoadPixelShader("Resources\\Shaders\\postpassthru-ps.cso", out _passThruPixelShader);
        }

        private void CreateTextures()
        {
            _quad = ContentBuilder.BuildViewportQuad(Game.GraphicsDevice, _vertexShaderByteCode);

            Texture2DDescription textureDesc = new Texture2DDescription();
            textureDesc.ArraySize = 1;
            textureDesc.BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource;
            textureDesc.CpuAccessFlags = CpuAccessFlags.None;
            textureDesc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            textureDesc.Height = Game.RenderTargetView.GetTextureDescription().Height;
            textureDesc.Width = Game.RenderTargetView.GetTextureDescription().Width;
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
            Texture2D _sceneTexture = new Texture2D(Game.GraphicsDevice, textureDesc);           
            _inputTextureSampler = new SamplerState(Game.GraphicsDevice, samplerStateDesc);

            //Sum Texture
            //-----------
            Texture2D _sumTexture = new Texture2D(Game.GraphicsDevice, textureDesc);
            _sumTextureShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, _sumTexture, resourceViewDesc);
            _sumTextureRenderTargetView = new RenderTargetView(Game.GraphicsDevice, _sumTexture, renderTargetViewDesc);
            _sumTextureSampler = new SamplerState(Game.GraphicsDevice, samplerStateDesc);

            //Temp Texture
            //--------
            Texture2D _tempTexture = new Texture2D(Game.GraphicsDevice, textureDesc);
            _tempTextureShaderResourceView = new ShaderResourceView(Game.GraphicsDevice, _tempTexture, resourceViewDesc);
            _tempTextureRenderTargetView = new RenderTargetView(Game.GraphicsDevice, _tempTexture, renderTargetViewDesc);
            _tempTextureSampler = new SamplerState(Game.GraphicsDevice, samplerStateDesc);
        }

        private void WriteShaderConstants()
        {
            int inputTextureWidth = InputTexture.GetTextureDescription().Width;
            int inputTextureHeight = InputTexture.GetTextureDescription().Height;          
            DataBox dataBox = Game.GraphicsDevice.ImmediateContext.MapSubresource(_passThruFixConstantsBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            float invWidth = 1 / inputTextureWidth;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref invWidth);
            float invHeight = 1 / inputTextureHeight;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref invHeight);
            Game.GraphicsDevice.ImmediateContext.UnmapSubresource(_passThruFixConstantsBuffer, 0);        
        }
    }
}
