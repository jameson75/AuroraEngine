using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Scene
{
    public class NullSceneNode : SceneNode
    {
        public NullSceneNode(IGameApp game)
            : base(game)
        { }

        public NullSceneNode(IGameApp game, string name)
            : base(game, name)
        { }      
    }   
}
