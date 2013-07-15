﻿using System;
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
        private List<Particle> _particles = new List<Particle>();
        private List<ParticleLink> _links = new List<ParticleLink>();
        private Mesh _dynamicMesh = null;

        #region ITransformable Members
        public Transform Transform { get; set; }
        #endregion    

        public Vector3 EmissionDirection { get; set; }
        
        public float EmissionRangePitch { get; set; }
        
        public float EmissionRangeYaw { get; set; }
        
        public EmitterBehavior Behavior { get; set; }

        public ParticleDescription DefaultParticleDescription { get; set; }

        public ReadOnlyCollection<Particle> Particles { get { return _particles.AsReadOnly(); } }

        public ReadOnlyCollection<ParticleLink> Links { get { return _links.AsReadOnly(); } }

        public Emitter(IGameApp game, Effect effect)
        {              
            _dynamicMesh = CreateDynamicMesh(game, effect);
        }

        private static Mesh CreateDynamicMesh(IGameApp game, Effect effect)
        {
            //Create indices, texcoords and verts for quad singularity...

            short[] indices = {0, 1, 2, 2, 3, 0};
            Vector3[] points = ContentBuilder.CreateQuadPoints(Rectangle.Empty);
            Vector2[] texCoords = ContentBuilder.CreateQuadTextureCoords();
            BasicVertexDualTextureIPosition[] verts = texCoords.Select(t => new BasicVertexDualTextureIPosition(t, Vector2.Zero)).ToArray();
            
            //Describe a dynamic mesh with an index buffer.
            MeshDescription meshDesc = new MeshDescription();
            BufferDescription vertexBufferDesc = new BufferDescription();
            vertexBufferDesc.BindFlags = BindFlags.VertexBuffer;
            vertexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            vertexBufferDesc.SizeInBytes = verts.Length * BasicVertexDualTextureIPosition.VertexElementSize;
            vertexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            vertexBufferDesc.StructureByteStride = 0;
            DXBuffer vBuffer = DXBuffer.Create<BasicVertexDualTextureIPosition>(game.GraphicsDevice, verts, vertexBufferDesc);
            meshDesc.VertexBuffer = vBuffer;
            meshDesc.VertexCount = verts.Length;
            meshDesc.VertexLayout = new InputLayout(game.GraphicsDevice, effect.SelectShaderByteCode(), BasicVertexDualTextureIPosition.InputElements);
            meshDesc.VertexStride = BasicVertexDualTextureIPosition.VertexElementSize;
            BufferDescription indexBufferDesc = new BufferDescription();
            indexBufferDesc.BindFlags = BindFlags.IndexBuffer;
            indexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            indexBufferDesc.SizeInBytes = indices.Length * sizeof(short);
            indexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            DXBuffer iBuffer = DXBuffer.Create<short>(game.GraphicsDevice, indices, indexBufferDesc);
            meshDesc.IndexCount = indices.Length;
            meshDesc.IndexBuffer = iBuffer;
            meshDesc.Topology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            return new Mesh(game, meshDesc);
        }

        public List<Particle> Emit()
        {
            return Emit(DefaultParticleDescription);
        }

        public List<Particle> Emit(ParticleDescription customParticleDescription)
        {
            List<Particle> pList = Spawn(customParticleDescription);
            _particles.AddRange(pList);
            return pList;
        }

        public List<Particle> Emit(IEnumerable<Particle> particles)
        {
            _particles.AddRange(particles);
            return particles.ToList();
        }

        public List<Particle> CreateParticles(int count, ParticleDescription customParticleDescription = null)
        {
            return Spawn(customParticleDescription != null ? customParticleDescription : DefaultParticleDescription);
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

        public void Kill(IEnumerable<Particle> pList)
        {
            foreach (Particle p in pList)
                Kill(p);
        }

        private List<Particle> Spawn(ParticleDescription description)
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
                p.Color = description.ColorOverLife.GetValueAtAge(0);
                p.Opacity = description.OpacityOverLife.GetValueAtAge(0);
                p.Age = 0;
                p.Velocity = randomVelocity;
                p.Transform = this.Transform;
                p.Texture = description.Texture;
                p.Effect = description.Effect;
                pList.Add(p);
            }
            return pList;
        }

        public void Link(Particle particle1, Particle particle2)
        {
            if(!_links.Exists( e=> (e.P1 == particle1 && e.P2 == particle2) || (e.P1 == particle2 && e.P2 == particle2)))
                _links.Add(new ParticleLink(particle1, particle2));
        }

        public void Link(IEnumerable<Particle> particles)
        {         
            Particle[] pArray = particles.ToArray();
            for(int i = 0; i < pArray.Length; i+=2)            
                Link(pArray[i], pArray[i + 1]);
        }

        public void Unlink(Particle particle1, Particle particle2)
        {
            _links.RemoveAll(e => (e.P1 == particle1 && e.P2 == particle2) || (e.P1 == particle2 && e.P2 == particle2));
        }

        public void Unlink(Particle p)
        {
            _links.RemoveAll(e => e.P1 == p || e.P2 == p);
        }

        public void Draw(long gameTime)
        {
            foreach(Particle p in _particles )
            {
                p.Effect.World = this.Transform.ToMatrix() * p.Transform.ToMatrix(); //DESIGN FLAW (Use consistent method to aquire this object's world transform).
                p.Effect.View; //DESIGN FLAW (Access game services to get the current camera/view).
                p.Effect.Projection; //DESIGN FLAW. (Access game services to get the current camera/projection).
                if (p.Effect is IParticleEffect)
                {
                    ((IParticleEffect)p.Effect).Color = p.Color;
                    ((IParticleEffect)p.Effect).Opacity = p.Opacity;
                    ((IParticleEffect)p.Effect).Size = p.Size;
                    ((IParticleEffect)p.Effect).SetTexture(p.Texture);
                }                
            }            
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
        //public ColorOverLife ColorOverLife { get; set; }
        //public FloatOverLife OpacityOverLife { get; set; }
        //public float Scale { get; set; }
        //public float ScaleRandomness { get; set; }
        //public float PointSize { get; set; }
        //public int RandomSeed { get; set; }
        //public Texture2D Texture { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParticleDescription
    {
        public ColorOverLife ColorOverLife { get; set; }
        public FloatOverLife OpacityOverLife { get; set; }        
        public Texture2D Texture { get; set; }
        public Effect Effect { get; set; }
        //public Model CustomModel { get; set; }
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

        #region IRigidBody Members
        public Vector3 CenterOfMass { get; set; }
        #endregion
        
        #region Particle State
        public ulong Age { get; set; }
        public ulong Life { get; set; }
        public float Velocity { get; set; }
        public Color Color { get; set; }
        public float Opacity { get; set; }
        public Vector2 Size { get; set; }
        #endregion    

        #region Shared Attributes
        public Texture2D Texture { get; set; }
        public Effects.Effect Effect { get; set; }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class EmitterAction
    {
        public EmitterAction() { }
        public EmitterAction(ulong time, EmitterTask task) { Time = time; Task = task; }
        public EmitterAction(ulong time, ParticleDescription customParticleDescription, EmitterTask task = EmitterTask.EmitCustom) { Time = time; CustomParticleDescriptionArg = customParticleDescription; Task = task; }
        public EmitterAction(ulong time, IEnumerable<Particle> particleArgs, EmitterTask task) { Time = time; ParticleArgs = particleArgs; Task = task; }
        public EmitterAction(ulong time, Transform transform, EmitterTask task = EmitterTask.Transform) { Time = time; Transform = transform; Task = task; }
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

        public void SetConstant(T value)
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
        public class AnimationLookup : Dictionary<Particle, TransformAnimation> { }
        public AnimationLookup TargetAnimations { get; set; }

        public override void UpdateParticleTransform(ulong time, Particle p)
        {
            if (TargetAnimations != null)
            {
                p.Transform = TargetAnimations[p].GetValueAtT(time);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IParticleEffect
    {
        public Color Color { get; set; }
        public Vector2 Size { get; set; }
        public float Opacity { get; set; }
    }
}
    
