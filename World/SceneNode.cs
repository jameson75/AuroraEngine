using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CipherPark.AngelJacket.Core.World
{
    [Obsolete]
    public interface ISceneObject
    {

    }

    public class SceneNode
    {
        private IGameApp _game = null;
        private SceneNode _parent = null;
        private SceneNodes _children = null;
        public SceneNode(IGameApp game)
        {
            _game = game;
            _children = new SceneNodes(this);
        }

        public SceneNode Parent { get { return _parent; } set { _parent = value; } }
        public SceneNodes Children { get { return _children; } }
        
        [Obsolete]
        public ISceneObject SceneObject { get; set; }
    }
     
    public class SceneNodes :  System.Collections.ObjectModel.ObservableCollection<SceneNode>
    {
        private SceneNode _owner = null;

        [Obsolete]
        public SceneNodes()
        { }

        public SceneNodes(SceneNode owner)
        {
            _owner = owner;
        }
       
        public void AddRange(IEnumerable<SceneNode> nodes)
        {
            foreach( SceneNode node in nodes )
                this.Add(node);
        }
    }

    public class NullSceneNode : SceneNode
    {
        public NullSceneNode(IGameApp game)
            : base(game)
        { }
    }

    public class ModelSceneNode : SceneNode
    {
        public ModelSceneNode(IGameApp game)
            : base(game)
        { }

        public ModelSceneNode(Model model) : base(model.Game)
        {
            Model = model;
        }

        public Model Model { get; set; }            
    }

    public class CameraSceneNode : SceneNode
    {
        
        public CameraSceneNode(IGameApp game)
            : base(game)
        { }

        public CameraSceneNode(Camera camera) : base(camera.Game)
        {
            Camera = camera;
        }

        public Camera Camera { get; set; }
    }
}
