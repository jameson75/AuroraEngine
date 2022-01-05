using System;
using System.Collections.Generic;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.Utils;
using SharpDX.DirectInput;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// An MIT License.
///////////////////////////////////////////////////////////////////////////////'

namespace CipherPark.Aurora.Core.UI.Controls
{
    public class TabControl : SelectControl
    {
        private Dictionary<UIControl, UIControl> CachedFocusedElements = new Dictionary<UIControl, UIControl>();

        public bool EnableDefaultFocus { get; set; }

        public TabControl(Components.IUIRoot visualRoot) 
            : base(visualRoot)
        {
            EnableDefaultFocus = true;
        }

        protected override void OnItemAdded(ItemControl item)
        {
            if (item is TabView == false)
                throw new ArgumentException("item", "item must be of type or derivative of ScreenControl");          
            base.OnItemAdded(item);           
        }

        protected override void OnItemRemoved(ItemControl item)
        {
            base.OnItemRemoved(item);
            SelectedItemIndex = -1;
        }

        public int ActiveTabIndex
        {
            get { return SelectedItemIndex; }
            set { SelectedItemIndex = value; }
        }

        public TabView ActiveTab
        {
            get { return (TabView)SelectedItem; }
            set { SelectedItem = value; }
        }

        protected override void OnInitialize()
        {
            foreach (TabView item in Items)
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

            if (ActiveTab != null)
                ActiveTab.Update(gameTime);
            
            base.OnUpdate(gameTime);
        }

        protected override void OnDraw()
        {
            if( ActiveTab != null )
                ActiveTab.Draw();
            base.OnDraw();
        }

        protected override void OnSelectedItemChanging()
        {
            if (ActiveTab != null)             
            {
                ActiveTab.EnableFocus = false; 
                UIControl focusedControl = VisualRoot.FocusManager.FocusedControl;
                if( focusedControl != null && ActiveTab.IsDescendant(focusedControl))
                {
                    VisualRoot.FocusManager.LeaveFocus(VisualRoot.FocusManager.FocusedControl);
                    CachedFocusedElements.Add(ActiveTab, focusedControl);
                }
            }
            base.OnSelectedItemChanging();
        }

        protected override void OnSelectedItemChanged()
        {
            if (ActiveTab != null)
            {
                ActiveTab.EnableFocus = true;
                if (CachedFocusedElements.ContainsKey(ActiveTab))
                {
                    UIControl cachedFocusedControl = CachedFocusedElements[ActiveTab];
                    VisualRoot.FocusManager.SetFocus(cachedFocusedControl);
                    CachedFocusedElements.Remove(ActiveTab);
                }
                else if (EnableDefaultFocus)
                {
                    //UIControl defaultFocusTarget = VisualRoot.FocusManager.GetFirstInTabOrder(ActiveScreen);
                    UIControl defaultFocusTarget = VisualRoot.FocusManager.GetNext(ActiveTab);
                    if (defaultFocusTarget != null)
                        VisualRoot.FocusManager.SetFocus(defaultFocusTarget);
                    //VisualRoot.FocusManager.SetNextFocus(ActiveScreen);
                }
            }
                    
            base.OnSelectedItemChanged();
        }      
    }

    public class TabView : ListControlItem, ICommandControl, ICustomFocusContainer
    {
        CommandControlWireUp _wireUp = null;

        public TabView(IUIRoot visualRoot)
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
