using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace CipherPark.AngelJacket.Core.World
{
    public class Camera
    {
        public Matrix ViewMatrix = Matrix.Identity;
        public Matrix ProjectionMatrix = Matrix.Identity;

        public Camera()
        { }

        public Camera(Matrix viewMatrix, Matrix projectionMatrix)
        {
            ViewMatrix = viewMatrix;
            ProjectionMatrix = projectionMatrix;
        }
    }
}
