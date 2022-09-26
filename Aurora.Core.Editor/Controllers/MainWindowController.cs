using Aurora.Core.Editor.Dom;
using Aurora.Core.Editor.Util;
using Aurora.Sample.Editor.Scene;
using Aurora.Sample.Editor.Windows;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.World.Scene;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Aurora.Core.Editor
{
    public class MainWindowController
    {
        private IEditorGameApp game;
        private SceneModifierTwoWayDataBinding sceneModifierTwoWayBinding;

        public MainWindowController(IEditorGameApp game)
        {
            this.game = game;            
            ViewModel = new MainWindowViewModel();
            sceneModifierTwoWayBinding = new SceneModifierTwoWayDataBinding(ViewModel, game.Services.GetService<SceneModifierService>());
        }

        public MainWindowViewModel ViewModel { get; }

        public void UIOpenProject()
        {
            if (ViewModel.IsProjectDirty)
            {
                UISaveProjectBeforeClosing();
            }

            string filePath = PresentOpenProjectDialog();
            if (filePath != null)
            {
                OpenProject(filePath);
            }
        }

        public void UISaveProjectBeforeClosing()
        {
            var result = MessageBox.Show("Save current project before closing?", "Save Current Project", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                UISaveProject();
            }
        }

        public void UISaveProject()
        {
            if (ViewModel.IsProjectDirty)
            {
                if (ViewModel.ProjectFileName != null)
                {
                    SaveProject(ViewModel.ProjectFileName);
                }
                else
                {
                    UISaveAsProject();
                }
            }
        }       

        public void UISaveAsProject()
        {
            var filePath = PresentSaveProjectDialog(System.IO.Path.GetFileName(ViewModel.ProjectFileName));
            if (filePath != null)
            {
                SaveProject(filePath);
            }
        }

        public void UIImportModel()
        {
            string filePath = PresentImportModelDialog();
            if (filePath != null)
            {
                ImportModel(filePath);
            }
        }

        public void UIChangeSettings()
        {
            var dialog = new SettingsDialog();
            dialog.ShowDialog();
            game.ChangeViewportColor(dialog.SelectedViewportColor);
        }

        public void UIAddLight()
        {
            /*
            var dialog = new AddLightDialog();
            dialog.ShowDialog();
            game.AddLight(dialog.LightInfo);
            */
            AddLight();
        }

        public string PresentOpenProjectDialog()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".aurora";
            dialog.Filter = "Aurora Project (.aurora)|*.aurora";
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string PresentImportModelDialog()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".x";
            dialog.Filter = "Direct X (.x)|*.x";
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string PresentSaveProjectDialog(string suggestedFileName = null)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.DefaultExt = ".aurora";
            dialog.Filter = "Aurora Project (.aurora)|*.aurora";
            dialog.FileName = suggestedFileName;
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public void NewProject()
        {
            ViewModel.Project = new ProjectViewModel(game);            
        }

        public void OpenProject(string filePath)
        {
            ViewModel.Project = DomSerializer.LoadProject(game, filePath);
            ViewModel.ProjectFileName = filePath;
        }

        public void SaveProject(string filePath)
        {
            DomSerializer.SaveProject(ViewModel.Project, filePath);
            if (ViewModel.ProjectFileName != filePath)
            {
                ViewModel.ProjectFileName = filePath;
            }
            ViewModel.IsProjectDirty = false;
        }

        public void ImportModel(string filePath)
        {
            var app = game;
            var effect = new BlinnPhongEffect2(app, SurfaceVertexType.PositionNormalColor);
            effect.AmbientColor = Color.White;            

            var gameSceneNode = new GameObjectSceneNode(app)
            {
                GameObject = ContentHelper.ImportGameObject(filePath, effect),
            };

            //TODO: Consider updating scene graph inside the view model's AddSceneNode method (instead of doing it below).

            app.Scene.Nodes.Add(gameSceneNode);

            ViewModel.Project.Scene.AddSceneNode(gameSceneNode);

            ViewModel.IsProjectDirty = true;
        }

        public void NotifyKeyboardShiftKeyEvent(bool keyDown)
        {
            var navigationService = game.Services.GetService<MouseNavigatorService>();
            navigationService.IsModeInversionOn = keyDown;            
        }

        public void EnterEditorMode(EditorMode mode)
        {
            game.EditorMode = mode;
        }

        public void ResetEditorCamera()
        {
            game.ResetCamera();
        }

        public void SetEditorTransformMode(EditorTransformMode transformMode)
        {
            game.EditorTransformMode = transformMode;            
        }

        public void AddLight()
        {
            var light = new DirectionalLight();
            light.Diffuse = Color.LightYellow;
            light.Direction = Vector3.Down;
            
            var ordinal = game.Scene.SelectLights().Count() + 1;
            var gameSceneNode = new GameObjectSceneNode(game)
            {
                Name = $"Light_{ordinal}",
                GameObject = new CipherPark.Aurora.Core.World.GameObject(game, new[] { light }),
            };

            game.Scene.Nodes.Add(gameSceneNode);

            ViewModel.Project.Scene.AddSceneNode(gameSceneNode);

            ViewModel.IsProjectDirty = true;
        }       

        public void MarkSelectedNodeLocation()
        {
            var selectedNode = ViewModel.Project.Scene
                                        .SelectedNode?
                                        .DataModel
                                        .As<GameObjectSceneNode>();
            if (selectedNode != null)
            {
                var sceneAdornmentService = game.Services.GetService<SceneAdornmentService>();
                sceneAdornmentService.MarkLocation(selectedNode);
            }
        }

        public void ClearAllLocationMarkers()
        {
            var sceneAdornmentService = game.Services.GetService<SceneAdornmentService>();
            sceneAdornmentService.RemoveAllMarkers();
        }

        public void NotifyVisualizationOfDoubleClick(System.Windows.Point mousePoint)
        {
            var sceneModifierService = game.Services.GetService<SceneModifierService>();
            sceneModifierService.NotifyMouseDoubleTap(new SharpDX.Point((int)mousePoint.X, (int)mousePoint.Y));
        }
    }

    public class SceneAdornmentService
    {        
        private readonly List<GameObjectSceneNode> markers;

        public SceneAdornmentService()
        {
            markers = new List<GameObjectSceneNode>();
        }

        public void MarkLocation(GameObjectSceneNode targetNode)
        {           
            var referenceObjectRoot = targetNode.Scene
                                                .SelectReferenceObjectRoot();

            if (referenceObjectRoot != null)
            {
                foreach (var referenceObjectNode in referenceObjectRoot.GameObjectChildren())
                {
                    if (referenceObjectNode.GetGameObject().IsReferenceGridObject() == true)
                    {
                        var referenceGrid = referenceObjectNode.GetGameObject().GetReferenceGrid();
                        
                        var markerNode = new GameObjectSceneNode(targetNode.Game)
                        {
                            GameObject = new CipherPark.Aurora.Core.World.GameObject(targetNode.Game, new object[]
                            {
                                new EditorObjectContext
                                {
                                    IsShadowAdronment = true,
                                    TargetNode = targetNode,
                                }
                            })
                            {
                                Renderer = new ModelRenderer(CreateMarkerModel(targetNode, referenceObjectNode, referenceGrid))
                            },
                        };

                        referenceObjectNode.Children.Add(markerNode);
                        markers.Add(markerNode);
                    }
                }
            }            
        }

        public void RemoveAllMarkers()
        {
            foreach (var node in markers)
            {
                node.Orphan();
            }

            markers.Clear();
        }

        private static Model CreateMarkerModel(GameObjectSceneNode targetNode, GameObjectSceneNode referenceObjectNode, ReferenceGrid referenceGrid)
        {
            const float Padding = 0.5f;
            var renderBox_ws = targetNode.GetWorldBoundingBox();
            renderBox_ws.Inflate(Padding, Padding, Padding);
            var meshPoints_ws = renderBox_ws.GetCorners();
            var plane_ws = new Plane(-referenceObjectNode.WorldPosition(),
                                      referenceObjectNode.LocalToWorldNormal(referenceGrid.Normal));
            var projectedMeshPoints_ws = ProjectPoints(plane_ws, meshPoints_ws);
            var projectedMeshPoints_rgs = projectedMeshPoints_ws.Select(x => referenceObjectNode.WorldToLocalCoordinate(x))
                                                                .ToArray();
            var meshVerts = projectedMeshPoints_rgs.Select(p => new VertexPositionColor
            {
                Position = new Vector4(p, 1),
                Color = Color.LightCyan.ToVector4(),
            }).ToArray();

            var meshIndices = new short[]
            {
               7, 4, 5, //Front 1
               5, 6, 7, //Front 2
               3, 2, 1, //Back 1
               1, 0, 3, //Back 2
               4, 0, 1, //Top 1
               1, 5, 4, //Top 2
               7, 6, 2, //Bottom 1
               2, 3, 7, //Bottom 2
               6, 5, 1, //Right 1
               1, 2, 6, //Right 2
               7, 3, 0, //Left 1
               0, 4, 7, //Left 2
            };

            var gameApp = referenceObjectNode.Game;

            var meshEffect = new FlatEffect(gameApp, SurfaceVertexType.PositionColor);

            var mesh = ContentBuilder.BuildMesh(gameApp.GraphicsDevice,
                                            meshEffect.GetVertexShaderByteCode(),
                                            meshVerts,
                                            meshIndices,                                            
                                            VertexPositionColor.InputElements,
                                            VertexPositionColor.ElementSize,
                                            new BoundingBox(),
                                            SharpDX.Direct3D.PrimitiveTopology.TriangleList);

            return new StaticMeshModel(gameApp)
            {
                Effect = meshEffect,
                Mesh = mesh,
            };
        }

        private static Vector3[] ProjectPoints(Plane plane, Vector3[] points)
           => points.Select(x => plane.ProjectPoint(x).Value).ToArray();
    }
}
