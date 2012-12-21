using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.Module;

namespace CipherPark.AngelJacket.Core.System.Animation
{
    public class AnimationClock
    {
        private IGameApp _game = null;

        public AnimationClock(IGameApp game)
        {
            _game = game;    
        }

        public void Start(Animation animation)
        {

        }

        public void Stop(Animation animation)
        {

        }       
    }

    public class Animation
    {
        
    }

    public class PropertyAnimation : Animation
    { }

    public class TransformAnimation : Animation
    { }
}
