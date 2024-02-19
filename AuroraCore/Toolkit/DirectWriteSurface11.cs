using System;
using SharpDX.Direct2D1;
using DXGIResource = SharpDX.DXGI.Resource;
using DXGISurface1 = SharpDX.DXGI.Surface1;
using DXGIKeyedMutex = SharpDX.DXGI.KeyedMutex;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.Effects;
using System.Collections.Generic;
using CipherPark.Aurora.Core.Extensions;

namespace CipherPark.Aurora.Core.Toolkit
{
    public class DirectWriteSurface11 : IDisposable
    {
        private const int Infinite = -1;        
        private readonly DirectWriteContext11 directWriteAppSupport;
        private readonly IGameApp game;
        private RenderTarget d2dRenderTarget;
        private DXGIKeyedMutex dxgiKeyedMutex11;
        private DXGIKeyedMutex dxgiKeyedMutex10;
        private PassThruPostEffect offscreenTextureEffect;
        private ShaderResourceView sharedTex11Rsv;
        private SharpDX.DirectWrite.Factory dwFactory;
        private RenderTargetView d3d11RenderTargetView;
        private RenderTargetView originalRenderView;
        private DepthStencilView originalDepthStencilView;

        public DirectWriteSurface11(IGameApp gameApp, DirectWriteContext11 directWriteContext11)
        {            
            this.directWriteAppSupport = directWriteContext11;
            this.game = gameApp;
        }

        public DirectWriteContext11 DirectWriteAppSupport { get => directWriteAppSupport; }

        public Texture2DDescription TargetTextureDescription { get; private set; }

        public void Initialize(RenderTargetView renderTarget)
        {
            d3d11RenderTargetView = renderTarget;
            Initialize(renderTarget.GetTextureDescription());
        }

        public void Initialize(Texture2DDescription targetTextureDescription11)
        { 
            // Now what we need is the IDXGISurface to the D3D11 texture.
             var sharedTex11 = CreateSharedTexture(out sharedTex11Rsv, targetTextureDescription11);

            // Get IDXGIRESOURCE from d3d11 sharedTex
            DXGIResource dxgiResource11 = ComObject.As<DXGIResource>(sharedTex11.NativePointer);

            // Get SHAREDHANDLE from IDXGIRESOURCE
            IntPtr sharedHandle = dxgiResource11.SharedHandle;

            // Get IDXGISURFACE from SHAREDHANDLE
            DXGISurface1 dxgiSurface10 = directWriteAppSupport.D3d10Device.OpenSharedResource<DXGISurface1>(sharedHandle);

            // Get the Keyed Mutexes...
            // Now, you need the IDXGIKeyedMutex handles for both the D3D11 and D3D10 versions of your shared resource.
            // You will use these to lock the resource to which ever device is doing any read/write operations with the resource.

            // The [D3D10] KEYED MUTEX is from the dxgi surface
            dxgiKeyedMutex10 = ComObject.As<DXGIKeyedMutex>(dxgiSurface10.NativePointer);

            // D3D11 ALSO needs a [mutex] handle, but it will take it on the D3D11 texture
            dxgiKeyedMutex11 = ComObject.As<DXGIKeyedMutex>(sharedTex11.NativePointer);

            // Connect the IDXGISURFACE to the output of D2D
            // remember that the dxgiSurface goes THRU d3d10 to
            // the d3d11 tex underneath it all.
            Factory d2dFactory = new Factory(FactoryType.SingleThreaded);

            d2dRenderTarget = new RenderTarget(
                d2dFactory,
                dxgiSurface10,
                new RenderTargetProperties(
                    RenderTargetType.Hardware,
                    new PixelFormat(
                        SharpDX.DXGI.Format.Unknown,
                        AlphaMode.Premultiplied),
                    0,
                    0,
                    RenderTargetUsage.None,
                    FeatureLevel.Level_10));
            
            dwFactory = new SharpDX.DirectWrite.Factory(SharpDX.DirectWrite.FactoryType.Shared);            

            offscreenTextureEffect = new PassThruPostEffect(game);

            TargetTextureDescription = targetTextureDescription11;
        }    

