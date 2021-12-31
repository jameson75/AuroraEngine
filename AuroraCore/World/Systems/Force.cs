using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Systems
{
    /// <summary>
    /// Represents the state of a body's motion.
    /// </summary>
    public class BodyMotion
    {
        /// <summary>
        /// Time domain of velocity         
        /// </summary>
        public TimeDomainUnits TimeDomainUnits { get; set; }
        /// <summary>
        /// Linear velocity.
        /// </summary>
        public float LinearVelocity { get; set; }
        /// <summary>
        /// Direction/Heading of linear motion (specified in world coordinates).
        /// </summary>
        public Vector3 Direction { get; set; }
        /// <summary>
        /// Angular velocity.
        /// </summary>
        public float AngularVelocity { get; set; }
        /// <summary>
        /// Rotation axis of angular motion (specified in world coordinates).
        /// </summary>
        public Vector3 AngularVector { get; set; }                  
    }

    public enum TimeDomainUnits
    {
        Milliseconds = 0,
        Seconds,
        Minutes,
        Hours
    }
}
