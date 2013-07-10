﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Effects
{
    public class BasicEffectEx : Effect
    {
        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;
        private BasicEffect _effect = null;
        private bool _enableVertexColor = false;

        public BasicEffectEx(Device device) : base(device)
        {
            _effect = new BasicEffect(device);
        }

        public override Matrix World
        {
            get { return _world; }
            set
            {
                _world = value;
                _effect.SetWorld(_world);
            }
        }

        public override Matrix View
        {
            get { return _view; }
            set
            {
                _view = value;
                _effect.SetView(_view);
            }
        }

        public override Matrix Projection
        {
            get { return _projection; }
            set
            {
                _projection = value;
                _effect.SetProjection(_projection);
            }
        }

        public bool EnableVertexColor
        {
            get { return _enableVertexColor; }
            set
            {
                _enableVertexColor = value;
                _effect.SetVertexColorEnabled(_enableVertexColor);
            }
        }

        public void EnableDefaultLighting()
        {
            _effect.EnableDefaultLighting();
        }

        public override void Apply()
        {
            _effect.Apply();
        }

        public override byte[] SelectShaderByteCode()
        {
            return _effect.SelectShaderByteCode();
        }

        public void SetTexture(ShaderResourceView texture)
        {
            _effect.SetTexture(texture);
        }

        public void SetTextureEnabled(bool enabled)
        {
            _effect.SetTextureEnabled(enabled);
        }
    }
}
