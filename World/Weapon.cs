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

namespace CipherPark.AngelJacket.Core.World
{
    public class Projectile : ITransformable 
    {        
        public Transform Transform { get; set; }
    }

    public class Weapon 
    {
        public Emitter Emitter { get; set; }
        
        public abstract KeyframeAnimationController CreateDischargeAnimation(Vector3 location, Vector3 direction, float velocity)
        
        public void Draw(gameTime)
        { }
    }
}
