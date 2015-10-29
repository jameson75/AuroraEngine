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
        private long? _lastUpdateTime = null;

        public IRigidBody Target { get; set; }
        public Motion Motion { get; set; }

        public RigidBodyAnimationController()
        {
            
        }

        public RigidBodyAnimationController(Motion motion, IRigidBody target)
        {
            Motion = motion;
            Target = target;
        }

        #region IAnimationController Members

        public override void Reset()
        {
            _animationStartTime = null;
            _lastUpdateTime = null;
        }

        public override void UpdateAnimation(GameTime gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime.GetTotalSimtime();

            if (_lastUpdateTime == null)
                _lastUpdateTime = _animationStartTime;

            ulong windowT = (ulong)(gameTime.GetTotalSimtime() - _lastUpdateTime.Value);

            if (Target != null)
                Motion.TransformTarget(windowT, Target);

            _lastUpdateTime = gameTime.GetTotalSimtime();
        }

        #endregion
    }
}
