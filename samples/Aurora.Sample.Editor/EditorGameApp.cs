using CipherPark.Aurora.Core;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.UI.Controls;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Utils;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Windows;
using Aurora.Sample.Editor.Services;
using Aurora.Sample.Editor.Scene;
using Aurora.Sample.Editor.Scene.UI.Behavior;
using Aurora.Core.Editor;
using System.Linq;
using CipherPark.Aurora.Core.Animation;
using Aurora.Core.Editor.Util;

namespace Aurora.Sample.Editor
{
    public class EditorGameApp : GameAppWPF, IContainActiveScene, IEditorGameApp
    {
        private readonly UIElement imageHost;
        private Color viewportColor = Color.Black;
        private EditorMode editorMode;
        private EditorTransformMode editorTransformMode;
        
        public UITree UI { get; private set; }
        public SceneGraph Scene { get; private set; }
        public WorldSimulator Simulator { get; private set; }

        public EditorGameApp(UIElement imageHost)
        {
            this.imageHost = imageHost;
            ViewportSizeChanged += ContentBuilderGameApp_ViewportSizeChanged;
        }

        #region Initialization

        protected override void OnInitializing()
        {
            //Register game app services.                            
            Services.RegisterService(new InputService(this, new MouseCoordsTransfomerWPF(imageHost))); //Required For Graphics UI
            Services.RegisterService(new MouseNavigatorService(this));
            Services.RegisterService(new SceneModifierService(this, true));
            Services.RegisterService(new SceneAdornmentService());
        }

        protected override void OnInitialized()
        {
            InitializeUI();
            InitializeScene();
            InitializeSimulation();
            InitializeEventHandling();
        }

        private void InitializeScene()
        {
            Scene = new SceneGraph(this);
            Scene.CameraNode = new CameraSceneNode(
                new Camera(this)
                {
                    ViewMatrix = GetDefaultViewMatrix(),
                    ProjectionMatrix = CalculateProjection(),
                });

            AddReferenceGrid();
            PointCameraToReferenceGrid();
        }

        private void InitializeUI()
        {
            const int LabelWidth = 200;

            UI = new UITree(this);
            UI.Theme = new SampleGameAppTheme(this);
            
            var navigatorControl = new NavigatorControl(UI);
            UI.Controls.Add(navigatorControl);            

            var rotationOverlayControl = new ContentControl(
                UI,
                new ImageContent(ContentImporter.LoadTexture(UI.Game.GraphicsDeviceContext, @"Assets\UI\cross_hair.png"))
                {
                    ScaleImage = true,
                    PredefinedBlend = PredefinedBlend.AlphaBlend,
                })
            {
                Size = new Size2F(MouseNavigatorService.RotationEllipseDiameter, MouseNavigatorService.RotationEllipseDiameter),
                HorizontalAlignment = CipherPark.Aurora.Core.UI.Controls.HorizontalAlignment.Center,
                VerticalAlignment = CipherPark.Aurora.Core.UI.Controls.VerticalAlignment.Center,
                Behavior = new RotationOverlayControlBehavior(),
            };

            Panel navigationOverlayPanel = new Panel(UI)
            {
                IsFullSize = true,
                ZOrder = 1,
            };

            navigationOverlayPanel.Children.Add(rotationOverlayControl);
            UI.Controls.Add(navigationOverlayPanel);

            ContentControl editorModelLabel = new ContentControl(UI, new TextContent()
            {
                PredefinedBlend = PredefinedBlend.Opacity,
                BlendFactor = new Color4(.5f, .5f, .5f, .5f),
            })
            {
                Size = new Size2F(LabelWidth, 18),
                Behavior = new EditorModeLabelBehavior(),
            };
            editorModelLabel.Content.As<TextContent>().Color = Color.DarkGray;
            UI.Controls.Add(editorModelLabel);

            ContentControl coordinatesLabel = new ContentControl(UI, new TextContent()
            {
                PredefinedBlend = PredefinedBlend.Opacity,
                BlendFactor = new Color4(.5f, .5f, .5f, .5f),
            });
            coordinatesLabel.Size = new Size2F(LabelWidth, 18);
            coordinatesLabel.Position = new Vector2(0, 20);
            coordinatesLabel.Behavior = new CoordinatesLabelBehavior();
            coordinatesLabel.Content.As<TextContent>().Color = Color.DarkGray;
            UI.Controls.Add(coordinatesLabel);
        }

        private void InitializeSimulation()
        {
            Simulator = new WorldSimulator(this);            
        }

        private void InitializeEventHandling()
        {
            Services.GetService<SceneModifierService>().ActivatedOnDoubleTap += SceneModifierService_ActivatedOnDoubleTap;
        }

        private void SceneModifierService_ActivatedOnDoubleTap(object sender, ActivatedOnDoubleTapArgs args)
        {
            if (args.Activated)
            {
                EditorMode = EditorMode.SelectSceneObject;
            }
            else
            {
                EditorMode = EditorMode.RotateCamera;
            }
        }

        public void ClearScene(bool resetCamera)
        {
            Scene.Nodes.Clear(n => !n.IsEditorNode());
            if (resetCamera)
            {
                ResetCamera();
            }
        }

        #endregion

        protected override void OnUpdate()
        {
            //Update the input service (poll directx input devices).
            Services.GetService<IInputService>().UpdateState();

            //Update Simulator
            Simulator.Update(GameTime);

            //Update scene
            Scene.Update(GameTime);

            //Update Game UI.
            UI.Update(GameTime);
        }

