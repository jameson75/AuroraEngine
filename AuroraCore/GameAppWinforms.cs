using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Windows;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.XAudio2;
using SharpDX.DirectInput;
using CipherPark.Aurora.Core.Services;
using System.Windows.Forms;
using SharpDX.Mathematics.Interop;
using SharpDX.Mathematics;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core
{
    public class GameAppWinforms : IGameApp
    {
        private SharpDX.Direct3D11.Device _graphicsDevice = null;

        #region Graphics 
        private SwapChain _swapChain = null;
        private DeviceContext _graphicsDeviceContext = null;
        private Texture2D _renderTargetBuffer = null;
        private RenderTargetView _renderTargetView = null;
        private ShaderResourceView _renderTargetShaderResource = null;
        private Texture2D _depthStencilBuffer = null;
        private DepthStencilView _depthStencilView = null;
        #endregion

        #region Audio
        private XAudio2 _xaudio2Device = null;
        private MasteringVoice _masteringVoice = null;
        #endregion

        #region Input
        private IntPtr _deviceHwnd = IntPtr.Zero;
        private DirectInput _directInput = null;
        private Mouse _mouse = null;
        private Keyboard _keyboard = null;
        #endregion

        private ServiceTable _services = null;
        private bool _closeFormOnUpdate = false;
        private GameTime _gameTime = null;
        private Form _form = null;

        public GameAppWinforms()
        {
            _services = new ServiceTable();
            _gameTime = new GameTime();
        }        

        protected GameTime GameTime { get { return _gameTime; } } 

        public void Run(Form form)
        {
            _form = form;
            
            form.FormClosing += form_FormClosing;
            form.SizeChanged += form_SizeChanged;
            
            InitializeDirectXResources(form);           

            Initialize();

            LoadContent();

            RenderLoop.Run(form, delegate()
            {
                Update();
                Draw();
                if (_closeFormOnUpdate)
                    form.Close();                  
            });                    
        }

        public void ToggleFullScreen()
        {
            SwapChain.IsFullScreen = !SwapChain.IsFullScreen;                  
        }

        public event Action BuffersResizing;

        public event Action BuffersResized;

        public event Action ViewportSizeChanged;

        protected virtual void Update()
        { }

        protected virtual void Draw()
        { }

        protected virtual void Initialize()
        { }

        protected virtual void LoadContent()
        { }

        protected virtual void UnloadContent()
        { }

        protected virtual void Uninitialize()
        {
            UninitializeDirectXResources();
        }

        protected void Exit()
        {
            _closeFormOnUpdate = true;
        }

        protected void ReinitializeDevice()
        {
            UninitializeDirectXResources();
            InitializeDirectXResources(_form);
        }

        protected virtual void OnBuffersResizing()
        {
            BuffersResizing?.Invoke();
        }

        protected virtual void OnBuffersResized()
        {
            BuffersResized?.Invoke();
        }

        private void InitializeDirectXResources(Form form)
        {           
            _deviceHwnd = form.Handle;

            //Initialize Direct3D11 Resources.
            //--------------------------------
            SwapChainDescription chainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput | Usage.ShaderInput,                
            };

            SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, chainDesc, out _graphicsDevice, out _swapChain);
            _graphicsDeviceContext = _graphicsDevice.ImmediateContext;

            // Ignore all windows events
            var factory = _swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

            CreateResourcesFromSwapChain(form);            

            //Initialize XAudio2 Resources.
            //-----------------------------
            _xaudio2Device = new XAudio2();
            _masteringVoice = new MasteringVoice(_xaudio2Device);

            //Initialize Direct Input Resources.
            //----------------------------------
            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
            _keyboard.Acquire();
            _mouse = new Mouse(_directInput);
            _mouse.Acquire();
        }

        private void UninitializeDirectXResources()
        {
            _swapChain.IsFullScreen = false;

            //Dispose of graphic resources
            //----------------------------
            _renderTargetView.Dispose();
            _renderTargetShaderResource.Dispose();
            _renderTargetBuffer.Dispose();
            _swapChain.Dispose();
            _depthStencilBuffer.Dispose();
            _depthStencilView.Dispose();
            _graphicsDevice.Dispose();
            _graphicsDeviceContext.Dispose();

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

        private void CreateResourcesFromSwapChain(Form form)
        {
            _renderTargetBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);
            _renderTargetView = new RenderTargetView(_graphicsDevice, _renderTargetBuffer);
            _renderTargetShaderResource = new ShaderResourceView(_graphicsDevice, _renderTargetBuffer);

            Texture2DDescription renderStencilDesc = new Texture2DDescription()
            {
                Format = BestSupportedDepthStencilFormat(_graphicsDevice, false, true),
                ArraySize = 1,
                MipLevels = 1,
                Width = form.ClientSize.Width,
                Height = form.ClientSize.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };
            _depthStencilBuffer = new Texture2D(_graphicsDevice, renderStencilDesc);
            DepthStencilViewDescription dsvd = new DepthStencilViewDescription();
            dsvd.Dimension = DepthStencilViewDimension.Texture2D;
            dsvd.Flags = DepthStencilViewFlags.None;
            dsvd.Format = BestSupportedDepthStencilFormat(_graphicsDevice, false);
            _depthStencilView = new DepthStencilView(_graphicsDevice, _depthStencilBuffer, dsvd);
            _graphicsDeviceContext.Rasterizer.SetViewports(new[] { (RawViewportF) new ViewportF(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f) });
            _graphicsDeviceContext.OutputMerger.SetTargets(DepthStencil, RenderTargetView);

            DepthStencilStateDescription depthStencilStateDesc = DepthStencilStateDescription.Default();
            DepthStencilState dsState = new DepthStencilState(_graphicsDevice, depthStencilStateDesc);
            _graphicsDeviceContext.OutputMerger.DepthStencilState = dsState;
        }

        private void ResizeGraphicsBuffers()
        {
            if (_swapChain != null)
            {
                _renderTargetBuffer.Dispose();
                _renderTargetView.Dispose();
                _renderTargetShaderResource.Dispose();
                _depthStencilBuffer.Dispose();
                _depthStencilView.Dispose();                                
                //_graphicsDeviceContext.ClearState();
                OnBuffersResizing();
                _swapChain.ResizeBuffers(0, _form.ClientSize.Width, _form.ClientSize.Height, Format.Unknown, SwapChainFlags.None);                
                CreateResourcesFromSwapChain(_form);
                OnBuffersResized();
            }
        }

        private void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnloadContent();
            Uninitialize();
        }

        private void form_SizeChanged(object sender, EventArgs e)
        {
           
            if(_form.ClientSize.Width != 0 && _form.ClientSize.Height != 0)
                ResizeGraphicsBuffers();
        }       

        #region IGameApp Members

        public ServiceTable Services
        {
            get { return _services; }
        }

        public DeviceContext GraphicsDeviceContext
        {
            get { return _graphicsDeviceContext; }
        }

        public SharpDX.Direct3D11.Device GraphicsDevice
        {
            get { return _graphicsDevice; }
        }

        public MasteringVoice MasteringVoice
        {
            get { return _masteringVoice; }
        }

        public XAudio2 AudioDevice
        {
            get { return _xaudio2Device; }
        }

        public Keyboard Keyboard
        {
            get { return _keyboard; }
        }

        public Mouse Mouse
        {
            get { return _mouse; }
        }

        public IntPtr DeviceHwnd
        {
            get { return _deviceHwnd; }
        }

        public RenderTargetView RenderTargetView
        {
            get { return _renderTargetView; }
        }

        public ShaderResourceView RenderTargetShaderResource
        {
            get { return _renderTargetShaderResource; }
        }

        public DepthStencilView DepthStencil
        {
            get { return _depthStencilView; }
        }

        public SwapChain SwapChain
        {
            get { return _swapChain; }
        }

        public bool IsWindowed
        {
            get
            {
                RawBool isWindowed = false;
                Output output = null;
                _swapChain.GetFullscreenState(out isWindowed, out output);
                return isWindowed;
            }
            set
            {
                try
                {
                    //NOTE: This call is expected to fail for Windows store Apps.
                    //https://msdn.microsoft.com/en-us/library/windows/desktop/bb174579(v=vs.85).aspx
                    _swapChain.SetFullscreenState(value, null);
                }
                catch { }
            }
        }

        #endregion

        private static Format BestSupportedDepthStencilFormat(SharpDX.Direct3D11.Device graphicsDevice, bool includeStencil = true, bool usingAsShaderResource = false)
        {
            if ((graphicsDevice.CheckFormatSupport(Format.D32_Float_S8X24_UInt) & FormatSupport.DepthStencil) != 0)
                return (includeStencil) ? Format.D32_Float_S8X24_UInt : (usingAsShaderResource) ?  Format.R32_Typeless : Format.D32_Float;
            else
                return (includeStencil) ? Format.D24_UNorm_S8_UInt : (usingAsShaderResource) ? Format.R16_Typeless : Format.D16_UNorm;
        }
    }
}
