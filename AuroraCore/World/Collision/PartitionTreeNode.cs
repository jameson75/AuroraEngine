using System.Collections.Generic;

///////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////

namespace CipherPark.Aurora.Core.World.Collision
{
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