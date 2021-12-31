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
    /// <summary>
    /// Represents an event of two world objects colliding.
    /// </summary>
    public class CollisionEvent
    {
        /// <summary>
        /// First object involved in the collision.
        /// </summary>
        public Collider Object1 { get; set; }

        /// <summary>
        /// Second object involved in the collision.
        /// </summary>        
        public Collider Object2 { get; set; }

        /// <summary>
        /// The first object's location at the collision.
        /// </summary>
        public Vector3 Object1LocationAtCollision { get; set; }

        /// <summary>
        /// The second object's location at the collision.
        /// </summary>
        public Vector3 Object2LocationAtCollision { get; set; }
    }
}
