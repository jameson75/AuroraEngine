using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;

namespace CipherPark.AngelJacket.Core.Animation
{
    public struct Transform
    {
        private static Transform _zero = new Transform { Rotation = Quaternion.Zero, Translation = Vector3.Zero };
        private static Transform _identity = new Transform { Rotation = Quaternion.Identity, Translation = Vector3.Zero };
        public Quaternion Rotation { get; set; }
        public Vector3 Translation { get; set; }
        public Matrix ToMatrix() { return Matrix.AffineTransformation(1.0f, Rotation, Translation); }
        public Transform(Matrix m)
            : this()
        {
            Rotation = Quaternion.RotationMatrix(m);
            Translation = m.TranslationVector;
        }
        public Transform(Vector3 translation)
            : this()
        {
            Rotation = Quaternion.Identity;
            Translation = translation;
        }
        public Transform(Quaternion rotation, Vector3? translation)
            : this()
        {
            Rotation = rotation;
            if (translation != null)
                Translation = translation.Value;
        }
        public static Transform Zero { get { return _zero; } }
        public static Transform Identity { get { return _identity; } }
    }
    
    public interface ITransformable
    {
        Transform Transform { get; set; }
    }
}
