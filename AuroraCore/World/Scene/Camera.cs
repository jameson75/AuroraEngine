﻿using SharpDX;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Effects;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World
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
            _game.BuffersResizing += _game_BuffersResizing;
            _game.BuffersResized += _game_BuffersResized;
        }

        private void _game_BuffersResizing()
        {
            _postEffectChain.Dispose();
        }

        private void _game_BuffersResized()
        {            
            _postEffectChain.Initialize();          
        }

        public Camera(IGameApp game, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _game = game;          
            _postEffectChain = PostEffectChain.Create(game);
            ViewMatrix = viewMatrix;
            ProjectionMatrix = projectionMatrix;
            _game.BuffersResizing += _game_BuffersResizing;
            _game.BuffersResized += _game_BuffersResized;            
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

        public Vector3 Forward
        {
            get
            {
                return Camera.ViewMatrixToTransform(this.ViewMatrix).ToMatrix().Backward;
            }
        }

        public Vector3 Up
        {
            get
            {
                return Camera.ViewMatrixToTransform(this.ViewMatrix).ToMatrix().Down;
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
            return new Transform(cameraRotation, cameraTranslation, 1);
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
