﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using CipherPark.AngelJacket.Core.Animation;

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

        public static Transform ViewToTransform(Matrix viewMatrix)
        {
            Matrix viewRotation = viewMatrix * Matrix.Translation(-viewMatrix.TranslationVector);
            Vector3 viewTranslation = (viewMatrix * Matrix.Invert(viewRotation)).TranslationVector;
            Quaternion q = Quaternion.RotationMatrix(viewRotation);
            Quaternion cameraRotation = new Quaternion(q.X, q.Y, q.Z, -q.W);
            Vector3 cameraTranslation = -viewTranslation;
            return new Transform(cameraRotation, cameraTranslation);
        }

        public static Matrix TransformToView(Transform transform)
        {
            Quaternion q = new Quaternion(transform.Rotation.X, transform.Rotation.Y, transform.Rotation.Z, -transform.Rotation.W);
            Matrix viewRotation = Matrix.RotationQuaternion(q);
            Matrix viewTranslation = Matrix.Translation(-transform.Translation);
            return viewTranslation * viewRotation;
        }
    }    
}