        public RenderTarget D2dRenderTarget { get => d2dRenderTarget; }

        public SharpDX.DirectWrite.Factory DwFactory { get => dwFactory; }

        public void DrawText(IEnumerable<DirectText> directTextCollection)
        {
            OnBeginDraw();

            // LOCK the texture and draw d2d
            dxgiKeyedMutex10.Acquire(0, Infinite);

            d2dRenderTarget.BeginDraw();
            d2dRenderTarget.Transform = Matrix3x2.Identity;

            d2dRenderTarget.Clear(Color.Transparent);

            // Use the DrawText method of the D2D render target interface to draw.           
            
            foreach (var directText in directTextCollection)
            {
                if (directText.TextLayout == null)
                {
                    d2dRenderTarget.DrawText(
                      directText.Text,          // The string to render.
                      directText.TextFormat,    // The text format.
                      directText.LayoutRect,    // The region of the window where the text will be rendered.
                      directText.Brush          // The brush used to draw the text.
                    );
                }
                else
                {
                    d2dRenderTarget.DrawTextLayout(
                        directText.LayoutPosition,
                        directText.TextLayout,
                        directText.Brush);
                }
            }

            d2dRenderTarget.EndDraw();
            
            // because we release on 1,
            // the only bit of code that can pick up the mutex next
            // must pick up on 1
            dxgiKeyedMutex10.Release(1);

            // RENDER THE TEXT LAYER
            dxgiKeyedMutex11.Acquire(1, Infinite);

            offscreenTextureEffect.InputTexture = sharedTex11Rsv;
            offscreenTextureEffect.Apply();

            dxgiKeyedMutex11.Release(0);

            OnEndDraw();
        }
        
        private void OnBeginDraw()
        {
            if (d3d11RenderTargetView != null)
            {
                //capture orginal render targets.           
                originalRenderView = game.GraphicsDeviceContext.OutputMerger.GetRenderTargets(1, out originalDepthStencilView)[0];

                //set temp render target.
                game.GraphicsDeviceContext.OutputMerger.SetTargets(d3d11RenderTargetView);

                //clear temp render target.
                game.GraphicsDeviceContext.ClearRenderTargetView(d3d11RenderTargetView, Color.Transparent);
            }
        }

        private void OnEndDraw()
        {
            if (d3d11RenderTargetView != null)
            {
                //restore the original render targets.
                game.GraphicsDeviceContext.OutputMerger.SetTargets(originalDepthStencilView, originalRenderView);

                //Release the COM pointers created during the call to OutputMergerStage::GetRenderTargets() in OnBeginDraw().
                originalDepthStencilView?.Dispose();
                originalRenderView.Dispose();
            }
        }       

        private Texture2D CreateSharedTexture(out ShaderResourceView resourceView, Texture2DDescription targetTextureDescription11)
        {            
            Texture2DDescription desc = new Texture2DDescription()
            {
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                Width = targetTextureDescription11.Width,
                Height = targetTextureDescription11.Height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.SharedKeyedmutex,
                SampleDescription = new SharpDX.DXGI.SampleDescription()
                {
                    Count = 1,
                    Quality = 0,
                },
                Usage = ResourceUsage.Default
            };

            var texture = new Texture2D(game.GraphicsDevice, desc);
            resourceView = new ShaderResourceView(game.GraphicsDevice, texture);
            return texture;
        }

        public void Dispose()
        {
            d2dRenderTarget?.Dispose();
            dxgiKeyedMutex11?.Dispose();
            dxgiKeyedMutex10?.Dispose();
            offscreenTextureEffect?.Dispose();
            sharedTex11Rsv?.Dispose();
            dwFactory?.Dispose();
            d3d11RenderTargetView?.Dispose();

            d2dRenderTarget = null;
            dxgiKeyedMutex11 = null;
            dxgiKeyedMutex10 = null;
            offscreenTextureEffect = null;
            sharedTex11Rsv = null;
            dwFactory = null;
            d3d11RenderTargetView = null;
        }
    }
}
