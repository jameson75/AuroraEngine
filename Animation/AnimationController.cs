using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;
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

    public abstract class KeyframeAnimationController<TTarget, TAnimation> : IAnimationController
    {
        private IGameApp _game = null;        
        protected KeyframeAnimationController(IGameApp game)
        {
            _game = game;
        }
        protected KeyframeAnimationController(IGameApp game, TAnimation animation, TTarget target)
        {
            _game = game;
            Target = target;
            Animation = animation;
        }
        public IGameApp Game { get { return _game; } } 
        public TTarget Target { get; set; }
        public TAnimation Animation { get; set; }
        public abstract void Start();
        public abstract void UpdateAnimation(long gameTime);
    }

    public class TransformAnimationController : KeyframeAnimationController<ITransformable, TransformAnimation>
    {
        private long? _animationStartTime = null;

        public TransformAnimationController(IGameApp game) : base(game)
        { }

        public TransformAnimationController(IGameApp game, TransformAnimation animation, ITransformable target)
            : base(game, animation, target)
        { }

        public override void Start()
        {
            _animationStartTime = null;
        }

        public override void UpdateAnimation(long gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime;
            ulong timeT = (ulong)(gameTime - _animationStartTime.Value);
            Target.Transform = Animation.GetValueAtT(timeT);
        }
    }

    public class EmitterAnimationController : IAnimationController
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
}