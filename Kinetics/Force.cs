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
    public struct Motion
    {
        private static Motion _identity = new Motion();

        public float LinearVelocity { get; set; }
        public Vector3[] LinearPath { get; set; }
        //public float AngularVelocity { get; set; }
        //public Vector3 AngularVector { get; set; }     
        public static Motion Identity { get { return _identity; } }    
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeT">time in seconds</param>
        /// <returns></returns>
        public Matrix GetTransformationAtT(float timeT)
        {           
            //TODO: Implement catmull for curved paths...
            //TODO: Implement hermite for acceleration curves... 

            Vector3 translation = Vector3.Zero;
            float step = LinearVelocity * timeT;
            if (LinearPath != null)
            {
                Vector3 pathOrigin = Vector3.Zero;
                Vector3 pathDirection = Vector3.Zero;
                float pathDistance = 0;
                float accumLength = 0;
                for (int i = 0; i < LinearPath.Length; i++)
                {
                    pathDistance = Vector3.Distance(LinearPath[i], pathOrigin);
                    accumLength+= pathDistance;
                    if (accumLength > step)
                    {
                        pathDirection = LinearPath[i] - pathOrigin;
                        pathDirection.Normalize();
                        break;
                    }
                    pathOrigin = LinearPath[i];
                }
                float pathStep = step - accumLength + pathDistance;
                translation = pathOrigin + (pathDirection * pathStep);
            }           

            return Matrix.Translation(translation);
        }    
    }

    ///// <summary>
    ///// Represents net force.
    ///// </summary>
    //public class NetForce
    //{
    //    List<Force> _forces = new List<Force>();
        
    //    //public float? LinearVelocityCap { get; set; }
        
    //    //public float? AngularVelocityCap { get; set; }

    //    public List<Force> Forces { get { return _forces; } }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="timeT">time in millseconds</param>
    //    /// <returns></returns>
    //    public Transform GetTransformDeltaAtT(ulong timeT)
    //    {          
    //        Force netForce = CalculateNetForce();
    //        return netForce.GetTransformDeltaAtT(timeT);
    //    }
  
    //    public Force CalculateNetForce()
    //    {
    //        Force netForce = new Force();
            
    //        foreach (Force force in _forces)
    //        {
    //            netForce.LinearVelocity += force.LinearVelocity;
    //            netForce.AngularVelocity += force.AngularVelocity;
    //        }
            
    //        //if (LinearVelocityCap.HasValue)
    //        //    netForce.LinearVelocity = new Vector3(Math.Min(netForce.LinearVelocity.X, LinearVelocityCap.Value),
    //        //                                          Math.Min(netForce.LinearVelocity.Y, LinearVelocityCap.Value),
    //        //                                          Math.Min(netForce.LinearVelocity.Z, LinearVelocityCap.Value));
    //        //if (AngularVelocityCap.HasValue)
    //        //    netForce.AngularVelocity = Quaternion.RotationAxis(netForce.AngularVelocity.Axis, Math.Min(netForce.AngularVelocity.Angle, AngularVelocityCap.Value));            

    //        return netForce;
    //    }

    //    public void RemoveImpulses()
    //    {
    //        List<Force> pendingDelete = _forces.Where(f => f.IsImpulse).ToList();
    //        pendingDelete.ForEach(f => _forces.Remove(f));
    //    }
    //}
}
