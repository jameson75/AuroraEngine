﻿using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils;

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
    /// Provides services for querying the states of the game's input devices.
    /// </summary>
    public interface IInputService
    {
        /// <summary>
        /// Retrieves the game's input state manager.
        /// </summary>
        /// <returns></returns>       
        InputState GetInputState();

        /// <summary>
        /// Retreives the game's buffered input state manager.
        /// </summary>
        /// <returns></returns>
        BufferedInputState GetBufferedInputState();

        /// <summary>
        /// Updates the input state.
        /// </summary>
        void UpdateState();  
    }

    /// <summary>
    /// Provides services for querying the states of the game's input devices.
    /// </summary>
    public sealed class InputService : IInputService
    {
        #region Fields
        InputState _ism = null;
        BufferedInputState _cim = null;
        #endregion

        #region Constructors
        public InputService(IGameApp game)
        {
            _ism = new InputState(game);
            _cim = new BufferedInputState(_ism);
            Create(_ism, _cim);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Retreives the game's input state manager.
        /// </summary>
        /// <returns>The game's input state manager.</returns>
        public InputState GetInputState()
        {
            return _ism;
        }

        /// <summary>
        /// Retreives the game's control input state manager.
        /// </summary>
        /// <returns></returns>
        public BufferedInputState GetBufferedInputState()
        {
            return _cim;
        }

        /// <summary>
        /// Updates the state of the input state and, then, the control-input-state.
        /// </summary>
        public void UpdateState()
        {
            _ism.UpdateState();
            _cim.UpdateState();
        }
        #endregion

        /// <summary>
        /// Constructs a new input game-service initialized with the game's input state manager.
        /// </summary>
        /// <param name="ism">The game's input state manager.</param>
        /// <param name="cim">The game's control input state manager.</param>
        /// <param name="fm">The game's control focus manager.</param>
        private void Create(InputState ism, BufferedInputState cim)
        {
            _ism = ism;
            _cim = cim;
        }
    }
}
