﻿using System;
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

namespace CipherPark.AngelJacket.Core.World
{
    /// <summary>
    /// A collision detection system for world objects.
    /// </summary>
    public class CollisionDetector
    {
        const int PartitionTreeDepth = 3;
        IGameApp _game = null;
        WorldSpacePartitionTree _partitionTree = null;
        List<ObservedWorldObject> observedObjects = new List<ObservedWorldObject>();
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
        public void AddObservedObject(WorldObject obj)
        {
            observedObjects.Add(new ObservedWorldObject() { WorldObject = obj, PreviousTransform = obj.Transform });    
        }

        /// <summary>
        /// Removes an world object from collision detection observation
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveObservedObject(WorldObject obj)
        {
            observedObjects.Remove(observedObjects.First(o => o.WorldObject == obj));
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
            //***********************************************************************************
            //Basic, Broad-Phase collision detection using spatial partitioning.
            //***********************************************************************************

            List<CollisionEvent> collisionEvents = new List<CollisionEvent>();                 

            //Stage I. Broad Phase
            //--------------------
            
            //1. One-Time partition World Space into in a 3-level oct-tree.
            IGameContextService contextService = (IGameContextService)_game.Services.GetService(typeof(IGameContextService));                     
            if( _partitionTree == null )
                _partitionTree = WorldSpacePartitionTree.CreateOctTree(contextService.Context.Value.ActionBounds, PartitionTreeDepth);

            //2. Assign/Re-assign each observed object to all oct-tree leaves where a potential collision may occur.
            _partitionTree.Assign(observedObjects);                  

            //Stage II. Narrow Phase
            //----------------------
            
            //Test objects associated with each oct-tree leaf for potential collisions.

            foreach(WorldSpacePartitionNode node in _partitionTree.Leaves)
            {
                foreach (ObservedWorldObject objectA in node.WorldObjects)
                {                
                    //TODO: Optimize by placing this code in the Assing() method of WorldPartitionTree and storing the values as member
                    //variables.
                    
                    BoundingSphere sphereA = objectA.WorldObject.WorldBoundingSphere();
                    Vector3 movementA = objectA.WorldObject.ParentToWorldNormal(Vector3.Normalize(objectA.WorldObject.Transform.Translation - objectA.PreviousTransform.Translation));
                    float lengthA = Vector3.Distance(objectA.WorldObject.Transform.Translation, objectA.PreviousTransform.Translation);
                    
                    foreach(ObservedWorldObject objectB in node.WorldObjects)
                    {
                        //TODO: Optimize by placing this code in the Assing() method of WorldPartitionTree and storing the values as member
                        //variables.
                        
                        BoundingSphere sphereB = objectB.WorldObject.WorldBoundingSphere();
                        Vector3 movementB = objectB.WorldObject.ParentToWorldNormal(Vector3.Normalize(objectB.WorldObject.Transform.Translation - objectB.PreviousTransform.Translation));
                        float lengthB = Vector3.Distance(objectB.WorldObject.Transform.Translation, objectB.PreviousTransform.Translation);

                        //TODO: Optimize by performing a lookup that disreguards this test if these two objects
                        //have already been tested (ie: when objectA was objectB and objectB was objectA).

                        //*****************************************************************************************
                        //The following technique was dervied from this tutorial.
                        //http://www.gamasutra.com/view/feature/131424/pool_hall_lessons_fast_accurate_.php?page=2
                        //*****************************************************************************************
                        
                        //1. Calculate the movement vector for A relative to B. (ie: in B's frame of reference).
                        Vector3 movementArB = Vector3.Normalize(movementA - movementB);
                        
                        //2. Get the direction vector of A to B
                        Vector3 dirAB = Vector3.Normalize(objectB.WorldObject.Transform.Translation - objectA.WorldObject.Transform.Translation);
                        
                        //3. Check to see if A is actually moving towards or away from B.
                        if (Vector3.Dot(movementArB, dirAB) > 0 )
                        {
                            /*
                            if( wsSphereObservedObject.Intersects(ref wsSphereTargetObject) )
                            {                           
                                //Add to colliding pairs if and only if it's a new pair.
                                if (collisionEvents.Count(p => (p.Item1 == observedObject && p.Item2 == targetObject) ||
                                                               (p.Item1 == targetObject && p.Item2 == observedObject)) == 0)
                                {
                                    collisionEvents.Add(new Tuple<WorldObject, WorldObject>(observedObject, targetObject));
                                }                            
                            }
                            */

                        }                       
                    }
                }               
            }

            NotifyHandlers(collisionEvents);
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

    /// <summary>
    /// A contract for any listener of collision events.
    /// </summary>
    public interface ICollisionResponseHandler
    {
        void OnCollision(CollisionEvent collisionEvent);
    }

    /// <summary>
    /// Represents a oct-space partition tree for the world.
    /// </summary>
    public class WorldSpacePartitionTree
    {
        private const int ChildNodesPerParent = 8;
        private WorldSpacePartitionNode _root = null;
        private List<WorldSpacePartitionNode> _leaves = null;

        /// <summary>
        /// All the leaf nodes of the space partition tree.
        /// </summary>
        /// <remarks>
        /// Leaf nodes are the only nodes in the tree that contain world objects.
        /// </remarks>
        public List<WorldSpacePartitionNode> Leaves { get { return _leaves; } }

        /// <summary>
        /// Creates a new WorldSpacePartitionTree with a specified total volumen and tree depth. 
        /// </summary>
        /// <param name="totalVolume"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static WorldSpacePartitionTree CreateOctTree(BoundingBox totalVolume, int depth)
        {
            List<WorldSpacePartitionNode> allNodes = new List<WorldSpacePartitionNode>();
            WorldSpacePartitionTree newTree = new WorldSpacePartitionTree();
            WorldSpacePartitionNode rootNode = new WorldSpacePartitionNode()
            {
                BoundingBox = totalVolume
            };
            _BuildOctTree(rootNode, totalVolume, depth);
            _FlattenToList(rootNode, allNodes);
            newTree._root = rootNode;
            newTree._leaves = allNodes.Where(n => n.Children.Count == 0).ToList();
            return newTree;
        }    

        /// <summary>
        /// Assigns/Re-assigns each object in the world to one or more partition spaces.
        /// </summary>
        /// <remarks>An object may span more than one space, in which case, it is assinged to all spaces it spans</remarks>
        /// <param name="observedObjects"></param>
        public void Assign(List<ObservedWorldObject> observedObjects)
        {           
            foreach (WorldSpacePartitionNode leaf in _leaves)
            {
                leaf.WorldObjects.Clear();
                foreach (ObservedWorldObject observed in observedObjects)
                {
                    BoundingBox leafBounds = leaf.BoundingBox;
                    if (observed.WorldObject.WorldBoundingSphere().Intersects(ref leafBounds))
                        leaf.WorldObjects.Add(observed);
                }
            }
        }    

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="bounds"></param>
        /// <param name="nLevels"></param>
        private static void _BuildOctTree(WorldSpacePartitionNode root, BoundingBox bounds, int nLevels)
        {
            if (nLevels == 0)
                return;
            else
            {
                //Min, Max and Mid points of the root node's bounding box.

                Vector3 Mid = new Vector3(bounds.Minimum.X + (bounds.Maximum.X - bounds.Minimum.X) / 2.0f,
                                          bounds.Minimum.Y + (bounds.Maximum.Y - bounds.Minimum.Y) / 2.0f,
                                          bounds.Minimum.Z + (bounds.Maximum.Z - bounds.Minimum.Z) / 2.0f);
                Vector3 Max = bounds.Maximum;
                Vector3 Min = bounds.Minimum;

                
                //The goal is to divde the root node bounding box into 8 boxes.
                //Order of child boxes...
                //1-4, start from front-lower-left and move clockwise. 5-8, start from back-lower-left and move clockwise.
                
                Vector3[] vecMin = new Vector3[]
                {
                    new Vector3(Min.X, Min.Y, Min.Z),
                    new Vector3(Min.X, Mid.Y, Min.Z),
                    new Vector3(Mid.X, Mid.Y, Min.Z),
                    new Vector3(Mid.X, Min.Y, Min.Z),
                    new Vector3(Min.X, Min.Y, Mid.Z),
                    new Vector3(Min.X, Mid.Y, Mid.Z),
                    new Vector3(Mid.X, Mid.Y, Mid.Z),
                    new Vector3(Mid.X, Min.Y, Mid.Z),
                };
                Vector3[] vecMax = new Vector3[]
                {
                    new Vector3(Mid.X, Mid.Y, Mid.Z),
                    new Vector3(Mid.X, Max.Y, Mid.Z),
                    new Vector3(Max.X, Max.Y, Mid.Z),
                    new Vector3(Max.X, Mid.Y, Mid.Z),
                    new Vector3(Mid.X, Mid.Y, Max.Z),
                    new Vector3(Mid.X, Max.Y, Max.Z),
                    new Vector3(Max.X, Max.Y, Max.Z),
                    new Vector3(Max.X, Mid.Y, Max.Z),
                };
                for (int i = 0; i < ChildNodesPerParent; i++)
                {
                    WorldSpacePartitionNode child = new WorldSpacePartitionNode();
                    root.Children.Add(child);
                    BoundingBox childBounds = new BoundingBox(vecMin[i], vecMax[i]);
                    _BuildOctTree(child, childBounds, nLevels - 1);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="allNodes"></param>
        private static void _FlattenToList(WorldSpacePartitionNode root, List<WorldSpacePartitionNode> allNodes)
        {
            allNodes.Add(root);
            root.Children.ForEach(c => _FlattenToList(c, allNodes));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WorldSpacePartitionNode
    {
        private List<ObservedWorldObject> _observedWorldObjects = new List<ObservedWorldObject>();
        private List<WorldSpacePartitionNode> _children = new List<WorldSpacePartitionNode>();   
   
        /// <summary>
        /// 
        /// </summary>
        public BoundingBox BoundingBox { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ObservedWorldObject> WorldObjects { get { return _observedWorldObjects; } }
        /// <summary>
        /// 
        /// </summary>
        public List<WorldSpacePartitionNode> Children { get { return _children; } } 
    }  

    /// <summary>
    /// 
    /// </summary>
    public class ObservedWorldObject 
    {
        /// <summary>
        /// 
        /// </summary>
        public WorldObject WorldObject { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Transform PreviousTransform { get; set; }
    }

    /// <summary>
    /// Represents an event of two world objects colliding.
    /// </summary>
    public class CollisionEvent
    {
        /// <summary>
        /// First object involved in the collision.
        /// </summary>
        public WorldObject Object1 { get; set; }

        /// <summary>
        /// Second object involved in the collision.
        /// </summary>        
        public WorldObject Object2 { get; set; }

        /// <summary>
        /// The first object's location at the collision.
        /// </summary>
        public Vector3 Object1LocationAtCollision { get; set; }

        /// <summary>
        /// The second object's location at the collision.
        /// </summary>
        public Vector3 Object2LocationAtCollision { get; set; }
    }    
}
