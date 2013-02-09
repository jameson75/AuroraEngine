using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;


namespace CipherPark.AngelJacket.Core.World.Scene
{
    public class NullSceneNode : SceneNode
    {
        public NullSceneNode(Scene scene)
            : base(scene)
        { }
    }
}
