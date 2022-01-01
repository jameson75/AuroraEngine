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
