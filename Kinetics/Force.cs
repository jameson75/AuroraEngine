using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;

namespace CipherPark.AngelJacket.Core.Kinetics
{
    /// <summary>
    /// Represents a force (in the most singular sense).
    /// </summary>
    /// <remarks>
    /// Velocities are measured in seconds.
    /// </remarks>
    public class Force
    {
        public Force()
        {
            LinearVelocity = Vector3.Zero;
            AngularVelocity = Quaternion.Zero;
        }
        public Vector3 LinearVelocity { get; set; }
        public Quaternion AngularVelocity { get; set; }
        public bool IsImpulse { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeT">time in millseconds</param>
        /// <returns></returns>
        public Transform GetTransformDeltaAtT(ulong timeT)
        {
            //timeT is specified in milliseconds, however, velocities are specified distance-per-seconds.
            //We convert timeT into seconds (with decimal percision).
            float s_timeT = (float)timeT / 1000.0f;          
            Vector3 translationDelta = LinearVelocity * s_timeT;
            Quaternion rotationDelta = AngularVelocity * s_timeT;
            return new Transform(rotationDelta, translationDelta);
        }
    }

    /// <summary>
    /// Represents net force.
    /// </summary>
    public class NetForce
    {
        List<Force> _forces = new List<Force>();
        
        //public float? LinearVelocityCap { get; set; }
        
        //public float? AngularVelocityCap { get; set; }

        public List<Force> Forces { get { return _forces; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeT">time in millseconds</param>
        /// <returns></returns>
        public Transform GetTransformDeltaAtT(ulong timeT)
        {          
            Force netForce = CalculateNetForce();
            return netForce.GetTransformDeltaAtT(timeT);
        }
  
        public Force CalculateNetForce()
        {
            Force netForce = new Force();
            
            foreach (Force force in _forces)
            {
                netForce.LinearVelocity += force.LinearVelocity;
                netForce.AngularVelocity += force.AngularVelocity;
            }
            
            //if (LinearVelocityCap.HasValue)
            //    netForce.LinearVelocity = new Vector3(Math.Min(netForce.LinearVelocity.X, LinearVelocityCap.Value),
            //                                          Math.Min(netForce.LinearVelocity.Y, LinearVelocityCap.Value),
            //                                          Math.Min(netForce.LinearVelocity.Z, LinearVelocityCap.Value));
            //if (AngularVelocityCap.HasValue)
            //    netForce.AngularVelocity = Quaternion.RotationAxis(netForce.AngularVelocity.Axis, Math.Min(netForce.AngularVelocity.Angle, AngularVelocityCap.Value));            

            return netForce;
        }

        public void RemoveImpulses()
        {
            List<Force> pendingDelete = _forces.Where(f => f.IsImpulse).ToList();
            pendingDelete.ForEach(f => _forces.Remove(f));
        }
    }
}
