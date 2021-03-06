using CipherPark.Aurora.Core;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.UI.Controls;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Scene;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Windows;
using Aurora.Sample.Editor.Services;
using Aurora.Sample.Editor.Scene;
using Aurora.Sample.Editor.Scene.UI.Behavior;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Aurora.Sample.Editor
{
    public class EditorGameApp : GameAppWPF, IContainActiveScene
    {
        private readonly UIElement imageHost;
        private Color viewportColor = Color.Black;
        private EditorMode editorMode = EditorMode.RotateCamera;

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
            
            AddReferenceGrid();
        }

        private void InitializeUI()
        {
            UI = new UITree(this);
            UI.Theme = new SampleGameAppTheme(this);

            var navigatorControl = new NavigatorControl(UI)
            {
                Behavior = new NavigatorControlBehavior(),
            };
            navigatorControl.NodeTransformed += NavigatorControl_NodeTransformed;
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
                    BlendFactor = new Color4(.5f, .5f, .5f , .5f),
                })
            {                
                Size = new Size2F(100, 18),
                Behavior = new EditorModeLabelBehavior(),
            };
            editorModelLabel.Content.As<TextContent>().Color = Color.DarkGray;
            UI.Controls.Add(editorModelLabel);
        }      

        public void ClearScene(bool resetCamera)
        {
            Scene.Nodes.Clear();
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

        private void AddReferenceGrid()
        {
            var referenceGridNode = new GameObjectSceneNode(this)
            {
                Name = "Reference Grid Node",
                GameObject = new GameObject(this)
                {
                    Renderer = new ReferenceGridRenderer(this, new Scene.ReferenceGrid(
                        5000,
                        10,
                        10,
                        5000,
                        10,
                        10)),
                }
            };
            referenceGridNode.GameObject.AddContext(new EditorObjectContext
            {
                IsTraversingPlane = true,
            });
            /*
            referenceGridNode.Transform = new CipherPark.Aurora.Core.Animation.Transform(Matrix.Translation(100, 100, 100));
            Scene.CameraNode.Camera.ViewMatrix = Matrix.LookAtLH(new Vector3(100, 101, 100), new Vector3(100, 100, 100), Vector3.UnitZ);
            */            
            Scene.CameraNode.Camera.ViewMatrix = Matrix.LookAtLH(new Vector3(0, 101, 0), new Vector3(0, 0, 0), Vector3.UnitZ);
            Scene.Nodes.Add(referenceGridNode);            
        }

        private void NavigatorControl_NodeTransformed(object sender, NodeTransformedArgs args)
        {
            NodeTransformed?.Invoke(this, args);
        }

        public event Action<object, NodeTransformedArgs> NodeTransformed;

        public void ChangeViewportColor(Color newViewportColor)
        {
            viewportColor = newViewportColor;
        }

        public EditorMode EditorMode { get; set; }

        public EditorTransformPlane TransformPlane { get; set; }

        public void ResetCamera()
        {
            Scene.CameraNode.Camera.ViewMatrix = GetDefaultViewMatrix();
        }
    }

    public class ProjectViewModel
    {
        public SceneViewModel Scene { get; } = new SceneViewModel();
        public string Filename { get; set; }
        public bool IsDirty { get; set; }

        public void AddNode(GameObjectSceneNode gameObjectNode, string gameObjectResourceFilename)
        {
            var sceneNodeViewModel = new SceneNodeViewModel();
            sceneNodeViewModel.Name = gameObjectNode.Name;
            sceneNodeViewModel.Matrix = gameObjectNode.Transform.ToMatrix().ToArray();
            sceneNodeViewModel.Flags = gameObjectNode.Flags;
            sceneNodeViewModel.Visible = gameObjectNode.Visible;           
            if(gameObjectNode.GameObject.Renderer is ModelRenderer)
            {
                sceneNodeViewModel.GameObjectType = GameObjectType.GeometricModel;
                sceneNodeViewModel.GameObjectDescription = new GameObjectDescription()
                {
                    Filename = gameObjectResourceFilename,
                    EffectType = gameObjectNode.GameObject.Renderer.As<ModelRenderer>()
                                               .Model
                                               .Effect
                                               .GetType()
                                               .FullName,
                    ModelType = gameObjectNode.GameObject.Renderer.As<ModelRenderer>()
                                               .Model
                                               .GetType()
                                               .FullName,
                };            
            }
            sceneNodeViewModel.DataModel = gameObjectNode;

            if(gameObjectNode.Parent != null)
            {
                var parentSceneNodeViewModel = Scene.FindSceneNodeViewModel(n => n.DataModel == gameObjectNode.Parent);
                if( parentSceneNodeViewModel == null)
                {
                    throw new InvalidOperationException("Expected project scene node not found.");
                }
                parentSceneNodeViewModel.Children.Add(sceneNodeViewModel);
            }
            else
            {
                Scene.Children.Add(sceneNodeViewModel);
            }
        }       
    }

    public class SceneViewModel
    {
        public List<SceneNodeViewModel> Children { get; } = new List<SceneNodeViewModel>();

        public SceneNodeViewModel FindSceneNodeViewModel(Func<SceneNodeViewModel, bool> condition)
        {
            return FindSceneNodeViewModel(Children, condition);      
        }

        private static SceneNodeViewModel FindSceneNodeViewModel(List<SceneNodeViewModel> children, Func<SceneNodeViewModel, bool> condition)
        {
            foreach(var child in children)
            {
                if(condition(child))
                {
                    return child;
                }
                
                var descendant = FindSceneNodeViewModel(child.Children, condition);
                
                if( descendant != null)
                {
                    return descendant;
                }    
            }

            return null;
        }
    }

    public class SceneNodeViewModel
    {
        public string Name { get; set; }
        public float[] Matrix { get; set; }
        public ulong Flags { get; set; }
        public bool Visible { get; set; }
        public GameObjectType GameObjectType { get; set; }
        public GameObjectDescription GameObjectDescription { get; set; }
        [JsonIgnore]
        public SceneNode DataModel { get; set; }
        public List<SceneNodeViewModel> Children { get; } = new List<SceneNodeViewModel>();
    }

    public class GameObjectDescription
    {
        public string Filename { get; set; }
        public string EffectType { get; set; }
        public string ModelType { get; set; }
    }

    public enum GameObjectType
    {
        GeometricModel
    }
}