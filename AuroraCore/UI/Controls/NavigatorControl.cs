using System.Linq;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.UI.Components;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Controls
{   
    public class NavigatorControl : UIControl
    {          
        private MouseNavigatorService mouseNavigatorService;
        private SceneModifierService sceneModifierService;

        public NavigatorControl(IUIRoot visualRoot) : base(visualRoot)
        {          
            
        }

        /*
        public MouseNavigatorService.NavigationMode NavigationMode { get => mouseNavigatorService.Mode; set => mouseNavigatorService.Mode = value; }    
        public bool IsInPickingMode { get => sceneModifierService.IsActive; set => sceneModifierService.IsActive = value; }
        public TranslationPlane TransformPlane { get => sceneModifierService.TranslationPlane; set => sceneModifierService.TranslationPlane = value; }
        */

        public void ClearCachedServices()
        {
            mouseNavigatorService = null;
            sceneModifierService = null;
        }

        protected override void OnUpdate(GameTime gameTime)
        {            
            IInputService inputService = Game.Services.GetService<IInputService>();
            InputState inputState = inputService.GetInputState();

            if (mouseNavigatorService == null)
            {
                mouseNavigatorService = Game.Services.GetService<MouseNavigatorService>();
            }

            if (sceneModifierService == null)
            {
                sceneModifierService = Game.Services.GetService<SceneModifierService>();
            }

            if ((inputService.IsMouseInViewport(inputState) && Game.IsViewportWindowActive) || this.Capture)
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
                Capture = true;
                mouseNavigatorService.NotifyMouseDown(location);
                sceneModifierService.NotifyMouseDown(location);
            }
        }
        
        protected void OnMouseUp(InputState.MouseButton mouseButton, Point location)
        {
            if (mouseButton == InputState.MouseButton.Left && this.Capture)
            {
                Capture = false;
                mouseNavigatorService.NotifyMouseUp(location);
                sceneModifierService.NotifyMouseUp(location);
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
            sceneModifierService.NotifyMouseMove(leftButtonDown, location);
        } 
    }
}
