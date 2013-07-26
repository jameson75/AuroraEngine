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
        bool IsAnimationComplete { get; }
        event EventHandler AnimationComplete;
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class AnimationController : IAnimationController
    {
        private bool _isAnimationComplete = false;
        
        public bool IsAnimationComplete
        {
            get { return _isAnimationComplete; }
        }

        public abstract void Start();
        
        public abstract void UpdateAnimation(long gameTime);
        
        public event EventHandler AnimationComplete;

        protected  virtual void OnAnimationComplete()
        {
            _isAnimationComplete = true;
            EventHandler handler = AnimationComplete;
            if (handler != null)
                AnimationComplete(this, EventArgs.Empty);
        }        
    }

    /// <summary>
    /// 
    /// </summary>
    public class KeyframeAnimationController : AnimationController
    {
        private long? _animationStartTime = null;

        bool Loop { get; set; }

        public KeyframeAnimationController() : base()
        { }

        public KeyframeAnimationController(TransformAnimation animation, ITransformable target)           
        {   
            Target = target;
            Animation = animation;
        }

        public override void Start()
        {         
            _animationStartTime = null;
        }

        public override void UpdateAnimation(long gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime;
            
            ulong elapsedTime = (ulong)(gameTime - _animationStartTime.Value);

            if (Target != null && Animation != null)
            {
                Target.Transform = Animation.GetValueAtT(elapsedTime);                
                //check whether we've updated the target with the last key frame.
                if (elapsedTime >= Animation.RunningTime && !IsAnimationComplete)
                {
                    //if Loop is specified, we start the animation over by resetting the animationStartTime.
                    if (Loop)
                        _animationStartTime = null;
                    //otherwise, we signal this animation as complete                    
                    else                        
                        OnAnimationComplete();
                }
            }
            else
                throw new InvalidOperationException("Target or Animation was not initialized.");
        }

        public ITransformable Target { get; set; }

        public TransformAnimation Animation { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EmitterAnimationController : AnimationController
    {
        private long? _animationStartTime = null;
        private long? _lastEmitTime = null;

        public Emitter Target { get; set; }
        public List<EmitterAction> Actions { get; set; }       
        public bool ExplicitEmissionsOnly { get; set; }

        public override void Start()
        {

        }

        public override void UpdateAnimation(long gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime;
           
            long elapsedTime = gameTime - _animationStartTime.Value;

            if (Target != null)
            {
                if (_lastEmitTime == null)
                    _lastEmitTime = gameTime;

                if (!ExplicitEmissionsOnly && gameTime - _lastEmitTime > 1000)
                    Target.Emit();

                if (Actions != null)
                {
                    foreach (EmitterAction action in this.Actions)
                    {
                        if (action.Time <= (ulong)elapsedTime)
                        {
                            switch (action.Task)
                            {
                                case EmitterAction.EmitterTask.KillAll:
                                    Target.KillAll();
                                    break;
                                case EmitterAction.EmitterTask.Kill:
                                    Target.Kill(action.ParticleArgs);
                                    break;
                                case EmitterAction.EmitterTask.Emit:
                                    Target.Emit();
                                    break;
                                case EmitterAction.EmitterTask.EmitCustom:
                                    Target.Emit(action.CustomParticleDescriptionArg);
                                    break;
                                case EmitterAction.EmitterTask.EmitExplicit:
                                    Target.Emit(action.ParticleArgs);
                                    break;
                                case EmitterAction.EmitterTask.Link:
                                    Target.Link(action.ParticleArgs);
                                    break;
                                case EmitterAction.EmitterTask.Transform:
                                    Target.Transform = action.Transform;
                                    break;
                            }
                        }
                    }
                    this.Actions.RemoveAll(a => a.Time <= (ulong)elapsedTime);
                }              
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RigidBodyAnimationController : AnimationController
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

        public override void Start()
        {
            _animationStartTime = null;           
        }

        public override void UpdateAnimation(long gameTime)
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

    /// <summary>
    /// 
    /// </summary>
    public class MasterController : AnimationController
    {
        List<IAnimationController> _slaves = new List<IAnimationController>();

        public override void Start()
        {
            
        }

        public override void UpdateAnimation(long gameTime)
        {
            //**********************************************************************************
            //NOTE: We use an auxilary controller collection to enumerate through, in 
            //the event that an updated controller alters this Simulator's Animation Controllers
            //collection.
            //**********************************************************************************
            List<IAnimationController> auxAnimationControllers = new List<IAnimationController>(_slaves);            
            foreach (IAnimationController controller in auxAnimationControllers)
            {
                controller.UpdateAnimation(gameTime);
                if (controller.IsAnimationComplete)
                    _slaves.Remove(controller);
            }

            if (_slaves.Count == 0 && !IsAnimationComplete)
                OnAnimationComplete();
        }

        public List<IAnimationController> Slaves { get { return _slaves; } }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParticleSystemController : AnimationController
    {
        private List<EmitterAnimationController> _emitterControllers = new List<EmitterAnimationController>();
        private long? _animationStartTime = null;

        public ParticleSolver Solver { get; set; }

        public override void Start()
        {
            
        }

        public override void UpdateAnimation(long gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime;

            long elapsedTime = gameTime - _animationStartTime.Value;

            if (Solver != null)
            {
                Particle[] allParticles = _emitterControllers.SelectMany(p => p.Target.Particles).ToArray();
                for (int i = 0; i < allParticles.Length; i++)
                    Solver.UpdateParticleTransform((ulong)elapsedTime, allParticles[i]);
            }
        }
    }
}