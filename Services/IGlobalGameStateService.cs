using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CipherPark.AngelJacket.Core.World;

namespace CipherPark.AngelJacket.Core.Services
{
    public class Player
    {
        public bool Active { get; set; }
        public ISceneObject Avatar { get; set; }
        public bool HasControl { get; set; }        
    }

    public class GameState
    {
        private Player[] _players = null;

        public Player[] Players
        {
            get
            {
                return _players;
            }
        }
    }

    public interface IGlobalGameStateService
    {
        GameState State { get; }
    }
}
