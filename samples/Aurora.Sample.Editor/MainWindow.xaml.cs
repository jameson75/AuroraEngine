using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Aurora.Core.Editor;
using Microsoft.Wpf.Interop.DirectX;

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
            InitializeComponent();
            InitializeGameHosting();
            InitializeControllers();
        }

        public SceneViewModel Scene { get; } = new SceneViewModel();

        private void InitializeControllers()
        {
            controller = new MainWindowController(game);
            DataContext = controller.ViewModel;            
            controller.NewProject(); //TODO: Remove implicitly created new project.            
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
            game.Initialize(this);
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
            controller.UIOpenProject();            
        }        

        private void Menu_File_Save_Click(object sender, RoutedEventArgs e)
        {
            controller.UISaveProject();
        }

        private void Menu_File_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            controller.UISaveAsProject();
        }

        private void Menu_File_Import_Click(object sender, RoutedEventArgs e)
        {
            controller.UIImportModel();
        }

        private void Menu_Project_Settings_Click(object sender, RoutedEventArgs e)
        {
            controller.UIChangeSettings();
        }
        #endregion

        #region ToolBar Handlers
        private void ToolBar_RotateCamera_Click(object sender, RoutedEventArgs e)
        {
            controller.EnterEditorMode(EditorMode.RotateCamera);
        }

        private void ToolBar_TraverseCamera_Click(object sender, RoutedEventArgs e)
        {
            controller.EnterEditorMode(EditorMode.TraverseCamera);
        }

        private void ToolBar_PanCamera_Click(object sender, RoutedEventArgs e)
        {
            controller.EnterEditorMode(EditorMode.PanCamera);
        }

        private void ToolBar_SelectSceneObject_Click(object sender, RoutedEventArgs e)
        {
            controller.EnterEditorMode(EditorMode.SelectSceneObject);
        }

        private void ToolBar_ResetCamera_Click(object sender, RoutedEventArgs e)
        {
            controller.ResetEditorCamera();
        }
        
        private void ToolBar_SelectTransformPlaneXZ_Click(object sender, RoutedEventArgs e)
        {
            controller.SetEditorTransformPlane(EditorTransformPlane.XZ);
        }

        private void ToolBar_SelectTransformPlaneY_Click(object sender, RoutedEventArgs e)
        {
            controller.SetEditorTransformPlane(EditorTransformPlane.XY);
        }
        #endregion

        private void SceneTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            controller.ViewModel.SelectedNode = (SceneNodeViewModel)e.NewValue;
        }
    }
}
