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

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World
{
    public class CollisionDetector
    {
        IGameApp _game = null;
        WorldSpacePartitionTree _partitionTree = null;
        List<ObservedWorldObject> observedObjects = new List<ObservedWorldObject>();
        List<ICollisionResponseHandler> registeredHandlers = new List<ICollisionResponseHandler>();        

        public CollisionDetector(IGameApp game)
        {
            _game = null;
        }

        public void AddObservedObject(WorldObject obj)
        {
            observedObjects.Add(new ObservedWorldObject() { WorldObject = obj, PreviousTransform = obj.Transform });    
        }

        public void RemoveObservedObject(WorldObject obj)
        {
            observedObjects.Remove(observedObjects.First(o => o.WorldObject == obj));
        }

        public void RegisterHandler(ICollisionResponseHandler handler)
        {
            registeredHandlers.Add(handler);
        }

        public void UnregisterHandler(ICollisionResponseHandler handler)
        {
            registeredHandlers.Remove(handler);
        }

        public void Update(GameTime gameTime)
        {
            IGameContextService contextService = (IGameContextService)_game.Services.GetService(typeof(IGameContextService));
            BoundingBox actionBounds = contextService.Context.ActionBounds;            

            //Broad Phase
            //-----------
            //1. Partition World Space.
            if( _partitionTree == null )
                _partitionTree = WorldSpacePartitionTree.CreateOctTree(actionBounds, 3);

            _partitionTree.Assign(observedObjects.Select(o => o.WorldObject).ToList());
            List<WorldSpacePartitionNode> leafPartitions = _partitionTree.Leaves;
            List<Tuple<WorldObject, WorldObject>> collidingPairs = new List<Tuple<WorldObject, WorldObject>>();

            //Narrow Phase
            //------------
            foreach(WorldSpacePartitionNode node in leafPartitions)
            {
                foreach (WorldObject observedObject in node.WorldObjects)
                {                    
                    BoundingSphere wsSphereObservedObject = observedObject.BoundingSphereInWorldSpace();
                    foreach(WorldObject targetObject in node.WorldObjects)
                    {
                        BoundingSphere wsSphereTargetObject = targetObject.BoundingSphereInWorldSpace();
                        if( wsSphereObservedObject.Intersects(ref wsSphereTargetObject) )
                        {
                            //Add to colliding pairs if and only if it's a new pair.
                            if (collidingPairs.Count(p => (p.Item1 == observedObject && p.Item2 == targetObject) ||
                                                          (p.Item1 == targetObject && p.Item2 == observedObject)) == 0)
                            {
                                collidingPairs.Add(new Tuple<WorldObject, WorldObject>(observedObject, targetObject));
                            }
                        }
                    }
                }               
            }

            NotifyHandlers(collidingPairs);
        }

        private void NotifyHandlers(IEnumerable<Tuple<WorldObject, WorldObject>> collidingPairs)
        {   
            foreach(var p in collidingPairs)
                registeredHandlers.ForEach(h => h.OnCollision(p.Item1, p.Item2));
        }
    }

    public interface ICollisionResponseHandler
    {
        void OnCollision(WorldObject obj1, WorldObject obj2);
    }

    public class WorldSpacePartitionTree
    {
        private const int ChildNodesPerParent = 8;
        private WorldSpacePartitionNode _root = null;
        private List<WorldSpacePartitionNode> _leaves = null;

        public static WorldSpacePartitionTree CreateOctTree(BoundingBox totalBounds, int nLevels)
        {
            List<WorldSpacePartitionNode> allNodes = new List<WorldSpacePartitionNode>();
            WorldSpacePartitionTree newTree = new WorldSpacePartitionTree();
            WorldSpacePartitionNode rootNode = new WorldSpacePartitionNode()
            {
                BoundingBox = totalBounds
            };
            _BuildOctTree(rootNode, totalBounds, nLevels);
            _FlattenToList(rootNode, allNodes);
            newTree._root = rootNode;
            newTree._leaves = allNodes.Where(n => n.Children.Count == 0).ToList();
            return newTree;
        }    

        public void Assign(List<WorldObject> observedObjects)
        {           
            foreach (WorldSpacePartitionNode leaf in _leaves)
            {
                leaf.WorldObjects.Clear();
                foreach (WorldObject wo in observedObjects)
                {
                    BoundingBox leafBounds = leaf.BoundingBox;
                    if (wo.BoundingSphereInWorldSpace().Intersects(ref leafBounds))
                        leaf.WorldObjects.Add(wo);
                }
            }
        }

        public List<WorldSpacePartitionNode> Leaves { get { return _leaves; } }

        private static void _BuildOctTree(WorldSpacePartitionNode root, BoundingBox bounds, int nLevels)
        {
            if (nLevels == 0)
                return;
            else
            {
                Vector3 Mid = new Vector3(bounds.Minimum.X + (bounds.Maximum.X - bounds.Minimum.X) / 2.0f,
                                          bounds.Minimum.Y + (bounds.Maximum.Y - bounds.Minimum.Y) / 2.0f,
                                          bounds.Minimum.Z + (bounds.Maximum.Z - bounds.Minimum.Z) / 2.0f);
                Vector3 Max = bounds.Maximum;
                Vector3 Min = bounds.Minimum;

                //Min/Max of octree nodes' bounding boxes.
                //Order of nodes: 
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

        private static void _FlattenToList(WorldSpacePartitionNode root, List<WorldSpacePartitionNode> allNodes)
        {
            allNodes.Add(root);
            root.Children.ForEach(c => _FlattenToList(c, allNodes));
        }
    }

    public class WorldSpacePartitionNode
    {
        private List<WorldObject> _worldObject = new List<WorldObject>();
        private List<WorldSpacePartitionNode> _children = new List<WorldSpacePartitionNode>();
        private WorldSpacePartitionTree[] _adjacentNodes = new WorldSpacePartitionTree[6];
        public BoundingBox BoundingBox { get; set; }
        public List<WorldObject> WorldObjects { get; set; }
        public List<WorldSpacePartitionNode> Children { get; set; }      
    }

    public static class WorldObjectExtension
    {
        public static BoundingSphere BoundingSphereInWorldSpace(this WorldObject wo)
        {
             return new BoundingSphere(wo.WorldTransform().Translation, BoundingSphere.FromBox(wo.BoundingBox).Radius);
        }

        public static BoundingBox BoundingBoxInWorldSpace(this WorldObject wo)
        {
            return new BoundingBox(wo.ParentToWorldCoordinate(wo.BoundingBox.Minimum), wo.ParentToWorldCoordinate(wo.BoundingBox.Maximum));
        }
    }

    public class ObservedWorldObject 
    {
        public WorldObject WorldObject { get; set; }
        public Transform PreviousTransform { get; set; }
    }
}
