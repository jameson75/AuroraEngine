using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.Services;
using CipherPark.AngelJacket.Core.Utils;
using SharpDX.DirectInput;

namespace CipherPark.AngelJacket.Core.UI.Controls
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
            if (item is ScreenPanel == false)
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

        public ScreenPanel ActiveScreen
        {
            get { return (ScreenPanel)SelectedItem; }
            set { SelectedItem = value; }
        }

        public override void Update(long gameTime)
        {
            IInputService inputServices = (IInputService)Game.Services.GetService(typeof(IInputService));
            
            if (inputServices == null)
                throw new InvalidOperationException("InputServices not provided.");

            InputState inputStateManager = inputServices.GetInputState();
            
            if (inputStateManager.IsKeyReleased(Key.Back))
                SelectPreviousItem();

            if (ActiveScreen != null)
                ActiveScreen.Update(gameTime);
            
            base.Update(gameTime);
        }

        public override void Draw(long gameTime)
        {
            if( ActiveScreen != null )
                ActiveScreen.Draw(gameTime);
            base.Draw(gameTime);
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
                    UIControl defaultFocusTarget = VisualRoot.FocusManager.GetFirstInTabOrder(ActiveScreen, false, false);
                    if (defaultFocusTarget != null)
                        VisualRoot.FocusManager.SetFocus(defaultFocusTarget);
                    //VisualRoot.FocusManager.SetNextFocus(ActiveScreen);
                }
            }
                    
            base.OnSelectedItemChanged();
        }

        //[Obsolete]
        //public override UIControl _GetNextFocusableChild(UIControl fromControl)
        //{
        //    if (ActiveScreen != null)
        //        return ActiveScreen._GetNextFocusableChild(fromControl);
        //    else
        //        return null;
        //}
    }

    public class ScreenPanel : ListControlItem, ICommandControl
    {
        CommandControlWireUp _wireUp = null;

        public ScreenPanel(Components.IUIRoot visualRoot)
            : base(visualRoot)
        {
            _wireUp = new CommandControlWireUp(this);
            _wireUp.ChildControlCommand += CommandControlWireUp_ChildControlCommand;
        }

        void CommandControlWireUp_ChildControlCommand(object sender, ControlCommandArgs args)
        {
            OnCommand(args.CommandName);
        }       
    }    
}
