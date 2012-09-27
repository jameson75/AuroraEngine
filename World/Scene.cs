using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.Module;

namespace CipherPark.AngelJacket.Core.World
{
    public class Scene
    {
        private IGameApp _game = null;

        private SceneNodes _nodes = null;

        public Scene(IGameApp game)
        {
            _game = game;
        }

        public SceneNodes Nodes { get { return _nodes; } }
    }
}
