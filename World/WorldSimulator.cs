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

        public IGameApp Game { get { return _game; } }    

        public WorldSimulator(IGameApp game)
        {
            _game = game;
        }

        public WorldSimulatorSettings Settings { get; set; }

        public void Update(Scene.Scene scene, long gameTime)
        {

        }
    }

    public class WorldSimulatorSettings
    {
        
    }
}
