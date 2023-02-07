using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Animation
{
    /// <summary>
    /// 
    /// </summary>
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    public struct Transform
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    {
        private static Transform _zero = new Transform { Rotation = Quaternion.Zero, Translation = Vector3.Zero, Scale = 1f };
        private static Transform _identity = new Transform { Rotation = Quaternion.Identity, Translation = Vector3.Zero, Scale = 1f };

        public float Scale { get; set; }
        
        public Quaternion Rotation { get; set; }

        public Vector3 Translation { get; set; }

        public Matrix ToMatrix() { return Matrix.AffineTransformation(Scale, Rotation, Translation); }

        public Transform(Matrix m)
            : this()
        {
            /* https://erkaman.github.io/posts/model_matrix_recover.html */

            Scale = m.Row1.Length(); //Assumed that all scaling is uniform.
            Translation = m.TranslationVector;
            var mR = Matrix.Invert(Matrix.Scaling(Scale)) * m;
            Rotation = Quaternion.RotationMatrix(mR);
        }

        public Transform(Quaternion rotation, Vector3 translation, float scale)
        {
            Rotation = rotation;
            Translation = translation;
            Scale = scale;
        }

        public Transform(Quaternion rotation)
        {
            Rotation = rotation;
            Translation = Vector3.Zero;
            Scale = 1;
        }
        
        public Transform(Vector3 translation)
            : this()
        {
            Rotation = Quaternion.Identity;
            Translation = translation;
            Scale = 1;
        }
        
        public Transform(float scale)
        {
            Rotation = Quaternion.Identity;
            Translation = Vector3.Zero;
            Scale = scale;
        }
        
        public static Transform Zero { get { return _zero; } }

        public static Transform Identity { get { return _identity; } }

        public static Transform Multiply(Transform t1, Transform t2)
        {
            return new Transform(t1.ToMatrix() * t2.ToMatrix());
        }

        public static Transform Invert(Transform t)
        {
            return new Transform(Matrix.Invert(t.ToMatrix()));
        }

        public static Transform Lerp(Transform t1, Transform t2, float step)
        {
            return new Transform(Quaternion.Lerp(t1.Rotation, t2.Rotation, step),
                                 Vector3.Lerp(t1.Translation, t2.Translation, step),
                                 MathUtil.Lerp(t1.Scale, t2.Scale, step));
        }

        public static Transform operator * (Transform t, Matrix m)
        {
            return new Transform(t.ToMatrix() * m);
        }

        public static Transform operator * (Matrix m, Transform t)
        {
            return new Transform(m * t.ToMatrix());
        }

        public static Transform operator * (Transform t1, Transform t2)
        {
            return new Transform(t1.ToMatrix() * t2.ToMatrix());
        }

        public static Transform operator + (Transform t, Vector3 v)
        {
            return new Transform(t.Rotation, t.Translation + v, t.Scale);
        }

        public static Transform operator + (Vector3 v, Transform t)
        {
            return new Transform(t.Rotation, v + t.Translation, t.Scale);
        }

        public static Transform operator - (Transform t, Vector3 v)
        {
            return new Transform(t.Rotation, t.Translation - v, t.Scale);
        }

        public static Transform operator - (Vector3 v, Transform t)
        {
            return new Transform(t.Rotation, v - t.Translation, t.Scale);
        }

        public static bool operator == (Transform t1, Transform t2)
        {
            return t1.Rotation == t2.Rotation && t1.Translation == t2.Translation & t1.Scale == t2.Scale;
        }

        public static bool operator !=(Transform t1, Transform t2)
        {
            return t1.Rotation != t2.Rotation || t1.Translation != t2.Translation || t1.Scale != t2.Scale;
        }
    }
}
