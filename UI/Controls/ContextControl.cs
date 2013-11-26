using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public abstract class ContextControl<T> : ContainerControl, Components.ICustomFocusManager 
        where T : UIControl
    {
        private T _subControl = null;

        protected ContextControl(IUIRoot visualRoot, System.Func<IUIRoot,T> controlCreator)
            : base(visualRoot)
        {
            _subControl = controlCreator(visualRoot);
            _subControl.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            _subControl.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;
            Children.Add(_subControl);
        }

        public UIControl Owner { get; set; }

        public ContextControlActivation Activation { get; set; }

        public ContextControlDisplaySide DisplaySide { get; set; }

        protected override void OnUpdate(long gameTime)
        {
            if (this.HasFocus || _subControl.HasFocus)
            {
                //**********************************************
                // if back button pressed                
                // close this submenu.
                // return focus to owner.
                // null owner.
                //**********************************************

                Services.IInputService inputServices = (Services.IInputService)Game.Services.GetService(typeof(Services.IInputService));
                if (inputServices == null)
                    throw new InvalidOperationException("Input services not available.");

                InputState inputState = inputServices.GetInputState();

                bool closeButtonPressed = (inputState.IsKeyHit(Key.Back)) ||
                    //(Orienation == MenuOrientation.Horizontal && inputState.IsKeyHit(Key.Left)) ||
                                          (inputState.IsGamepadButtonHit(0, SharpDX.XInput.GamepadButtonFlags.Back)) ||
                                          (inputState.IsGamepadButtonHit(0, SharpDX.XInput.GamepadButtonFlags.B));

                if (closeButtonPressed)
                {
                    this.Visible = false;
                    this.Owner.HasFocus = true;
                    this.Owner = null;
                }
            }
            base.OnUpdate(gameTime);
        }

        #region ICustomFocusManager

        public void SetNextFocus(UIControl owner)
        {
            //NOTE: By doing nothing, we effectively disable forward-tabbing out of a submenu.
            //The goal is to make sure the submenu can't lose focus unless it's closed.
        }

        public void SetPreviousFocus(UIControl owner)
        {
            //NOTE: By doing nothing, we effectively disable backward-tabbing out of a submenu.
        }

        #endregion
    }

    public enum ContextControlActivation
    {
        Click,
        Select
    }

    public enum ContextControlDisplaySide
    {
        Left,
        Above,
        Right,
        Bottom
    }

    
}