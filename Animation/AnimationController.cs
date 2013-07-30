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
        private const int EmissionRate = 1000;

        public Emitter Target { get; set; }
        public List<EmitterAction> ExplicitTasks { get; set; }
        public bool IsExplicitMode { get; set; }

        public override void Start() { }

        public override void UpdateAnimation(long gameTime)
        {
            if (Target == null)
                throw new InvalidOperationException("Target not specified");

            else if (IsExplicitMode && ExplicitTasks == null)
                throw new InvalidOperationException("Actions not specified");

            if (_animationStartTime == null)
                _animationStartTime = gameTime;

            if (_lastEmitTime == null)
                _lastEmitTime = gameTime;

            long elapsedTime = gameTime - _animationStartTime.Value;
            long elapsedEmitTime = gameTime - _lastEmitTime.Value;

            if (!IsExplicitMode)
            {
                if (elapsedEmitTime > EmissionRate)
                    Target.Emit();
            }
            else
            {
                foreach (EmitterAction action in this.ExplicitTasks)
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

                this.ExplicitTasks.RemoveAll(a => a.Time <= (ulong)elapsedTime);
                
                if (ExplicitTasks.Count == 0)
                    OnAnimationComplete();
            }           
        }
    }

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

        public override void Start()
        {
            _animationStartTime = null;           
        }

        public override void UpdateAnimation(long gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime;

            ulong elapsedTime = (ulong)(gameTime - _animationStartTime.Value);

            if (Target != null)                             
                Target.Transform = new Transform(Delta.GetTransformationAtT((float)elapsedTime / 1000.0f));                         

            //OnAnimationComplete();
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class CompositeAnimationController : AnimationController
    {
        List<IAnimationController> _children = new List<IAnimationController>();

        public CompositeAnimationController()
        { }

        public CompositeAnimationController(IEnumerable<IAnimationController> children)
        {
            _children.AddRange(children);
        }

        public override void Start() { }

        public override void UpdateAnimation(long gameTime)
        {
            //**********************************************************************************
            //NOTE: We use an auxilary controller collection to enumerate through, in 
            //the event that an updated controller alters this Simulator's Animation Controllers
            //collection.
            //**********************************************************************************
            List<IAnimationController> auxAnimationControllers = new List<IAnimationController>(_children);
            foreach (IAnimationController controller in auxAnimationControllers)
            {
                controller.UpdateAnimation(gameTime);
                if (controller.IsAnimationComplete)
                    _children.Remove(controller);
            }

            if (_children.Count == 0 && !IsAnimationComplete)
                OnAnimationComplete();
        }

        public List<IAnimationController> Children { get { return _children; } }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParticleSystemController : AnimationController
    {
        private List<EmitterAnimationController> _emitterControllers = new List<EmitterAnimationController>();
        private long? _animationStartTime = null;

        public ParticleSolver Solver { get; set; }

        public override void Start() { }

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