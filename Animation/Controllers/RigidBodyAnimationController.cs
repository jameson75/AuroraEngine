using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.Kinetics;
using CipherPark.AngelJacket.Core.World.Geometry;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Animation.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class RigidBodyAnimationController : AnimationController
    {
        private long? _animationStartTime = null;

        public IRigidBody Target { get; set; }

        public Motion Delta { get; set; }

        public RigidBodyAnimationController()
        {
            Delta = Motion.Identity;
        }

        public RigidBodyAnimationController(Motion delta, IRigidBody target)
        {
            Delta = delta;
            Target = target;
        }

        #region IAnimationController Members

        public override void Reset()
        {
            _animationStartTime = null;
        }

        public override void UpdateAnimation(GameTime gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime.GetTotalSimtime();

            ulong elapsedTime = (ulong)(gameTime.GetTotalSimtime() - _animationStartTime.Value);

            if (Target != null)
                Target.Transform = new Transform(Delta.GetTransformationAtT((float)elapsedTime / 1000.0f));

            //OnAnimationComplete();
        }

        #endregion
    }
}
