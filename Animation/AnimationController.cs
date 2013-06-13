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

namespace CipherPark.AngelJacket.Core.Animation
{
    public interface IAnimationController
    {       
        void Start();
        void UpdateAnimation(long gameTime);
    }

    public class KeyframeAnimationController : IAnimationController
    {
        private long? _animationStartTime = null;

        public KeyframeAnimationController() : base()
        { }

        public KeyframeAnimationController(TransformAnimation animation, ITransformable target)           
        {   
            Target = target;
            Animation = animation;
        }

        public void Start()
        {         
            _animationStartTime = null;
        }

        public void UpdateAnimation(long gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime;
            ulong timeT = (ulong)(gameTime - _animationStartTime.Value);
            if(Target != null && Animation != null)            
                Target.Transform = Animation.GetValueAtT(timeT);
        }

        public ITransformable Target { get; set; }

        public TransformAnimation Animation { get; set; }
    }

    public class ParticleSystemAnimationController : IAnimationController
    {        
        #region IAnimationController Members

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void UpdateAnimation(long gameTime)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class RigidBodyAnimationController : IAnimationController
    {
        private long? _animationStartTime = null;   
        
        public NetForce Motion { get; set; } 

        public IRigidBody Target { get; set; }
        
        public RigidBodyAnimationController()
        { }
       
        public RigidBodyAnimationController(NetForce motion, IRigidBody rigidBody)
        {
            Motion = motion;
            Target = rigidBody;
        }     

        #region IAnimationController Members   

        public void Start()
        {
            _animationStartTime = null;           
        }

        public void UpdateAnimation(long gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime;
            ulong timeT = (ulong)(gameTime - _animationStartTime.Value);
            if (Target != null && Motion != null)
            {
                Target.Transform = Transform.Multiply(Target.Transform, Motion.GetTransformDeltaAtT(timeT));
                Motion.RemoveImpulses();
            }
        }

        #endregion
    }
}