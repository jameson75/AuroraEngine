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
                    return Transform.Identity; // _cachedTransform;
            }
            set
            {
                if (Camera != null)
                {
                    //Transform, but track the look-at target if one was specified.
                    if (LookAtTarget != null)
                        Camera.ViewMatrix = TransformToLookAtTargetMatrix(value);
                    //Transform exactly as specified.
                    else
                        Camera.ViewMatrix = Camera.TransformToViewMatrix(value);
                }
                //else
                //    _cachedTransform = value;
            }
        }     

        public override void Update(GameTime gameTime)
        {
            //Update, but track the look-at-target, if one was specified.
            if (LookAtTarget != null && Camera != null)
                Camera.ViewMatrix = TransformToLookAtTargetMatrix(Transform);            
                        
            base.Update(gameTime);
        }
        
        private Matrix TransformToLookAtTargetMatrix(Transform t)
        {
            Matrix viewMatrix = Camera.TransformToViewMatrix(t);
            Vector3 up = LookAtUp != null ? LookAtUp.Value : new Vector3(viewMatrix.Column2.ToArray().Take(3).ToArray());
            Vector3 lookAt = this.WorldToParent(LookAtTarget.ParentToWorld(LookAtTarget.Transform)).Translation;
            Vector3 eye = t.Translation;
            return Matrix.LookAtLH(eye, lookAt, up);           
        }      
    }

    public class CameraLookAtAnimationController : AnimationController
    {
        private long? _animationStartTime = null;
        private Vector3 _startLookAt = Vector3.Zero;

        public CameraSceneNode CameraNode { get; set; }
        public ITransformable Target { get; set; }
        public Vector3? EndLookAt { get; set; }
        public ulong PanTime { get; set; }
        public bool LockTargetOnComplete { get; set; }

        public override void Start()
        {
            _animationStartTime = null;
        }

        public override void UpdateAnimation(GameTime gameTime)
        {
            //Both cannot be specified.
            if(Target != null && EndLookAt != null)
                throw new InvalidOperationException("A transformable target and end-look-at vector were both specified");

            //Exactly one must be specified.
            if(Target == null && EndLookAt == null)
                throw new InvalidOperationException("Niether transformable target nor end-look-at vector was specified.");

            if(CameraNode == null)
                throw new InvalidOperationException("CameraNode property was null");

            if(PanTime <= 0)
                throw new InvalidOperationException("PanTime was not greater than zero");

            if (_animationStartTime == null)
            {
                _animationStartTime = gameTime.GetTotalSimtime();
                _startLookAt = new Vector3(CameraNode.Camera.ViewMatrix.Column2.ToArray().Take(3).ToArray());               
            }          
 
            ulong elapsedTime = (ulong)(gameTime.GetTotalSimtime() - _animationStartTime.Value);
            float step = (float)elapsedTime / (float)PanTime;
            Vector3 currentLookAt = Vector3.Zero;

            if(Target != null )
            {
                
            }
        }
    }
}
