using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using SharpDX;
using SharpDX.Direct3D11;
using DXBuffer = SharpDX.Direct3D11.Buffer;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Effects;

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
    public class Emitter : ITransformable
    {      
        #region ITransformable Members
        public Transform Transform { get; set; }
        public ITransformable TransformableParent { get; set; }
        #endregion    

        public Vector3 EmissionDirection { get; set; }
        
        public float EmissionRangePitch { get; set; }
        
        public float EmissionRangeYaw { get; set; }
        
        public EmitterBehavior Behavior { get; set; }

        public ParticleDescription DefaultParticleDescription { get; set; }                     

        public List<Particle> CreateParticles(int count)
        {
            List<Particle> pList = new List<Particle>();
            for (int i = 0; i < count; i++)
            {              
                Particle p = new Particle();
                p.Life = 0;
                p.Age = 0;
                p.Velocity = 0;
                p.Transform = this.Transform;
                p.Description = null;
                pList.Add(p);
            }
            return pList;           
        }       

        public List<Particle> Spawn(ParticleDescription description)
        {
            Random randomGen = new Random();
            List<Particle> pList = new List<Particle>();
            int randomBirthCount = (Behavior.BirthRateRandomness == 0) ? Behavior.BirthRate : 
                randomGen.Next(Behavior.BirthRate, Behavior.BirthRateRandomness);                
            for (int i = 0; i < randomBirthCount; i++)
            {
                ulong randomLife = (Behavior.LifeRandomness == 0) ? Behavior.Life :
                    Behavior.Life + (ulong)randomGen.Next(Behavior.LifeRandomness);
                float randomVelocity = (Behavior.InitialVelocityRandomness == 0) ? Behavior.InitialVelocity :
                    Behavior.InitialVelocity + (float)randomGen.Next(Behavior.InitialVelocityRandomness);               
                Particle p = new Particle();
                p.Life = randomLife;              
                p.Age = 0;
                p.Velocity = randomVelocity;
                p.Transform = this.Transform;
                p.Description = description;                
                pList.Add(p);
            }
            return pList;
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
        //public float Scale { get; set; }
        //public float ScaleRandomness { get; set; }
        //public float PointSize { get; set; }
        //public int RandomSeed { get; set; }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class ParticleDescription
    {        
        public Mesh Mesh;
        public Effect Effect;         
    }

    /// <summary>
    /// 
    /// </summary>
    public class Particle : IRigidBody
    {
        #region ITransformable Members
        public Transform Transform { get; set; }
        public ITransformable TransformableParent { get; set; }
        #endregion

        #region IRigidBody Members
        public Vector3 CenterOfMass { get; set; }
        public float Velocity { get; set; }
        #endregion
        
        #region Particle Attributes
        public ulong Age { get; set; }
        public ulong Life { get; set; }
        public ParticleDescription Description { get; set; }
        #endregion          
    }

    /// <summary>
    /// 
    /// </summary>
    public class EmitterAction
    {
        public EmitterAction() { }
        public EmitterAction(ulong time, EmitterTask task) { Time = time; Task = task; }
        public EmitterAction(ulong time, ParticleDescription customParticleDescription) { Time = time; CustomParticleDescriptionArg = customParticleDescription; Task = EmitterTask.EmitCustom; }
        public EmitterAction(ulong time, IEnumerable<Particle> particleArgs, EmitterTask task) { Time = time; ParticleArgs = particleArgs; Task = task; }
        public EmitterAction(ulong time, Transform transform) { Time = time; Transform = transform; Task = EmitterTask.Transform; }
        public ulong Time { get; set; }
        public ParticleDescription CustomParticleDescriptionArg { get; set; }
        public IEnumerable<Particle> ParticleArgs { get; set; }
        public Transform Transform { get; set; }
        public enum EmitterTask
        {
            Emit,
            EmitCustom,
            EmitExplicit,
            Kill,
            KillAll,
            Link,
            Transform
        }
        public EmitterTask Task { get; set; }
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

        public void Clear()
        {
            _values.Clear();
        }

        public void SetScalar(T value)
        {
            this.Clear();
            this.SetValueAtAge(value, 0);
        }
    }   

    /// <summary>
    /// 
    /// </summary>
    public class FloatOverLife : ValueOverLife<float>
    {
        public static FloatOverLife FromScalar(float value)
        {
            FloatOverLife fov = new FloatOverLife();
            fov.SetScalar(value);
            return fov;
        }

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
        public static ColorOverLife FromScalar(Color value)
        {
            ColorOverLife cov = new ColorOverLife();
            cov.SetScalar(value);
            return cov;
        }

        protected override Color Lerp(Color v0, Color v1, float percentage)
        {
            return Color.Lerp(v0, v1, percentage);
        }

        protected override Color Default
        {
            get { return Color.Transparent; }
        }
    }

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
    
