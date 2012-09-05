using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.Module;

namespace CipherPark.AngelJacket.Core.World
{
    public class AnimationSystem
    {
        private IGameApp _game = null;

        public AnimationSystem(IGameApp game)
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
