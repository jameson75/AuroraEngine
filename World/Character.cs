using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Interop;
using CipherPark.AngelJacket.Core.World;
using CipherPark.AngelJacket.Core.Services;

namespace CipherPark.AngelJacket.Core.World
{
    public class Model
    {
        private IGameApp _game = null;

        public Model(IGameApp game)
        {
            _game = game;
        }


        public IGameApp Game { get { return _game; } }

        public Mesh Mesh { get; set; }

        public IGameEffect Effect { get; set; }
    }

    public interface IGameEffect
    {

    }

    public class BasicGameEffect : IGameEffect
    {
        private BasicEffect _innerEffect = null;

        public new BasicGameEffect(BasicEffect basicEffect)
        {
            _innerEffect = basicEffect;
        }
    }
}
