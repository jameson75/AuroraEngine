using Aurora.Core.Editor.Dom;
using Aurora.Sample.Editor.Windows;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.World.Scene;
using SharpDX;
using System;
using System.ComponentModel;
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

            app.Scene.Nodes.Add(gameSceneNode);

            ViewModel.Project.Scene.AddSceneNode(gameSceneNode);

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
            switch (transformPlane)
            {
                case EditorTransformPlane.XZ:
                    game.ReferenceGridMode = ReferenceGridMode.Flat;
                    break;
                case EditorTransformPlane.Y:
                    game.ReferenceGridMode = ReferenceGridMode.Box;
                    break;
                default:
                    throw new ArgumentException("Unsupported editor transform plane");
            };
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
    }

    public class SceneModifierTwoWayDataBinding
    {
        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly SceneModifierService sceneModifierService;
        private ProjectViewModel projectViewModel;
        private SceneViewModel sceneViewModel;

        public SceneModifierTwoWayDataBinding(MainWindowViewModel mainWindowViewModel, SceneModifierService sceneModifierService)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            this.sceneModifierService = sceneModifierService;
            mainWindowViewModel.PropertyChanged += MainViewModel_PropertyChanged;
            UpdateListenersForProjectViewModel();
            ListenForSceneModificationNotifications();
        }

        private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.Project))
            {
                UpdateListenersForProjectViewModel();
            }
        }

        private void ProjectViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProjectViewModel.Scene))
            {
                UpdateListenersForSceneViewModel();
            }
        }

        private void SceneViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SceneViewModel.SelectedNode))
            {
                UpdatePickedNodeInScene(sceneViewModel.SelectedNode);
            }
        }

        private void ListenForSceneModificationNotifications()
        {           
            sceneModifierService.NodeTransformed += SceneModifierService_NodeTransformed;
            sceneModifierService.PickedNodeChanged += SceneModifierService_PickedNodeChanged;        
        }

        private void SceneModifierService_NodeTransformed(object sender, NodeTransformedArgs args)
        {
            var sceneNode = sceneViewModel.Nodes.FirstOrDefault(n => n.DataModel == args.TransfromedNode);
            if (sceneNode != null)
            {
                sceneNode.NotifyTransform();
            }
        }

        private void SceneModifierService_PickedNodeChanged(object sender, PickedNodeChangedArgs args)
        {
            if (args.PickedNode == null)
            {
                sceneViewModel.SelectedNode = null;
            }

            else
            {
                var sceneNode = sceneViewModel.Nodes.FirstOrDefault(n => n.DataModel == args.PickedNode);
                if (sceneNode != null)
                {
                    sceneViewModel.SelectedNode = sceneNode;
                }
            }
        }

        private void UpdatePickedNodeInScene(SceneNodeViewModel value)
        {           
            if (value == null)
            {
                sceneModifierService.UpdatePick(null);
            }
            else
            {
                var gameObjectNode = value.DataModel.As<GameObjectSceneNode>();
                if (gameObjectNode != null)
                {
                    sceneModifierService.UpdatePick(gameObjectNode);
                }
            }
        }

        private void UpdateListenersForProjectViewModel()
        {
            if (projectViewModel != null)
            {
                projectViewModel.PropertyChanged -= ProjectViewModel_PropertyChanged;
                projectViewModel = null;
            }

            if (mainWindowViewModel.Project != null)
            {
                projectViewModel = mainWindowViewModel.Project;
                projectViewModel.PropertyChanged += ProjectViewModel_PropertyChanged;
            }

            UpdateListenersForSceneViewModel();
        }

        private void UpdateListenersForSceneViewModel()
        {
            if (sceneViewModel != null)
            {
                sceneViewModel.PropertyChanged -= SceneViewModel_PropertyChanged;
                sceneViewModel = null;
            }

            if (mainWindowViewModel.Project?.Scene != null)
            {
                sceneViewModel = mainWindowViewModel.Project.Scene;
                sceneViewModel.PropertyChanged += SceneViewModel_PropertyChanged;
            }
        }
    }
}
