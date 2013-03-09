using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Scene
{
    public class CameraSceneNode : SceneNode
    {
        private Matrix _cachedViewMatrix = Matrix.Zero;

        public CameraSceneNode(IGameApp game)
            : base(game)
        {

        }

        public SceneNode LockOnTarget { get; set; }

        public CameraSceneNode(Camera camera, string name = null)
            : base(camera.Game, name)
        {
            Camera = camera;
        }

        public Camera Camera { get; set; }

        public override void Update(long gameTime)
        {
            if (Camera != null)
            {
                ////TODO: Figure out how to use
                //if (this.LockOnTarget != null)
                //{
                //    Matrix targetTransform = LockOnTarget.LocalToWorld(LockOnTarget.Transform.ToMatrix());
                //    Matrix cameraTransform = LocalToWorld(this.Transform.ToMatrix());
                //    Vector3 worldViewFrom = -(Camera.ViewMatrix * Matrix.Invert(Camera.ViewMatrix * Matrix.Translation(-Camera.ViewMatrix.TranslationVector))).TranslationVector + cameraTransform.TranslationVector;
                //    Vector3 targetWorldPosition = (targetTransform * Matrix.Invert(targetTransform * Matrix.Translation(-targetTransform.TranslationVector))).TranslationVector;
                //    Camera.ViewMatrix = Matrix.LookAtLH(worldViewFrom, targetWorldPosition, Vector3.UnitY);
                //}
                //else 
                //    Camera.ViewMatrix = Matrix.Translation(-LocalToWorld(Transform.ToMatrix()).TranslationVector) * _cachedViewMatrix;

                //if (this.LockOnTarget != null)
                //{   
                //    //Determine look at vector.
                //    Vector3 lookAt = LockOnTarget.LocalToWorld(LockOnTarget.Transform).Translation;
                //    //Determine look at 
                //    Vector3 location = LocalToWorld(this.Transform).Translation;
                //    Vector3 up = DetermineUpVector(location, lookAt);                  
                //    //Mat
                //    //-(Camera.ViewMatrix * Matrix.Invert(Camera.ViewMatrix * Matrix.Translation(-Camera.ViewMatrix.TranslationVector)))
                //    //Vector3 worldViewFrom = -(Camera.ViewMatrix * Matrix.Invert(Camera.ViewMatrix * Matrix.Translation(-Camera.ViewMatrix.TranslationVector))).TranslationVector + location.TranslationVector;
                //    //Vector3 targetWorldPosition = (lookAt * Matrix.Invert(lookAt * Matrix.Translation(-lookAt.TranslationVector))).TranslationVector;
                //    Camera.ViewMatrix = Matrix.LookAtLH(location, lookAt, up);
                //}

                //else
                //    Camera.ViewMatrix = Matrix.Translation(-LocalToWorld(Transform.ToMatrix()).TranslationVector) * _cachedViewMatrix;

            }
        }

        public override Transform Transform
        {
            get
            {
                return Camera.ViewToTransform(Camera.ViewMatrix);
            }
            set
            {
                if (LockOnTarget != null)
                {
                    Matrix specifiedNewView = Camera.TransformToView(value);
                    Vector3 up = new Vector3(specifiedNewView.Column2.ToArray().Take(3).ToArray());
                    Vector3 lookAt = LockOnTarget.LocalToWorld(LockOnTarget.Transform).Translation;
                    Vector3 eye = LocalToWorld(value).Translation;
                    Camera.ViewMatrix = Matrix.LookAtLH(eye, lookAt, up);
                }
                else
                    Camera.ViewMatrix = Camera.TransformToView(LocalToWorld(value));
            }
        }
    }
}
