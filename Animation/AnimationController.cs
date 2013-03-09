using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;

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
        public void Start();
        public void UpdateAnimation(long gameTime);
    }

    public abstract class AnimationController<TTarget, TAnimation> : IAnimationController
    {
        private IGameApp _game = null;        
        public AnimationController(IGameApp game)
        {
            _game = game;
        }
        public IGameApp Game { get { return _game; } } 
        public TTarget Target { get; set; }
        public TAnimation Animation { get; set; }    
    }

    public class TransformAnimationController : AnimationController<ITransformable, TransformAnimation>
    {
        private long? _animationStartTime = 0;

        public TransformAnimationController(IGameApp game)
        { }

        public void Start()
        {
            _animationStartTime = null;
        }

        public void UpdateAnimation(long gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime;
            ulong timeT = (ulong)(gameTime - _animationStartTime.Value);
            Target.Transform = Animation.GetValueAtT(timeT);
        }
    }
}