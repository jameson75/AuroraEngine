﻿using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.World;
using CipherPark.AngelJacket.Core.World.Renderers;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

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
        private EffectsCollection _effects = new EffectsCollection();
        private ModelCollection _models = new ModelCollection();
        private CameraCollection _cameras = new CameraCollection();
        private ObjectCollection _misc = new ObjectCollection();
        private SourceVoiceCollection _sounds = new SourceVoiceCollection();
        private AudioStreamCollection _music = new AudioStreamCollection();

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
    }

    public class EffectsCollection : Dictionary<string, Effect> { }

    public class ModelCollection : Dictionary<string, Model> { }

    public class CameraCollection : Dictionary<string, Camera> { }

    public class ObjectCollection : Dictionary<string, object> { }

    public class SourceVoiceCollection : Dictionary<string, SourceVoice> { }

    public class AudioStreamCollection : Dictionary<string, XAudio2StreamingManager> { }
}
