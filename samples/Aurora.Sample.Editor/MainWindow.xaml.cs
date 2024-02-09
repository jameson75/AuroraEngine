using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Aurora.Core.Editor;
using Aurora.Core.Editor.Util;
using CipherPark.Aurora.Core.Extensions;
using CipherPark.Aurora.Core.Services;
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
        private SelectedNodeTwoWayBinding selectedNodeTwoWayBinding;
        private System.Windows.Point mouseLocationWhenContextMenuOpened;

        public MainWindow()
        {           
            InitializeComponent();
            InitializeGraphicsHost();           
        }        

        private void InitializeMVVM()
        {
            controller = new MainWindowController(game);
            DataContext = controller.ViewModel;
            selectedNodeTwoWayBinding = new SelectedNodeTwoWayBinding(sceneTreeView, controller.ViewModel);            
            controller.NewProject(); //TODO: Remove implicitly created new project            
        }

        private void InitializeGraphicsHost()
        {
            game = new EditorGameApp(host);
            interopImage = new D3D11Image();
            host.Loaded += Host_Loaded;
            host.SizeChanged += Host_SizeChanged; 
            host.MouseRightButtonDown += Host_MouseRightButtonDown;
            host.MouseDoubleClick += Host_MouseDoubleClick; ;
        }

        private void Host_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            controller.NotifyVisualizationOfDoubleClick(e.GetPosition(host));
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
            InitializeMVVM();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            RenderingEventArgs args = (RenderingEventArgs)e;    

            // It's possible for Rendering to call back twice in the same frame 
            // so only render when we haven't already rendered in this frame.
            if (lastRender != args.RenderingTime)
            {
                OnUpdate();
                interopImage.RequestRender();
                lastRender = args.RenderingTime;
            }            
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
            //TODO: Remove this method and peform OnUpdate() in OnRender handler
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                controller.NotifyKeyboardShiftKeyEvent(true);
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                controller.NotifyKeyboardShiftKeyEvent(false);
            }

            base.OnKeyUp(e);
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

        private void Menu_Scene_AddLight_Click(object sender, RoutedEventArgs e)
        {
            controller.UIAddLight();
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

        private void ToolBar_ResetCamera_Click(object sender, RoutedEventArgs e)
        {           
            controller.ResetEditorCamera();
        }

        private void ToolBar_SelectObject_Click(object sender, RoutedEventArgs e)
        {
            controller.EnterEditorMode(EditorMode.SelectSceneObject);
            controller.SetEditorTransformMode(EditorTransformMode.None);
        }

        private void ToolBar_SelectTransformPlaneXZ_Click(object sender, RoutedEventArgs e)
        {
            controller.EnterEditorMode(EditorMode.SelectSceneObject);
            controller.SetEditorTransformMode(EditorTransformMode.ViewSpaceTranslateXZ);
        }

        private void ToolBar_SelectTransformPlaneY_Click(object sender, RoutedEventArgs e)
        {
            controller.EnterEditorMode(EditorMode.SelectSceneObject);
            controller.SetEditorTransformMode(EditorTransformMode.ViewSpaceTranslateY);
        }

        private void ToolBar_SelectOrbitAboutX_Click(object sender, RoutedEventArgs e)
        {
            controller.EnterEditorMode(EditorMode.SelectSceneObject);
            controller.SetEditorTransformMode(EditorTransformMode.ParentSpaceRevolveX);
        }

        private void ToolBar_SelectOrbitAboutY_Click(object sender, RoutedEventArgs e)
        {
            controller.EnterEditorMode(EditorMode.SelectSceneObject);
            controller.SetEditorTransformMode(EditorTransformMode.ParentSpaceRevolveY);
        }

        private void ToolBar_SelectOrbitAboutZ_Click(object sender, RoutedEventArgs e)
        {
            controller.EnterEditorMode(EditorMode.SelectSceneObject);
            controller.SetEditorTransformMode(EditorTransformMode.ParentSpaceRevolveZ);
        }

        private void ToolBar_SelectRotateAboutX_Click(object sender, RoutedEventArgs e)
        {
            controller.EnterEditorMode(EditorMode.SelectSceneObject);
            controller.SetEditorTransformMode(EditorTransformMode.LocalSpaceRotateX);
        }

        private void ToolBar_SelectRotateAboutY_Click(object sender, RoutedEventArgs e)
        {
            controller.EnterEditorMode(EditorMode.SelectSceneObject);
            controller.SetEditorTransformMode(EditorTransformMode.LocalSpaceRotateY);
        }

        private void ToolBar_SelectRotateAboutZ_Click(object sender, RoutedEventArgs e)
        {
            controller.EnterEditorMode(EditorMode.SelectSceneObject);
            controller.SetEditorTransformMode(EditorTransformMode.LocalSpaceRotateZ);
        }

        private void ToolBar_SelectTranslateOrbit_Click(object sender, RoutedEventArgs e)
        {
            controller.EnterEditorMode(EditorMode.SelectSceneObject);
            controller.SetEditorTransformMode(EditorTransformMode.OrbitDistanceTranslate);
        }
        #endregion       

        private void Menu_Scene_MoveReferenceGrid(object sender, RoutedEventArgs e)
        {
            var guardianNode = this.game.Scene.SelectNodes(n => n.Name?.Contains("Gardian") == true).FirstOrDefault();
            if (guardianNode != null)
            {
                var referenceObjectRoot = this.game.Scene.SelectReferenceObjectRoot();
                var referenceGridNode = referenceObjectRoot.SelectNodes(n => n.GetGameObject()?.IsReferenceGridObject() == true).First().As<GameObjectSceneNode>();
                 
                var oldEye = game.Scene.CameraNode.WorldPosition();
                var oldLookAtRay = new SharpDX.Ray(game.Scene.CameraNode.WorldPosition(), SharpDX.Vector3.Normalize(game.Scene.CameraNode.Camera.Forward));               
                referenceGridNode.GetWorldBoundingBox().Intersects(ref oldLookAtRay, out SharpDX.Vector3 oldLookAt);

                var offset = Vector3.Zero.AddY(guardianNode.WorldPosition().Y - referenceGridNode.WorldPosition().Y);
                var newEye = oldEye + offset;
                var newLookAt = oldLookAt + offset;              

                referenceGridNode.TranslateTo(Vector3.Zero.AddY(guardianNode.WorldPosition().Y));
                                
                game.Scene.CameraNode.Camera.ViewMatrix = SharpDX.Matrix.LookAtLH(
                    newEye,
                    newLookAt,
                    SharpDX.Vector3.Up);

                //game.Services.GetService<MouseNavigatorService>().ResetTracking();
            }
        }

        private void Host_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ContextMenu menu = this.FindResource("sceneContextMenu") as ContextMenu;
            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;            
            menu.IsOpen = true;
            mouseLocationWhenContextMenuOpened = Mouse.GetPosition(host);
        }

        #region Context Menu Handlers
        private void ContextMenu_MarkLocation_Click(object sender, RoutedEventArgs e)
        {
            controller.MarkSelectedNodeLocation();
        }

        private void ContextMenu_ClearLocationMarkers_Click(object sender, RoutedEventArgs e)
        {
            controller.ClearAllLocationMarkers();
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            game.IsViewportFeedbackSuspended = true;
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            game.IsViewportFeedbackSuspended = false;
        }

        #endregion

        private void ContextMenu_NewActionRig_Click(object sender, RoutedEventArgs e)
        {
            controller.CreateNewActionRig(mouseLocationWhenContextMenuOpened);
        }

        private void ContextMenu_NewNavigationPath_Click(object sender, RoutedEventArgs e)
        {
            controller.CreateNewNavigationPath(mouseLocationWhenContextMenuOpened);
        }

        private void ContentMenu_ExtrudeNavigationPath_Click(object sender, RoutedEventArgs e)
        {
            controller.ExtrudeNavigationPath();
        }

        private void ContextMenu_BisectNavigationSegment_Click(object sender, RoutedEventArgs e)
        {
            controller.BisectNavigationPath();
        }

        private void ContentMenu_JustExecute_Click(object sender, RoutedEventArgs e)
        {
            controller.JustExecute();
        }

        private void Model_NewButton_Click(object sender, RoutedEventArgs e)
        {
            controller.NewModel();
        }
    }
}
