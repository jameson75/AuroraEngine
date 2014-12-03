using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using SharpDX.XInput;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.Services;
using CipherPark.AngelJacket.Core.Content;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.World;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Sequencer;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Effects;

namespace CipherPark.AngelJacket.Core.World.Scene
{
    public class MountNode : SceneNode
    {
        public MountNode(IGameApp game, string name = null) : base(game, name)
        { }

        public void Mount(MountNode target)
        {
            if (target.Parent == null)
                throw new InvalidOperationException("Target mount node does not have a parent");

            //convert the target mount node's parent's transform to the target mount node's space.
            Transform parentTransformMNS = target.WorldToLocal(target.Parent.WorldTransform());
            //calculate the new target mount node's parent transform.
            Transform newParentTransform = target.Parent.WorldToParent(this.LocalToWorld(parentTransformMNS));
            //set new parent transform.
            target.Parent.Transform = newParentTransform;
        }
    }
}
