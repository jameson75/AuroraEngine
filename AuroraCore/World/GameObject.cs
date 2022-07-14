using System;
using SharpDX;
using System.Linq;
using System.Collections.Generic;
using CipherPark.Aurora.Core.World.Collision;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Systems;

namespace CipherPark.Aurora.Core.World
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

        public GameObject(IGameApp game, object[] contextualContent) : this(game)
        {
            foreach (var contextualObject in contextualContent)
            {
                _contextualContent.Add(contextualObject.GetType(), contextualObject);
            }
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

        public IRigidBody RigidBody { get; set; }

        public ColliderCollection Colliders { get; set; }

        public IRenderer Renderer { get; set; }        

        public T GetContext<T>() where T : class
        {
            var type = typeof(T);
            if (!_contextualContent.ContainsKey(type))
                return FindContextByContract<T>();
            return (T)_contextualContent[type];
        }       

        public BoundingBox? GetBoundingBox()
        {
            var boundingContext = FindContextByContract<IProvideBoundingContext>();
            if (boundingContext != null)
                return boundingContext.GetBoundingBox();
            else if (Renderer is IProvideBoundingContext)
                return ((IProvideBoundingContext)Renderer).GetBoundingBox();
            return null;
        }

        public void AddContext<T>(T content) where T : class
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));
            var type = content.GetType();
            if (_contextualContent.ContainsKey(type))
                throw new InvalidOperationException("Content type already exists.");
            _contextualContent.Add(type, content);
        }      

        public BodyMotion BodyMotion { get; set; }

        public Action<GameTime> UpdateAction { get; set; }       

        public void Update(GameTime gameTime)
        {
            UpdateAction?.Invoke(gameTime);
            Renderer?.Update(gameTime);            
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T FindContextByContract<T>() where T : class
        {
            return (T)_contextualContent.FirstOrDefault(c => c.Key.IsAssignableFrom(typeof(T))).Value;
        }
    }
}
