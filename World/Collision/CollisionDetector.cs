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
    /// A collision detection system for world objects.
    /// </summary>
    public class CollisionDetector
    {
        const int PartitionTreeDepth = 3;
        IGameApp _game = null;
        //WorldSpacePartitionTree _partitionTree = null;
        List<ObservedCollidableObject> _observedObjects = new List<ObservedCollidableObject>();
        List<ICollisionResponseHandler> registeredHandlers = new List<ICollisionResponseHandler>();        

        /// <summary>
        /// Instanciates new collision detector.
        /// </summary>
        /// <param name="game"></param>
        public CollisionDetector(IGameApp game)
        {
            _game = null;
        }

        /// <summary>
        /// Adds a world object to be observed for collision detection
        /// </summary>
        /// <param name="obj"></param>
        public void AddObservedObject(ICollidable obj)
        {
            _observedObjects.Add(new ObservedCollidableObject() { ObservedObject = obj, PreviousTransform = obj.Transform });    
        }

        /// <summary>
        /// Removes an world object from collision detection observation
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveObservedObject(ICollidable obj)
        {
            _observedObjects.Remove(_observedObjects.First(o => o.ObservedObject == obj));
        }

        /// <summary>
        /// Registers a listener of colision events
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterHandler(ICollisionResponseHandler handler)
        {
            registeredHandlers.Add(handler);
        }

        /// <summary>
        /// Unregisters a listener from collision events
        /// </summary>
        /// <param name="handler"></param>
        public void UnregisterHandler(ICollisionResponseHandler handler)
        {
            registeredHandlers.Remove(handler);
        }

        /// <summary>
        /// Updates the state of collision detection.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            Update(gameTime, _observedObjects);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTme"></param>
        /// <param name="observedObjects"></param>
        private void Update(GameTime gameTme, List<ObservedCollidableObject> observedObjects)
        {
            //***********************************************************************************
            //Basic, Broad-Phase collision detection using spatial partitioning.
            //***********************************************************************************

            List<CollisionEvent> collisionEvents = new List<CollisionEvent>();                 

            //Stage I. Broad Phase
            //--------------------
            /*
            //1. One-Time partition World Space into in a 3-level oct-tree.
            IGameContextService contextService = (IGameContextService)_game.Services.GetService(typeof(IGameContextService));                     
            if( _partitionTree == null )
                _partitionTree = WorldSpacePartitionTree.CreateOctTree(contextService.Context.Value.ActionBounds, PartitionTreeDepth);

            //2. Assign/Re-assign each observed object to all oct-tree leaves where a potential collision may occur.
            _partitionTree.Assign(observedObjects);                  
            */

            //Stage II. Narrow Phase
            //----------------------
            
            //Test objects associated with each oct-tree leaf for potential collisions.

            //foreach(WorldSpacePartitionNode node in _partitionTree.Leaves)
            {
                foreach (ObservedCollidableObject objectA in observedObjects)
                {
                    Vector3 vectorA = objectA.ObservedObject.ParentToWorldNormal(objectA.ObservedObject.Transform.Translation - objectA.PreviousTransform.Translation);
                    foreach (ObservedCollidableObject objectB in observedObjects)
                    {
                        if (objectA == objectB)
                            continue;                  
                        
                        Vector3 vectorB = objectB.ObservedObject.ParentToWorldNormal(objectB.ObservedObject.Transform.Translation - objectB.PreviousTransform.Translation);
                        foreach (var colliderA in objectA.ObservedObject.Colliders)
                        {
                            foreach (var colliderB in objectB.ObservedObject.Colliders)
                            {
                                CollisionEvent collision = colliderA.DetectCollision(vectorA, colliderB, vectorB);
                                if (collision != null)
                                    collisionEvents.Add(collision);
                            }
                        }
                    }
                }        
            }

            //Take a snapshot of the current transforms of all observed objects.
            SnapshotTransforms(observedObjects);

            //Notify any listeners of collision events.
            NotifyHandlers(collisionEvents);
        }

        /// <summary>
        /// Records the current position of each observed object as there respective previous position.
        /// </summary>
        /// <param name="observedObjects"></param>
        private void SnapshotTransforms(List<ObservedCollidableObject> observedObjects)
        {
            observedObjects.ForEach(o => o.PreviousTransform = o.ObservedObject.Transform);
        }

        /// <summary>
        /// Notifies the registered listenees of any collision events that occured during the last call to Update().
        /// </summary>
        /// <param name="collisionEvent"></param>
        private void NotifyHandlers(IEnumerable<CollisionEvent> collisionEvent)
        {   
            foreach(var p in collisionEvent)
                registeredHandlers.ForEach(h => h.OnCollision(p));
        }
    } 
}

/// <summary>
/// 
/// </summary>
public static class NullableFloatExtension
{
    public static bool IsNullOrEqualTo(this Nullable<float> f, float value)
    {
        return (f == null || f.Value == value);
    }

    public static bool IsNullOrLessThan(this Nullable<float> f, float value)
    {
        return (f == null || f.Value < value);
    }

    public static bool IsNullOrGreaterThan(this Nullable<float> f, float value)
    {
        return (f == null || f.Value > value);
    }

    public static bool IsNullOrNotEqualTo(this Nullable<float> f, float value)
    {
        return (f == null || f.Value != value);
    }

    public static bool IsNullOrLessThanOrEqualTo(this Nullable<float> f, float value)
    {
        return (f == null || f.Value <= value);
    }

    public static bool IsNullOrGreaterThanOrEqualTo(this Nullable<float> f, float value)
    {
        return (f == null || f.Value >= value);
    }
}
