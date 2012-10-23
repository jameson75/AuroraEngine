using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils;

namespace CipherPark.AngelJacket.Core.Services
{
    /// <summary>
    /// Provides services for querying the states of the game's input devices.
    /// </summary>
    public interface IInputService
    {
        InputState GetInputState();
        /// <summary>
        /// Retreives the game's control input state manager.
        /// </summary>
        /// <returns></returns>
        ControlInputState GetControlInputState();

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
        ControlInputState _cim = null;
        #endregion

        #region Constructors
        public InputService(IGameApp game)
        {
            _ism = new InputState(game);
            _cim = new ControlInputState(_ism);
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
        public ControlInputState GetControlInputState()
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
        private void Create(InputState ism, ControlInputState cim)
        {
            _ism = ism;
            _cim = cim;
        }
    }
}
