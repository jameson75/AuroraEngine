using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CipherPark.AngelJacket.Core.World
{
    public class Scene
    {
        private Game _game = null;

        private SceneNodes _nodes = null;

        public Scene(Game game)
        {
            _game = game;
        }

        public SceneNodes Nodes { get { return _nodes; } }
    }
}
