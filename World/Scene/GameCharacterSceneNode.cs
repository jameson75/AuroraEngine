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
    public class GameCharacterSceneNode : SceneNode
    {           
        GameCharacter _character = null;       

        public GameCharacterSceneNode( GameCharacter character, string name = null)
            : base(character.Game, name)
        {
            _character = character;
            character.TransformableParent = this;
        }

        public GameCharacter GameCharacter
        {
            get
            {
                return _character;
            }
        }

        public override void Draw(GameTime gameTime)
        {           
            GameCharacter.Draw(gameTime);                         
            base.Draw(gameTime);
        }
    }
}
