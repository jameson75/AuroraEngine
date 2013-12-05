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
    public abstract class ContextControl<T> : ContainerControl, ICustomFocusContainer
        where T : UIControl
    {
        private T _subControl = null;

        protected ContextControl(IUIRoot visualRoot, System.Func<IUIRoot,T> controlCreator)
            : base(visualRoot)
        {
            HandleCloseKey = true;
            _subControl = controlCreator(visualRoot);
            _subControl.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            _subControl.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;           
            Children.Add(_subControl);          
        }

        public UIControl Owner { get; private set; }

        public ContextControlActivation Activation { get; set; }

        public ContextControlDisplaySide DisplaySide { get; set; }

        public bool HandleCloseKey { get; set; }

        public void BeginContext(UIControl owner)
        {
            this.Owner = owner;
            this.Visible = true;
            this.VisualRoot.FocusManager.SetNextFocus(this, true, false, false);  
            VisualRoot.FocusManager.ControlLostFocus += FocusManager_ControlLostFocus;
        }   

        public void EndContext()
        {
            VisualRoot.FocusManager.ControlLostFocus -= FocusManager_ControlLostFocus;            
            this.Visible = false;
            this.Owner.HasFocus = true;
            this.Owner = null;  
        }

        protected override void OnUpdate(long gameTime)
        {
            if (this.ContainsFocus)
            {               
                if (HandleCloseKey)
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
                                              (inputState.IsGamepadButtonHit(0, SharpDX.XInput.GamepadButtonFlags.Back)) ||
                                              (inputState.IsGamepadButtonHit(0, SharpDX.XInput.GamepadButtonFlags.B));

                    if (closeButtonPressed)
                        EndContext();
                }
            }
            base.OnUpdate(gameTime);
        }
   
        private void FocusManager_ControlLostFocus(object sender, FocusChangedEventArgs args)
        {
            if (!IsDescendant(args.Control))
                this.EndContext();
        }

        bool ICustomFocusContainer.CanFocusMoveInward
        {
            get { return true; }
        }    

        bool ICustomFocusContainer.CanFocusMoveOutward
        {
            get { return false; }
        }
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