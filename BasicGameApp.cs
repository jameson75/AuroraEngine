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
using CipherPark.AngelJacket.Core.Services;
using System.Windows.Forms;

namespace CipherPark.AngelJacket.Core
{
    public class BasicGameApp : IGameApp
    {
        private SharpDX.Direct3D11.Device _graphicsDevice = null;
        private SwapChain _swapChain = null;
        private DeviceContext _graphicsDeviceContext = null;
        private Texture2D _renderTargetBuffer = null;
        private RenderTargetView _renderTargetView = null;
        private Texture2D _depthStencilBuffer = null;
        private DepthStencilView _depthStencilView = null;
        private XAudio2 _xaudio2Device = null;
        private MasteringVoice _masteringVoice = null;
        private IntPtr _deviceHwnd = IntPtr.Zero;
        private DirectInput _directInput = null;
        private Mouse _mouse = null;
        private Keyboard _keyboard = null;
        private ServiceTable _services = null;

        public BasicGameApp()
        {
            _services = new ServiceTable();
        }

        public void Run(Form form)
        {
            InitializeDirectXResources(form);    

            LoadContent();

            RenderLoop.Run(form, delegate()
            {
                Update();
                Draw();
            });

            UnloadContent();
        }

        protected virtual void Update()
        { }

        protected virtual void Draw()
        { }

        protected virtual void LoadContent()
        { }

        protected virtual void UnloadContent()
        { }

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
                Usage = Usage.RenderTargetOutput
            };
            SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, chainDesc, out _graphicsDevice, out _swapChain);
            _graphicsDeviceContext = _graphicsDevice.ImmediateContext;

            _renderTargetBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);
            _renderTargetView = new RenderTargetView(_graphicsDevice, _renderTargetBuffer);

            Texture2DDescription renderStencilDesc = new Texture2DDescription()
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = form.ClientSize.Width,
                Height = form.ClientSize.Height,
                SampleDescription = new SampleDescription(1,0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };
            _depthStencilBuffer = new Texture2D(_graphicsDevice, renderStencilDesc);
            _depthStencilView = new DepthStencilView(_graphicsDevice, _depthStencilBuffer);
            _graphicsDeviceContext.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height));
            _graphicsDeviceContext.OutputMerger.SetTargets(DepthStencil, RenderTarget);

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

        public RenderTargetView RenderTarget
        {
            get { return _renderTargetView; }
        }

        public DepthStencilView DepthStencil
        {
            get { return _depthStencilView; }
        }

        public SwapChain SwapChain
        {
            get { return _swapChain; }
        }

        #endregion
    }
}
