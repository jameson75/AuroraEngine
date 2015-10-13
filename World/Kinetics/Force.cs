using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Kinetics
{
    /// <summary>
    /// This application considers a "Motion" to be something which describes a body's
    /// linear and angular velocities over time.
    /// </summary>
    public class Motion
    {
        /// <summary>
        /// Linear velocity in milliseconds.
        /// </summary>
        public float LinearVelocity { get; set; }
        /// <summary>
        /// Direction/Heading of linear motion (specified in world coordinates).
        /// </summary>
        public Vector3 Direction { get; set; }
        /// <summary>
        /// Angular velocity in milliseconds.
        /// </summary>
        public float AngularVelocity { get; set; }
        /// <summary>
        /// Rotation axis of angular motion (specified in world coordinates).
        /// </summary>
        public Vector3 AngularVector { get; set; }          
        /// <summary>
        /// Calculates the transformation matrix of an object in [this] motion for the specified time window.
        /// </summary>
        /// <param name="timeT">time in seconds</param>
        /// <returns></returns>
        public void TransformTarget(ulong windowT, ITransformable target)
        {          
            //TODO: Implement hermite for acceleration curves... 
            if (LinearVelocity >= 0)
            {
                float displacementDistance = windowT / LinearVelocity;
                Vector3 displacementVector = displacementDistance * Vector3.Normalize(Direction);
                Matrix transformationMatrix = Matrix.Translation(displacementVector);
                Matrix transform = target.Transform.ToMatrix() * transformationMatrix;
                target.Transform = new Transform(transform);
            }            
        }    
    }
}
