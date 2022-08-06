using CipherPark.Aurora.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Services
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

        /// <summary>
        /// Determines if the mouse coordinates are within the game's view port.
        /// </summary>
        /// <param name="inputState"></param>
        /// <returns></returns>
        bool IsMouseInViewport(InputState inputState);
    }

    /// <summary>
    /// Provides services for querying the states of the game's input devices.
    /// </summary>
    public sealed class InputService : IInputService
    {
        #region Fields
        InputState ism = null;
        BufferedInputState cim = null;
        IGameApp game = null;
        #endregion

        #region Constructors
        public InputService(IGameApp game)
        {
            ism = new InputState(game);
            cim = new BufferedInputState(ism);
            this.game = game;
            Create(ism, cim);
        }

        public InputService(IGameApp game, IMouseCoordsTransfomer mouseCoordsTransfomer)
        {
            ism = new InputState(game, mouseCoordsTransfomer);
            cim = new BufferedInputState(ism);
            this.game = game;
            Create(ism, cim);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Retreives the game's input state manager.
        /// </summary>
        /// <returns>The game's input state manager.</returns>
        public InputState GetInputState()
        {
            return ism;
        }

        /// <summary>
        /// Retreives the game's control input state manager.
        /// </summary>
        /// <returns></returns>
        public BufferedInputState GetBufferedInputState()
        {
            return cim;
        }

        /// <summary>
        /// Updates the state of the input state and, then, the control-input-state.
        /// </summary>
        public void UpdateState()
        {
            ism.UpdateState();
            cim.UpdateState();
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
            this.ism = ism;
            this.cim = cim;
        }

        public bool IsMouseInViewport(InputState state)
        {
            var location = state.GetMouseLocation();
            var renderTargetSize = game.RenderTargetView.GetTexture2DSize();
            return location.X >= 0 && location.X <= renderTargetSize.Width &&
                   location.Y >= 0 && location.Y <= renderTargetSize.Height;
        }
    }
}
