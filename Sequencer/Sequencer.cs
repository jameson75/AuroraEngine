using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.UI.Components;

namespace CipherPark.AngelJacket.Core.Sequencer
{
    public class Sequencer
    {
        private IGameApp _game = null;     

        public IGameApp Game { get { return _game; } }

        public GameAssets Assets { get; set; }

        public Sequence Sequence { get; set; }
 
        public Sequencer(IGameApp game)
        {
            _game = game;
        }

        public void Update(Scene scene, UITree ui, long gameTime)
        {

        }
    }

    public class Sequence : List<Trigger>
    {

    }

    public class Trigger
    {
        public long Time { get; set; }

        public Trigger() { }

        public Trigger(long time)
        {
            Time = time;
        }
    }
}
