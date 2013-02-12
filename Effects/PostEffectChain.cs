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

namespace CipherPark.AngelJacket.Core.Effects
{
    public class PostEffectChain : System.Collections.ObjectModel.ObservableCollection<PostEffect>
    {
        //private Texture2D _texture = null;
        private RenderTargetView _textureRenderTarget = null;
        private ShaderResourceView _textureShaderResource = null;
        //private Texture2D _auxTexture = null;
        private RenderTargetView _auxTextureRenderTarget = null;
        private ShaderResourceView _auxTextureShaderResource = null;
        private ShaderResourceView _depthShaderResource = null;
        private IGameApp _game = null;
        private Matrix _quadTransform = Matrix.Zero;
        private Matrix _viewMatrix = Matrix.Zero;
        private Matrix _projectionMatrix = Matrix.Zero;
        
        private RenderTargetView _originalRenderView = null;
        private DepthStencilView _originalDepthStencilView = null;
        private bool _isEffectInProgress = false;
        private PassThruPostEffect passThruEffect = null;

        private PostEffectChain(IGameApp game)
        {
            _game = game;
        }

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
            effectChain._textureRenderTarget = new RenderTargetView(game.GraphicsDevice, _texture, targetDesc);
            effectChain._auxTextureRenderTarget = new RenderTargetView(game.GraphicsDevice, _auxTexture, targetDesc);

            ShaderResourceViewDescription resourceDesc = new ShaderResourceViewDescription();
            resourceDesc.Format = targetDesc.Format;
            resourceDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            resourceDesc.Texture2D.MostDetailedMip = 0;
            resourceDesc.Texture2D.MipLevels = 1;
            effectChain._textureShaderResource = new ShaderResourceView(game.GraphicsDevice, _texture, resourceDesc);
            effectChain._auxTextureShaderResource = new ShaderResourceView(game.GraphicsDevice, _auxTexture, resourceDesc);

            ShaderResourceViewDescription depthResourceDesc = new ShaderResourceViewDescription();
            depthResourceDesc.Format = SharpDX.DXGI.Format.R32_Float;
            depthResourceDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            depthResourceDesc.Texture2D.MostDetailedMip = 0;
            depthResourceDesc.Texture2D.MipLevels = 1;            
            effectChain._depthShaderResource = new ShaderResourceView(game.GraphicsDevice, game.DepthStencil.ResourceAs<Texture2D>(), depthResourceDesc);

            effectChain.passThruEffect = new PassThruPostEffect(game.GraphicsDevice, game);           
            //byte[] targetShaderByteCode = effectChain.passThruEffect.SelectShaderByteCode();        
            
            return effectChain;
            //TODO: Validate this method call.
        }

        public void Begin(long gameTime)
        {
            _originalRenderView = _game.GraphicsDeviceContext.OutputMerger.GetRenderTargets(1, out _originalDepthStencilView)[0];            
            _game.GraphicsDeviceContext.OutputMerger.SetTargets(_originalDepthStencilView, _textureRenderTarget);
            _game.GraphicsDeviceContext.ClearRenderTargetView(_textureRenderTarget, Color.Black);
           // _game.GraphicsDeviceContext.ClearDepthStencilView(_game.DepthStencil, DepthStencilClearFlags.Depth, 1.0f, 0);
            _isEffectInProgress = true;
        }

        public void End(long gameTime)
        {
            //TODO: Validate this method call.   
            
            foreach (PostEffect postEffect in this)
            {
                _game.GraphicsDeviceContext.OutputMerger.SetTargets(_auxTextureRenderTarget);
                //_game.GraphicsDeviceContext.ClearRenderTargetView(_auxTextureRenderTarget, Color.Black);
                //_game.GraphicsDeviceContext.ClearDepthStencilView(_game.DepthStencil, DepthStencilClearFlags.Depth, 1.0f, 0);                
                postEffect.Texture = _textureShaderResource;
                //postEffect.Depth = _depthShaderResource;
                postEffect.Apply();       
                Swap<ShaderResourceView>(ref _textureShaderResource, ref _auxTextureShaderResource);
                Swap<RenderTargetView>(ref _textureRenderTarget, ref _auxTextureRenderTarget);
            }
                
            _game.GraphicsDeviceContext.OutputMerger.SetTargets(_originalDepthStencilView, _originalRenderView);            
            //_game.GraphicsDeviceContext.ClearDepthStencilView(_game.DepthStencil, DepthStencilClearFlags.Depth, 1.0f, 0);
            passThruEffect.Texture = _textureShaderResource;
            passThruEffect.Apply();        

            this._isEffectInProgress = false;
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public bool IsEffectInProgress { get { return _isEffectInProgress; } }
    }
}
