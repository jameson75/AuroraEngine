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
    
    /// <summary>
    /// 
    /// </summary>
    public interface IGameStateService
    {
        GameState State { get; }
        GameTime GameTime { get; }
        void Update(GameTime GameTime, GameState State);
    }

    /// <summary>
    /// 
    /// </summary>
    public class GameStateSerivce : IGameStateService
    {
        IGameApp _game = null;
        public GameStateSerivce(IGameApp game)
        {
            _game = game;
        }
        public GameState State { get; private set; }
        public GameTime GameTime { get; private set; }
        public void Update(GameTime gameTime, GameState state)
        {
            GameTime = gameTime;
            State = state;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum GameState
    {
        Running,
        Paused
    }    
}
