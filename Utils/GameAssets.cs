using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core;

namespace CipherPark.AngelJacket.Core.Utils
{
    public class GameAssets
    {
        private IGameApp _game = null;

        public IGameApp Game { get { return _game; } }

        public GameAssets(IGameApp game)
        {
            _game = game;
        }       
    }
}
