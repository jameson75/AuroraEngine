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

        public SceneNode LockOnTarget { get; set; }

        public Camera Camera
        {
            get;
            set;
        }

        public override Transform Transform
        {
            get
            {
                if (Camera != null)
                    return Camera.ViewToTransform(Camera.ViewMatrix);
                else
                    return _cachedTransform;
            }
            set
            {
                if (Camera != null)
                {
                    //***************************************************************************************
                    //Commented out because it sets the camera's view matrix in world-space... 
                    //we want the Transform to represent the view matrix the camera's local-space, instead.
                    //****************************************************************************************
                    //if (LockOnTarget != null)
                    //{
                    //    Matrix specifiedNewView = Camera.TransformToView(value);
                    //    Vector3 up = new Vector3(specifiedNewView.Column2.ToArray().Take(3).ToArray());
                    //    Vector3 lookAt = LockOnTarget.LocalToWorld(LockOnTarget.Transform).Translation;
                    //    Vector3 eye = LocalToWorld(value).Translation;
                    //    Camera.ViewMatrix = Matrix.LookAtLH(eye, lookAt, up);
                    //}
                    //else
                    //    Camera.ViewMatrix = Camera.TransformToView(LocalToWorld(value));

                    if (LockOnTarget != null)
                    {
                        Matrix specifiedNewView = Camera.TransformToView(value);
                        Vector3 up = new Vector3(specifiedNewView.Column2.ToArray().Take(3).ToArray());
                        Vector3 lookAt = this.WorldToLocal(LockOnTarget.LocalToWorld(LockOnTarget.Transform)).Translation;
                        Vector3 eye = value.Translation;
                        Camera.ViewMatrix = Matrix.LookAtLH(eye, lookAt, up);
                    }
                    else
                        Camera.ViewMatrix = Camera.TransformToView(value);
                }
                else
                    _cachedTransform = value;
            }
        }
    }
}
