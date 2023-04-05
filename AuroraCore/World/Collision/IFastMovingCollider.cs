using CipherPark.Aurora.Core.Animation;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Collision
{
    public interface IFastMovingCollider
    {
        PredictedCollisionEvent PredictCollision(
            ITransformable transformableParent,
            Vector3 displacementVector,
            Collider targetCollider,
            ITransformable targetTransformableParent,
            Vector3 targetColliderDisplacementVector);

        bool IsFastMovingCollider { get; }
    }
}