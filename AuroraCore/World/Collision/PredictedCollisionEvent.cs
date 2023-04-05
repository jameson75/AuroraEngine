using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Collision
{
    public class PredictedCollisionEvent : CollisionEvent
    {
        /// <summary>
        /// Collision point of the first collider.
        /// </summary>
        public Vector3 PredictedCollisionLocation1 { get; set; }

        /// <summary>
        /// The second object's location at the collision.
        /// </summary>
        public Vector3 PredictedCollisionLocation2 { get; set; }
    }
}
