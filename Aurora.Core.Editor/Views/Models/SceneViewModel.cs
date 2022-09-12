using System;
using System.Collections.ObjectModel;
using System.Linq;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.World.Scene;

namespace Aurora.Core.Editor
{
    public class SceneViewModel : ViewModelBase<SceneGraph>
    {
        private SceneNodeViewModel selectedNode;

        public SceneViewModel(SceneGraph dataModel)
            : base(dataModel)
        {
            
        }

        public ObservableCollection<SceneNodeViewModel> Nodes { get; } = new ObservableCollection<SceneNodeViewModel>();

        public void AddSceneNode(GameObjectSceneNode gameSceneNode)
        {     
            var sceneNode = new SceneNodeViewModel(gameSceneNode);

            Nodes.Add(sceneNode);            
        }

        public SceneNodeViewModel SelectedNode
        {
            get => selectedNode;
            set
            {
                selectedNode = value;
                OnPropertyChanged(nameof(SelectedNode));
                UpdatePickedNodeInScene(value);
            }
        }

        private void UpdatePickedNodeInScene(SceneNodeViewModel value)
        {
            var sceneModifierService = DataModel.Game.Services.GetService<SceneModifierService>();
            if (sceneModifierService != null)
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
        }
    }    
}
