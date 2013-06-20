using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
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
    /// <summary>
    /// 
    /// </summary>
    public class Emitter
    {
        private List<Particle> _particles = new List<Particle>();
        private List<ParticleLink> _links = new List<ParticleLink>();

        public Transform Transform { get; set; } 
        
        public Vector3 EmissionDirection { get; set; }
        
        public float EmissionRangePitch { get; set; }
        
        public float EmissionRangeYaw { get; set; }
        
        public EmissionDescription DefaultParticleDescription { get; set; }

        public ReadOnlyCollection<Particle> Particles { get { return _particles.AsReadOnly(); } }

        public ReadOnlyCollection<ParticleLink> Links { get { return _links.AsReadOnly(); } }

        public Particle Emit()
        {
            return Emit(DefaultParticleDescription);
        }

        public Particle Emit(EmissionDescription pd)
        {
            Particle p = Spawn(pd);
            _particles.Add(p);
            return p;
        }

        public void KillAll()
        {
            _particles.Clear();
            _links.Clear();
        }

        public void Kill(Particle p)
        {
            _particles.Remove(p);
            Unlink(p);
        }

        private static Particle Spawn(EmissionDescription description)
        {
            Particle p = new Particle();
            return p;
        }

        public void Link(Particle particle1, Particle particle2)
        {
            if(!_links.Exists( e=> (e.P1 == particle1 && e.P2 == particle2) || (e.P1 == particle2 && e.P2 == particle2)))
                _links.Add(new ParticleLink(particle1, particle2));
        }

        public void Unlink(Particle particle1, Particle particle2)
        {
            _links.RemoveAll(e => (e.P1 == particle1 && e.P2 == particle2) || (e.P1 == particle2 && e.P2 == particle2));
        }

        public void Unlink(Particle p)
        {
            _links.RemoveAll(e => e.P1 == p || e.P2 == p);
        }
    } 

    /// <summary>
    /// 
    /// </summary>
    public class ParticleLink
    {
        public ParticleLink()
        { }
        public ParticleLink(Particle p1, Particle p2)
        {
            P1 = p1;
            P2 = p2;
        }
        public Particle P1 { get; set; }
        public Particle P2 { get; set; }
    }    

    /// <summary>
    /// 
    /// </summary>
    public class EmissionDescription
    {
        public int BirthRate { get; set; }
        public int BirthRateRandomness { get; set; }
        public int Life { get; set; }
        public int LifeRandomness { get; set; }
        public float Speed { get; set; }
        public int SpeedRandomness { get; set; }
        public Color? Color { get; set; }
        public Dictionary<float, Color> ColorOverTime { get; set; }
        public Dictionary<float, float> OpacityOverLife { get; set; }
        public float Scale { get; set; }
        public float ScaleRandomness { get; set; }
        public float PointSize { get; set; }
        public int RandomSeed { get; set; }
        public Texture2D Texture { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Particle : ITransformable
    {
        #region ITransformable Members

        public Transform Transform
        {
            get;
            set;
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class Emission
    {
        public ulong Time { get; set; }        
        public EmissionDescription EmissionDescription { get; set; }
        public Particle Particle1 { get; set; }
        public Particle Particle2 { get; set; }
        [Flags]
        public enum EmissionTask
        {
            Emit =          0x01,
            EmitParticle =   0x02,
            Kill =          0x04,
            KillAll =       0x08,
            Link =          0x10
        }
        public EmissionTask Tasks { get; set; }
    }
}
    
