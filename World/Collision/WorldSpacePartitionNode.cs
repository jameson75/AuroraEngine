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
}
