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

namespace CipherPark.AngelJacket.Core.Kinetics
{
    /// <summary>
    /// 
    /// </summary>
    public class Emitter : IRigidBody
    {
        private List<Particle> _particles = new List<Particle>();
        private List<ParticleLink> _links = new List<ParticleLink>();

        #region ITransformable Members
        public Transform Transform { get; set; }
        #endregion
        #region IRigidBody Members
        public Vector3 CenterOfMass { get; set; }
        #endregion

        public Vector3 EmissionDirection { get; set; }
        
        public float EmissionRangePitch { get; set; }
        
        public float EmissionRangeYaw { get; set; }
        
        public EmitterBehavior Behavior { get; set; }

        public ParticleDescription DefaultParticleDescription { get; set; }

        public ReadOnlyCollection<Particle> Particles { get { return _particles.AsReadOnly(); } }

        public ReadOnlyCollection<ParticleLink> Links { get { return _links.AsReadOnly(); } }

        public List<Particle> Emit()
        {
            return Emit(DefaultParticleDescription);
        }

        public List<Particle> Emit(ParticleDescription pd)
        {
            List<Particle> pList = Spawn(pd);
            _particles.AddRange(pList);
            return pList;
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

        private List<Particle> Spawn(ParticleDescription description)
        {
            List<Particle> pList = new List<Particle>();
            for (int i = 0; i < Behavior.BirthRate; i++)
            {
                Particle p = new Particle();
                p.Life = Behavior.Life;
                p.Color = description.ColorOverLife.GetValueAtAge(0);
                p.Opacity = description.OpacityOverLife.GetValueAtAge(0);
                p.Age = 0;
                p.Velocity = Behavior.InitialVelocity;
                p.Transform = this.Transform;
                pList.Add(p);
            }
            return pList;
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
    public class EmitterBehavior
    {
        public int BirthRate { get; set; }
        public int BirthRateRandomness { get; set; }
        public ulong Life { get; set; }
        public int LifeRandomness { get; set; }
        public float InitialVelocity { get; set; }
        public int InitialVelocityRandomness { get; set; }
        public ColorOverLife ColorOverLife { get; set; }
        public FloatOverLife OpacityOverLife { get; set; }
        //public float Scale { get; set; }
        //public float ScaleRandomness { get; set; }
        //public float PointSize { get; set; }
        //public int RandomSeed { get; set; }
        public Texture2D Texture { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParticleDescription
    {
        public ColorOverLife ColorOverLife { get; set; }
        public FloatOverLife OpacityOverLife { get; set; }       
        public Texture2D Texture { get; set; }
        public Model CustomModel { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Particle : IRigidBody
    {
        private ColorOverLife _colorOverLife = new ColorOverLife();
        private FloatOverLife _opacityOverLife = new FloatOverLife();

        #region ITransformable Members
        public Transform Transform { get; set; }
        #endregion
        #region Particle State
        public ulong Age { get; set; }
        public ulong Life { get; set; }
        public float Velocity { get; set; }
        public Color Color { get; set; }
        public float Opacity { get; set; }
        #endregion
        #region Shared Attributes
        public ParticleDescription SharedDescription { get; set; }
        #endregion
        #region IRigidBody Members
        public Vector3 CenterOfMass { get; set; }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class Emission
    {
        public ulong Time { get; set; }        
        public ParticleDescription CustomParticleDescription { get; set; }
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

    /// <summary>
    /// 
    /// </summary>
    public abstract class ValueOverLife<T>
    {
        private SortedList<float, T> _values = new SortedList<float, T>();

        protected abstract T Lerp(T v0, T v1, float percentage);

        protected abstract T Default { get; }

        public T GetValueAtAge(float age)
        {
            float? ageKey0 = GetActiveKeyForAge(age);
            float? ageKey1 = GetNextKey(age);
            T val0 = (ageKey0.HasValue) ? _values[ageKey0.Value] : this.Default;
            if (!ageKey1.HasValue)
                return val0;
            else
            {                
                T val1 = _values[ageKey1.Value];
                return Lerp(val0, val1, age);
            }
        }

        public void SetValueAtAge(T value, float age)
        {
            if(_values.ContainsKey(age))
                _values[age] = value;
            else 
                _values.Add(age, value);
        }

        public float? GetActiveKeyForAge(float age)
        {
            foreach (float key in _values.Keys)
                if (key <= age)
                    return key;
            return null;
        }

        public float? GetNextKey(float key)
        {
            int i = _values.IndexOfKey(key);
            if (i == -1)
                throw new ArgumentException("key does not exist.", "key");
            else if (i == _values.Keys.Count - 1)
                return null;
            else
                return _values.Keys[i + 1];
        }

        public float? GetPreviousKey(float key)
        {
            int i = _values.IndexOfKey(key);
            if (i == -1)
                throw new ArgumentException("key does not exist.", "key");
            else if (i == 0)
                return null;
            else
                return _values.Keys[i - 1];
        }
    }   

    /// <summary>
    /// 
    /// </summary>
    public class FloatOverLife : ValueOverLife<float>
    {
        protected override float Lerp(float v0, float v1, float percentage)
        {
            return v0 + (percentage * (v1 - v0));
        }

        protected override float Default
        {
            get { return 0.0f; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ColorOverLife : ValueOverLife<Color>
    {

        protected override Color Lerp(Color v0, Color v1, float percentage)
        {
            return Color.Lerp(v0, v1, percentage);
        }

        protected override Color Default
        {
            get { return Color.Transparent; }
        }
    }

    public abstract class ParticleSolver
    {
        public abstract void UpdateParticleTransform(ulong time, Particle p);
    }

    public class ParticleKeyframeSolver : ParticleSolver
    {
        public TransformAnimation Animation { get; set; }

        public override void UpdateParticleTransform(ulong time, Particle p)
        {
            if (Animation != null)
                p.Transform = Animation.GetValueAtT(time);
        }
    }
}
    
