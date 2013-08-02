using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.Kinetics;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World
{
    public static class Weapon 
    {
        public static RigidBodyAnimationController CreateDischargeAnimation(IRigidBody projectile)
        {
            //Create path...
            Vector3[] path = new Vector3[]
            { 
                new Vector3(200, 20, 0),
                new Vector3(200, 20, 200),
                new Vector3(200, 20, 400),
                new Vector3(0, 20, 400)
            };
            Motion motion = new Motion();
            motion.LinearPath = path;
            motion.LinearVelocity = 200.0f;
            RigidBodyAnimationController animationPathController = new RigidBodyAnimationController(motion, projectile);
            //IAnimationController projectileAnimationController = projectile.Animation;
            //return new CompositeAnimationController(new IAnimationController[] { animationPathController, /*projectileAnimationController*/ });
            return animationPathController;
        }
    }

    public class Projectile : ComplexModel, IRigidBody
    {        
        #region IRigidBody 
        public Vector3 CenterOfMass { get; set; }
        #endregion

        public Projectile(IGameApp game)
            : base(game)
        { }
    }
}
