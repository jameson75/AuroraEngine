using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace CipherPark.AngelJacket.Core.World
{
    public class Camera
    {
        private IGameApp _game = null;    

        public Camera(IGameApp game)
        {
            _game = game;
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
        
        public Matrix ProjectionMatrix { get; set; }
    }
}
