using System;
using CipherPark.KillScript.Core.Animation;
using CipherPark.KillScript.Core.World.Collision;
using CipherPark.KillScript.Core.World.Scene;
using CipherPark.KillScript.Core.World.Geometry;
using CipherPark.KillScript.Core.Systems;
using System.Collections.Generic;

namespace CipherPark.KillScript.Core.World
{
    public class GameObject : IDisposable
    {       
        GameObjectSceneNode _containerNode = null;
        Dictionary<Type, object> _contextualContent = null;

        public GameObject(IGameApp game)
        {
            Game = game;
            _contextualContent = new Dictionary<Type, object>();
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

        public T GetContext<T>() where T : class
        {
            var type = typeof(T);
            if (!_contextualContent.ContainsKey(type))
                return null;
            return (T)_contextualContent[type];
        }         

        public void AddContext<T>(T content) where T : class
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));
            var type = typeof(T);
            if (_contextualContent.ContainsKey(type))
                throw new InvalidOperationException("Content type already exists.");
            _contextualContent.Add(type, content);
        }
       
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

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (Renderer != null)
                Renderer.Dispose();
        }
    }
}
