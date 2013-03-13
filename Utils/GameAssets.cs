using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.World;
using CipherPark.AngelJacket.Core.World.ParticleSystem;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Effects;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Utils
{
    public class GameAssets
    {
        private IGameApp _game = null;
        private Dictionary<string, Camera> _cameras = new Dictionary<string, Camera>();
        private Dictionary<string, Model> _models = new Dictionary<string, Model>();
        private Dictionary<string, Animation.Animation> _animations = new Dictionary<string, Animation.Animation>();
        private Dictionary<string, Effect> _effects = new Dictionary<string, Effect>();
        private SceneNodes _sceneNodes = new SceneNodes();

        public GameAssets(IGameApp game)
        {
            _game = game;
        }

        public IGameApp Game { get { return _game; } }
        
        public Model Player1 { get; set; }

        public Dictionary<string, Camera> Cameras { get { return _cameras; } }

        public Dictionary<string, Model> Models { get { return _models; } }

        public Dictionary<string, Animation.Animation> Animations { get { return _animations; } }

        public SceneNodes SceneNodes { get { return _sceneNodes; } }

        public Dictionary<string, Effect> Effects { get { return _effects; } }
    }
}
