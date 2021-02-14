﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.KillScript.Core.Animation;
using CipherPark.KillScript.Core.World.Geometry;
using CipherPark.KillScript.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.World.Scene
{
    public class CameraSceneNode : SceneNode
    {
        private Transform _nodeTransform = Transform.Identity;

        public CameraSceneNode(IGameApp game, string name = null)
            : base(game, name)
        { }       

        public CameraSceneNode(Camera camera, string name = null)
            : base(camera.Game, name)
        {
            Camera = camera;
        }

        public ITransformable LockInTarget { get; set; }

        public Vector3? LockUp { get; set; }      

        public Camera Camera { get; set; }

        /* These methods have been replaced by the alternate methods below
         * These methods manipulate the camera's view matrix such that the view
         * matrix represents the local transform the of camera node.
         * which makes it impractical to utilize the camera's view matrix directly.
         * The methods below make this practicle by manipulating the camera's view matrix
         * such that camera's view matrix always represents the world transform of the camer node.
         * /         
        public override Transform Transform
        {                        
            get
            {
                if (Camera != null)
                    return Camera.ViewMatrixToTransform(Camera.ViewMatrix);
                else
                    return Transform.Identity;
            }
            set
            {
                if (Camera != null)
                {
                    //Transform, but track the lock-in target if one was specified.
                    if (LockInTarget != null)
                        Camera.ViewMatrix = TransformToLockInViewMatrix(value);
                    //Transform exactly as specified.
                    else
                        Camera.ViewMatrix = Camera.TransformToViewMatrix(value);
                }                
            }
        }     

        public override void Update(GameTime gameTime)
        {
            //Update, but track the lock-in-target, if one was specified.
            if (Camera != null && LockInTarget != null)
                Camera.ViewMatrix = TransformToLockInViewMatrix(Transform);            
                        
            base.Update(gameTime);
        }

        public void LookAtTarget(Vector3 lookAtTarget)
        {
            Vector3 eye = Camera.ViewMatrix.TranslationVector;
            Vector3 target = this.WorldToParent(new Transform(lookAtTarget)).Translation;
            Vector3 up = Vector3.Zero;
            if (LockUp != null)
                up = this.WorldToParent(new Transform(LockUp.Value)).Translation;
            else
                up = new Vector3(Camera.ViewMatrix.Column2.ToArray().Take(3).ToArray());
            Camera.ViewMatrix = Matrix.LookAtLH(eye, target, up);
        }
        
        private Matrix TransformToLockInViewMatrix(Transform t)
        {
            Matrix viewMatrix = Camera.TransformToViewMatrix(t);                    
            Vector3 up = LockUp != null ? LockUp.Value //TODO: Transform this NORMAL to local space.
                : new Vector3(viewMatrix.Column2.ToArray().Take(3).ToArray());
            Vector3 target = this.WorldToParent(LockInTarget.ParentToWorld(LockInTarget.Transform)).Translation;
            Vector3 eye = t.Translation;
            return Matrix.LookAtLH(eye, target, up);           
        }
        */

        public override Transform Transform
        {
            get
            {
                if (Camera != null)
                    return this.WorldToParent(Camera.ViewMatrixToTransform(Camera.ViewMatrix));
                else
                    return Transform.Identity;
            }
            set
            {
                if (Camera != null)
                {
                    //Transform, but track the lock-in target if one was specified.
                    if (LockInTarget != null)
                        Camera.ViewMatrix = LocalTransformToLockInViewMatrix(value);
                    //Transform exactly as specified.
                    else
                        Camera.ViewMatrix = Camera.TransformToViewMatrix(this.ParentToWorld(value));
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            //Update, but track the lock-in-target, if one was specified.
            if (Camera != null && LockInTarget != null)
                Camera.ViewMatrix = this.ParentToWorld(LocalTransformToLockInViewMatrix(Transform));

            base.Update(gameTime);
        }

        public void LookAtTarget(Vector3 lookAtTarget)
        {
            Vector3 eye = Camera.ViewMatrix.TranslationVector;
            Vector3 target = lookAtTarget;
            Vector3 up = LockUp ?? new Vector3(Camera.ViewMatrix.Column2.ToArray().Take(3).ToArray());
            Camera.ViewMatrix = Matrix.LookAtLH(eye, target, up);
        }

        private Matrix LocalTransformToLockInViewMatrix(Transform t)
        {
            Matrix viewMatrix = Camera.TransformToViewMatrix(this.ParentToWorld(t));
            //NOTE: It's expected that LockUp vector is in world coordinates.
            Vector3 up = LockUp != null ? LockUp.Value :
                                          new Vector3(viewMatrix.Column2.ToArray().Take(3).ToArray());
            Vector3 target = LockInTarget.WorldTransform().Translation;
            Vector3 eye = t.Translation;
            return Matrix.LookAtLH(eye, target, up);
        }
    }

    public class CameraLookAtAnimationController : SimulatorController
    {
        private long? _animationStartTime = null;       
        private Transform _startCamTransform = Transform.Identity;

        public CameraSceneNode CameraNode { get; set; }
        public ITransformable LockInTarget { get; set; }
        public Vector3? LookAtTarget { get; set; }
        public Vector3? LockUp { get; set; }
        public ulong AnimationRunningTime { get; set; }
        public bool PreserveLockIn { get; set; }

        protected override void OnSimulationReset()
        {
            _animationStartTime = null;
        }

        public override void Update(GameTime gameTime)
        {
            //Both cannot be specified.
            if(LockInTarget != null && LookAtTarget != null)
                throw new InvalidOperationException("A transformable target and end-look-at vector were both specified");

            //Exactly one must be specified.
            if(LockInTarget == null && LookAtTarget == null)
                throw new InvalidOperationException("Niether transformable target nor end-look-at vector was specified.");

            if(CameraNode == null)
                throw new InvalidOperationException("CameraNode property was null");

            if(AnimationRunningTime <= 0)
                throw new InvalidOperationException("PanTime was not greater than zero");

            Vector3 camEye = CameraNode.Transform.Translation;
            
            if (_animationStartTime == null)
            {
                _animationStartTime = gameTime.GetTotalSimtime();               
                _startCamTransform = CameraNode.Transform;
                CameraNode.LockInTarget = null;
                CameraNode.LockUp = null;
            }          
 
            ulong elapsedTime = (ulong)(gameTime.GetTotalSimtime() - _animationStartTime.Value);
            float step = MathUtil.Clamp((float)elapsedTime / (float)AnimationRunningTime, 0, 1);            
            
            Vector3 endTarget = Vector3.Zero;            
            if (LockInTarget != null)
                endTarget = CameraNode.WorldToParent(LockInTarget.ParentToWorld(LockInTarget.Transform)).Translation;
            else
                endTarget = CameraNode.WorldToParent(new Transform(LookAtTarget.Value)).Translation;  
            
            Vector3 endUp = Vector3.Zero;
            if(LockUp != null)
               endUp = LockUp.Value; //TODO: Transform this NORMAL to local space.
            else
               endUp = new Vector3(CameraNode.Camera.ViewMatrix.Column2.ToArray().Take(3).ToArray());      

            Matrix endViewMatrix = Matrix.LookAtLH(camEye, endTarget, endUp);
            Transform endCamTransform = Camera.ViewMatrixToTransform(endViewMatrix);

            CameraNode.Transform = Transform.Lerp(_startCamTransform, endCamTransform, step);

            if (elapsedTime >= AnimationRunningTime && !IsSimulationFinal)
            {
                if (PreserveLockIn)
                {
                    CameraNode.LockInTarget = this.LockInTarget;
                    CameraNode.LockUp = this.LockUp;
                }
                OnSimulationComplete();
            }
        }       
    }
}
