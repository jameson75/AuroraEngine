using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using DXBuffer = SharpDX.Direct3D11.Buffer;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Effects;

namespace CipherPark.AngelJacket.Core.Kinetics
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ParticleSolver
    {
        public abstract void UpdateParticleTransform(ulong time, Particle p);
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParticleKeyframeSolver : ParticleSolver
    {       
        public AnimationLookup TargetAnimations { get; set; }
        public override void UpdateParticleTransform(ulong time, Particle p)
        {
            if (TargetAnimations != null)
            {
                p.Transform = TargetAnimations[p].GetValueAtT(time);
            }
        }
        public class AnimationLookup : Dictionary<Particle, TransformAnimation> { }
    }    
}
