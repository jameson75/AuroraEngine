using Aurora.Sample.Editor.Windows;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Scene;
using Newtonsoft.Json;
using SharpDX;
using System.Collections.Generic;
using System.Windows;

namespace Aurora.Sample.Editor
{
    public class MainWindowController
    {
        private EditorGameApp game;
        private ProjectViewModel dom = new ProjectViewModel();

        public MainWindowController(EditorGameApp game)
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
            dialog.DefaultExt = ".aurora";https://social.msdn.microsoft.com/Forums/vstudio/en-US/d3f223ac-7fca-486e-8939-adb46e9bf6c9/how-can-i-get-yesno-from-a-messagebox-in-wpf?forum=wpf
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
            /*
            BlinnPhongEffect2 effect = new BlinnPhongEffect2(game, SurfaceVertexType.InstancePositionNormalColor);                        
            effect.AmbientColor = SharpDX.Color.White;
            effect.Lighting = new Light[]
            {
                    new PointLight
                    {
                        Diffuse = SharpDX.Color.White,
                        Transform = new CipherPark.Aurora.Core.Animation.Transform(new Vector3(500, 500, 500))
                    },
                    new PointLight
                    {
                        Diffuse = SharpDX.Color.White,
                        Transform = new CipherPark.Aurora.Core.Animation.Transform(new Vector3(-500, -500, -500))
                    }
            };
            */

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
}
