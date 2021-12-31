using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.KillScript.Core;
using CipherPark.KillScript.Core.World.Scene;
using CipherPark.KillScript.Core.Animation;
using CipherPark.KillScript.Core.Services;
using CipherPark.KillScript.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.World
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
