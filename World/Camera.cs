using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

namespace CipherPark.AngelJacket.Core.World
{
    public class Camera
    {
        private IGameApp _game = null;
        private PostEffectChain _postEffectChain = null;

        public Camera(IGameApp game)
        {
            _game = game;
            _postEffectChain = PostEffectChain.Create(game);
            ViewMatrix = Matrix.Identity;
            ProjectionMatrix = Matrix.Identity;
        }

        public Camera(IGameApp game, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _game = game;
            ViewMatrix = viewMatrix;
            ProjectionMatrix = projectionMatrix;
        }

        public IGameApp Game { get { return _game; } }
        
        public Matrix ViewMatrix { get; set; }

        //public Model LockonTarget { get; set; }        
       
        public Matrix ProjectionMatrix { get; set; }

        public PostEffectChain PostEffectChain { get { return _postEffectChain; } }
    }

    
}
