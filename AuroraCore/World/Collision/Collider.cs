using CipherPark.Aurora.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Collision
{ 
    public abstract class Collider 
    {
        public abstract CollisionEvent DetectCollision(
            ITransformable transformableContainer, 
            Collider targetCollider, 
            ITransformable targetTransformableContainer);
    }
}
