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
        private PartitionTreeNode partitionTree;

        /// <summary>
        /// Instanciates new collision detector.
        /// </summary>
        /// <param name="game"></param>
        public CollisionDetector()
        {            
            collisionEvents = new List<CollisionEvent>();
            gameObjects = new List<GameObject>();
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
        /// Updates the state of collision detection.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            collisionEvents.Clear();

            //***********************************************************************************
            //Basic, Broad-Phase collision detection using spatial partitioning.
            //***********************************************************************************                            

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

        private void DetectCollisions(IList<GameObject> gameObjects)
        {
            var gameObjectsB = gameObjects.ToList();
            foreach (var gameObjectA in gameObjects)
            {
                gameObjectsB.Remove(gameObjectA);
                foreach (var gameObjectB in gameObjects)
                {
                    var collisionEvent = gameObjectA.Collider.DetectCollision(gameObjectA.ContainerNode, gameObjectB.Collider, gameObjectB.ContainerNode);
                    if (collisionEvent != null)
                    {
                        collisionEvents.Add(collisionEvent);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PartitionTreeNode
    {
        private List<GameObject> gameObjects { get; set; }

        public PartitionTreeNode(IList<GameObject> gameObjects)
        {
            this.gameObjects = new List<GameObject>(gameObjects);
        }

        /// <summary>
        /// 
        /// </summary>
        public List<GameObject> GameObjects { get => gameObjects; }

        /// <summary>
        /// 
        /// </summary>
        public PartitionTreeNode LeftChild { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PartitionTreeNode RightChild { get; set; }
    }
}