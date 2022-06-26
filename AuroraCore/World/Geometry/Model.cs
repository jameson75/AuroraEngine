using System;
using SharpDX;
using CipherPark.Aurora.Core.Effects;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Geometry
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Model : IDisposable
    {
        private IGameApp _game = null;
    
        public Model(IGameApp game)
        {
            _game = game;            
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IGameApp Game { get { return _game; } }     
        
        /// <summary>
        /// 
        /// </summary>
        public SurfaceEffect Effect { get; set; }    
        
        /// <summary>
        /// 
        /// </summary>
        public abstract BoundingBox BoundingBox { get; }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Effect?.Dispose();
            OnDispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Draw();           

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnApplyingEffect()
        { } 
        
        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnDispose()
        { }
    }   
}
