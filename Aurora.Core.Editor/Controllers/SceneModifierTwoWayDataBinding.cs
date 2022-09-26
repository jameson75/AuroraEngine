using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.World.Scene;
using System.ComponentModel;
using System.Linq;

namespace Aurora.Core.Editor
{
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
