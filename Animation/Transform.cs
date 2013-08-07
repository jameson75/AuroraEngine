using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Animation
{
    /// <summary>
    /// 
    /// </summary>
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

        public static Transform Multiply(Transform t1, Transform t2)
        {
            return new Transform(t1.ToMatrix() * t2.ToMatrix());
        }

        public static Transform Invert(Transform t)
        {
            return new Transform(Matrix.Invert(t.ToMatrix()));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ITransformable
    {
        Transform Transform { get; set; }
        ITransformable TransformableParent { get; set; }      
    }

    /// <summary>
    /// 
    /// </summary>
    public static class TransformableExtension
    {
        public static Matrix LocalToWorld(this ITransformable transformable, Matrix localTransform)
        {
            MatrixStack stack = new MatrixStack();
            stack.Push(localTransform);
            ITransformable node = transformable.TransformableParent;
            while (node != null)
            {
                stack.Push(node.Transform.ToMatrix());
                node = node.TransformableParent;
            }
            return stack.Transform;
        }

        public static Matrix WorldToLocal(this ITransformable transformable, Matrix worldTransform)
        {
            MatrixStack stack = new MatrixStack();
            stack.Push(worldTransform);
            ITransformable node = transformable.TransformableParent;
            while (node != null)
            {
                stack.Push(Matrix.Invert(node.Transform.ToMatrix()));
                node = node.TransformableParent;
            }
            return stack.ReverseTransform;
        }

        public static Transform LocalToWorld(this ITransformable transformable, Transform localTransform)
        {
            return new Transform(transformable.LocalToWorld(localTransform.ToMatrix()));
        }

        public static Transform WorldToLocal(this ITransformable transformable, Transform worldTransform)
        {
            return new Transform(transformable.WorldToLocal(worldTransform.ToMatrix()));
        }
    }
}
