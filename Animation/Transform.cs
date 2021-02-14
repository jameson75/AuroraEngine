using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.KillScript.Core.Module;
using CipherPark.KillScript.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.Animation
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

        public Transform(Quaternion rotation, Vector3? translation = null)
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

        public static Transform Lerp(Transform t1, Transform t2, float step)
        {
            return new Transform(Quaternion.Lerp(t1.Rotation, t2.Rotation, step),
                                 Vector3.Lerp(t1.Translation, t2.Translation, step));
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
        public static Vector3 ParentToWorldCoordinate(this ITransformable transformable, Vector3 postion)
        {
            if (transformable.TransformableParent != null)
                return transformable.TransformableParent.LocalToWorldCoordinate(postion);
            else
                return postion;
        }

        public static Vector3 WorldToParentCoordinate(this ITransformable transformable, Vector3 position)
        {
            if (transformable.TransformableParent != null)
                return transformable.TransformableParent.WorldToLocalCoordinate(position);
            else
                return position;
        }

        public static Vector3 LocalToWorldCoordinate(this ITransformable transformable, Vector3 position)
        {
            MatrixStack stack = new MatrixStack();
            stack.Push(transformable.Transform.ToMatrix());
            ITransformable node = transformable.TransformableParent;
            while (node != null)
            {
                stack.Push(node.Transform.ToMatrix());
                node = node.TransformableParent;
            }
            return Vector3.TransformCoordinate(position, stack.Transform);
        }

        public static Vector3 WorldToLocalCoordinate(this ITransformable transformable, Vector3 position)
        {
            MatrixStack stack = new MatrixStack();
            stack.Push(Matrix.Invert(transformable.Transform.ToMatrix()));
            ITransformable node = transformable.TransformableParent;
            while (node != null)
            {
                stack.Push(Matrix.Invert(node.Transform.ToMatrix()));
                node = node.TransformableParent;
            }
            return Vector3.TransformCoordinate(position, stack.ReverseTransform);
        }

        public static Vector3 ParentToWorldNormal(this ITransformable transformable, Vector3 normal)
        {
            if (transformable.TransformableParent != null)
                return transformable.TransformableParent.LocalToWorldNormal(normal);
            else
                return normal;
        }

        public static Vector3 WorldToParentNormal(this ITransformable transformable, Vector3 normal)
        {
            if (transformable.TransformableParent != null)
                return transformable.TransformableParent.WorldToLocalNormal(normal);
            else
                return normal;
        }

        public static Vector3 LocalToWorldNormal(this ITransformable transformable, Vector3 normal)
        {
            MatrixStack stack = new MatrixStack();
            stack.Push(transformable.Transform.ToMatrix());
            ITransformable node = transformable.TransformableParent;
            while (node != null)
            {
                stack.Push(node.Transform.ToMatrix());
                node = node.TransformableParent;
            }
            return Vector3.TransformNormal(normal, stack.Transform);
        }

        public static Vector3 WorldToLocalNormal(this ITransformable transformable, Vector3 normal)
        {
            MatrixStack stack = new MatrixStack();
            stack.Push(Matrix.Invert(transformable.Transform.ToMatrix()));
            ITransformable node = transformable.TransformableParent;
            while (node != null)
            {
                stack.Push(Matrix.Invert(node.Transform.ToMatrix()));
                node = node.TransformableParent;
            }
            return Vector3.TransformNormal(normal, stack.ReverseTransform);
        }

        public static Matrix LocalToWorld(this ITransformable transformable, Matrix localTransform)
        {
            MatrixStack stack = new MatrixStack();
            stack.Push(transformable.Transform.ToMatrix());
            ITransformable node = transformable.TransformableParent;
            while (node != null)
            {
                stack.Push(node.Transform.ToMatrix());
                node = node.TransformableParent;
            }
            return localTransform * stack.Transform;
        }

        public static Matrix WorldToLocal(this ITransformable transformable, Matrix worldTransform)
        {
            MatrixStack stack = new MatrixStack();
            stack.Push(Matrix.Invert(transformable.Transform.ToMatrix()));
            ITransformable node = transformable.TransformableParent;
            while (node != null)
            {
                stack.Push(Matrix.Invert(node.Transform.ToMatrix()));
                node = node.TransformableParent;
            }
            return worldTransform * stack.ReverseTransform;
        }

        public static Matrix ParentToWorld(this ITransformable transformable, Matrix localTransform)
        {
            if (transformable.TransformableParent != null)
                return transformable.TransformableParent.LocalToWorld(localTransform);
            else
                return localTransform;
        }

        public static Matrix WorldToParent(this ITransformable transformable, Matrix worldTransform)
        {
            if (transformable.TransformableParent != null)
                return transformable.TransformableParent.WorldToLocal(worldTransform);
            else
                return worldTransform;            
        }

        public static Transform LocalToWorld(this ITransformable transformable, Transform localTransform)
        {
            return new Transform(transformable.LocalToWorld(localTransform.ToMatrix()));
        }

        public static Transform WorldToLocal(this ITransformable transformable, Transform worldTransform)
        {
            return new Transform(transformable.WorldToLocal(worldTransform.ToMatrix()));
        }

        public static Transform ParentToWorld(this ITransformable transformable, Transform localTransform)
        {
            return new Transform(transformable.ParentToWorld(localTransform.ToMatrix()));
        }

        public static Transform WorldToParent(this ITransformable transformable, Transform worldTransform)
        {
            return new Transform(transformable.WorldToParent(worldTransform.ToMatrix()));
        }

        public static Transform WorldTransform(this ITransformable transformable)
        {
            return new Transform(transformable.ParentToWorld(transformable.Transform.ToMatrix()));
        }

        public static void Translate(this ITransformable transformable, Vector3 offset)
        {
            transformable.Transform = new Transform(transformable.Transform.Rotation, transformable.Transform.Translation + offset);
        }

        public static Vector3 WorldPosition (this ITransformable transformable)
        {
            return transformable.WorldTransform().Translation;
        }
    }
}
