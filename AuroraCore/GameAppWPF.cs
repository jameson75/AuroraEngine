using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.XAudio2;
using SharpDX.DirectInput;
using DXDevice = SharpDX.Direct3D11.Device;
using DXGIResource = SharpDX.DXGI.Resource;
using DXResource = SharpDX.Direct3D11.Resource;
using CipherPark.Aurora.Core.Services;
using System.Windows;
using System.Windows.Interop;

namespace CipherPark.Aurora.Core
{   
    public class GameAppWPF : IGameApp
    {
        private DriverType _driverType = DriverType.Null;
        private DXDevice _graphicsDevice = null;
        private RenderTargetView _renderTargetView = null;
        private ShaderResourceView _renderTargetShaderResource = null;
        private Texture2D _depthStencilBuffer = null;
        private DepthStencilView _depthStencilView = null;
        private MasteringVoice _masteringVoice = null;
        private XAudio2 _xaudio2Device = null;
        private DirectInput _directInput = null;
        private Mouse _mouse = null;
        private Keyboard _keyboard = null;
        private IntPtr _deviceHwnd = IntPtr.Zero;
        private int _width = 0;
        private int _height = 0;
        private Window _window = null;
        
        
        private bool isInitializing = false;

        public GameTime GameTime { get; } = new GameTime();        

        public IntPtr DeviceHwnd { get { return _deviceHwnd; } }

        public virtual bool IsViewportWindowActive { get { return _window.IsActive; } }

        public DeviceContext GraphicsDeviceContext { get { return _graphicsDevice.ImmediateContext; } }

        public DXDevice GraphicsDevice { get { return _graphicsDevice; } }

        public RenderTargetView RenderTargetView { get { return _renderTargetView; } }

        public ShaderResourceView RenderTargetShaderResource { get { return _renderTargetShaderResource; } }

        public DepthStencilView DepthStencil { get { return _depthStencilView; } }

        public MasteringVoice MasteringVoice { get { return _masteringVoice; } }

        public XAudio2 AudioDevice { get { return _xaudio2Device; } }

        public Keyboard Keyboard { get { return _keyboard; } }

        public Mouse Mouse { get { return _mouse; } }

        public event Action BuffersResizing;

        public event Action BuffersResized;

        public event Action ViewportSizeChanged;

        public ServiceTable Services { get; } = new ServiceTable();

        public GameAppWPF()
        {
            
        }

        public void Initialize(Window window)
        {
            //NOTE: Any initialization code that depends on render targets being created
            //should be placed in ContinueInitialize();

            _window = window;

            //We only initialize the directX3D device and XAudio at this point.
            //We don't create render targets until the very first call to Render. 
            //This is because we're rendering to a DirectX9 buffer, which we only have
            //access to via Render().
            InitializeDirectXDevice();
            //Cache device window
            _deviceHwnd = new WindowInteropHelper(window).Handle;           
            //Set a flag to indicate that we need to continue intializing.
            isInitializing = true;
            //Register game app services.
            OnInitializing();
        }

        public void Update()
        {            
            if (!isInitializing)
            {
                GameTime.Update();
                OnUpdate();      
            }      
        }

        public void Render(IntPtr pResource, bool isNewSurface)
        {
            // If we've gotten a new DX9 Surface, we need to initialize the renderTarget.
            // This happens the first time the surface is created and each time it's resized.    
            if (isNewSurface)
            {
                if (isInitializing)
                {
                    InitializeDirectXRenderViews(pResource);
                    ContinueInitialize();
                }
                else
                    RecreateDirectXRenderViews(pResource);               
            }

            OnRender(isNewSurface);
        }

        public void Uninitialize()
        {
            OnUninitializing();

            //Dispose of graphic resources
            //----------------------------
            _renderTargetView.Dispose();
            _renderTargetShaderResource.Dispose();
            _depthStencilBuffer.Dispose();
            _depthStencilView.Dispose();
            _graphicsDevice.Dispose();

            //Dispose of audio resources
            //--------------------------
            _masteringVoice.DestroyVoice();
            _masteringVoice.Dispose();
            _xaudio2Device.Dispose();

            //Dispose of input resources
            //--------------------------            
            _keyboard.Dispose();
            _mouse.Dispose();
            _directInput.Dispose();
        }        

        protected virtual void OnInitializing()
        {

        }

        protected virtual void OnInitialized()
        {

        }

        protected virtual void OnUpdate()
        {

        }

        protected virtual void OnRender(bool isNewSurface)
        {
            
        }

        protected virtual void OnUninitializing()
        {

        }

        #region Helpers

        private void InitializeDirectXDevice()
        {
            DriverType[] driverTypes = new[]
          {
                DriverType.Hardware,
                DriverType.Warp,
                DriverType.Reference
            };

            // DX10 or 11 devices are suitable
            FeatureLevel[] featureLevels = new[]
            {
                FeatureLevel.Level_11_0,
                FeatureLevel.Level_10_1,
                FeatureLevel.Level_10_0,
            };

            for (int driverTypeIndex = 0; driverTypeIndex < driverTypes.Length; driverTypeIndex++)
            {
                try
                {
                    _graphicsDevice = new DXDevice(driverTypes[driverTypeIndex], DeviceCreationFlags.Debug | DeviceCreationFlags.BgraSupport, featureLevels);
                    _driverType = driverTypes[driverTypeIndex];
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Could not create a Direct3D 10 or 11 device", ex);
                }

                if (_graphicsDevice != null)
                    break;
            }

            //Initialize XAudio2 Resources.
            //-----------------------------
            _xaudio2Device = new XAudio2();
            _masteringVoice = new MasteringVoice(AudioDevice);

            //Initialize Direct Input Resources.
            //----------------------------------
            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
            _keyboard.Acquire();
            _mouse = new Mouse(_directInput);
            _mouse.Acquire();
        }

