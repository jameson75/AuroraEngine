using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Services;
using CipherPark.AngelJacket.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Collision
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Collider : ITransformable
    {
        private ColliderCollection _children = null;

        public ColliderCollection Children { get { return _children; } }

        public WorldObject Container
        {
            get
            {
                ITransformable p = TransformableParent;
                while (p != null)
                {
                    if (p is WorldObject)
                        return (WorldObject)p;
                }
                return null;
            }
        }

        public Transform Transform { get; set; }

        public ITransformable TransformableParent { get; set; }

        public Collider()
        {
            _children = new ColliderCollection();
            _children.CollectionChanged += Children_CollectionChanged;
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
