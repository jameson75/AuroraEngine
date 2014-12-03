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
    public class WorldObjectSceneNode : SceneNode
    {           
        WorldObject _worldObject = null;        

        public WorldObjectSceneNode( WorldObject worldObject, string name = null)
            : base(worldObject.Game, name)
        {
            _worldObject = worldObject;
            worldObject.TransformableParent = this;
        }

        public WorldObject WorldObject
        {
            get
            {
                return _worldObject;
            }
        }


        public override void Update(GameTime gameTime)
        {
            WorldObject.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {           
            WorldObject.Draw(gameTime);                         
            base.Draw(gameTime);
        }
    }    
}
