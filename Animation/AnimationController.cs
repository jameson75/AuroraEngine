using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.Kinetics;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.World.ParticleSystem;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Animation
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAnimationController
    {       
        void Start();
        void UpdateAnimation(long gameTime);
    }

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
    public class EmitterAnimationController : IAnimationController
    {
        private long? _animationStartTime = null;

        public Emitter Target { get; set; }

        public void Start()
        {
            
        }

        public void UpdateAnimation(long gameTime)
        {
            foreach(Emission emission in this.Emissions)
            {
                if (emission.Time > gameTime - _animationStartTime.Value)
                {
                    if ((emission.Tasks & Emission.EmissionTask.KillAll) != 0)
                        Target.KillAll();
                    else if ((emission.Tasks & Emission.EmissionTask.Kill) != 0)
                        Target.Kill(emission.Particle1);

                    if ((emission.Tasks & Emission.EmissionTask.Emit) != 0)
                        Target.Emit();

                    if ((emission.Tasks & Emission.EmissionTask.EmitParticle) != 0)
                        Target.Emit(emission.EmissionDescription);

                    if ((emission.Tasks & Emission.EmissionTask.Link) != 0)
                        Target.Link(emission.Particle1, emission.Particle2);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParticleAnimationController : IAnimationController
    {        
        private long? _animationStartTime = null;
        #region IAnimationController Members
        public Emitter Target { get; set; }

        public void Start()
        {
            
        }

        public void UpdateAnimation(long gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime;
            foreach (Particle p in Target.Particles)
            {
                if (p.Age > p.Life)
                    Target.Kill(p);
                else
                {

                } 
            }
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
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

        public virtual void UpdateAnimation(long gameTime)
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