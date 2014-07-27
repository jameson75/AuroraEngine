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
    public class PostEffectChain : System.Collections.ObjectModel.ObservableCollection<PostEffect>
    {
        //private Texture2D _texture = null;
        private RenderTargetView _tempRenderTarget = null;
        private ShaderResourceView _tempShaderResource = null;
        //private Texture2D _auxTexture = null;
        private RenderTargetView _tempAuxRenderTarget = null;
        private ShaderResourceView _tempAuxTextureShaderResource = null;
        //private ShaderResourceView _depthShaderResource = null;
        private IGameApp _game = null;
        //private Matrix _quadTransform = Matrix.Zero;
        //private Matrix _viewMatrix = Matrix.Zero;
        //private Matrix _projectionMatrix = Matrix.Zero;        
        private RenderTargetView _originalRenderView = null;
        private DepthStencilView _originalDepthStencilView = null;
        //private bool _isEffectInProgress = false;
        private PassThruPostEffect passThruEffect = null;        
        
        private PostEffectChain(IGameApp game)
        {
            _game = game;            
        }

        public ShaderResourceView InputTexture { get; set; }

        public RenderTargetView OutputTexture { get; set; }

        public BlendState BlendState { get; set; }

        public static PostEffectChain Create(IGameApp game)
        {           
            PostEffectChain effectChain = new PostEffectChain(game);

            Texture2DDescription textureDesc = new Texture2DDescription();
            textureDesc.ArraySize = 1;
            textureDesc.BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource;
            textureDesc.CpuAccessFlags = CpuAccessFlags.None;
            textureDesc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            textureDesc.Height = game.RenderTarget.ResourceAs<Texture2D>().Description.Height;
            textureDesc.Width = game.RenderTarget.ResourceAs<Texture2D>().Description.Width;
            textureDesc.MipLevels = 1;
            textureDesc.OptionFlags = ResourceOptionFlags.None;
            textureDesc.Usage = ResourceUsage.Default;
            textureDesc.SampleDescription.Count = 1;
            Texture2D _texture = new Texture2D(game.GraphicsDevice, textureDesc);
            Texture2D _auxTexture = new Texture2D(game.GraphicsDevice, textureDesc);

            RenderTargetViewDescription targetDesc = new RenderTargetViewDescription();
            targetDesc.Format = textureDesc.Format;
            targetDesc.Dimension = RenderTargetViewDimension.Texture2D;
            targetDesc.Texture2D.MipSlice = 0;
            effectChain._tempRenderTarget = new RenderTargetView(game.GraphicsDevice, _texture, targetDesc);
            effectChain._tempAuxRenderTarget = new RenderTargetView(game.GraphicsDevice, _auxTexture, targetDesc);

            ShaderResourceViewDescription resourceDesc = new ShaderResourceViewDescription();
            resourceDesc.Format = targetDesc.Format;
            resourceDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            resourceDesc.Texture2D.MostDetailedMip = 0;
            resourceDesc.Texture2D.MipLevels = 1;
            effectChain._tempShaderResource = new ShaderResourceView(game.GraphicsDevice, _texture, resourceDesc);
            effectChain._tempAuxTextureShaderResource = new ShaderResourceView(game.GraphicsDevice, _auxTexture, resourceDesc);

            //ShaderResourceViewDescription depthResourceDesc = new ShaderResourceViewDescription();
            //depthResourceDesc.Format = SharpDX.DXGI.Format.R32_Float;
            //depthResourceDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            //depthResourceDesc.Texture2D.MostDetailedMip = 0;
            //depthResourceDesc.Texture2D.MipLevels = 1;            
            //effectChain._depthShaderResource = new ShaderResourceView(game.GraphicsDevice, game.DepthStencil.ResourceAs<Texture2D>(), depthResourceDesc);

            effectChain.passThruEffect = new PassThruPostEffect(game);           
                 
            return effectChain;            
        }

        //public void Begin(long gameTime)
        //{
        //    if (_isEffectInProgress == true)
        //        throw new InvalidOperationException("Post effect was not properly ended. Require a call to PostEffectChain.End()");

        //    //1. Save orginal render targets.            
        //    _originalRenderView = _game.GraphicsDeviceContext.OutputMerger.GetRenderTargets(1, out _originalDepthStencilView)[0];            

        //    //2. Replace the original render targets in the graphics output with a temp target and clear it.
        //    //(All rendering will drawn on the temp target. Later, in PostEffectChain.End(),
        //    //these targets are set as input to the first post process effect.            
        //    _game.GraphicsDeviceContext.OutputMerger.SetTargets(_originalDepthStencilView, _textureRenderTarget);
        //    _game.GraphicsDeviceContext.ClearRenderTargetView(_textureRenderTarget, Color.Black);           

        //    //3. Since Begin() and End() need to be called in pairs... 
        //    //We save a flag to indicate that a call to Begin() was made.
        //    //In PostEffectChain.End(), we'll unset it when a matching call to End() is made.           
        //    _isEffectInProgress = true;
        //}

        //public void End(long gameTime)
        //{            
        //    if (_isEffectInProgress == false)
        //        throw new InvalidOperationException("Post effect was not properly started. Require a call to PostEffectChain.Begin()");
            
        //    //1. Call each posteffect in the chain to process the current input texture.
        //    //The very first, "current" input texture is the temp render target set in PostEffectChain.Begin().
        //    //The output texture from each posteffect becomes the input texture for the following
        //    //post effect in the chain.
        //    foreach (PostEffect postEffect in this)
        //    {
        //        _game.GraphicsDeviceContext.OutputMerger.SetTargets(_auxTextureRenderTarget);                               
        //        postEffect.InputTexture = _textureShaderResource;                
        //        postEffect.Apply();       
        //        Swap<ShaderResourceView>(ref _textureShaderResource, ref _auxTextureShaderResource);
        //        Swap<RenderTargetView>(ref _textureRenderTarget, ref _auxTextureRenderTarget);
        //    }
                
        //    //2. We replace the temp render targets in the graphics output (we did this in PostEffectChain.Begin()),
        //    //with original render target.
        //    _game.GraphicsDeviceContext.OutputMerger.SetTargets(_originalDepthStencilView, _originalRenderView);   

        //    //3. We render the output texture from the very last post effect to the original 
        //    //render target.
        //    passThruEffect.InputTexture = _textureShaderResource;
        //    passThruEffect.Apply();        

        //    //4. We unset this flag to indicate a matching End() call was called to an unmatched Begin() call.
        //    this._isEffectInProgress = false;
        //}
       
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
                    Swap<ShaderResourceView>(ref _tempShaderResource, ref _tempAuxTextureShaderResource);
                    Swap<RenderTargetView>(ref _tempRenderTarget, ref _tempAuxRenderTarget);
                }
            }

            //5. We replace the temp render targets in the graphics output with the chain output texture.
            _game.GraphicsDeviceContext.ClearRenderTargetView(OutputTexture, Color.Black);
            _game.GraphicsDeviceContext.OutputMerger.SetTargets(OutputTexture);            

            //6. We render the texture drawn by the very last post effect to this chain's output texture.
            //NOTE: We specify an optional blend state to allow for blending effects like overlays.
            passThruEffect.InputTexture = _tempShaderResource;
            passThruEffect.BlendState = this.BlendState;
            passThruEffect.Apply();           

            //7. We replace the chain output texture in the graphics output.
            _game.GraphicsDeviceContext.OutputMerger.SetTargets(_originalDepthStencilView, _originalRenderView);
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
