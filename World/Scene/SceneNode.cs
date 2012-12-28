using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Systems.Animation;

namespace CipherPark.AngelJacket.Core.World.Scene
{
    [Obsolete]
    public interface ISceneObject
    {

    }

    public abstract class SceneNode : ITransformable
    {
        private IGameApp _game = null;
        private SceneNode _parent = null;
        private SceneNodes _children = null;
        
        
        public SceneNode(IGameApp game)
        {
            _game = game;
            _children = new SceneNodes();
            Transform = new Transform { Translation = Vector3.Zero, Rotation = Quaternion.Identity };
        }

        public SceneNode Parent { get { return _parent; } set { _parent = value; } }

        public SceneNodes Children { get { return _children; } }
        
        public Transform Transform { get; set; }

        [Obsolete]
        public ISceneObject SceneObject { get; set; }

        public virtual void Draw(long gameTime) { }

        public virtual void Update(long gameTime) { }
    }
     
    public class SceneNodes :  System.Collections.ObjectModel.ObservableCollection<SceneNode>
    {   
        public SceneNodes()
        { }    
       
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

        public ModelSceneNode(Geometry.Model model) : base(model.Game)
        {
            Model = model;
        }

        public Geometry.Model Model { get; set; }

        public override void Draw(long gameTime)
        {
            if (Model != null)
            {
                Model.Draw(gameTime);
            }
        }
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
