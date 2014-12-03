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
using CipherPark.AngelJacket.Core.Services;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World
{
    public interface IWeapon
    {
        void Fire(); 
    }

    public abstract class BasicWeapon : BasicWorldObject, IWeapon
    {
        public BasicWeapon(IGameApp game)
            : base(game)
        { }

        public abstract void Fire();               
    }
 
    public class Gun : BasicWeapon
    {
        public Gun(IGameApp game)
            : base(game)
        { }

        public override void Fire()
        {
            
        }   
    }

    public class MissleLauncher : BasicWeapon
    {
        public MissleLauncher(IGameApp game)
            : base(game)
        { }

        public override void Fire()
        {
            
        }
    }

    public class Projectile : BasicWorldObject
    {
        public Projectile(IGameApp game)
            : base(game)
        { }
    }
}
