using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.Services;
using CipherPark.AngelJacket.Core.Utils;
using SharpDX;

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public class FocusManager
    {
        private UIControl _focusedControl = null;
        private IUIRoot _visualRoot = null;

        public FocusManager(IUIRoot visualRoot)
        {
            _visualRoot = visualRoot;
        }

        public UIControl FocusedControl
        {
            get { return _focusedControl; }
        }

        public void SetFocus(UIControl control)
        {
            if (control == null)
            {
                if (_focusedControl != null)
                    LeaveFocus(_focusedControl);
            }
            else
            {
                //TODO: Implement "Focus Scope"                
                
                _focusedControl = control;
                
                //TODO: Fire Event handlers.
            }
        }

        public void LeaveFocus(UIControl control)
        {
            if (control == null)
                throw new ArgumentNullException("control");
        
            //TODO: Implement "Focus Scope"         

            if (_focusedControl == control)
                _focusedControl = null;
                //TODO: Fire Event handlers.
        }

        public void Update()
        {
            IInputService inputService = (IInputService)_visualRoot.Game.Services.GetService(typeof(IInputService));
            ControlInputState state = inputService.GetControlInputState();
            InputState.MouseButton[] buttonsDown = state.InputStateManager.GetMouseButtonsDown();
            if (buttonsDown.Any(x => x == InputState.MouseButton.Left || x == InputState.MouseButton.Right))
            {
                UIControl focusTarget = GetHitFocusTarget(_visualRoot.Controls, state.InputStateManager.GetMouseLocation());
                if (focusTarget != null && focusTarget != _focusedControl)
                    SetFocus(focusTarget);
            }
            else if (state.GetKeysDown().Contains(VirtualKey.Tab))
            {
                //[Obsolete]
                //UIControl focusTarget = FocusManager.GetNextTabTarget(_focusedControl);  

                UIControl startFromControl = null;
                if (_focusedControl != null)
                    startFromControl = _focusedControl;
                else
                {
                    UIControl[] tabOrderedControls = FocusManager.ToTabOrderedControlArray(_visualRoot.Controls);
                    if (tabOrderedControls.Length > 0)
                        startFromControl = tabOrderedControls[0];
                }

                if (startFromControl != null)
                {
                    UIControl focusTarget = GetNextFocusableTarget(startFromControl, true);
                    if (focusTarget != null)
                        SetFocus(focusTarget);
                }
            }            
        }      

        public UIControl GetNextFocusableTarget(UIControl startFromControl, bool searchUpwards = false)
        {
            if (startFromControl == null)
                throw new ArgumentNullException();

            UIControl[] startFromControlSiblingsAndSelf = (startFromControl.Parent != null) ?
                     FocusManager.ToTabOrderedControlArray(startFromControl.Parent.Children) : FocusManager.ToTabOrderedControlArray(_visualRoot.Controls);
            int startFromControlIndex = Array.IndexOf(startFromControlSiblingsAndSelf, startFromControl);
            if (startFromControlIndex == -1)
                throw new InvalidOperationException("control did not have a parent nor was it a top level control of the visual root.");

            for (int i = startFromControlIndex; i < startFromControlSiblingsAndSelf.Length; i++)
            {
                if (IsEligibleForFocus(startFromControlSiblingsAndSelf[i]))
                    return startFromControlSiblingsAndSelf[i];
                else
                {
                    UIControl[] startFromChildren = FocusManager.ToTabOrderedControlArray(startFromControlSiblingsAndSelf[i].Children);
                    for (int j = 0; j < startFromChildren.Length; j++)
                    {
                        UIControl focusableChild = GetNextFocusableTarget(startFromChildren[j]);
                        if (focusableChild != null)
                            return focusableChild;
                    }
                    
                    //***********************************************************************************************************************
                    //NOTE: What we want to do is start searching up the tree only after we've finished searching down the original subtree.
                    //***********************************************************************************************************************
                    if (searchUpwards && startFromControl.Parent != null)
                    {
                        UIControl[] parentSiblingsAndParent = (startFromControl.Parent.Parent != null) ?
                            FocusManager.ToTabOrderedControlArray(startFromControl.Parent.Parent.Children) : FocusManager.ToTabOrderedControlArray(_visualRoot.Controls);
                        int parentIndex = Array.IndexOf(parentSiblingsAndParent, startFromControl.Parent);
                        if (parentIndex == -1)
                            throw new InvalidOperationException("control's parent did not have a parent nor was it's parent a top level control of the visual root.");
                        if (parentIndex + 1 < parentSiblingsAndParent.Length - 1)
                            return GetNextFocusableTarget(parentSiblingsAndParent[parentIndex + 1], true);
                    }                    
                }
            }

            return null;          
        }

        public static bool IsEligibleForFocus(UIControl control)
        {
            return control.Visible && control.Enabled && control.CanFocus && control.EnableFocus;
        }

        //[Obsolete]
        //private static UIControl _GetNextTabTarget(UIControl fromControl)
        //{
        //    //NOTE: Unlike WinForms, we don't use the concept of a focus container.
        //    //instead. We allow a controls parent to determine how to navigate 
        //    //items.
        //    if (fromControl == null || fromControl.Parent == null)
        //    {
        //        UIControl[] tabOrderedControls = FocusManager.ToTabOrderedControlArray(fromControl.VisualRoot.Controls);
        //        int K = 0;
        //        if (fromControl != null)
        //        {
        //            int fromControlIndex = Array.IndexOf(tabOrderedControls, fromControl);
        //            if (fromControlIndex == -1)
        //                throw new InvalidOperationException("fromControl must either have a parent or be be an immediate child of the root");
        //            K = fromControlIndex + 1;
        //        }                  
        //        for (int i = K; i < tabOrderedControls.Length; i++)
        //        {
        //            if (tabOrderedControls[i].Visible && tabOrderedControls[i].Enabled && tabOrderedControls[i].CanFocus)
        //                return tabOrderedControls[i];
        //        }
        //        return null;
        //    }
        //    else
        //        return fromControl.Parent._GetNextFocusableChild(fromControl);          
        //}

        public static UIControl GetFirstHitSibling(IEnumerable<UIControl> siblings, DrawingPoint mouseLocation, bool mustBeVisible = true)
        {
            UIControl[] zOrderedSiblings = FocusManager.ToZOrderedControlArray(siblings);
            for (int i = 0; i < zOrderedSiblings.Length; i++)
                if ((zOrderedSiblings[i].Visible || !mustBeVisible) && zOrderedSiblings[i].Bounds.Contains(mouseLocation))
                    return zOrderedSiblings[i];
            return null;
        }          

        public static UIControl GetHitFocusTarget(IEnumerable<UIControl> siblings, DrawingPoint mouseLocation)
        {
            UIControl hitSibling = FocusManager.GetFirstHitSibling(siblings, mouseLocation); //NOTE: we get the first VISIBLE hit sibling.
            if( hitSibling != null )
            {                               
                UIControl focusTarget = GetHitFocusTarget(hitSibling.Children, mouseLocation);
                if (focusTarget != null)
                    return focusTarget;          
                else
                {
                    if (hitSibling.Enabled && hitSibling.CanFocus)
                    {
                        if (hitSibling.Bounds.Contains(mouseLocation))
                            return hitSibling;
                    }
                }
            }
            return null;
        }

        public static UIControl[] ToZOrderedControlArray(IEnumerable<UIControl> controls)
        {
            SortedList<float, List<UIControl>> table = new SortedList<float, List<UIControl>>();
            foreach (UIControl control in controls)
            {
                if (!table.ContainsKey(control.ZOrder))
                    table.Add(control.ZOrder, new List<UIControl>());
                table[control.ZOrder].Add(control);
            }

            List<UIControl> zOrderedList = new List<UIControl>();
            foreach (float key in table.Keys)
                foreach (UIControl control in table[key])
                    zOrderedList.Add(control);

            return zOrderedList.ToArray();
        }

        public static UIControl[] ToTabOrderedControlArray(IEnumerable<UIControl> controls)
        {
            SortedList<float, List<UIControl>> table = new SortedList<float, List<UIControl>>();
            foreach (UIControl control in controls)
            {
                if (!table.ContainsKey(control.TabOrder))
                    table.Add(control.TabOrder, new List<UIControl>());
                table[control.TabOrder].Add(control);
            }

            List<UIControl> tabOrderedList = new List<UIControl>();
            foreach (float key in table.Keys)
                foreach (UIControl control in table[key])
                    tabOrderedList.Add(control);

            return tabOrderedList.ToArray();
        }
    }
}
