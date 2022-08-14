using System.Collections.ObjectModel;
using System.Linq;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.World.Scene;

namespace Aurora.Core.Editor
{
    public class SceneViewModel : ViewModelBase<SceneGraph>
    {        
        public SceneViewModel(SceneGraph dataModel)
            : base(dataModel)
        {
            ListenForSceneNotifications();
        }

        public ObservableCollection<SceneNodeViewModel> Nodes { get; } = new ObservableCollection<SceneNodeViewModel>();

        public void AddSceneNode(GameObjectSceneNode gameSceneNode)
        {     
            var sceneNode = new SceneNodeViewModel(gameSceneNode);

            Nodes.Add(sceneNode);            
        }

        private void ListenForSceneNotifications()
        {
            var sceneModifierService = DataModel.Game.Services.GetService<SceneModifierService>();
            if (sceneModifierService != null)
            {
                sceneModifierService.NodeTransformed += SceneModifierService_NodeTransformed;
            }
        }
       
        private void SceneModifierService_NodeTransformed(object sender, NodeTransformedArgs args)
        {
            var sceneNode = Nodes.FirstOrDefault(n => n.DataModel == args.TransfromedNode);
            if (sceneNode != null)
            {
                sceneNode.Matrix = args.TransfromedNode.Transform.ToMatrix().ToArray();                
            }
        }
    }    
}
