using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.World;
using CipherPark.AngelJacket.Core.World.ParticleSystem;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.Animation;

namespace CipherPark.AngelJacket.Core.Utils
{
    public class GameAssets
    {
        private IGameApp _game = null;
        private Dictionary<string, Camera> _cameras = new Dictionary<string, Camera>();
        private Dictionary<string, SceneNode> _sceneNodes = new Dictionary<string, SceneNode>();
        private Dictionary<string, Animation.Animation> _animations = new Dictionary<string, Animation.Animation>();

        public GameAssets(IGameApp game)
        {
            _game = game;
        }

        public IGameApp Game { get { return _game; } }
        
        public Model Player1 { get; set; }

        public Dictionary<string, Camera> Cameras { get { return _cameras; } }

        public Dictionary<string, SceneNode> SceneNodes { get { return _sceneNodes; } }

        public Dictionary<string, Animation.Animation> Animations { get { return _animations; } }
    }
}
