using Aurora.Core.Editor.Dom;
using Aurora.Core.Editor.Environment;
using Aurora.Sample.Editor.Windows;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Scene;
using Newtonsoft.Json;
using System.Windows;

namespace Aurora.Core.Editor
{
    public class MainWindowController
    {
        private IEditorGameApp game;

        public MainWindowController(IEditorGameApp game)
        {
            this.game = game;
            
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
            DomSerializer.LoadProject(game, filePath);
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
            var effect = new FlatEffect(app, SurfaceVertexType.PositionNormalColor);
            var model = ContentImporter.ImportX(app, filePath, effect, XFileChannels.Mesh | XFileChannels.DefaultMaterialColor, XFileImportOptions.IgnoreMissingColors);

            var gameSceneNode = new GameObjectSceneNode(app)
            {
                GameObject = new GameObject(app, new[]
                {
                    new GameObjectMeta()
                    {
                        Filename = filePath,
                    }
                })
                {
                    Renderer = new ModelRenderer(app)
                    {
                        Model = model
                    }
                },
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
        }             
    }
}
