using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

namespace CipherPark.AngelJacket.Core.Effects
{
    public class BasicEffectEx
    {
        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;
        private BasicEffect _effect = null;
        private bool _enableVertexColor = false;

        public BasicEffectEx(Device device)
        {
            _effect = new BasicEffect(device);
        }

        public Matrix World
        {
            get { return _world; }
            set
            {
                _world = value;
                _effect.SetWorld(_world);
            }
        }

        public Matrix View
        {
            get { return _view; }
            set
            {
                _view = value;
                _effect.SetView(_view);
            }
        }

        public Matrix Projection
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

        public void Apply()
        {
            _effect.Apply();
        }

        public byte[] SelectShaderByteCode()
        {
            return _effect.SelectShaderByteCode();
        }
    }
}
