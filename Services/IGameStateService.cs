using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.World;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

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
