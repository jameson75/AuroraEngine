using System;
using System.Collections.Generic;
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
        List<WorldObject> observedObjects = new List<WorldObject>();
        List<ICollisionResponseHandler> registeredHandlers = new List<ICollisionResponseHandler>();
        IGameApp _game = null;

        public CollisionDetector(IGameApp game)
        {
            _game = null;
        }

        public void AddObservedObject(WorldObject obj)
        {
            observedObjects.Add(obj);    
        }

        public void RemoveObservedObject(WorldObject obj)
        {
            observedObjects.Remove(obj);
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
            WorldSpacePartitionTree partitionTree = WorldSpacePartitionTree.CreateQuadTree(actionBounds, 3);
            partitionTree.Assign(observedObjects);          
            List<WorldSpacePartitionNode> leafPartitions = partitionTree.FlattenLeavesToList();     
            List<Tuple<WorldObject, WorldObject>> collidingPairs = new List<Tuple<WorldObject, WorldObject>>();

            //Narrow Phase
            //------------
            foreach(WorldSpacePartitionNode node in leafPartitions)
            {
                foreach (WorldObject observedObject in node.WorldObjects)
                {                    
                    BoundingSphere wSphereObservedObject = observedObject.WorldSphere();
                    foreach(WorldObject targetObject in node.WorldObjects)
                    {
                        BoundingSphere wSphereTargetObject = targetObject.WorldSphere();
                        if( wSphereObservedObject.Intersects(ref wSphereTargetObject ) )
                        {
                            //TODO: Add to colliding pairs if and only if it's a new pair.           
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
        public static WorldSpacePartitionTree CreateQuadTree(BoundingBox totalBounds, int nLevels)
        {
            throw new NotFiniteNumberException();
        }

        public List<WorldSpacePartitionNode> FlattenLeavesToList() 
        {
            throw new NotImplementedException();
        }

        internal void Assign(List<WorldObject> observedObjects)
        {
            throw new NotImplementedException();
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
        public static BoundingSphere WorldSphere(this WorldObject wo)
        {
             return new BoundingSphere(wo.WorldTransform().Translation, BoundingSphere.FromBox(wo.BoundingBox).Radius);
        }
    }
}
