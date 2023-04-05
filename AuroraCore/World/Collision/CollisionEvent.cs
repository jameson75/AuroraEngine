using CipherPark.Aurora.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Collision
{
    /// <summary>
    /// Represents an event of two world objects colliding.
    /// </summary>
    public class CollisionEvent
    {
        /// <summary>
        /// First collider involved in the collision.
        /// </summary>
        public Collider Collider1 { get; set; }

        /// <summary>
        /// Second collider involved in the collision.
        /// </summary>        
        public Collider Collider2 { get; set; } 
        
        /// <summary>
        /// 
        /// </summary>
        public ITransformable TransformableContainer1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ITransformable TransformableContainer2 { get; set; }
    }
}
