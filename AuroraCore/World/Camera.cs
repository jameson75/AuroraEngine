using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Effects;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World
{
    public class Camera
    {   
        private PostEffectChain _postEffectChain = null;
        private IGameApp _game = null;

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
            _postEffectChain = PostEffectChain.Create(game);
            ViewMatrix = viewMatrix;
            ProjectionMatrix = projectionMatrix;
        }

        public IGameApp Game { get { return _game; } }
        
        public Matrix ViewMatrix { get; set; }
             
        public Vector3 Location
        {
            get
            {
                return Camera.ViewMatrixToTransform(this.ViewMatrix).Translation;
            }
        }

        public Matrix ProjectionMatrix { get; set; }

        public PostEffectChain PostEffectChain { get { return _postEffectChain; } }

        public static Transform ViewMatrixToTransform(Matrix viewMatrix)
        {
            Matrix viewRotation = viewMatrix * Matrix.Translation(-viewMatrix.TranslationVector);
            Vector3 viewTranslation = (viewMatrix * Matrix.Invert(viewRotation)).TranslationVector;
            Quaternion q = Quaternion.RotationMatrix(viewRotation);
            Quaternion cameraRotation = new Quaternion(q.X, q.Y, q.Z, -q.W);
            Vector3 cameraTranslation = -viewTranslation;
            return new Transform(cameraRotation, cameraTranslation);
        }

        public static Matrix TransformToViewMatrix(Transform transform)
        {
            Quaternion q = new Quaternion(transform.Rotation.X, transform.Rotation.Y, transform.Rotation.Z, -transform.Rotation.W);
            Matrix viewRotation = Matrix.RotationQuaternion(q);
            Matrix viewTranslation = Matrix.Translation(-transform.Translation);
            return viewTranslation * viewRotation;
        }
    }    
}
