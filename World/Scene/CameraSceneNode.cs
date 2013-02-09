﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;

namespace CipherPark.AngelJacket.Core.World.Scene
{
    public class CameraSceneNode : SceneNode
    {
        private Matrix _cachedViewMatrix = Matrix.Zero;

        public CameraSceneNode(Scene scene)
            : base(scene)
        {

        }

        public SceneNode LockOnTarget { get; set; }

        public CameraSceneNode(Scene scene, Camera camera)
            : base(scene)
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
                    //Vector3 oldZAxis = new Vector3(Camera.ViewMatrix.Column3.ToArray().Take(3).ToArray());
                    //Vector3 oldYAxis = new Vector3(Camera.ViewMatrix.Column2.ToArray().Take(3).ToArray());
                    //Vector3 oldLocation = (Camera.ViewMatrix * Matrix.Invert(Camera.ViewMatrix * Matrix.Translation(-Camera.ViewMatrix.TranslationVector))).TranslationVector;
                    ////Determine look at vector.
                    //Vector3 lookAt = LockOnTarget.LocalToWorld(LockOnTarget.Transform).Translation;
                    ////Determine look at 
                    //Vector3 location = LocalToWorld(value).Translation;
                    ////Vector3 up = DetermineUpVector(location, lookAt);                    
                    //Vector3 newZAxis = Vector3.Normalize(Vector3.Subtract(lookAt, location));
                    ////if (oldLocation != location)
                    ////{
                    //Vector3 rotAxis = Vector3.Cross(oldZAxis, newZAxis);
                    //float cosTheta = Vector3.Dot(oldZAxis, newZAxis);
                    //Vector3 up = new Vector3(Vector3.Transform(oldZAxis, Matrix.RotationAxis(rotAxis, cosTheta)).ToArray().Take(3).ToArray());
                    ////Mat
                    ////-(Camera.ViewMatrix * Matrix.Invert(Camera.ViewMatrix * Matrix.Translation(-Camera.ViewMatrix.TranslationVector)))
                    ////Vector3 worldViewFrom = -(Camera.ViewMatrix * Matrix.Invert(Camera.ViewMatrix * Matrix.Translation(-Camera.ViewMatrix.TranslationVector))).TranslationVector + location.TranslationVector;
                    ////Vector3 targetWorldPosition = (lookAt * Matrix.Invert(lookAt * Matrix.Translation(-lookAt.TranslationVector))).TranslationVector;
                    //Camera.ViewMatrix = Matrix.LookAtLH(location, lookAt, up);
                    ////}
                    ////else 
                    ////{
                    ////    Vector3 up = new Vector3(Vector3.Transform(

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

        //private Vector3 DetermineUpVector(Vector3 camLocation, Vector3 camLookAt)
        //{
        //    Vector4 r3 = LocalToWorld(this.Transform).ToMatrix().Row3;
        //    Vector4 r2  = LocalToWorld(this.Transform).ToMatrix().Row2;
        //    Vector3 orientationZ = Vector3.Normalize(new Vector3(r3.X, r3.Y, r3.Z));
        //    Vector3 orientationY = Vector3.Normalize(new Vector3(r2.X, r2.Y, r3.Z));          
        //    Vector3 u = Vector3.Normalize(camLookAt - camLocation);
        //    Vector3 v = orientationZ;
        //    Vector3 n = Vector3.Cross(u, v);
        //    float cosTheta = Vector3.Dot(u, v);           
        //    float theta = (float)Math.Acos(cosTheta);
        //    float thetaDegrees = MathUtil.RadiansToDegrees(theta); //for debugging purposes.            
        //    Vector3 up = Vector3.Normalize(Vector3.Transform(orientationY, Quaternion.RotationAxis(n, theta)));
        //    return up;
        //}
    }
}
