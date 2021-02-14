using System;
using System.Collections.Generic;
using CipherPark.KillScript.Core.Module;
using CipherPark.KillScript.Core.Services;
using CipherPark.KillScript.Core.Utils;
using SharpDX.DirectInput;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////'

namespace CipherPark.KillScript.Core.UI.Controls
{
    public class ScreenControl : SelectControl
    {
        private Dictionary<UIControl, UIControl> CachedFocusedElements = new Dictionary<UIControl, UIControl>();

        public bool EnableDefaultFocus { get; set; }

        public ScreenControl(Components.IUIRoot visualRoot) 
            : base(visualRoot)
        {
            EnableDefaultFocus = true;
        }

        protected override void OnItemAdded(ItemControl item)
        {
            if (item is UIScreen == false)
                throw new ArgumentException("item", "item must be of type or derivative of ScreenControl");          
            base.OnItemAdded(item);           
        }

        protected override void OnItemRemoved(ItemControl item)
        {
            base.OnItemRemoved(item);
            SelectedItemIndex = -1;
        }

        public int ActiveScreenIndex
        {
            get { return SelectedItemIndex; }
            set { SelectedItemIndex = value; }
        }

        public UIScreen ActiveScreen
        {
            get { return (UIScreen)SelectedItem; }
            set { SelectedItem = value; }
        }

        protected override void OnInitialize()
        {
            foreach (MenuItem item in Items)
                item.Initialize();
            base.OnInitialize();
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            IInputService inputServices = (IInputService)Game.Services.GetService(typeof(IInputService));
            
            if (inputServices == null)
                throw new InvalidOperationException("InputServices not provided.");

            InputState inputStateManager = inputServices.GetInputState();
            
            if (inputStateManager.IsKeyReleased(Key.Back) ||
                inputStateManager.IsGamepadButtonDown(0, SharpDX.XInput.GamepadButtonFlags.Back))
                SelectPreviousItem();

            if (ActiveScreen != null)
                ActiveScreen.Update(gameTime);
            
            base.OnUpdate(gameTime);
        }

        protected override void OnDraw()
        {
            if( ActiveScreen != null )
                ActiveScreen.Draw();
            base.OnDraw();
        }

        protected override void OnSelectedItemChanging()
        {
            if (ActiveScreen != null)             
            {
                ActiveScreen.EnableFocus = false; 
                UIControl focusedControl = VisualRoot.FocusManager.FocusedControl;
                if( focusedControl != null && ActiveScreen.IsDescendant(focusedControl))
                {
                    VisualRoot.FocusManager.LeaveFocus(VisualRoot.FocusManager.FocusedControl);
                    CachedFocusedElements.Add(ActiveScreen, focusedControl);
                }
            }
            base.OnSelectedItemChanging();
        }

        protected override void OnSelectedItemChanged()
        {
            if (ActiveScreen != null)
            {
                ActiveScreen.EnableFocus = true;
                if (CachedFocusedElements.ContainsKey(ActiveScreen))
                {
                    UIControl cachedFocusedControl = CachedFocusedElements[ActiveScreen];
                    VisualRoot.FocusManager.SetFocus(cachedFocusedControl);
                    CachedFocusedElements.Remove(ActiveScreen);
                }
                else if (EnableDefaultFocus)
                {
                    //UIControl defaultFocusTarget = VisualRoot.FocusManager.GetFirstInTabOrder(ActiveScreen);
                    UIControl defaultFocusTarget = VisualRoot.FocusManager.GetNext(ActiveScreen);
                    if (defaultFocusTarget != null)
                        VisualRoot.FocusManager.SetFocus(defaultFocusTarget);
                    //VisualRoot.FocusManager.SetNextFocus(ActiveScreen);
                }
            }
                    
            base.OnSelectedItemChanged();
        }      
    }

    public class UIScreen : ListControlItem, ICommandControl, Components.ICustomFocusContainer
    {
        CommandControlWireUp _wireUp = null;

        public UIScreen(Components.IUIRoot visualRoot)
            : base(visualRoot)
        {
            _wireUp = new CommandControlWireUp(this);
            _wireUp.ChildControlCommand += CommandControlWireUp_ChildControlCommand;
        }

        void CommandControlWireUp_ChildControlCommand(object sender, ControlCommandArgs args)
        {
            OnCommand(args.CommandName);
        }

        public override bool CanReceiveFocus
        {
            get
            {
                return false;
            }
        }

        public bool CanTabOutward
        {
            get { return false; }
        }

        public bool CanTabInward
        {
            get { return true; }
        }
    }    
}
