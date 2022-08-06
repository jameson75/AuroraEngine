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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Aurora.Core.Editor
{
    public class MainWindowController
    {
        private IEditorGameApp game;        

        public MainWindowController(IEditorGameApp game)
        {
            this.game = game;
            game.NodeTransformed += Game_NodeTransformed;
            ViewModel = new MainWindowViewModel();            
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
            var filePath = PresentSaveProjectDialog();
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
        
        public void NewProject()
        {
            ViewModel.Project = new ProjectViewModel()
            {
                Scene = new SceneViewModel(),
            };
        }

        public void OpenProject(string filePath)
        {
            var json = System.IO.File.ReadAllText(filePath);
            ViewModel.Project = JsonConvert.DeserializeObject<ProjectViewModel>(json);
            ViewModel.ProjectFileName = filePath;
            //game.ClearScene(true);
            BuildSceneGraphFromDomChildren(ViewModel.Project.Scene.Nodes);        
        }        

        public void SaveProject(string filePath)
        {
            var json = JsonConvert.SerializeObject(ViewModel.Project);
            System.IO.File.WriteAllText(filePath, json);
            if (ViewModel.ProjectFileName != filePath)
            {
                ViewModel.ProjectFileName = filePath;
            }
            ViewModel.IsProjectDirty = false;
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
            ViewModel.Project.AddSceneNode(node, filePath);
            ViewModel.IsProjectDirty = true;
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

        private void BuildSceneGraphFromDomChildren(IEnumerable<SceneNodeViewModel> children)
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
                    domNode.GameObjectDescription.FileName,
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
            var domNode = ViewModel.Project.Scene.FindSceneNodeViewModel(n => n.DataModel == args.TransfromedNode);
            if( domNode != null)
            {
                domNode.Matrix = args.TransfromedNode.Transform.ToMatrix().ToArray();
                ViewModel.IsProjectDirty = true;
            }
        }
    }

    public class ProjectViewModel : ViewModelBase
    {
        private SceneViewModel scene;

        public SceneViewModel Scene
        {
            get => scene;
            set
            {
                scene = value;
                OnPropertyChanged(nameof(Scene));                
            }
        }

        public void AddSceneNode(GameObjectSceneNode gameObjectNode, string gameObjectResourceFileName)
        {
            var sceneNodeViewModel = new SceneNodeViewModel();
            sceneNodeViewModel.Name = gameObjectNode.Name ?? System.IO.Path.GetFileNameWithoutExtension(gameObjectResourceFileName);
            sceneNodeViewModel.Matrix = gameObjectNode.Transform.ToMatrix().ToArray();
            sceneNodeViewModel.Flags = gameObjectNode.Flags;
            sceneNodeViewModel.Visible = gameObjectNode.Visible;
            if (gameObjectNode.GameObject.Renderer is ModelRenderer)
            {
                sceneNodeViewModel.GameObjectType = GameObjectType.GeometricModel;
                sceneNodeViewModel.GameObjectDescription = new GameObjectDescription()
                {
                    FileName = gameObjectResourceFileName,
                    EffectType = gameObjectNode.GameObject.Renderer.As<ModelRenderer>()
                                               .Model
                                               .Effect
                                               .GetType()
                                               .Name,
                    ModelType = gameObjectNode.GameObject.Renderer.As<ModelRenderer>()
                                               .Model
                                               .GetType()
                                               .Name,
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
                Scene.Nodes.Add(sceneNodeViewModel);
            }
        }
    }

    public class SceneViewModel : ViewModelBase
    {
        [JsonProperty("Children")]
        public ObservableCollection<SceneNodeViewModel> Nodes { get; } = new ObservableCollection<SceneNodeViewModel>();

        public SceneNodeViewModel FindSceneNodeViewModel(Func<SceneNodeViewModel, bool> condition)
        {
            return FindSceneNodeViewModel(Nodes, condition);
        }

        private static SceneNodeViewModel FindSceneNodeViewModel(IEnumerable<SceneNodeViewModel> children, Func<SceneNodeViewModel, bool> condition)
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

    public class SceneNodeViewModel : ViewModelBase
    {
        private string name;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public float[] Matrix { get; set; }
        public ulong Flags { get; set; }
        public bool Visible { get; set; }
        public GameObjectType GameObjectType { get; set; }
        public GameObjectDescription GameObjectDescription { get; set; }
        [JsonIgnore]
        public SceneNode DataModel { get; set; }
        public ObservableCollection<SceneNodeViewModel> Children { get; } = new ObservableCollection<SceneNodeViewModel>();
    }

    public class GameObjectDescription
    {
        public string FileName { get; set; }
        public string EffectType { get; set; }
        public string ModelType { get; set; }
    }

    public enum GameObjectType
    {
        GeometricModel
    }
    
    public class MainWindowViewModel : ViewModelBase
    {
        private ProjectViewModel project;
        private bool isProjectDirty;
        private string projectFileName;
        private SceneNodeViewModel selectedNode;

        public ProjectViewModel Project
        {
            get => project;
            set
            {
                project = value;
                OnPropertyChanged(nameof(Project));                
            }
        }

        public bool IsProjectDirty
        {
            get => isProjectDirty;
            set
            {
                isProjectDirty = value;
                OnPropertyChanged(nameof(IsProjectDirty));                
            }
        }

        public string ProjectFileName
        {
            get => projectFileName;
            set
            {
                projectFileName = value;
                OnPropertyChanged(nameof(ProjectFileName));                
            }
        }

        public SceneNodeViewModel SelectedNode
        {
            get => selectedNode;
            set
            {
                selectedNode = value;
                OnPropertyChanged(nameof(SelectedNode));
            }
        }            
    }

    public class ViewModelBase : INotifyPropertyChanged
    {
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;        
    }
}
