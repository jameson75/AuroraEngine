using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Systems.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;

namespace CipherPark.AngelJacket.Core.World.Scene
{
    [Obsolete]
    public interface ISceneObject
    {

    }

    public abstract class SceneNode : ITransformable
    {
        private Scene _scene = null;
        private SceneNode _parent = null;
        private SceneNodes _children = null;        
        
        public SceneNode(Scene scene)
        {
            _scene = scene;
            _children = new SceneNodes();
            Transform = Transform.Identity;
        }

        public Scene Scene { get { return _scene; } }

        public SceneNode Parent { get { return _parent; } set { _parent = value; } }

        public SceneNodes Children { get { return _children; } }
        
        public Transform Transform { get; set; }

        [Obsolete]
        public ISceneObject SceneObject { get; set; }

        public virtual void Draw(long gameTime) { }

        public virtual void Update(long gameTime) { }
    }
     
    public class SceneNodes :  ObservableCollection<SceneNode>
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
        public NullSceneNode(Scene scene)
            : base(scene)
        { }
    }

    public class ModelSceneNode : SceneNode
    {
        public ModelSceneNode(Scene scene)
            : base(scene)
        { }

        public ModelSceneNode(Scene scene, Model model) : base(scene)
        {
            Model = model;
        }

        public Model Model { get; set; }

        public override void Draw(long gameTime)
        {
            if (Model != null)
            {
                Matrix cachedTransform = Model.Transform;
                Model.Transform = Scene.WorldTransform.Transform * cachedTransform;            
                Model.Draw(gameTime);
                Model.Transform = cachedTransform;
            }
        }
    }

    public class CameraSceneNode : SceneNode
    {
        private Matrix _cachedViewMatrix = Matrix.Zero;

        public CameraSceneNode(Scene scene)
            : base(scene)
        {
            scene.BeginUpdate += Scene_BeginUpdate;
            scene.EndDraw += Scene_EndDraw;
        }
        
        public CameraSceneNode(Scene scene, Camera camera) : base(scene)
        {
            Camera = camera;           
            scene.EndDraw += Scene_EndDraw; scene.BeginUpdate += Scene_BeginUpdate;
            scene.EndDraw += Scene_EndDraw;
        }

        public Camera Camera { get; set; }

        public override void Update(long gameTime)
        {
            if (Camera != null)
            {                
                Camera.ViewMatrix = Scene.WorldTransform.Transform * _cachedViewMatrix;
            }
        }

        private void Scene_EndDraw(object sender, EventArgs e)
        {
            if (Camera != null)
            {
                Camera.ViewMatrix = _cachedViewMatrix;
            }
        }

        private void Scene_BeginUpdate(object sender, EventArgs e)
        {
            if (Camera != null)
            {
                _cachedViewMatrix = Camera.ViewMatrix;
            }
        }
    }
}
