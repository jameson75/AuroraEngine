using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.Aurora.Core;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Collision
{
    public interface ICollidable : ITransformable
    {
        ObservableCollection<Collider> Colliders { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class Collider : ITransformable
    {
        private ColliderCollection _children = null;

        public ColliderCollection Children { get { return _children; } }       

        public Transform Transform { get; set; }

        public ITransformable TransformableParent { get; set; }   

        public Collider()
        {
            _children = new ColliderCollection();
            _children.CollectionChanged += Children_CollectionChanged;
        }

        /// <summary>
        /// Calculates the transform of this collider object before prior to displacement.
        /// </summary>
        /// <param name="displacementVector"></param>
        /// <returns></returns>
        public Transform PreviousWorldTransform(Vector3 displacementVector)
        {
            Transform worldTransform = this.WorldTransform();
            return new Transform(worldTransform.Rotation, worldTransform.Translation - displacementVector, worldTransform.Scale);
        }

        /// <summary>
        /// Detects wheter this collider will collide with a target collider in the current time window.        
        /// </summary>
        /// <param name="displacementVector">The world-space displacment vector of this collider</param>
        /// <param name="targetCollider">The target collider which we are testing against</param>
        /// <param name="targetColliderDisplacementVector">The world-space displacement vector to the target collider.</param>
        /// <returns></returns>
        public abstract CollisionEvent DetectCollision(Vector3 displacementVector, Collider targetCollider, Vector3 targetColliderDisplacementVector);

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (Collider child in args.NewItems)
                        OnChildAdded(child);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (Collider child in args.OldItems)
                        OnChildRemoved(child);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (Collider child in args.OldItems)
                        OnChildRemoved(child);
                    foreach (Collider child in args.NewItems)
                        OnChildAdded(child);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    OnChildReset();
                    break;
            }
        }

        protected virtual void OnChildAdded(Collider child)
        {
            if (child.TransformableParent != this)
                child.TransformableParent = this;
        }

        protected virtual void OnChildRemoved(Collider child)
        {
            if (child.TransformableParent == this)
                child.TransformableParent = null;
        }

        protected virtual void OnChildReset()
        { }

        protected CollisionEvent DeferDetectCollision(Vector3 displacementVector, Collider targetCollider, Vector3 targetColliderDisplacementVector)
        {
            CollisionEvent ce = targetCollider.DetectCollision(targetColliderDisplacementVector, this, displacementVector);
            if (ce != null)
                return new CollisionEvent()
                {
                    Object1 = ce.Object2,
                    Object2 = ce.Object1,
                    Object1LocationAtCollision = ce.Object2LocationAtCollision,
                    Object2LocationAtCollision = ce.Object1LocationAtCollision
                };
            else
                return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ColliderCollection : System.Collections.ObjectModel.ObservableCollection<Collider>
    {

    }
}
