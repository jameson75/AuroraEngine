using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.Aurora.Core.World;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Services
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
