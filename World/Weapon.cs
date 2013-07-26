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
using CipherPark.AngelJacket.Core.World.Geometry;

namespace CipherPark.AngelJacket.Core.World
{
    public abstract class Projectile : ITransformable 
    {    
        public Transform Transform { get; set; }
        ITransformable ITransformable.TransformableParent { get; set; }
    }

    public class Weapon 
    {
        private List<Emitter> _emitters = new List<Emitter>();
        private Frames _frames = new Frames();

        public List<Emitter> Emitters { get { return _emitters; } }

        public Frames Frames { get { return _frames; } }

        public MasterController CreateDischargeAnimation()
        {
            SimpleEmittingProjectile projectile = new SimpleEmittingProjectile();
            
            return null;
        }
        
        public void Draw(long gameTime)
        { }
    }

    public class SimpleEmittingProjectile
    {
        private Emitter _emitter = new Emitter();
    }
}
