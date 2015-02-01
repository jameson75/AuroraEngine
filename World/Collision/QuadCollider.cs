using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Services;
using CipherPark.AngelJacket.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Collision
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
                else if (targetCollider is BoundingBoxCollider)
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
