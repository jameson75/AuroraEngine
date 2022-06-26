using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Scene;
using Microsoft.Wpf.Interop.DirectX;
using SharpDX;

namespace Aurora.Sample.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private D3D11Image interopImage;
        private EditorGameApp game;
        private TimeSpan lastRender;
        private MainWindowController controller;

        public MainWindow()
        {
            controller = new MainWindowController();
            InitializeComponent();
            InitializeGameHosting();
        }

        public void InitializeGameHosting()
        {
            game = new EditorGameApp(host);
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
            this.OnUpdate(); 
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UninitializeRendering();
        }

        private void InitializeVisualization()
        {
            var hWnd = new WindowInteropHelper(this).Handle;
            game.Initialize(hWnd);
        }

        private void InitializeRendering()
        {           
            hostImage.Source = interopImage;
            interopImage.WindowOwner = new WindowInteropHelper(this).Handle;
            interopImage.OnRender = this.OnRender;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            interopImage.RequestRender();            
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

        #region Menu Handlers
        private void Menu_File_Open_Click(object sender, RoutedEventArgs e)
        {
            string filePath = controller.ChooseFile();
            if(filePath != null)
            {
                controller.ImportModel(game, filePath);                
            }
        }
        #endregion
    }

    public class MainWindowController
    {
        public string ChooseFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".x";
            dialog.Filter = "Direct X (.x)|*.x";
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public byte[] LoadFile(string filePath)
        {
            return System.IO.File.ReadAllBytes(filePath);
        }

        public void ImportModel(EditorGameApp game, string filePath)
        {
            /*
            BlinnPhongEffect2 effect = new BlinnPhongEffect2(game, SurfaceVertexType.InstancePositionNormalColor);                        
            effect.AmbientColor = SharpDX.Color.White;
            effect.Lighting = new Light[]
            {
                    new PointLight
                    {
                        Diffuse = SharpDX.Color.White,
                        Transform = new CipherPark.Aurora.Core.Animation.Transform(new Vector3(500, 500, 500))
                    },
                    new PointLight
                    {
                        Diffuse = SharpDX.Color.White,
                        Transform = new CipherPark.Aurora.Core.Animation.Transform(new Vector3(-500, -500, -500))
                    }
            };
            */
            FlatEffect effect = new FlatEffect(game, SurfaceVertexType.PositionColor);
            var model = ContentImporter.ImportX(game, filePath, effect, XFileChannels.Mesh | XFileChannels.DefaultMaterialColor , XFileImportOptions.IgnoreMissingColors);
            game.Scene.Nodes.Add(new GameObjectSceneNode(game)
            {
                GameObject = new GameObject(game)
                {
                    Renderer = new ModelRenderer(game)
                    {
                        Model = model
                    }
                },
            });
        }
    }
}
