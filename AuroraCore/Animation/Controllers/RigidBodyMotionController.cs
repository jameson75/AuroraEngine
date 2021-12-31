using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.Module;
using CipherPark.Aurora.Core.Systems;
using CipherPark.Aurora.Core.World.Geometry;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Animation.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class RigidBodyMotionController : SimulatorController
    {
        private long? _animationStartTime = null;
        private long? _lastUpdateTime = null;

        public IRigidBody Target { get; set; }       

        public RigidBodyMotionController()
        { }

        public RigidBodyMotionController(IRigidBody target)
        {           
            Target = target;
        }

        #region IAnimationController Members

        protected override void OnSimulationReset()
        {
            _animationStartTime = null;
            _lastUpdateTime = null;
        }

        public override void Update(GameTime gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime.GetTotalSimtime();

            if (_lastUpdateTime == null)
                _lastUpdateTime = _animationStartTime;

            ulong windowT = (ulong)(gameTime.GetTotalSimtime() - _lastUpdateTime.Value);

            if (Target != null)
                TransformTarget(windowT);

            _lastUpdateTime = gameTime.GetTotalSimtime();
        }

        /// <summary>
        /// Displaces an object in motion for the specified time window.
        /// </summary>
        /// <param name="windowT">Length of the time window in milliseconds</param>
        /// <returns></returns>
        public void TransformTarget(ulong windowT)
        {
            //TODO: Implement angular transform.             
            if (Target.BodyMotion.LinearVelocity >= 0)
            {
                float cWindowT = 0;
                switch (Target.BodyMotion.TimeDomainUnits)
                {
                    case Systems.TimeDomainUnits.Seconds:
                        cWindowT = (float)windowT / 1000.0f;
                        break;
                    case Systems.TimeDomainUnits.Minutes:
                        cWindowT = (float)windowT / 60000.0f;
                        break;
                    case Systems.TimeDomainUnits.Hours:
                        cWindowT = (float)windowT / 3600000.0f;
                        break;
                    case TimeDomainUnits.Milliseconds:
                        cWindowT = (float)windowT;
                        break;
                }
                float displacementDistance = cWindowT * Target.BodyMotion.LinearVelocity;
                Vector3 displacementVector = displacementDistance * Vector3.Normalize(Target.WorldToParentNormal(Target.BodyMotion.Direction));
                Vector3 newTranslation = Target.Transform.Translation + displacementVector;
                Target.Transform = new Transform(Target.Transform.Rotation, newTranslation);
            }
        }
        #endregion
    }
}
