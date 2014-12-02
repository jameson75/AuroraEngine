using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Animation.Controllers;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.Kinetics;
using CipherPark.AngelJacket.Core.Animation.Controllers;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World
{
    public abstract class Weapon : IRigidBody
    {
        public Transform Mount { get; set; }
        public abstract void Fire();
    }

    public class Gun : Weapon
    {
        public override void Fire()
        {
            
        }
    }

    public class MissleLauncher : Weapon
    {
        public override void Fire()
        {
            
        }
    }

    public class Projectile : IRigidBody
    {        
        public Model Model { get; set; }

        #region IRigidBody 
        public Vector3 CenterOfMass { get; set; }
        #endregion

        public Projectile(IGameApp game)           
        { }

        public Transform Transform { get; set; }

        public ITransformable TransformableParent { get; set; }
    }
}
