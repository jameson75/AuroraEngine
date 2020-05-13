using System;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Collision;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Systems;

namespace CipherPark.AngelJacket.Core.World
{
    public class GameObject /* : ITransformable */
    {       
        GameObjectSceneNode _containerNode = null;

        public GameObject(IGameApp game)
        {
            Game = game;
        }

        public GameObjectSceneNode ContainerNode
        {
            get { return _containerNode; }
            set
            {
                if (value == null)
                {
                    var c = _containerNode;
                    _containerNode = null;
                    if (c.GameObject == this)
                        c.GameObject = null;
                }
                else
                {
                    _containerNode = value;
                    if (_containerNode.GameObject != this)
                        _containerNode.GameObject = this;
                }
            }
        }

        public IGameApp Game { get; private set; }

        /*
        public string Name { get; set; }
        */

        public IRigidBody RigidBody { get; set; }

        public ColliderCollection Colliders { get; set; }

        public IRenderer Renderer { get; set; }        
       
        /*
        public Transform Transform { get; set; }

        public ITransformable TransformableParent { get; set; }
        */        

        public BodyMotion BodyMotion { get; set; }

        public Action<GameTime> UpdateAction { get; set; }       

        public void Update(GameTime gameTime)
        {
            Renderer?.Update(gameTime);
            UpdateAction?.Invoke(gameTime);
            OnUpdate(gameTime);
        }

        public void Draw()
        {                  
            Renderer?.Draw(_containerNode);
            OnDraw();
        }        

        protected virtual void OnDraw()
        { }

        protected virtual void OnUpdate(GameTime gameTime)
        { }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnInitialize()
        { }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnUninitialize()
        { }
    }
}
