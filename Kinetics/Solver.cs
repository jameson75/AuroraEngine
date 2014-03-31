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
        public abstract void Step(ulong time, ParticleSystem system);
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParticleKeyframeSolver : ParticleSolver
    {       
        public AnimationLookup TargetAnimations { get; set; }
        
        public override void Step(ulong time, ParticleSystem system)
        {
            if (TargetAnimations != null)
            {
                List<Particle> expiredTargets = new List<Particle>();
                foreach (Particle p in TargetAnimations.Keys)
                {
                    if (time <= TargetAnimations[p].RunningTime)
                        p.Transform = TargetAnimations[p].GetValueAtT(time);
                    else
                    {
                        expiredTargets.Add(p);
                        OnTargetAnimationComplete(p);
                    }
                }
                foreach (Particle p in expiredTargets)
                    TargetAnimations.Remove(p);
            }
        }

        protected virtual void OnTargetAnimationComplete(Particle p)
        { }

        public class AnimationLookup : Dictionary<Particle, TransformAnimation> { }
    } 
}
