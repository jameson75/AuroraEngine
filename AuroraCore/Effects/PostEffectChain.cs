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

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public class PostEffectChain : System.Collections.ObjectModel.ObservableCollection<PostEffect>, IDisposable
    {        
        private RenderTargetView _tempRenderTarget = null;
        private ShaderResourceView _tempShaderResource = null;       
        private RenderTargetView _tempAuxRenderTarget = null;
        private ShaderResourceView _tempAuxShaderResource = null;
        private IGameApp _game = null;
        private DepthStencilView _originalDepthStencilView = null;
        private PassThruPostEffect passThruEffect = null;
        private RenderTargetView _originalRenderView = null;

        private PostEffectChain(IGameApp game)
        {
            _game = game;
        }       

        public void Dispose()
        {
            _tempRenderTarget.Dispose();
            _tempRenderTarget = null;

            _tempAuxRenderTarget.Dispose();
            _tempAuxRenderTarget = null;

            _tempShaderResource.Dispose();
            _tempShaderResource = null;

            _tempAuxShaderResource.Dispose();
            _tempAuxShaderResource = null;

            passThruEffect.Dispose();
            passThruEffect = null;
        }

        public ShaderResourceView InputTexture { get; set; }

        public RenderTargetView OutputView { get; set; }

        public BlendState BlendState { get; set; }

        public static PostEffectChain Create(IGameApp game)
        {
            var chain = new PostEffectChain(game);
            chain.Initialize();
            return chain;
        }

        public void Initialize()
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
            Texture2D texture = new Texture2D(_game.GraphicsDevice, textureDesc);
            Texture2D auxTexture = new Texture2D(_game.GraphicsDevice, textureDesc);

            RenderTargetViewDescription targetDesc = new RenderTargetViewDescription();
            targetDesc.Format = textureDesc.Format;
            targetDesc.Dimension = RenderTargetViewDimension.Texture2D;
            targetDesc.Texture2D.MipSlice = 0;
            this._tempRenderTarget = new RenderTargetView(_game.GraphicsDevice, texture, targetDesc);
            this._tempAuxRenderTarget = new RenderTargetView(_game.GraphicsDevice, auxTexture, targetDesc);

            ShaderResourceViewDescription resourceDesc = new ShaderResourceViewDescription();
            resourceDesc.Format = targetDesc.Format;
            resourceDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            resourceDesc.Texture2D.MostDetailedMip = 0;
            resourceDesc.Texture2D.MipLevels = 1;
            this._tempShaderResource = new ShaderResourceView(_game.GraphicsDevice, texture, resourceDesc);
            this._tempAuxShaderResource = new ShaderResourceView(_game.GraphicsDevice, auxTexture, resourceDesc);

            this.passThruEffect = new PassThruPostEffect(_game);                   
        }     

        public void Apply()
        {        
            //1. Save orginal render targets.            
            _originalRenderView = _game.GraphicsDeviceContext.OutputMerger.GetRenderTargets(1, out _originalDepthStencilView)[0];
          
            //2. We replace the original render targets with a temp target.
            _game.GraphicsDeviceContext.OutputMerger.SetTargets(_tempRenderTarget);

            //3. We render the chain input texture to the temp render target. 
            //render target.
            passThruEffect.InputTexture = InputTexture;
            passThruEffect.Apply();

            //We unbind the tempRenderTarget from the pipline so we can use it as a shader resource
            //in the loop below.
            _game.GraphicsDeviceContext.OutputMerger.SetTargets((RenderTargetView)null);

            //4. Call each posteffect in the chain to process the current input texture.           
            //The output texture from each posteffect becomes the input texture for the following
            //post effect in the chain.
            foreach (PostEffect postEffect in this)
            {
                if (postEffect.Enabled)
                {
                    //_game.GraphicsDeviceContext.OutputMerger.SetTargets(_tempAuxRenderTarget);
                    _game.GraphicsDeviceContext.ClearRenderTargetView(_tempAuxRenderTarget, Color.Black);
                    postEffect.InputTexture = _tempShaderResource;
                    postEffect.OutputTexture = _tempAuxRenderTarget;                    
                    postEffect.Apply();
                    Swap<ShaderResourceView>(ref _tempShaderResource, ref _tempAuxShaderResource);
                    Swap<RenderTargetView>(ref _tempRenderTarget, ref _tempAuxRenderTarget);
                }
            }

            //5. We replace the temp render targets in the graphics output with the chain output texture.
            _game.GraphicsDeviceContext.ClearRenderTargetView(OutputView, Color.Black);
            _game.GraphicsDeviceContext.OutputMerger.SetTargets(OutputView);            

            //6. We render the texture drawn by the very last post effect to this chain's output texture.
            //NOTE: We specify an optional blend state to allow for blending effects like overlays.
            passThruEffect.InputTexture = _tempShaderResource;
            passThruEffect.BlendState = this.BlendState;
            passThruEffect.Apply();           

            //7. We replace the chain output texture in the graphics output.
            _game.GraphicsDeviceContext.OutputMerger.SetTargets(_originalDepthStencilView, _originalRenderView);

            //8. It's important to immediate COM Release() any swap chain resource.
            _originalDepthStencilView.Dispose();
            _originalRenderView.Dispose();
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        //public bool IsEffectInProgress { get { return _isEffectInProgress; } }
    }
}
