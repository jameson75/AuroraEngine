using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.World.Scene;

namespace CipherPark.AngelJacket.Core.World
{
    public class WorldSimulator
    {
        IGameApp _game = null;
        Scene.Scene _scene = null;

        public IGameApp Game { get { return _game; } }

        public Scene.Scene Scene { get { return _scene; } }

        public WorldSimulator(Scene.Scene scene)
        {
            _game = scene.Game;
            _scene = scene;
        }
    }
}
