using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.Utils;

namespace CipherPark.AngelJacket.Core.Sequencer
{
    public class Sequencer
    {
        private IGameApp _game = null;
        private GameAssets _assets = null;
        private Scene _scene = null;

        public IGameApp Game { get { return _game; } }

        public GameAssets Assets { get { return _assets; } }

        public Scene Scene { get { return _scene; } } 

        public Sequencer(GameAssets assets, Scene scene)
        {
            _game = assets.Game;
            _assets = assets;
            _scene = scene;
        }
    }
}
