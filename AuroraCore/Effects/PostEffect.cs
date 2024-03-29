﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public abstract class PostEffect : Effect
    {
        private RenderTargetView originalRenderTarget = null;
        private DepthStencilView originalDepthStencilView = null;
        
        public bool Enabled { get; set; }

        public ShaderResourceView InputTexture { get; set; }

        public RenderTargetView OutputTexture { get; set; }

        protected PostEffect(IGameApp game)
            : base(game)
        {
            Enabled = true;
        }

        protected override void OnBeginApply()
        {
            originalRenderTarget = Game.GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out originalDepthStencilView)[0];
            base.OnBeginApply();
        }

        protected override void OnEndApply()
        {
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(originalDepthStencilView, originalRenderTarget);
            //It's important to immediately COM Release() any swap chain resource.
            originalDepthStencilView?.Dispose();
            originalRenderTarget?.Dispose();
            base.OnEndApply();
        }

        [Obsolete]
        public ShaderResourceView Depth { get; set; }

        public static Texture2DDescription CreateCommonTextureDesc(Size2 size)
        {
            Texture2DDescription textureDesc = new Texture2DDescription();
            textureDesc.ArraySize = 1;
            textureDesc.BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource;
            textureDesc.CpuAccessFlags = CpuAccessFlags.None;
            textureDesc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            textureDesc.Height = size.Height; 
            textureDesc.Width = size.Width; 
            textureDesc.MipLevels = 1;
            textureDesc.OptionFlags = ResourceOptionFlags.None;
            textureDesc.Usage = ResourceUsage.Default;
            textureDesc.SampleDescription.Count = 1;
            return textureDesc;
        }

        public static ShaderResourceViewDescription CreateCommonShaderResourceViewDesc(Texture2DDescription textureDesc)
        {
            ShaderResourceViewDescription resourceViewDesc = new ShaderResourceViewDescription();
            resourceViewDesc.Format = textureDesc.Format;
            resourceViewDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            resourceViewDesc.Texture2D.MostDetailedMip = 0;
            resourceViewDesc.Texture2D.MipLevels = 1;
            return resourceViewDesc;
        }

        public static RenderTargetViewDescription CreateCommonRenderTargetViewDesc(Texture2DDescription textureDesc)
        {
            RenderTargetViewDescription renderTargetViewDesc = new RenderTargetViewDescription();
            renderTargetViewDesc.Format = textureDesc.Format;
            renderTargetViewDesc.Dimension = RenderTargetViewDimension.Texture2D;
            renderTargetViewDesc.Texture2D.MipSlice = 0;
            return renderTargetViewDesc;
        }

        public static SamplerStateDescription CreateCommonSamplerStateDesc()
        {
            SamplerStateDescription samplerStateDesc = SamplerStateDescription.Default();
            samplerStateDesc.Filter = Filter.MinMagMipLinear;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;
            samplerStateDesc.AddressV = TextureAddressMode.Clamp;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;
            return samplerStateDesc;
        }
    }
}
