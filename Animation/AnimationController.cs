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
    public class ParticleSystemController : IAnimationController
    {
        private long? _animationStartTime = null;
        private long? _lastEmitTime = null;

        public Emitter Target { get; set; }
        public List<EmitterAction> Actions { get; set; }
        public ParticleSolver Solver { get; set; }
        public bool ExplicitEmissionsOnly { get; set; }

        public void Start()
        {
            
        }

        public void UpdateAnimation(long gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime;
            
            long animationTime = gameTime - _animationStartTime.Value;

            if( Target != null)
            {
                if (_lastEmitTime == null)
                    _lastEmitTime = gameTime;

                if (!ExplicitEmissionsOnly && gameTime - _lastEmitTime > 1000)
                    Target.Emit();

                if (Actions != null)
                {
                    foreach (EmitterAction action in this.Actions)
                    {
                        if (action.Time > (ulong)(gameTime - _animationStartTime.Value))
                        {
                            if ((action.Tasks & EmitterAction.EmitterTasks.KillAll) != 0)
                                Target.KillAll();
                            else if ((action.Tasks & EmitterAction.EmitterTasks.Kill) != 0)
                                Target.Kill(action.ParticleArg1);

                            if ((action.Tasks & EmitterAction.EmitterTasks.Emit) != 0)
                                Target.Emit();

                            if ((action.Tasks & EmitterAction.EmitterTasks.EmitParticle) != 0)
                                Target.Emit(action.CustomParticleDescriptionArg);

                            if ((action.Tasks & EmitterAction.EmitterTasks.Link) != 0)
                                Target.Link(action.ParticleArg1, action.ParticleArg2);
                        }
                    }
                }
           
                foreach (Particle p in Target.Particles)
                {
                    //update particle age.
                    p.Age = (ulong)animationTime;
                    if (p.Age > p.Life)
                        Target.Kill(p);
                    else
                    {
                        float normalizedAge = p.Age / (float)p.Life;
                        p.Color = p.SharedDescription.ColorOverLife.GetValueAtAge(normalizedAge);
                        p.Opacity = p.SharedDescription.OpacityOverLife.GetValueAtAge(normalizedAge);
                        if( Solver != null )
                            Solver.UpdateParticleTransform((ulong)animationTime, p);
                    }
                }
            }
        }      
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