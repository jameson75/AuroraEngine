using System.Linq;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.Content;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Controls
{
    public enum ActionMode
    {
        None,
        Select,
        Navigate      
    }

    public enum CameraMode
    {
        None,
        Rotate,
        Pan
    }

    public class NavigatorControl : UIControl
    {          
        private MouseNavigatorService mouseNavigatorService;

        public NavigatorControl(IUIRoot visualRoot) : base(visualRoot)
        {
            mouseNavigatorService = new MouseNavigatorService(visualRoot.Game);
        }       
        
        public MouseNavigatorService.NavigationMode NavigationMode { get => mouseNavigatorService.Mode; set => mouseNavigatorService.Mode = value; }    

        protected override void OnUpdate(GameTime gameTime)
        {            
            IInputService inputServices = Game.Services.GetService<IInputService>();
            InputState inputState = inputServices.GetInputState();

            if (IsMouseInViewport(inputState) || this.Capture)
            {
                InputState.MouseButton[] pressedMouseButtons = inputState.GetMouseButtonsPressed();
                for (int i = 0; i < pressedMouseButtons.Length; i++)
                    OnMouseDown(pressedMouseButtons[i], inputState.GetMouseLocation());

                InputState.MouseButton[] downMouseButtons = inputState.GetMouseButtonsDown();
                if (inputState.GetPreviousMouseLocation() != inputState.GetMouseLocation())
                    OnMouseMove(downMouseButtons, inputState.GetMouseLocation());

                InputState.MouseButton[] releasedMouseButtons = inputState.GetMouseButtonsReleased();
                for (int i = 0; i < releasedMouseButtons.Length; i++)
                    OnMouseUp(releasedMouseButtons[i], inputState.GetMouseLocation());

                if (inputState.GetMouseWheelDelta() != 0)
                    OnMouseWheel(inputState.GetMouseWheelDelta());
            }
        }

        protected void OnMouseDown(InputState.MouseButton mouseButton, Point location)
        {
            if (mouseButton == InputState.MouseButton.Left)
            {
                this.Capture = true;
                mouseNavigatorService.BeginMouseTracking(location);
            }
        }
        
        protected void OnMouseUp(InputState.MouseButton mouseButton, Point location)
        {
            if (mouseButton == InputState.MouseButton.Left && this.Capture)
            {
                this.Capture = false;
                mouseNavigatorService.EndMouseTracking(location);
            }
        }

        protected void OnMouseWheel(float mouseWheelDelta)
        {
            mouseNavigatorService.NotifyMouseWheelChange(mouseWheelDelta);
        }

        protected void OnMouseMove(InputState.MouseButton[] mouseButtonsDown, Point location)
        {
            bool leftButtonDown = mouseButtonsDown.Contains(InputState.MouseButton.Left);
            mouseNavigatorService.NotifyMouseMove(leftButtonDown, location);
        }

        private bool IsMouseInViewport(InputState state)
        {
            var location = state.GetMouseLocation();
            var renderTargetSize = Game.RenderTargetView.GetTexture2DSize();
            return location.X >= 0 && location.X <= renderTargetSize.Width &&
                   location.Y >= 0 && location.Y <= renderTargetSize.Height;
        }
    }
}
