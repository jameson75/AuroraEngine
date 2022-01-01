using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.Aurora.Core;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Collision
{
    public class QuadCollider : Collider
    {
        public BoundingQuadOA Quad { get; set; }
        public bool EnableFastMovingObjectDetection { get; set; }

        public override CollisionEvent DetectCollision(Vector3 direction, Collider targetCollider, Vector3 targetColliderDirection)
        {
            CollisionEvent collisionEvent = null;
            if (EnableFastMovingObjectDetection)
            {
                if (targetCollider is SphereCollider)
                {
                    //The SphereCollider already has logic to detect fast moving sphere-quad/plane collisions.
                    //So, we defer to it.
                    collisionEvent = this.DeferDetectCollision(direction, targetCollider, targetColliderDirection);
                }
                else if (targetCollider is QuadCollider)
                {

                }
                else if (targetCollider is BoxCollider)
                {
                    //The BoundingBoxCollider already has logic to detect fast moving boundingbox-quad/plane collisions.
                    //So, we defer to it.
                    collisionEvent = this.DeferDetectCollision(direction, targetCollider, targetColliderDirection);
                }
            }
            return collisionEvent;
        }

        public bool UseEntirePlane { get; set; }
    }
}
