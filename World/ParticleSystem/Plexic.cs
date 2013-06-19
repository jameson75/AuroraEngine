using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.ParticleSystem
{
    //public class PlexicEmitter
    //{        
    //    private List<Vector3> _litVertices = new List<Vector3>();
    //    private List<PlexicParticle> _particles = new List<PlexicParticle>();

    //    public Vector3[] Vertices { get; set; }
        
    //    private Plane Plane { get; set; }
        
    //    public void Emit()
    //    {
    //        for (int i = 0; i < Vertices.Length; i++)
    //        {                
    //            //**********************************************************************************************
    //            //MATH: The back side of the plane is the considered to be the side in the opposite direction
    //            //of the plane's normal. We determine whether the vertice is on the plane or in back
    //            //of the plane by, first, determining the direction vector between the plane and 
    //            //the vertice. If the angle between the normal of the plane and this direction vector
    //            //is greater or equal to 180 degrees it means the vertice is on or 
    //            //behind the plane. (NOTE: cos(theta) where 180 >= theta > 360 is always negative)
    //            //**********************************************************************************************
               
    //            Vector3 pointOnPlane = Vector3.Normalize(Plane.Normal) * Plane.D;
    //            Vector3 vertDir = Vector3.Normalize(Vertices[i] - pointOnPlane);
    //            if( Vector3.Dot(vertDir, pointOnPlane) >= 0)
    //            {
    //                if (!_litVertices.Contains(Vertices[i]))
    //                {
    //                    PlexicParticleDescription pDesc = new PlexicParticleDescription();
    //                    PlexicParticle p = PlexicEmitter.Spawn(pDesc);
    //                    _litVertices.Add(Vertices[i]);
    //                }
    //            }                
    //        }
    //    }

    //    public void Update(long gameTime)
    //    {
           
    //    }

    //    private static PlexicParticle Spawn(PlexicParticleDescription desc)
    //    {
    //        PlexicParticle p = new PlexicParticle();

    //        return p;
    //    }
    //}

    //public class PlexicParticle
    //{

    //}

    //public class PlexicParticleDescription
    //{

    //}
}
