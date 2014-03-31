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
    public class ParticleDescription
    {
        public Mesh Mesh { get; set; }
        public ForwardEffect Effect { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Particle : IRigidBody
    {
        #region Constructor
        public Particle()
        {
            IsVisible = true;
        }
        #endregion

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
        public bool IsVisible { get; set; }
        #endregion          
    }
    
    ///// <summary>
    ///// 
    ///// </summary>
    //public abstract class ValueOverLife<T>
    //{
    //    private SortedList<float, T> _values = new SortedList<float, T>();

    //    protected abstract T Lerp(T v0, T v1, float percentage);

    //    protected abstract T Default { get; }

    //    public T GetValueAtAge(float age)
    //    {
    //        float? ageKey0 = GetActiveKeyForAge(age);
    //        float? ageKey1 = GetNextKey(age);
    //        T val0 = (ageKey0.HasValue) ? _values[ageKey0.Value] : this.Default;
    //        if (!ageKey1.HasValue)
    //            return val0;
    //        else
    //        {
    //            T val1 = _values[ageKey1.Value];
    //            return Lerp(val0, val1, age);
    //        }
    //    }

    //    public void SetValueAtAge(T value, float age)
    //    {
    //        if (_values.ContainsKey(age))
    //            _values[age] = value;
    //        else
    //            _values.Add(age, value);
    //    }

    //    public float? GetActiveKeyForAge(float age)
    //    {
    //        foreach (float key in _values.Keys)
    //            if (key <= age)
    //                return key;
    //        return null;
    //    }

    //    public float? GetNextKey(float key)
    //    {
    //        int i = _values.IndexOfKey(key);
    //        if (i == -1)
    //            throw new ArgumentException("key does not exist.", "key");
    //        else if (i == _values.Keys.Count - 1)
    //            return null;
    //        else
    //            return _values.Keys[i + 1];
    //    }

    //    public float? GetPreviousKey(float key)
    //    {
    //        int i = _values.IndexOfKey(key);
    //        if (i == -1)
    //            throw new ArgumentException("key does not exist.", "key");
    //        else if (i == 0)
    //            return null;
    //        else
    //            return _values.Keys[i - 1];
    //    }

    //    public void Clear()
    //    {
    //        _values.Clear();
    //    }

    //    public void SetScalar(T value)
    //    {
    //        this.Clear();
    //        this.SetValueAtAge(value, 0);
    //    }
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //public class FloatOverLife : ValueOverLife<float>
    //{
    //    public static FloatOverLife FromScalar(float value)
    //    {
    //        FloatOverLife fov = new FloatOverLife();
    //        fov.SetScalar(value);
    //        return fov;
    //    }

    //    protected override float Lerp(float v0, float v1, float percentage)
    //    {
    //        return v0 + (percentage * (v1 - v0));
    //    }

    //    protected override float Default
    //    {
    //        get { return 0.0f; }
    //    }
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //public class ColorOverLife : ValueOverLife<Color>
    //{
    //    public static ColorOverLife FromScalar(Color value)
    //    {
    //        ColorOverLife cov = new ColorOverLife();
    //        cov.SetScalar(value);
    //        return cov;
    //    }

    //    protected override Color Lerp(Color v0, Color v1, float percentage)
    //    {
    //        return Color.Lerp(v0, v1, percentage);
    //    }

    //    protected override Color Default
    //    {
    //        get { return Color.Transparent; }
    //    }
    //}
}
