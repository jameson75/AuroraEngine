using Aurora.Core.Editor.Dom;
using Aurora.Core.Editor.Util;
using Aurora.Sample.Editor.Scene;
using Aurora.Sample.Editor.Windows;
using CipherPark.Aurora.Core;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.World.Scene;
using SharpDX;
using System;
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
            //var effect = new BlinnPhongEffect2(app, SurfaceVertexType.SkinNormalTexture);
            effect.AmbientColor = Color.White;

            var gameSceneNode = new GameObjectSceneNode(app)
            {
                GameObject = ContentHelper.ImportGameObject(filePath, effect),
                Transform = new Transform(10),
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

        public void CreateNewActionRig(System.Windows.Point mouseLocation)
        {
            var dropLocation = game.GetDropLocation(new SharpDX.Point((int)mouseLocation.X, (int)mouseLocation.Y));
            var effect = new BlinnPhongEffect2(game, SurfaceVertexType.PositionNormalColor);
            effect.AmbientColor = Color.Red;

            var boxMesh = ContentBuilder.BuildLitWireBox(
                game.GraphicsDevice,
                effect.GetVertexShaderByteCode(),
                new BoundingBox(new Vector3(-5, -5, -5), new Vector3(5, 5, 5)),
                Color.Red);

            var cameraMesh = ContentBuilder.BuildLitWireCamera(
                game.GraphicsDevice,
                effect.GetVertexShaderByteCode(),
                new BoundingBox(new Vector3(-5, -5, -10), new Vector3(5, 5, 10)),
                Color.Red);

            var actionGameNode = new GameObjectSceneNode(game)
            {
                Name = "Action Node",
                GameObject = new CipherPark.Aurora.Core.World.GameObject(
                    game,
                    new EditorObjectContext
                    {
                        IsActionObject = true,
                    })
                {
                    Renderer = new ModelRenderer(game)
                    {
                        Model = new StaticMeshModel(game)
                        {
                            Effect = effect,
                            Mesh = boxMesh,
                        }
                    }
                }
            };

            var cameraGameNode = new GameObjectSceneNode(game)
            {
                Name = "Game Camera",
                GameObject = new CipherPark.Aurora.Core.World.GameObject(game, new EditorObjectContext { IsPickable = true, IsSatelliteObject = true })
                {
                    Renderer = new ModelRenderer(game)
                    {
                        Model = new StaticMeshModel(game)
                        {
                            Effect = effect,
                            Mesh = cameraMesh,
                        }
                    }
                }
            };

            actionGameNode.Reposition(dropLocation.Add(0, 5, 0));
            game.Scene.Nodes.Add(actionGameNode);
            ViewModel.Project.Scene.AddSceneNode(actionGameNode);

            actionGameNode.Children.Add(cameraGameNode);
            cameraGameNode.Reposition(new Vector3(0, 300, 0));
            cameraGameNode.PointZAtTarget(Vector3.BackwardLH, actionGameNode.WorldPosition());
            ViewModel.Project.Scene.AddSceneNode(cameraGameNode);

            ViewModel.IsProjectDirty = true;
        }

        public void CreateNewNavigationPath(System.Windows.Point mouseLocation)
        {
            var dropLocation = game.GetDropLocation(new SharpDX.Point((int)mouseLocation.X, (int)mouseLocation.Y));
            var navigationPath = new NavigationPath();

            var pathRootNode = new GameObjectSceneNode(game)
            {
                Name = "Path Root Node",
                GameObject = new CipherPark.Aurora.Core.World.GameObject(
                    game,
                    new object[]
                    {
                        new EditorObjectContext()
                        {
                            IsPathRootObject = true,
                        },
                        navigationPath,
                    })
                {
                    Renderer = new NavigationPathRenderer(game)
                    {
                        NavigationPath = navigationPath,
                    }
                }
            };

            game.Scene.Nodes.Add(pathRootNode);

            var pathNodeDropLocations = new[]
            {
                dropLocation,
                dropLocation.AddZ(300),
                dropLocation.AddZ(600)
            };

            for (int i = 0; i < pathNodeDropLocations.Length; i++)
            {
                var pathNode = CreateNavigationPathNode();
                pathNode.Reposition(pathNodeDropLocations[i].Add(0, 4, 0));
                pathRootNode.Children.Add(pathNode);
                navigationPath.Nodes.Add(pathNode);
            }

            ViewModel.IsProjectDirty = true;
        }

        public void JustExecute()
        {
            var actionNode = game.Scene.Select(x => x.IsActionNode()).FirstOrDefault();
            var pathRootNode = game.Scene.Select(x => x.IsPathRootNode()).FirstOrDefault();
            var navigationPathNodes = pathRootNode.GetGameObject().GetNavigationPath().Nodes;
            var controller = new ActionPathController(actionNode, navigationPathNodes);
            game.Simulator.Controllers.Add(controller);
        }

        public void ExtrudeNavigationPath()
        {
            var selectedNode = GetSelectedNode();

            if (selectedNode != null && selectedNode.IsPathNode())
            {
                var newNode = CreateNavigationPathNode();

                const float OffsetLength = 300f;

                var pathRootNode = selectedNode.Parent;
                var navigationPath = pathRootNode.GetGameObject().GetNavigationPath();
                var selectedPathNodeIndex = navigationPath.Nodes.IndexOf(selectedNode);
                var selectedSceneNodeIndex = pathRootNode.Children.IndexOf(selectedNode);

                if (selectedPathNodeIndex == -1)
                {
                    throw new InvalidOperation("Node not found in path.");
                }

                if (selectedSceneNodeIndex == -1)
                {
                    throw new InvalidOperation("Node not found in scene.");
                }

                newNode.Transform = selectedNode.Transform;

                if (navigationPath.Nodes.Count == 1)
                {
                    var translation = Vector3.UnitZ * OffsetLength;
                    newNode.Translate(translation);
                    navigationPath.Nodes.Add(newNode);
                    pathRootNode.Children.Add(newNode);
                }

                else
                {
                    if (pathRootNode.Children.FirstOrDefault() == selectedNode)
                    {
                        var nodeA = navigationPath.Nodes[1];
                        var nodeB = navigationPath.Nodes[0];
                        var translationDir = Vector3.Normalize(nodeB.Position() - nodeA.Position());
                        var translation = translationDir * OffsetLength;
                        newNode.Translate(translation);
                        navigationPath.Nodes.Insert(0, newNode);
                        pathRootNode.Children.Insert(0, newNode);
                    }

                    else if (pathRootNode.Children.LastOrDefault() == selectedNode)
                    {
                        var nodeA = navigationPath.Nodes[navigationPath.Nodes.Count - 2];
                        var nodeB = navigationPath.Nodes[navigationPath.Nodes.Count - 1];
                        var translationDir = Vector3.Normalize(nodeB.Position() - nodeA.Position());
                        var translation = translationDir * OffsetLength;
                        newNode.Translate(translation);
                        navigationPath.Nodes.Add(newNode);
                        pathRootNode.Children.Add(newNode);
                    }
                }
            }
        }

        public void BisectNavigationPath()
        {
            var selectedNode = GetSelectedNode();

            if (selectedNode != null && selectedNode.IsPathNode())
            {
                var newNode = CreateNavigationPathNode();

                var pathRootNode = selectedNode.Parent;
                var navigationPath = pathRootNode.GetGameObject().GetNavigationPath();
                var selectedPathNodeIndex = navigationPath.Nodes.IndexOf(selectedNode);
                var selectedSceneNodeIndex = pathRootNode.Children.IndexOf(selectedNode);

                if (selectedPathNodeIndex == -1)
                {
                    throw new InvalidOperation("Node not found in path.");
                }

                if (selectedSceneNodeIndex == -1)
                {
                    throw new InvalidOperation("Node not found in scene.");
                }

                newNode.Transform = selectedNode.Transform;

                if (selectedPathNodeIndex < navigationPath.Nodes.Count - 1)
                {
                    var nodeA = navigationPath.Nodes[selectedPathNodeIndex];
                    var nodeB = navigationPath.Nodes[selectedPathNodeIndex + 1];
                    var translation = (nodeB.Position() - nodeA.Position()) * 0.5f;
                    newNode.Translate(translation);
                    navigationPath.Nodes.Insert(selectedPathNodeIndex + 1, newNode);
                    pathRootNode.Children.Insert(selectedSceneNodeIndex + 1, newNode);
                }
            }
        }

        public void NewModel()
        {
            var model = CharacterBuilder.BuildMainCharacter(game);

            var node = new GameObjectSceneNode(game)
            {
                Name = "Test Model",
                GameObject = new CipherPark.Aurora.Core.World.GameObject(
                    game,
                    new[]
                    {
                        new EditorObjectContext
                        {
                            IsPickable = true,
                        }
                    })
                {
                    Renderer = new ModelRenderer(game)
                    {
                        Model = model,
                    }
                }
            };

            game.Scene.Nodes.Add(node);
            ViewModel.Project.Scene.AddSceneNode(node);
        }

        private GameObjectSceneNode CreateNavigationPathNode()
        {
            var effect = new BlinnPhongEffect2(game, SurfaceVertexType.PositionNormalColor);
            effect.AmbientColor = Color.CornflowerBlue;

            var boxMesh = ContentBuilder.BuildLitWireBox(
                    game.GraphicsDevice,
                    effect.GetVertexShaderByteCode(),
                    new BoundingBox(new Vector3(-4, -4, -4), new Vector3(4, 4, 4)),
                    Color.CornflowerBlue);

            var node = new GameObjectSceneNode(game)
            {
                Name = "Path Node",
                GameObject = new CipherPark.Aurora.Core.World.GameObject(
                    game,
                    new[]
                    {
                        new EditorObjectContext
                        {
                            IsPathObject = true,
                            IsPickable = true,
                        }
                    })
                {
                    Renderer = new ModelRenderer(game)
                    {
                        Model = new StaticMeshModel(game)
                        {
                            Effect = effect,
                            Mesh = boxMesh,
                        }
                    }
                }
            };

            return node;
        }

        private GameObjectSceneNode GetSelectedNode()
            => game.Services.GetService<SceneModifierService>().PickedNode;
    }

    public class NavigationPath
    {
        public IList<ITransformable> Nodes { get; } = new List<ITransformable>();
    }

    public class NavigationPathRenderer : IRenderer
    {
        private readonly IEditorGameApp game;
        private List<Transform> navigationPathSnapshot;
        private Mesh pathMesh;
        private SurfaceEffect pathMeshEffect;

        public NavigationPathRenderer(IEditorGameApp game)
        {
            this.game = game;
            navigationPathSnapshot = new List<Transform>();
        }

        public NavigationPath NavigationPath { get; set; }

        public void Dispose()
        {
            if (pathMesh != null)
            {
                pathMesh.Dispose();
            }
            pathMeshEffect.Dispose();
        }

        public void Draw(ITransformable container)
        {
            if (pathMesh != null)
            {
                var cameraNode = game.GetActiveScene()
                                     .CameraNode;
                pathMeshEffect.World = Matrix.Identity;
                pathMeshEffect.View = cameraNode.RiggedViewMatrix;
                pathMeshEffect.Projection = cameraNode.ProjectionMatrix;
                pathMeshEffect.Apply();
                pathMesh.Draw();
                pathMeshEffect.Restore();
            }
        }

        public void Update(GameTime gameTime)
        {
            if (IsMeshUpdateRequired())
            {
                UpdateMesh();
            }
        }

        private bool IsMeshUpdateRequired()
        {
            if (NavigationPath == null)
            {
                return navigationPathSnapshot != null;
            }

            if (navigationPathSnapshot == null)
            {
                return true;
            }

            if (NavigationPath.Nodes.Count != navigationPathSnapshot.Count)
            {
                return true;
            }

            for (int i = 0; i < NavigationPath.Nodes.Count; i++)
            {
                if (NavigationPath.Nodes[i].Transform != navigationPathSnapshot[i])
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateMesh()
        {
            if (NavigationPath == null)
            {
                navigationPathSnapshot = null;
            }

            else
            {
                var verts = NavigationPath.Nodes.Select(x =>
                   new VertexPositionColor
                   {
                       Color = Color.Yellow.ToVector4(),
                       Position = new Vector4(x.Transform.Translation, 1),
                   }).ToArray();

                var indicies = ContentBuilder.CreateLineListIndices(verts.Length);

                pathMeshEffect = new FlatEffect(game, SurfaceVertexType.PositionColor);

                pathMesh = ContentBuilder.BuildMesh(
                    game.GraphicsDevice,
                    pathMeshEffect.GetVertexShaderByteCode(),
                    verts,
                    indicies,
                    VertexPositionColor.InputElements,
                    VertexPositionColor.ElementSize,
                    BoundingBoxExtension.Empty,
                    SharpDX.Direct3D.PrimitiveTopology.LineList);

                navigationPathSnapshot = NavigationPath.Nodes.Select(x => x.Transform).ToList();
            }
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

    public class ActionPathController : SimulatorController
    {
        private const float animationStep = 3.0f;
        private int ni = 1;
        private bool onetimeSetupComplete;

        public ActionPathController(ITransformable navigator, IList<ITransformable> path)
        {
            Navigator = navigator;
            Path = path;
        }

        public ITransformable Navigator { get; }

        public IList<ITransformable> Path { get; }

        public override void Update(GameTime gameTime)
        {
            OnetimeSetupNavigator();

            if (ni >= Path.Count)
            {
                SignalComplete();
                return;
            }

            ITransformable nextNodeAlongPath = Path[ni];
            Vector3 targetPositionWs = nextNodeAlongPath.WorldPosition();
            var distanceToTarget = Vector3.Distance(Navigator.WorldPosition(), targetPositionWs);
            float step = System.Math.Min(animationStep, distanceToTarget);

            if (step > 0.1)
            {
                Vector3 stepDirectionWs = Vector3.Normalize(targetPositionWs - Navigator.WorldPosition());
                Vector3 stepVectorWs = stepDirectionWs * step;
                Navigator.Translate(Navigator.WorldToParentCoordinate(stepVectorWs));
            }
            else
            {
                ni++;
                PointNavigatorToNextNode(true);
            }
        }

        private void PointNavigatorToNextNode(bool animate)
        {
            if (ni < Path.Count)
            {
                //Navigator.PointZAtTarget(Path[ni].WorldPosition());
            }
        }

        private void OnetimeSetupNavigator()
        {
            if (!onetimeSetupComplete)
            {
                if (Path.Count != 0)
                {
                    Navigator.Reposition(Path[0], Vector3.Zero);
                }
 
                 PointNavigatorToNextNode(false);
                 onetimeSetupComplete = true;
            }
        }
    }

    public class CharacterBuilder
    {
        const int VoxelSize = 2;

        public static Model BuildMainCharacter(IGameApp game)
        {
            /*
            const int headWidth = 20;
            const int headHeight = 20;
            const int headDepth = 30;
            var character = CreateBoxFourteen(headWidth, headHeight, headDepth);
            
            var normals = character.Geometry.GetIndexMappedNormals();
            var vertices = character.Geometry.GetIndexMappedPoints().Select((x,i) => new VertexPositionNormalColor(x, normals[i], Color.AliceBlue.ToVector4())).ToArray();            
            var boundingBox = BoundingBox.FromPoints(character.Geometry.Points);

            var effect = new BlinnPhongEffect2(game, SurfaceVertexType.PositionNormalColor);
            effect.Lighting = new CipherPark.Aurora.Core.Effects.Light[2]
            {
                new DirectionalLight() { Diffuse = Color.White, Direction = Vector3.Down },
                new DirectionalLight() { Diffuse = Color.White, Direction = Vector3.Left }
            };

            
            //var effect = new FlatEffect(game, SurfaceVertexType.PositionColor);           

            var mesh = ContentBuilder.BuildMesh(game.GraphicsDevice,
                                        effect.GetVertexShaderByteCode(),
                                        vertices,
                                        null,
                                        VertexPositionNormalColor.InputElements,
                                        VertexPositionNormalColor.ElementSize,
                                        boundingBox,
                                        SharpDX.Direct3D.PrimitiveTopology.TriangleList);
            */

            var voxelBuffer = new VoxelBuffer(20, 20, 20, Enumerable.Repeat(Color.Wheat.ToVector4(), 20 * 20 * 20).ToArray());
            var voxelGeometry = CreateVoxelGeometry(voxelBuffer);
            return CreateVoxelModel(voxelGeometry, game);

            /*
            return new StaticMeshModel(game)
            {
                Mesh = mesh,
                Effect = effect,
            };
            */
        }        
       
        public static Model CreateVoxelModel(VoxelGeometry geometry, IGameApp game)
        {
            var points = geometry.FrontQuads.Concat(
                geometry.BackQuads).Concat(
                geometry.LeftQuads).Concat(
                geometry.RightQuads).Concat(
                geometry.TopQuads).Concat(
                geometry.BottomQuads)
                .Select(x => x.ToTriangles())
                .SelectMany(x => x)
                .ToArray();
            var vertices = points.Select((x, i) => new VertexPositionColor(x, Color.Violet.ToVector4())).ToArray();
            var boundingBox = BoundingBox.FromPoints(points);
            var effect = new FlatEffect(game, SurfaceVertexType.PositionColor);
            var mesh = ContentBuilder.BuildMesh(
                                        effect.Game.GraphicsDevice,
                                        effect.GetVertexShaderByteCode(),
                                        vertices,
                                        null,
                                        VertexPositionColor.InputElements,
                                        VertexPositionColor.ElementSize,
                                        boundingBox,
                                        SharpDX.Direct3D.PrimitiveTopology.TriangleList);
            return new StaticMeshModel(game)
            {
                Mesh = mesh,
                Effect = effect,
            };
        }

        public static VoxelGeometry CreateVoxelGeometry(VoxelBuffer buffer)
        {
            var width_x_height = buffer.Width * buffer.Height;
            var transparent = Color.Transparent.ToVector4();
            var half_voxel_size = VoxelSize / 2.0f;
            var voxelGeometry = new VoxelGeometry();

            for (int k = 0; k < buffer.Depth; k++)
            {
                var depthOffset = k * width_x_height;
                var nextDepthOffset = (k + 1) * width_x_height;
                var prevDepthOffset = (k - 1) * width_x_height;
                for(int j = 0; j < buffer.Height; j++)
                {
                    var heightOffset = j * buffer.Height;
                    var nextHeightOffset = (j + 1) * buffer.Height;
                    var prevHeightOffset = (j - 1) * buffer.Height;
                    for(int i = 0; i < buffer.Width; i++)
                    {
                        var c = i + heightOffset + depthOffset; //current
                        var l = (i != 0) ? c - 1 : (int?)null; //left neighbor
                        var r = (i != buffer.Width - 1) ? c + 1 : (int?)null; //right neighbor
                        var bm = (j != 0) ? i + prevHeightOffset + depthOffset : (int?)null; //bottom neighbor
                        var t = (j != buffer.Height - 1) ? i + nextHeightOffset + depthOffset : (int?)null; //top neighbor
                        var f = (k != 0) ? i + heightOffset + prevDepthOffset : (int?)null; //front neighbor
                        var bk = (k != buffer.Depth - 1) ? i + heightOffset + nextDepthOffset : (int?)null; //back neighbor

                        var cp = new Vector3((-buffer.Width / 2.0f + i) * VoxelSize,
                                             (-buffer.Height / 2.0f + j) * VoxelSize,
                                             (-buffer.Depth / 2.0f + k) * VoxelSize);
                        var p0 = new Vector3(cp.X - half_voxel_size, cp.Y - half_voxel_size, cp.Z - half_voxel_size);
                        var p1 = new Vector3(cp.X - half_voxel_size, cp.Y + half_voxel_size, cp.Z - half_voxel_size);
                        var p2 = new Vector3(cp.X + half_voxel_size, cp.Y + half_voxel_size, cp.Z - half_voxel_size);
                        var p3 = new Vector3(cp.X + half_voxel_size, cp.Y - half_voxel_size, cp.Z - half_voxel_size);
                        var p4 = new Vector3(cp.X - half_voxel_size, cp.Y + half_voxel_size, cp.Z + half_voxel_size);
                        var p5 = new Vector3(cp.X - half_voxel_size, cp.Y - half_voxel_size, cp.Z + half_voxel_size);
                        var p6 = new Vector3(cp.X + half_voxel_size, cp.Y - half_voxel_size, cp.Z + half_voxel_size);
                        var p7 = new Vector3(cp.X + half_voxel_size, cp.Y + half_voxel_size, cp.Z + half_voxel_size);

                        if (l == null || buffer.Data[l.Value] == transparent)
                        {
                            voxelGeometry.LeftQuads.Add(new GeometryQuad(p0, p5, p4, p1));
                        }

                        if (r == null || buffer.Data[r.Value] == transparent)
                        {
                            voxelGeometry.RightQuads.Add(new GeometryQuad(p2, p7, p6, p3));
                        }

                        if (bm == null || buffer.Data[bm.Value] == transparent)
                        {
                            voxelGeometry.BottomQuads.Add(new GeometryQuad(p0, p3, p6, p5));
                        }

                        if (t == null || buffer.Data[t.Value] == transparent)
                        {
                            voxelGeometry.TopQuads.Add(new GeometryQuad(p1, p4, p7, p2));
                        }

                        if (f == null || buffer.Data[f.Value] == transparent)
                        {
                            voxelGeometry.FrontQuads.Add(new GeometryQuad(p0, p1, p2, p3));
                        }

                        if (bk == null || buffer.Data[bk.Value] == transparent)
                        {
                            voxelGeometry.BackQuads.Add(new GeometryQuad(p4, p5, p6, p7));
                        }
                    }
                }
            }
            return voxelGeometry;
        }

        /*
        public static CharacterPart CreateBoxFourteen(float width, float height, float depth)
        {
            var half_width = width / 2;
            var half_height = height / 2;
            var half_depth = depth / 2;            

            //front: 0, 1, 2, 3
            //back: 4, 5, 6, 7            
            //min: 0
            //max: 7
            //mids: 8 (front), 9 (left), 10 (back), 11 (right), 12 (top), 13 (bottom)
            var points = new Vector3[14]
            {
                new Vector3(-half_width, -half_height, -half_depth),
                new Vector3(-half_width, half_height, -half_depth),
                new Vector3(half_width, half_height, -half_depth),
                new Vector3(half_width, -half_height, -half_depth),
                new Vector3(-half_width, half_height, half_depth),
                new Vector3(-half_width, -half_height, half_depth),
                new Vector3(half_width, -half_height, half_depth),
                new Vector3(half_width, half_height, half_depth),
                new Vector3(0, 0, -half_depth),
                new Vector3(-half_width, 0, 0),
                new Vector3(0, 0, half_depth),
                new Vector3(half_width, 0, 0),
                new Vector3(0, half_height, 0),
                new Vector3(0, -half_height, 0)
            };

            var normals = points.Select(Vector3.Normalize).ToArray();

            var indices = new short[72]
            {
                //front
                0, 1, 8,
                1, 2, 8,
                2, 3, 8,
                3, 0, 8,

                //back
                4, 5, 10,
                5, 6, 10,
                6, 7, 10,
                7, 4, 10,

                //top
                1, 4, 12,
                4, 7, 12,
                7, 2, 12,
                2, 1, 12,

                //bottom
                0, 3, 13,
                3, 6, 13,
                6, 5, 13,
                5, 0, 13,

                //left
                0, 5, 9,
                5, 4, 9,
                4, 1, 9,
                1, 0, 9,

                //right
                2, 7, 11,
                7, 6, 11,
                6, 3, 11,
                3, 2, 11,
            };

            var faceNormals = new Vector3[24];
            for(int i = 0; i < faceNormals.Length; i++)
            {
                var k = i * 3;
                var p0 = points[indices[k]];
                var p1 = points[indices[k + 1]];
                var p2 = points[indices[k + 2]];
                faceNormals[i] = CalculateNormal(p0, p1, p2);
            }
            
            return new CharacterPart
            {
                Geometry = new CharacterGeomerty
                {
                    Points = points,
                    Indices = indices,
                    FaceNormals = faceNormals,
                }
            };
        }
        */

        public static Vector3 CalculateNormal(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            return Vector3.Normalize(Vector3.Cross(p1 - p0, p2 - p0));
        }

        /*
        public static Vector3[] CreateBlock(int width, int height, int depth, out short[] indices)
        {
            var xLength = width + 1;
            var yLength = height + 1;
            var zLength = depth + 1;
            var block = new Vector3[xLength * yLength * zLength];
            var iz = -zLength / 2f;
            var xySize = xLength * yLength;

            for (int i = 0; i < zLength; i++)
            {
                var iy = 0f;
                var depthOffset = xySize * i;
                for (int j = 0; j < yLength; j++)
                {
                    var ix = -xLength / 2f;
                    var heightOffset = j * xLength;
                    for (int k = 0; k < xLength; k++)
                    {
                        var x = ix;
                        var y = iy;
                        var z = iz;
                        var m = k + depthOffset + heightOffset;
                        block[m] = new Vector3(x, y, z);
                        ix+= VoxelSize;
                    }
                    iy+= VoxelSize;
                }
                iz+= VoxelSize;
            }            

            const int IndicesPerVoxel = 36;
            indices = new short[width * height * depth * IndicesPerVoxel];
            var iv = 0;
            for (int i = 0; i < depth; i++)
            {
                var depthOffset = xySize * i;
                for (int j = 0; j < height; j++)
                {
                    var heightOffset = j * xLength;
                    for (int k = 0; k < width; k++)
                    {
                        //front = p0, p1, p2, p3 (clockwise)
                        //back = p5, p4, p7, p6 (clockwise)                        
                        var p0 = k + depthOffset + heightOffset;
                        var p1 = p0 + xLength;
                        var p2 = p1 + 1;
                        var p3 = p0 + 1;
                        var p4 = p0 + xySize;
                        var p5 = p4 + 1;
                        var p6 = p5 + xLength;
                        var p7 = p4 + xLength;

                        var triangles = new int[IndicesPerVoxel]
                        {
                            p0, p1, p2,
                            p2, p3, p0,
                            p4, p5, p6,
                            p6, p7, p4,
                            p3, p2, p6,
                            p6, p5, p3,
                            p4, p7, p1,
                            p1, p0, p4,
                            p1, p7, p6,
                            p6, p2, p1,
                            p4, p0, p3,
                            p3, p5, p4
                        };

                        try
                        {
                            Array.Copy(triangles.Select(Convert.ToInt16).ToArray(), 0, indices, iv, IndicesPerVoxel);
                            iv += IndicesPerVoxel;
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
            }

            return block;
        }
        */
    }

    public class Size3D
    {
        public Size3D() { }
        public Size3D(Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public Vector3 ToVector()
        {
            return new Vector3(X, Y, Z);
        }
        public static explicit operator Size3D(Vector3 v) => new Size3D(v);
        public static explicit operator Vector3(Size3D size) => size.ToVector();
    }
   
    public class CharacterPart
    {
        public CharacterGeomerty Geometry { get; set; }         
    }

    public class CharacterGeomerty
    {
        public Vector3[] Points { get; set; }
        public short[] Indices { get; set; }
        public Vector3[] FaceNormals { get; set; }
        public Vector3[] GetIndexMappedPoints()
            => Indices.Select(i => Points[i]).ToArray();
        public Vector3[] GetIndexMappedNormals()
            => Indices.Select((x, i) => FaceNormals[i / 3]).ToArray();
    }
    
    public class VoxelGeometry
    {
        public List<GeometryQuad> FrontQuads { get; } = new List<GeometryQuad>();
        public List<GeometryQuad> BackQuads { get; } = new List<GeometryQuad>();
        public List<GeometryQuad> LeftQuads { get; } = new List<GeometryQuad>();
        public List<GeometryQuad> RightQuads { get; } = new List<GeometryQuad>();
        public List<GeometryQuad> TopQuads { get; } = new List<GeometryQuad>();
        public List<GeometryQuad> BottomQuads { get; } = new List<GeometryQuad>();
    }

    public class GeometryQuad
    {
        public GeometryQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public Vector3 A { get; }
        public Vector3 B { get; }
        public Vector3 C { get; }
        public Vector3 D { get; }

        public Vector3[] ToTriangles()
            => new[] { A, B, C, C, D, A };
    }

    public class VoxelBuffer
    {
        public VoxelBuffer(int width, int height, int depth, Vector4[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length != width * height * depth)
            {
                throw new ArgumentException("Data has unexpected length. Should be width x height x depth.", nameof(data));
            }

            Width = width;
            Height = height;
            Depth = depth;
            Data = data;
        }
        public int Width { get; }
        public int Height { get; }
        public int Depth { get; }
        public Vector4[] Data { get; }
    }

    public static class ExceptionHelper
    {
        public static void ThrowIfNull(object argument, string paramName = default)
        {
            if(argument == null)
            {
                throw new ArgumentNullException(paramName);
            }    
        }
    }
}
