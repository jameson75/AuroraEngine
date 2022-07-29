using Aurora.Sample.Editor.Windows;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Scene;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Aurora.Sample.Editor
{
    public class MainWindowController
    {
        private IEditorGameApp game;
        private ProjectViewModel dom = new ProjectViewModel();

        public MainWindowController(IEditorGameApp game)
        {
            this.game = game;
            game.NodeTransformed += Game_NodeTransformed;
        }        

        public void UIOpenProject()
        {
            if (IsDomDirty)
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
            if (IsDomDirty)
            {
                if (dom.Filename != null)
                {
                    SaveProject(dom.Filename);
                }
                else
                {
                    var filePath = PresentSaveProjectDialog();
                    if (filePath != null)
                    {
                        SaveProject(filePath);                        
                    }
                }
                dom.IsDirty = false;
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

        public string PresentSaveProjectDialog()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.DefaultExt = ".aurora";
            dialog.Filter = "Aurora Project (.aurora)|*.aurora";
            return dialog.ShowDialog() == true ? dialog.FileName : null;            
        }

        public bool IsDomDirty => dom?.IsDirty ?? false;

        public void OpenProject(string filePath)
        {
            var json = System.IO.File.ReadAllText(filePath);
            dom = JsonConvert.DeserializeObject<ProjectViewModel>(json);
            dom.Filename = filePath;
            //game.ClearScene(true);
            BuildSceneGraphFromDomChildren(dom.Scene.Children);           
        }        

        public void SaveProject(string filePath)
        {
            var json = JsonConvert.SerializeObject(dom);
            System.IO.File.WriteAllText(filePath, json);
            if (dom.Filename != filePath)
            {
                dom.Filename = filePath;
            }
        }

        public void ImportModel(string filePath)
        {
            FlatEffect effect = new FlatEffect(game, SurfaceVertexType.PositionColor);
            var model = ContentImporter.ImportX(game, filePath, effect, XFileChannels.Mesh | XFileChannels.DefaultMaterialColor , XFileImportOptions.IgnoreMissingColors);
            var node = new GameObjectSceneNode(game)
            {
                GameObject = new GameObject(game)
                {
                    Renderer = new ModelRenderer(game)
                    {
                        Model = model
                    }
                },
            };

            game.Scene.Nodes.Add(node);
            dom.AddNode(node, filePath);
            dom.IsDirty = true;
        }

        public void EnterEditorMode(EditorMode mode)
        {
            game.EditorMode = mode;
        }

        public void ResetEditorCamera()
        {
            game.ResetCamera();
        }

        public void SetEditorTransformPlane(EditorTransformPlane transformPlane)
        {
            game.TransformPlane = transformPlane;
        }

        private void BuildSceneGraphFromDomChildren(List<SceneNodeViewModel> children)
        {
            var parentLookup = new Dictionary<SceneNodeViewModel, SceneNode>();
            foreach (var child in children)
            {
                var gameObjectSceneNode = new GameObjectSceneNode(game)
                {
                    Name = child.Name,
                    Transform = new Transform(new Matrix(child.Matrix)),
                    Flags = child.Flags,
                    Visible = child.Visible,
                    GameObject = CreateGameObjectFromDomNode(child),
                };
                if (parentLookup.ContainsKey(child))
                    parentLookup[child].Children.Add(gameObjectSceneNode);
                else
                    game.Scene.Nodes.Add(gameObjectSceneNode);

                parentLookup.Add(child, gameObjectSceneNode);

                BuildSceneGraphFromDomChildren(child.Children);
            }
        }

        private GameObject CreateGameObjectFromDomNode(SceneNodeViewModel domNode)
        {
            if(domNode.GameObjectType == GameObjectType.GeometricModel)
            {
                var effect = new FlatEffect(game, SurfaceVertexType.PositionColor);

                var model = ContentImporter.ImportX(
                    game,
                    domNode.GameObjectDescription.Filename,
                    effect,
                    XFileChannels.Mesh | XFileChannels.DefaultMaterialColor, XFileImportOptions.IgnoreMissingColors);

                return new GameObject(game)
                {
                    Renderer = new ModelRenderer(model),
                };
            }

            return null;
        }

        private void Game_NodeTransformed(object sender, CipherPark.Aurora.Core.Services.NodeTransformedArgs args)
        {
            var domNode = dom.Scene.FindSceneNodeViewModel(n => n.DataModel == args.TransfromedNode);
            if( domNode != null)
            {
                domNode.Matrix = args.TransfromedNode.Transform.ToMatrix().ToArray();
                dom.IsDirty = true;
            }
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
            if (gameObjectNode.GameObject.Renderer is ModelRenderer)
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

            if (gameObjectNode.Parent != null)
            {
                var parentSceneNodeViewModel = Scene.FindSceneNodeViewModel(n => n.DataModel == gameObjectNode.Parent);
                if (parentSceneNodeViewModel == null)
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
            foreach (var child in children)
            {
                if (condition(child))
                {
                    return child;
                }

                var descendant = FindSceneNodeViewModel(child.Children, condition);

                if (descendant != null)
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
