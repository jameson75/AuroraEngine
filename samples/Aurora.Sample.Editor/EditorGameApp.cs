using CipherPark.Aurora.Core;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.UI.Controls;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Utils;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Windows;
using Aurora.Sample.Editor.Services;

namespace Aurora.Sample.Editor
{
    public class EditorGameApp : GameAppWPF, IContainActiveScene
    {
        private readonly UIElement imageHost;

        public UITree UI { get; private set; }
        public SceneGraph Scene { get; private set; }

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
        }

        protected override void OnInitialized()
        {
            InitializeUI();
            InitializeScene();
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

            //AddTestCube();
            AddTestQuad();
        }

        private void InitializeUI()
        {
            UI = new UITree(this);
            UI.Theme = new SampleGameAppTheme(this);

            UI.Controls.Add(new NavigatorControl(UI)
            {
                NavigationMode = MouseNavigatorService.NavigationMode.Rotate,
            });

            var crossHairControl = new ContentControl(
                UI,
                new ImageContent(ContentImporter.LoadTexture(UI.Game.GraphicsDeviceContext, @"Assets\UI\cross_hair.png"))
                {
                    ScaleImage = true,
                    PredefinedBlend = PredefinedBlend.AlphaBlend,
                });
            crossHairControl.Size = new Size2F(MouseNavigatorService.RotationEllipseDiameter, MouseNavigatorService.RotationEllipseDiameter);
            crossHairControl.HorizontalAlignment = CipherPark.Aurora.Core.UI.Controls.HorizontalAlignment.Center;
            crossHairControl.VerticalAlignment = CipherPark.Aurora.Core.UI.Controls.VerticalAlignment.Center;

            Panel crossHairPanelControl = new Panel(UI)
            {
                IsFullSize = true,
                ZOrder = 1,
            };

            crossHairPanelControl.Children.Add(crossHairControl);
            UI.Controls.Add(crossHairPanelControl);
        }

        #endregion

        protected override void OnUpdate()
        {
            //Update the input service (poll directx input devices).
            Services.GetService<IInputService>().UpdateState();

            //Update Game UI.
            UI.Update(GameTime);
        }

        protected override void OnRender(bool isNewSurface)
        {
            GraphicsDevice.ImmediateContext.ClearRenderTargetView(this.RenderTargetView, Color.Black);
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
            catch(Exception ex)
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

        private void AddTestCube()
        {
            var effect = new FlatEffect(this, SurfaceVertexType.PositionColor);
            
            var cube = CipherPark.Aurora.Core.Content.ContentBuilder.BuildColoredBox(
                this.GraphicsDevice,
                effect.GetVertexShaderByteCode(),
                new BoundingBox(new Vector3(-200, -200, -200), new Vector3(200, 200, 200)),               
                Color.Aquamarine);

            Scene.Nodes.Add(new GameObjectSceneNode(this)
            {
                Name = "Test Cube Node",
                GameObject = new GameObject(this)
                {
                    Renderer = new ModelRenderer(new StaticMeshModel(this)
                    {
                        Name = "Test Cube",
                        Mesh = cube,
                        Effect = effect,
                    })
                }
            });
        }

        private void AddTestQuad()
        {
            var effect = new FlatEffect(this, SurfaceVertexType.PositionColor);

            var quad = CipherPark.Aurora.Core.Content.ContentBuilder.BuildColoredQuad(
                this.GraphicsDevice,
                effect.GetVertexShaderByteCode(),
                RectangleDimensions.FromCenter(200, 200),
                Color.Aquamarine);

            Scene.Nodes.Add(new GameObjectSceneNode(this)
            {
                Name = "Test Quad Node",
                GameObject = new GameObject(this)
                {
                    Renderer = new ModelRenderer(new StaticMeshModel(this)
                    {
                        Name = "Test Quad",
                        Mesh = quad,
                        Effect = effect,
                    })
                }
            });
        }
    }
}