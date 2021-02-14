using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.KillScript.Core.Module;
using CipherPark.KillScript.Core.Systems;
using CipherPark.KillScript.Core.World.Geometry;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.Animation.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class KeyframeAnimationController : SimulatorController
    {
        private long? _animationStartTime = null;

        public bool Loop { get; set; }

        public KeyframeAnimationController()
            : base()
        { }

        public KeyframeAnimationController(TransformAnimation animation, ITransformable target)
        {
            Target = target;
            Animation = animation;
        }

        protected override void OnSimulationReset()
        {
            _animationStartTime = null;
        }

        public override void Update(GameTime gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime.GetTotalSimtime();

            ulong elapsedTime = (ulong)(gameTime.GetTotalSimtime() - _animationStartTime.Value);

            if (Target != null && Animation != null)
            {
                Target.Transform = Animation.GetValueAtT(elapsedTime);
                //check whether we've updated the target with the last key frame.
                if (elapsedTime >= Animation.RunningTime && !IsSimulationFinal)
                {
                    //if Loop is specified, we start the animation over by resetting the animationStartTime.
                    if (Loop)
                        _animationStartTime = null;
                    //otherwise, we signal this animation as complete                    
                    else
                        OnSimulationComplete();
                }
            }
            else
                throw new InvalidOperationException("Target or Animation was not initialized.");
        }

        public ITransformable Target { get; set; }

        public TransformAnimation Animation { get; set; }
    }   
}
