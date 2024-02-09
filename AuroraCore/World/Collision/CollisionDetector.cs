using System;
using System.Collections.Generic;
using System.Linq;

///////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////

namespace CipherPark.Aurora.Core.World.Collision
{
    /// <summary>
    /// A collision detection system for world objects.
    /// </summary>
    public class CollisionDetector
    {                
        private readonly List<CollisionEvent> collisionEvents;
        private readonly List<GameObject> gameObjects;
        private List<Collider> firedColliders;
        private List<CollisionEvent> allCollisionEventsInPreviousPass;
        private List<CollisionEvent> allCollisionEventsInCurrentPass;
#pragma warning disable CS0649
        private PartitionTreeNode partitionTree;
#pragma warning restore CS0649

        /// <summary>
        /// Instanciates new collision detector.
        /// </summary>
        /// <param name="game"></param>
        public CollisionDetector()
        {            
            collisionEvents = new List<CollisionEvent>();
            gameObjects = new List<GameObject>();
            firedColliders = new List<Collider>();
            allCollisionEventsInPreviousPass = new List<CollisionEvent>();
            allCollisionEventsInCurrentPass = new List<CollisionEvent>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameObject"></param>
        public void Observe(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collider"></param>
        public void UnObserve(GameObject gameObject)
        {
            gameObjects.Remove(gameObject);
        }

        /// <summary>
        /// 
        /// </summary>
        public List<CollisionEvent> CollisionEvents { get => collisionEvents; }

        /// <summary>
        /// Updates the state of collision detection.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            FlushCollisionEvents();

            //Stage I. Broad Phase
            //--------------------
            /*
                TODO: Implement broad phase by, optionally, creating sub-space partition tree.
            */

            //Stage II. Narrow Phase
            //----------------------          

            if (partitionTree != null)
            {
                DetectCollisions(partitionTree);
            }

            else
            {
                DetectCollisions(gameObjects);
            }
        }
        
        private void FlushCollisionEvents()
        {
            allCollisionEventsInPreviousPass.Clear();
            allCollisionEventsInPreviousPass.AddRange(allCollisionEventsInCurrentPass);
            allCollisionEventsInCurrentPass.Clear();
            collisionEvents.Clear();
        }

        private void DetectCollisions(PartitionTreeNode partitionTree)
        {
            if (partitionTree == null)
            {
                throw new ArgumentNullException(nameof(partitionTree));
            }
            
            //We only process a node's gameobjects if the node is a leaf node.
            //Otherwise, we continue to traverse the tree.
            if (partitionTree.LeftChild == null && partitionTree.RightChild == null)
            {                
                DetectCollisions(partitionTree.GameObjects);
            }
            else
            {
                DetectCollisions(partitionTree.RightChild);
                DetectCollisions(partitionTree.LeftChild);
            }
        }

        private void DetectCollisions(IList<GameObject> gameObjectsA)
        {
            var gameObjectsB = gameObjectsA.ToList();
            foreach (var gameObjectA in gameObjectsA)
            {
                gameObjectsB.Remove(gameObjectA);
                foreach (var gameObjectB in gameObjectsB)
                {
                    if (CanCollide(gameObjectA) && CanCollide(gameObjectB))
                    {
                        var collisionEvent = gameObjectA.Collider.DetectCollision(gameObjectA.ContainerNode, gameObjectB.Collider, gameObjectB.ContainerNode);
                        if (collisionEvent != null)
                        {
                            if (!IsEcho(collisionEvent) || (CanEchoCollisionFor(gameObjectA) && CanEchoCollisionFor(gameObjectB)))
                            {
                                RecordCollisionEvent(collisionEvent);
                            }

                            MonitorCollisionForEcho(collisionEvent);
                        }
                    }
                }
            }
        }

        private void MonitorCollisionForEcho(CollisionEvent collisionEvent)
        {
            allCollisionEventsInCurrentPass.Add(collisionEvent);
        }

        private bool CanEchoCollisionFor(GameObject gameObject)
        {
            return gameObject.Collider.Mode != ColliderMode.FireAlwaysNoEcho;
        }

        private bool IsEcho(CollisionEvent collisionEvent)
        {
            return allCollisionEventsInPreviousPass.Any(e => 
                (e.Collider1 == collisionEvent.Collider1 && e.Collider2 == collisionEvent.Collider2) ||
                (e.Collider1 == collisionEvent.Collider2 && e.Collider2 == collisionEvent.Collider1));
        }

        private bool CanCollide(GameObject gameObject)
        {
            return gameObject.Collider != null &&             
                   gameObject.Collider.Mode != ColliderMode.InActive &&
                   gameObject.Collider.Mode != ColliderMode.FireOnce || !firedColliders.Contains(gameObject.Collider);
        }      

        private void RecordCollisionEvent(CollisionEvent collisionEvent)
        {
            collisionEvents.Add(collisionEvent);

            if (!firedColliders.Contains(collisionEvent.Collider1))
            {
                firedColliders.Add(collisionEvent.Collider1);
            }
            
            if (!firedColliders.Contains(collisionEvent.Collider2))
            {
                firedColliders.Add(collisionEvent.Collider2);
            }
        }      
    }
}