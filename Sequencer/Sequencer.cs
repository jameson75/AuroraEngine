using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.Module;

namespace CipherPark.AngelJacket.Core.Sequencer
{
    public class Sequencer
    {
        private IGameApp _game = null;
        public Sequencer(IGameApp game)
        {
            _game = game;
        }      
    }
}
