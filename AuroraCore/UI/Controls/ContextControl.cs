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

        protected ContextControl(IUIRoot visualRoot, System.Func<IUIRoot,T> subControlCreator)
            : base(visualRoot)
        {
            HandleCloseKey = true;
            _subControl = subControlCreator(visualRoot);
            _subControl.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            _subControl.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;
            _subControl.SizeChanged += SubControl_SizeChanged;
            Children.Add(_subControl);          
        }

        private void SubControl_SizeChanged(object sender, EventArgs e)
        {
            this.SuspendLayout = true;
            this.Size = _subControl.Size;
            this.SuspendLayout = false;
        }

        public ContextControlResult Result { get; set; }

        public UIControl Owner { get; private set; }        

        public bool HandleCloseKey { get; set; }

        public T SubControl { get { return _subControl; } }

        public void BeginContext(UIControl owner)
        {            
            this.Owner = owner;
            this.Visible = true;
            UIControl nextFocusableControl = this.VisualRoot.FocusManager.GetNext(this);
            if (nextFocusableControl != null && this.IsDescendant(nextFocusableControl))
                this.VisualRoot.FocusManager.PostFocus(nextFocusableControl);
            VisualRoot.FocusManager.ControlLostFocus += FocusManager_ControlLostFocus;
            OnBeginContext();
        }   

        public void EndContext(ContextControlResult result)
        {
            VisualRoot.FocusManager.ControlLostFocus -= FocusManager_ControlLostFocus;            
            this.Visible = false;
            if (this.ContainsFocus)
                this.VisualRoot.FocusManager.PostFocus(this.Owner);
            this.Owner = null;
            this.Result = result;
            OnEndContext();
        }

        protected override void OnUpdate(GameTime gameTime)
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

                    bool cancelButtonPressed = (inputState.IsKeyHit(Key.Back)) ||
                                              (inputState.IsGamepadButtonHit(0, SharpDX.XInput.GamepadButtonFlags.Back)) ||
                                              (inputState.IsGamepadButtonHit(0, SharpDX.XInput.GamepadButtonFlags.B));

                    if (cancelButtonPressed)                    
                        EndContext(ContextControlResult.SelectCancel);                    
                }
            }
            base.OnUpdate(gameTime);
        }

        protected virtual void OnBeginContext()
        {
            EventHandler handler = ContextStarted;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnEndContext()
        {
            EventHandler handler = ContextClosed;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
   
        private void FocusManager_ControlLostFocus(object sender, FocusChangedEventArgs args)
        {
            if (IsDescendant(args.Control))
                this.EndContext(ContextControlResult.SelectCancel);
        }

        bool ICustomFocusContainer.CanTabInward
        {
            get { return true; }
        }    

        bool ICustomFocusContainer.CanTabOutward
        {
            get { return false; }
        }

        public event EventHandler ContextStarted;

        public event EventHandler ContextClosed;
    }  

    public class ContextMenu : ContextControl<Menu>
    {
        public ContextMenuDisplaySide DisplaySide { get; set; }       

        public ContextMenu(IUIRoot visualRoot)
            : base(visualRoot, ConstructMenu)
        {
            SubControl.ControlCommand+= MenuSubControl_ControlCommand; 
        }

        private void MenuSubControl_ControlCommand(object sender, ControlCommandArgs args)
        {            
 	        this.EndContext(ContextControlResult.SelectOK);
        }   

        private static Menu ConstructMenu(IUIRoot visualRoot)
        {
            return new Menu(visualRoot);
        }

        protected override void OnBeginContext()
        {
            if( this.SubControl.Items.Count > 0 )
                this.SubControl.SelectedItemIndex = 0;
        }
    }

    public enum ContextMenuDisplaySide
    {
        Left,
        Above,
        Right,
        Bottom
    }

    public enum ContextControlResult
    {
        SelectOK,
        SelectCancel
    }
}