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
    /// <summary>
    /// Provides base functionality for all animation types.
    /// </summary>
    public abstract class KeyframeAnimation
    {
        private AnimationKeyFrames _keyFrames = new AnimationKeyFrames();

        /// <summary>
        /// Constructs an Animation object with an empty list of key frames.
        /// </summary>
        protected KeyframeAnimation()
        { }

        /// <summary>
        /// Constructs an Animation object and initializes it with the specified keyframes.
        /// </summary>
        /// <param name="keyFrames">Set of key frames used to initialize this object.</param>
        protected KeyframeAnimation(IEnumerable<AnimationKeyFrame> keyFrames)
        {
            foreach (AnimationKeyFrame keyFrame in keyFrames)
                _keyFrames.Add(keyFrame.Time, keyFrame);
        }

        /// <summary>
        /// Gets the nearest key frame whose time is less than or equal to timeT.
        /// </summary>       
        /// <param name="timeT">The time in which the key frame is active.</param>
        /// <returns>The key frame which is active at time T</returns>              
        public AnimationKeyFrame GetActiveKeyFrameAtT(ulong timeT)
        {            
            for (int i = _keyFrames.Count - 1; i >= 0; i--)
            {
                if (_keyFrames[_keyFrames.Keys[i]].Time <= timeT)
                    return _keyFrames[_keyFrames.Keys[i]];
            }
            throw new InvalidOperationException("No key frame exist for specified time t");
        }

        /// <summary>
        /// Gets the next key frame in the time line.
        /// </summary>
        /// <param name="keyFrame">The preceding keyframe.</param>
        /// <returns>The next key frame in the time line.</returns>
        public AnimationKeyFrame GetNextKeyFrame(AnimationKeyFrame keyFrame)
        {
            int i = _keyFrames.IndexOfValue(keyFrame);
            if (i == -1)
                throw new ArgumentException("key frame does not exist in animation.", "keyFrame");
            else if (i == _keyFrames.Count - 1)
                return null;
            else
                return _keyFrames[_keyFrames.Keys[i + 1]];
        }

        /// <summary>
        /// Gets the previous key frame in the time line.
        /// </summary>
        /// <param name="keyFrame">The following keyframe.</param>
        /// <returns>The previous key frame in the time line.</returns>
        public AnimationKeyFrame GetPreviousKeyFrame(AnimationKeyFrame keyFrame)
        {
            int i = _keyFrames.IndexOfValue(keyFrame);
            if (i == -1)
                throw new ArgumentException("keyFrame is not an element of the specified property's animation", "keyFrame");
            else if (i == 0)
                return null;
            else
                return _keyFrames[_keyFrames.Keys[i - 1]];
        }

        /// <summary>
        /// Adds or replaces a key frame at the specified time in the time line.
        /// </summary>
        /// <param name="keyFrame">The key frame to be added or replaced in the time line.</param>
        /// <remarks>The AnimationKeyFrame.Time is used to determine where the key frame will be placed.</remarks>
        public void SetKeyFrame(AnimationKeyFrame keyFrame)
        {
            if (_keyFrames.ContainsKey(keyFrame.Time))
                _keyFrames[keyFrame.Time] = keyFrame;
            else
                _keyFrames.Add(keyFrame.Time, keyFrame);
        }

        /// <summary>
        /// Adds or replaces key frames at the specified times in the time line.
        /// </summary>
        /// <param name="keyFrame">The key frames to be added or replaced in the time line.</param>
        /// <remarks>The AnimationKeyFrame.Time is used to determine where each key frame will be placed.</remarks>
        public void SetKeyFrames(IEnumerable<AnimationKeyFrame> keyFrames)
        {
            foreach (AnimationKeyFrame keyFrame in keyFrames)
                SetKeyFrame(keyFrame);
        }

        /// <summary>
        /// Removes the key frame from the animation at the specified time.
        /// </summary>
        /// <param name="timeT">The key frame's time</param>
        public void RemoveKeyFrame(ulong timeT)
        {
            if (!_keyFrames.ContainsKey(timeT))
                throw new ArgumentException("No key frame exists with the specified time.", "timeT");
            _keyFrames.Remove(timeT);
        }

        /// <summary>
        /// Returns the number of key frames in the time line.
        /// </summary>
        public int FrameCount
        {
            get { return _keyFrames.Count; }
        }     

        /// <summary>
        /// Returns the total length in time it takes to complete the animation,
        /// which is simply the equivalent of the time value in the final key frame.        
        /// </summary>
        /// <remarks>
        /// If the animation contains no key frames, the running time is 0.
        /// </remarks>
        public ulong RunningTime
        {
            get
            {
                if (FrameCount == 0)
                    return 0;
                else
                    return _keyFrames[_keyFrames.Keys[_keyFrames.Count - 1]].Time;
            }                    
        }

        /// <summary>
        /// A helper method which allows derived classes to perform simple linear interpolation.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        protected static double Lerp(double v0, double v1, float percentage)
        {
            return v0 + (percentage * (v1 - v0));
        }        
    }
}
