using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class UIAnimationController
    {
        //TimeSpan startTime = TimeSpan.Zero;
        //UIAnimationControllerState _state = UIAnimationControllerState.Stopped;

        //public void Start(GameTime gameTime)
        //{
        //    startTime = gameTime.TotalGameTime;            
        //}

        //public void Step(GameTime gameTime)
        //{
        //    TimeSpan elapsedAnimationTime = GetElapsedAnimationTime(gameTime);
        //    foreach (UIAnimation animation in _animations)
        //    {
        //        UIAnimationKeyFrame endPoints = animation.GetFrameEndPoints(elapsedAnimationTime);
        //        endPoints.KeyFrame1.
        //    }
        //}

        //private TimeSpan GetElapsedAnimationTime(GameTime currentGameTime)
        //{
        //    return currentGameTime.TotalGameTime - startTime;
        //}
    }

    public enum UIAnimationControllerState
    {
        Stopped,
        Started
    }
}
    