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

namespace CipherPark.AngelJacket.Core.Systems
{
    /// <summary>
    /// Represents the state of a body's motion.
    /// </summary>
    public class Motion
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
        /// <summary>
        /// Displaces an object in motion for the specified time window.
        /// </summary>
        /// <param name="windowT">Length of the time window in milliseconds</param>
        /// <returns></returns>
        public void TransformTarget(ulong windowT, ITransformable target)
        {          
            //TODO: Implement angular transform.             
            if (LinearVelocity >= 0)
            {
                float cWindowT = 0;
                switch(TimeDomainUnits)
                {
                    case Systems.TimeDomainUnits.Seconds:
                        cWindowT = (float)windowT / 1000.0f;
                        break;
                    case Systems.TimeDomainUnits.Minutes:
                        cWindowT = (float)windowT / 60000.0f;
                        break;
                    case Systems.TimeDomainUnits.Hours:
                        cWindowT = (float)windowT / 3600000.0f;
                        break;
                    default:
                        cWindowT = (float)windowT;
                        break;
                }
                float displacementDistance = cWindowT * LinearVelocity;
                Vector3 displacementVector = displacementDistance * Vector3.Normalize(target.WorldToParentNormal(Direction));              
                Vector3 newTranslation = target.Transform.Translation + displacementVector;
                target.Transform = new Transform(target.Transform.Rotation, newTranslation);
            }            
        }    
    }

    public enum TimeDomainUnits
    {
        Milliseconds,
        Seconds,
        Minutes,
        Hours
    }
}