        protected override void OnRender(bool isNewSurface)
        {
            GraphicsDevice.ImmediateContext.ClearRenderTargetView(this.RenderTargetView, viewportColor);
            GraphicsDevice.ImmediateContext.ClearDepthStencilView(this.DepthStencil, DepthStencilClearFlags.Depth, 1, 0);
            bool success = Render();
            if (success)
            {
                GraphicsDevice.ImmediateContext.Flush();
            }
            else
            {
                App.Current.MainWindow.Close();
            }
        }

        private bool Render()
        {
            try
            {
                Scene.Draw();
                UI.Draw();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "A rendering error occured", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private Matrix CalculateProjection()
        {
            var viewport = GraphicsDevice.ImmediateContext.Rasterizer.GetViewports<ViewportF>()[0];
            return Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(60), viewport.Width / viewport.Height, 0.1f, 10000.0f);
        }

        private Matrix GetDefaultViewMatrix()
        {
            return Matrix.LookAtLH(new Vector3(0, 500, 0), Vector3.Zero, Vector3.UnitZ);
        }

        private void ContentBuilderGameApp_ViewportSizeChanged()
        {
            if (Scene?.CameraNode?.Camera != null)
                Scene.CameraNode.Camera.ProjectionMatrix = CalculateProjection();
        }

        private void AddReferenceGrid()
        {
            var referenceObjectRoot = new GameObjectSceneNode(this)
            {
                Name = "Reference Object Root",
                GameObject = new GameObject(this, new[]
                {
                    new EditorObjectContext
                    {
                        IsReferenceObjectRoot = true,
                    }
                }),
            };

            var referenceGridNode = CreateReferenceGridNode();
            
            referenceObjectRoot.Children.Add(referenceGridNode);
            
            Scene.Nodes.Add(referenceObjectRoot);
        }

        private GameObjectSceneNode CreateReferenceGridNode()
        {
            var referenceGrid = new ReferenceGrid(5000, 10, 10, 5000, 10, 10);

            var referenceGridNode = new GameObjectSceneNode(this)
            {
                Name = "Reference Grid Node",
                GameObject = new GameObject(this)
                {
                    Renderer = new ReferenceGridRenderer(this, referenceGrid),
                }
            };

            referenceGridNode.GameObject.AddContext(new EditorObjectContext
            {
                SupportsCameraTraversing = true,
                IsReferenceGrid = true,
            });

            referenceGridNode.GameObject.AddContext(referenceGrid);

           return referenceGridNode;
        }

        private void PointCameraToReferenceGrid()
        {
            var referenceGridNode = Scene.Select(n => n.Name == "Reference Grid Node").First();
            var referenceGridPosition = referenceGridNode.WorldPosition();
            Scene.CameraNode.Camera.ViewMatrix = Matrix.LookAtLH(referenceGridPosition + new Vector3(0, 100, 0), referenceGridPosition, Vector3.UnitZ);           
        }              

        public void ChangeViewportColor(Color newViewportColor)
        {
            viewportColor = newViewportColor;
        }

        public EditorMode EditorMode
        {
            get => editorMode;
            set
            {
                var mouseNavigatorService = Services.GetService<MouseNavigatorService>();
                var sceneModifierService = Services.GetService<SceneModifierService>();
                var activateScenePicker = false;
                var navigationMode = MouseNavigatorService.NavigationMode.InActive;
                switch (value)
                {
                    case EditorMode.RotateCamera:
                        navigationMode = MouseNavigatorService.NavigationMode.PlatformRotate;
                        break;
                    case EditorMode.TraverseCamera:
                        navigationMode = MouseNavigatorService.NavigationMode.PlaformTraverse;                        
                        break;
                    case EditorMode.SelectSceneObject:
                        activateScenePicker = true;
                        break;
                }
                mouseNavigatorService.Mode = navigationMode;
                sceneModifierService.IsActive = activateScenePicker;
                editorMode = value;
            }
        }

        public EditorTransformMode EditorTransformMode 
        { 
            get => editorTransformMode;
            set
            {
                var sceneModifierService = Services.GetService<SceneModifierService>();
                switch (value)
                {
                    case EditorTransformMode.None:
                        sceneModifierService.SelectedObjectTransformSpace = TransformSpace.None;
                        break;
                    case EditorTransformMode.ViewSpaceTranslateXZ:
                        sceneModifierService.SelectedObjectTransformSpace = TransformSpace.ViewSpaceTranslateXZ;
                        break;
                    case EditorTransformMode.ViewSpaceTranslateY:
                        sceneModifierService.SelectedObjectTransformSpace = TransformSpace.ViewSpaceTranslateY;
                        break;
                }
                editorTransformMode = value;
            }
        }

        internal void CreateNewActionRig(System.Windows.Point point)
        {
            throw new NotImplementedException();
        }

        public void ResetCamera()
        {
            PointCameraToReferenceGrid();
            Services.GetService<MouseNavigatorService>().ResetTracking();
        }

        public bool IsViewportFeedbackSuspended { get; set; }

        public override bool IsViewportWindowActive => base.IsViewportWindowActive && !IsViewportFeedbackSuspended;       

        public Vector3 GetDropLocation(SharpDX.Point mouseLocation)
        {            
            var cameraNode = this.GetActiveScene().CameraNode;
            var camera = cameraNode.Camera;
            var pickInfo = ScenePicker.PickNodes(
                this,
                mouseLocation.X,
                mouseLocation.Y,
                n => n.GameObject.IsReferenceGridObject())
                .GetClosest(camera.Location);
            return pickInfo != null ?
                pickInfo.IntersectionPoint :
                Vector3.Zero;
        }
    }    
}