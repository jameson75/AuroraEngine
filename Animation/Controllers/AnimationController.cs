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
    /// <summary>
    /// 
    /// </summary>
    public interface IAnimationController
    {       
        void Reset();
        void UpdateAnimation(GameTime gameTime);
        bool IsAnimationComplete { get; }
        void SetComplete();
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

        public void SetComplete()
        {
            OnAnimationComplete();
        }

        public abstract void Reset();
        
        public abstract void UpdateAnimation(GameTime gameTime);
        
        public event EventHandler AnimationComplete;

        protected virtual void OnAnimationComplete()
        {
            _isAnimationComplete = true;
            EventHandler handler = AnimationComplete;
            if (handler != null)
                AnimationComplete(this, EventArgs.Empty);
        }        
    }   
}