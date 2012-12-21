using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.Module;

namespace CipherPark.AngelJacket.Core.World.Scene
{
    public class Scene
    {
        private IGameApp _game = null;

        private SceneNodes _nodes = null;

        public Scene(IGameApp game)
        {
            _game = game;
            _nodes = new SceneNodes(this);
        }

        public IGameApp Game { get { return _game; } }

        public SceneNodes Nodes { get { return _nodes; } }

        public void Update(long gameTime)
        {
            foreach (SceneNode node in Nodes)
                _UpdateNodeHierarchy(gameTime, node);
        }

        private static void _UpdateNodeHierarchy(long gameTime, SceneNode parent)
        {
            parent.Update(gameTime);
            foreach (SceneNode child in parent.Children)
                _UpdateNodeHierarchy(gameTime, child);
        }

        public void Draw(long gameTime)
        {
            foreach (SceneNode node in Nodes)
                _DrawNodeHierarchy(gameTime, node);
        }

        private static void _DrawNodeHierarchy(long gameTime, SceneNode parent)
        {
            parent.Draw(gameTime);
            foreach (SceneNode child in parent.Children)
                _DrawNodeHierarchy(gameTime, child);
        }                
    }
}
