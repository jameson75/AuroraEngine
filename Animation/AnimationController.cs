using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;

using CipherPark.AngelJacket.Core.Module;

namespace CipherPark.AngelJacket.Core.Animation
{
    public class AnimationController
    {
        private IGameApp _game = null;

        public AnimationController(IGameApp game)
        {
            _game = game;
        }

        public void Start(Animation animation)
        { }

        public void Stop(Animation animation)
        { }
    }
}