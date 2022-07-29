using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Wpf.Interop.DirectX;

namespace Aurora.Sample.ContentBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private D3D11Image interopImage;
        private ContentBuilderGameApp game;
        private bool lastVisible;
        private TimeSpan lastRender;

        public MainWindow()
        {           
            InitializeComponent();
            InitializeGameHosting();
        }

        public void InitializeGameHosting()
        {
            game = new ContentBuilderGameApp();
            interopImage = new D3D11Image();
            host.Loaded += Host_Loaded;
            host.SizeChanged += Host_SizeChanged;            
        }

        private void Host_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double dpiScale = 1.0; // default value for 96 dpi

            // determine DPI
            // (as of .NET 4.6.1, this returns the DPI of the primary monitor, if you have several different DPIs)
            var hwndTarget = PresentationSource.FromVisual(this).CompositionTarget as HwndTarget;
            if (hwndTarget != null)
            {
                dpiScale = hwndTarget.TransformToDevice.M11;
            }

            int surfWidth = (int)(host.ActualWidth < 0 ? 0 : Math.Ceiling(host.ActualWidth * dpiScale));
            int surfHeight = (int)(host.ActualHeight < 0 ? 0 : Math.Ceiling(host.ActualHeight * dpiScale));

            // Notify the D3D11Image of the pixel size desired for the DirectX rendering.
            // The D3DRendering component will determine the size of the new surface it is given, at that point.
            interopImage.SetPixelSize(surfWidth, surfHeight);

            // Stop rendering if the D3DImage isn't visible - currently just if width or height is 0
            // TODO: more optimizations possible (scrolled off screen, etc...)
            bool isVisible = (surfWidth != 0 && surfHeight != 0);
            if (lastVisible != isVisible)
            {
                lastVisible = isVisible;
                if (lastVisible)
                {
                    CompositionTarget.Rendering += CompositionTarget_Rendering;
                }
                else
                {
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
                }
            }
        }
       
        private void Host_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeVisualization();
            InitializeRendering();
            InitializeUpdates();
        }        

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            RenderingEventArgs args = (RenderingEventArgs)e;
            // It's possible for Rendering to call back twice in the same frame 
            // so only render when we haven't already rendered in this frame.
            if (lastRender != args.RenderingTime)
            {
                interopImage.RequestRender();
                lastRender = args.RenderingTime;
            }
            //TODO: Uncomment this only after we've confirmed that navigation speed is constant, irrespective of frame rate.
            //this.OnUpdate(); 
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UninitializeRendering();
        }

        private void InitializeVisualization()
        {            
            game.Initialize(this);
        }

        private void InitializeRendering()
        {
            hostImage.Source = interopImage;
            interopImage.WindowOwner = new WindowInteropHelper(this).Handle;
            interopImage.OnRender = this.OnRender;
            interopImage.RequestRender();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }      

        private void InitializeUpdates()
        {
            //TODO: Remove this method and peform OnUpdate() in CompositionTarget_Rendering handler
            //only after we've confirmed that navigation speed will be constant, irrespective of frame rate,
            //when calling OnUpdate() from that handler.
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Hooks.DispatcherInactive += delegate
            {
                OnUpdate();
            };

        }

        private void UninitializeRendering()
        {
            game.Uninitialize();
            CompositionTarget.Rendering -= this.CompositionTarget_Rendering;
        }

        private void OnUpdate()
        {
            game.Update();
        }

        private void OnRender(IntPtr surface, bool isNewSurface)
        {
            game.Render(surface, isNewSurface);
        }        
    }
}