        private void InitializeDirectXRenderViews(IntPtr pResource)
        {
            CreateResourcesFromDX9Surface(pResource);
            isInitializing = false;
        }

        private void RecreateDirectXRenderViews(IntPtr pResource)
        {
            _graphicsDevice.ImmediateContext.OutputMerger.SetRenderTargets(null, (RenderTargetView)null);
            if (_renderTargetView != null)
            {
                _renderTargetView.Dispose();
                _renderTargetView.Dispose();
                _depthStencilBuffer.Dispose();
                _depthStencilView.Dispose();
                _renderTargetShaderResource.Dispose();
            }
            BuffersResizing?.Invoke();
            CreateResourcesFromDX9Surface(pResource);
            BuffersResized?.Invoke();
        }

        private void CreateResourcesFromDX9Surface(IntPtr pResource)
        {
            //Step 1. Create render target view and shader resource view.           
            //****************************************************************************************************
            //NOTE: The target buffer (pResource) beging rendered to is a directx9 api buffer. However, we're
            //using directX11.x to perform the rendering. Since directx11 cannot access a directx9 resource/buffer
            //directly we must leverage DXGI to act as adapter.            
            //****************************************************************************************************

            //Cast the directx9 buffer as a dxgi resource we can share with directx11.
            DXGIResource resource = ComObject.As<DXGIResource>(pResource);
            IntPtr sharedHandle = resource.SharedHandle;
            //Access the dxgi shared resource.
            DXResource tempResource11 = _graphicsDevice.OpenSharedResource<DXResource>(sharedHandle);
            Texture2D outputResource = ComObject.As<Texture2D>(tempResource11.NativePointer);
            //Describe the render target view we wish to create.
            RenderTargetViewDescription rtDesc = new RenderTargetViewDescription();
            rtDesc.Format = Format.B8G8R8A8_UNorm;
            rtDesc.Dimension = RenderTargetViewDimension.Texture2D;
            rtDesc.Texture2D.MipSlice = 0;            
            //Create a render target view based on our description and the dxgi shared resource.
            _renderTargetView = new RenderTargetView(_graphicsDevice, outputResource, rtDesc);
            //Create a render target shader resource based on the dxgi shared resource.
            _renderTargetShaderResource = new ShaderResourceView(GraphicsDevice, outputResource);

            //Step 2. Create depth / stencil buffer 
            //Describe the depth stencil buffer we wish to create.
            Texture2DDescription renderStencilDesc = new Texture2DDescription()
            {
                Format = BestSupportedDepthStencilFormat(GraphicsDevice, false, false),
                ArraySize = 1,
                MipLevels = 1,
                Width = outputResource.Description.Width,
                Height = outputResource.Description.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };
            //Create the buffer.
            _depthStencilBuffer = new Texture2D(GraphicsDevice, renderStencilDesc);
            //Describe the depth stencil view we wish to create.
            DepthStencilViewDescription dsvd = new DepthStencilViewDescription();
            dsvd.Dimension = DepthStencilViewDimension.Texture2D;
            dsvd.Flags = DepthStencilViewFlags.None;
            dsvd.Format = BestSupportedDepthStencilFormat(GraphicsDevice, false);
            //Create the depth stencil view from description and the buffer we just created.
            _depthStencilView = new DepthStencilView(GraphicsDevice, _depthStencilBuffer, dsvd);

            //Step 3. Set device targets.
            //Set the device's render target view and depth stencil view using the views we just created.
            _graphicsDevice.ImmediateContext.OutputMerger.SetRenderTargets(_depthStencilView, _renderTargetView);

            if (outputResource.Description.Width != _width || outputResource.Description.Height != _height)
            {
                _width = outputResource.Description.Width;
                _height = outputResource.Description.Height;
                ViewportF vp;
                vp.Width = (float)_width;
                vp.Height = (float)_height;
                vp.MinDepth = 0.0f;
                vp.MaxDepth = 1.0f;
                vp.X = 0;
                vp.Y = 0;
                _graphicsDevice.ImmediateContext.Rasterizer.SetViewport(vp);
                ViewportSizeChanged?.Invoke();
            }
        }        

        private void ContinueInitialize()
        {
            OnInitialized();
        }
        #endregion                     

        private static Format BestSupportedDepthStencilFormat(SharpDX.Direct3D11.Device graphicsDevice, bool includeStencil = true, bool usingAsShaderResource = false)
        {
            if ((graphicsDevice.CheckFormatSupport(Format.D32_Float_S8X24_UInt) & FormatSupport.DepthStencil) != 0)
                return (includeStencil) ? Format.D32_Float_S8X24_UInt : (usingAsShaderResource) ? Format.R32_Typeless : Format.D32_Float;
            else
                return (includeStencil) ? Format.D24_UNorm_S8_UInt : (usingAsShaderResource) ? Format.R16_Typeless : Format.D16_UNorm;
        }
    }    
}