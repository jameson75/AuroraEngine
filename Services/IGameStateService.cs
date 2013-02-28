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
        public bool HasControl { get; set; }        
    }

    public class GameState
    {
        public RunningState RunningState { get; set; }
    }

    public enum RunningState
    {
        Play
    }

    public interface IGameStateService
    {
        GameState State { get; }
    }
}
