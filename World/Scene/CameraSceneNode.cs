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
        private Transform _cachedTransform = Transform.Identity;
   

        public CameraSceneNode(IGameApp game)
            : base(game)
        { }       

        public CameraSceneNode(Camera camera, string name = null)
            : base(camera.Game, name)
        {
            Camera = camera;
        }

        public ITransformable LookAtTarget { get; set; }

        public Vector3? LookAtUp { get; set; }

        public Camera Camera { get; set; }

        public override Transform Transform
        {
            get
            {
                if (Camera != null)
                    return Camera.ViewMatrixToTransform(Camera.ViewMatrix);
                else
                    return _cachedTransform;
            }
            set
            {
                if (Camera != null)
                {
                    if (LookAtTarget != null)
                        Camera.ViewMatrix = TransformToLookAtTargetMatrix(value);
                    else
                        Camera.ViewMatrix = Camera.TransformToViewMatrix(value);
                }
                else
                    _cachedTransform = value;
            }
        }

        public override void Update(long gameTime)
        {
            if (LookAtTarget != null && Camera != null)            
                Camera.ViewMatrix = TransformToLookAtTargetMatrix(Transform);
            
            base.Update(gameTime);
        }        

        public Matrix TransformToLookAtTargetMatrix(Transform t, ITransformable lookAtTarget, Vector3? lookAtUp )
        {
            Matrix viewMatrix = Camera.TransformToViewMatrix(t);
            Vector3 up = lookAtUp != null ? lookAtUp.Value : new Vector3(viewMatrix.Column2.ToArray().Take(3).ToArray());
            Vector3 lookAt = this.WorldToParent(lookAtTarget.ParentToWorld(lookAtTarget.Transform)).Translation;
            Vector3 eye = t.Translation;
            return Matrix.LookAtLH(eye, lookAt, up);
        }
        
        private Matrix TransformToLookAtTargetMatrix(Transform t)
        {
            //Matrix viewMatrix = Camera.TransformToViewMatrix(t);
            //Vector3 up = LookAtUp != null ? LookAtUp.Value : new Vector3(viewMatrix.Column2.ToArray().Take(3).ToArray());
            //Vector3 lookAt = this.WorldToParent(LookAtTarget.ParentToWorld(LookAtTarget.Transform)).Translation;
            //Vector3 eye = t.Translation;
            //return Matrix.LookAtLH(eye, lookAt, up);
            return TransformToLookAtTargetMatrix(t, LookAtTarget, LookAtUp);
        }
    }
}
