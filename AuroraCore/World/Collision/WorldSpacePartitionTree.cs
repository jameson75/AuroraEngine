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

namespace CipherPark.AngelJacket.Core.World.Collision
{
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
            foreach (WorldSpacePartitionNode leafNode in _leaves)
            {
                leafNode.WorldObjects.Clear();
                foreach (ObservedWorldObject observed in observedObjects)
                {
                    //**************************************************************************
                    //TODO: Fix the code below.
                    //Need to figure out of the current node intersects the temporal volume.
                    //**************************************************************************
                    /*
                    float temporalVectorLength = Vector3.Distance(observed.WorldObject.Transform.Translation, observed.PreviousTransform.Translation);
                    Ray temporalRay = new Ray(observed.PreviousTransform.Translation,
                                              observed.WorldObject.Transform.Translation - observed.PreviousTransform.Translation);
                    float intersectionDistance = 0;
                    if (leafNode.BoundingBox.Intersects(ref temporalRay, out intersectionDistance) &&
                        intersectionDistance <= temporalVectorLength )
                        leafNode.WorldObjects.Add(observed);
                    */
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
}
