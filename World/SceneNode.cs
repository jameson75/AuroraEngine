using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CipherPark.AngelJacket.Core.World
{
    public interface ISceneObject
    {

    }

    public class SceneNode
    {
        private IGameApp _game = null;

        public SceneNode(IGameApp game)
        {
            _game = game;
        }

        public ISceneObject SceneObject { get; set; }
    }

    public class SceneNodes : List<SceneNode>
    { }
}
