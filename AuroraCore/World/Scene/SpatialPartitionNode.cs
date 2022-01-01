using System;
using System.Collections.Generic;
using System.Linq;
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

namespace CipherPark.Aurora.Core.World
{
    /// <summary>
    /// Represents an axis-aligned partition oct-tree node in world space.
    /// </summary>
    public class SpatialPartitionNode
    {
        private List<SceneNode> _partitonedObjects = new List<SceneNode>();
        private List<SpatialPartitionNode> _children = new List<SpatialPartitionNode>();

        /// <summary>
        /// 
        /// </summary>
        public BoundingBox BoundingBox { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<SceneNode> WorldObjects { get { return _partitonedObjects; } }
        /// <summary>
        /// 
        /// </summary>
        public List<SpatialPartitionNode> Children { get { return _children; } }        
    }  
}
