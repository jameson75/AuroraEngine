using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Utils.Toolkit;
using CipherPark.Aurora.Core.Systems;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Utils
{
    public class GameAssets
    {
        private IGameApp _game = null;
        private EffectsCollection _effects = new EffectsCollection();
        private ModelCollection _models = new ModelCollection();
        private CameraCollection _cameras = new CameraCollection();
        private ObjectCollection _misc = new ObjectCollection();
        private SourceVoiceCollection _sounds = new SourceVoiceCollection();
        private AudioStreamCollection _music = new AudioStreamCollection();
        private ParticleSystemCollection _systems = new ParticleSystemCollection();

        public GameAssets(IGameApp game)
        {
            _game = game;
        }

        public IGameApp Game { get { return _game; } }

        public EffectsCollection Effects { get { return _effects; } }

        public ModelCollection Models { get { return _models; } }

        public CameraCollection Cameras { get { return _cameras; } }

        public ObjectCollection Misc { get { return _misc; } }

        public SourceVoiceCollection Sounds { get { return _sounds; } }

        public AudioStreamCollection Music { get { return _music; } }

        public ParticleSystemCollection Systems { get { return _systems; } } 
    }

    public class EffectsCollection : Dictionary<string, Effect> { }

    public class ModelCollection : Dictionary<string, Model> { }

    public class CameraCollection : Dictionary<string, Camera> { }

    public class ObjectCollection : Dictionary<string, object> { }

    public class SourceVoiceCollection : Dictionary<string, SourceVoice> { }

    public class AudioStreamCollection : Dictionary<string, XAudio2StreamingManager> { }

    public class ParticleSystemCollection : Dictionary<string, ParticleSystem> { }
}
