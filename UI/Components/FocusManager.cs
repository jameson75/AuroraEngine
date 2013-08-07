using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.Services;
using CipherPark.AngelJacket.Core.Utils;
using SharpDX;
using SharpDX.DirectInput;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

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
                if (_focusedControl != control)
                {
                    if(_focusedControl != null )
                        LeaveFocus(_focusedControl);
                    _focusedControl = control;
                    OnSetFocus(_focusedControl);
                }
            }
        }

        public void LeaveFocus(UIControl control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (_focusedControl == control)
            {
                _focusedControl = null;
                OnLeaveFocus(control);
            }                
        }

        protected void OnLeaveFocus(UIControl control)
        {
            FocusChangedEventHandler handler = ControlLostFocus;
            if(handler != null)
                handler(this, new FocusChangedEventArgs(control));
        }

        protected void OnSetFocus(UIControl control)
        {
            FocusChangedEventHandler handler = ControlReceivedFocus;
            if (handler != null)
                handler(this, new FocusChangedEventArgs(control));
        }

        public event FocusChangedEventHandler ControlLostFocus;

        public event FocusChangedEventHandler ControlReceivedFocus;

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This method isn't meant to be called from application code. Calling this method may result in a stack flow from recursion.
        /// </remarks>
        public void Update()
        {
            IInputService inputService = (IInputService)_visualRoot.Game.Services.GetService(typeof(IInputService));
            BufferedInputState state = inputService.GetBufferedInputState();
            InputState.MouseButton[] buttonsDown = state.InputState.GetMouseButtonsDown();
            if (buttonsDown.Any(x => x == InputState.MouseButton.Left || x == InputState.MouseButton.Right))
            {
                UIControl focusTarget = GetHitFocusTarget(_visualRoot.Controls, state.InputState.GetMouseLocation());
                if (focusTarget != null && focusTarget != _focusedControl)
                    SetFocus(focusTarget);
            }
            else if (state.GetKeysDown().Contains(Key.Tab))
            {
                if (state.GetKeysDown().Any(k => k == Key.LeftShift || k == Key.RightShift))
                    SetPreviousFocus(_focusedControl);
                else
                    SetNextFocus(_focusedControl);
            }            
        }

        public void SetPreviousFocus(UIControl focusedControl)
        {

        }

        public void SetNextFocus(UIControl previousFocusedControl, bool includeChildren = true)
        {
            UIControl nextFocusControl = null;
            if (previousFocusedControl != null)
            {
                if (previousFocusedControl.CustomFocusManager != null)
                    previousFocusedControl.CustomFocusManager.SetNextFocus(previousFocusedControl);
                else
                    nextFocusControl = GetNextInTabOrder(previousFocusedControl, true, includeChildren);
            }
            else
            {
                UIControl[] tabOrderedControls = FocusManager.ToTabOrderedControlArray(_visualRoot.Controls);
                if (tabOrderedControls.Length > 0)
                    nextFocusControl = GetFirstInTabOrder(tabOrderedControls[0]);
            }

            if (nextFocusControl != null)
                SetFocus(nextFocusControl);
        }

        //public void SetNextFocus(UIControl focusedControl, bool includeChildren = true)
        //{
        //    UIControl startFromControl = null;
        //    if (focusedControl != null)
        //    {
        //        if (focusedControl.CustomFocusManager != null)
        //            focusedControl.CustomFocusManager.SetNextFocus(focusedControl);
        //        else
        //            startFromControl = GetNextInTabOrder(focusedControl, includeChildren);
        //    }

        //    else
        //    {
        //        UIControl[] tabOrderedControls = FocusManager.ToTabOrderedControlArray(_visualRoot.Controls);
        //        if (tabOrderedControls.Length > 0)
        //            startFromControl = tabOrderedControls[0];
        //    }

        //    if (startFromControl != null)
        //    {
        //        UIControl focusTarget = FindFirstFocusableTarget(startFromControl);   
        //        SetFocus(focusTarget);
        //    
        //}

        public UIControl GetNextInTabOrder(UIControl previousControl, bool searchAncestors = true, bool searchChildren = true)
        {
            UIControl startFromControl = null;

            //if we're searching children, use the previous control's first-sibling (in tab order).
            if (searchChildren && previousControl.Children.Count > 0)
            {
                UIControl[] tabOrderedPreviousControlChildren = ToTabOrderedControlArray(previousControl.Children);
                startFromControl = tabOrderedPreviousControlChildren[0];
            }

            //if we haven't found a control to start from yet, use the previous control's next sibling (in tab ordered).
            if(startFromControl == null)
            {
                UIControl[] tabOrderedSiblingsAndPrevious = (previousControl.Parent != null) ?
                    ToTabOrderedControlArray(previousControl.Parent.Children) : ToTabOrderedControlArray(previousControl.VisualRoot.Controls);
                int previousControlIndex = Array.IndexOf(tabOrderedSiblingsAndPrevious, previousControl);
                if( previousControlIndex != -1 && previousControlIndex + 1 < tabOrderedSiblingsAndPrevious.Length)
                    startFromControl = tabOrderedSiblingsAndPrevious[previousControlIndex + 1];
            }

            //if we haven't found a start control and we're searching ancestors we use the next-sibling (in tab order) of the closest ancestor who has a next-sibling.
            if (startFromControl == null && searchAncestors && previousControl.Parent != null)
            {
                UIControl p = previousControl.Parent;
                while (p != null)
                {
                    UIControl[] pSiblingsAndP = (p.Parent != null) ?
                        ToTabOrderedControlArray(p.Parent.Children) : ToTabOrderedControlArray(p.VisualRoot.Controls);
                    int pIndex = Array.IndexOf(pSiblingsAndP, p);
                    if( pIndex != -1 && pIndex + 1 < pSiblingsAndP.Length)
                    {
                        startFromControl = pSiblingsAndP[pIndex + 1];
                        break;
                    }
                }
            }

            if( startFromControl != null )
                return GetFirstInTabOrder(startFromControl, searchAncestors, true, searchChildren);
            else
                return null;
        }
    
        //public UIControl GetNextInTabOrder(UIControl previousControl, bool includeChildren = true)
        //{
        //    if (includeChildren && previousControl.Children.Count > 0)
        //        return previousControl.Children[0];
        //    else
        //    {
        //        UIControl[] tabOrderedSiblingsAndSelf = null;
        //        if (previousControl.Parent != null)
        //            tabOrderedSiblingsAndSelf = ToTabOrderedControlArray(previousControl.Parent.Children);
        //        else
        //            tabOrderedSiblingsAndSelf = ToTabOrderedControlArray(previousControl.VisualRoot.Controls);
        //        int startIndex = Array.IndexOf(tabOrderedSiblingsAndSelf, previousControl);
        //        if (startIndex >= 0 && startIndex > tabOrderedSiblingsAndSelf.Length - 1)
        //            return tabOrderedSiblingsAndSelf[startIndex + 1];
        //        else
        //            return null;
        //    }
        //}

        //public UIControl GetNextInTabOrder(UIControl previousControl, bool searchAncestors = true, bool searchChildren = true)
        //{
        //    if (previousControl == null)
        //        throw new ArgumentNullException("previousControl");

        //    UIControl[] previousControlSiblingsAndPrevious = null;     
        //    previousControlSiblingsAndPrevious = (previousControl.Parent != null) ?
        //              FocusManager.ToTabOrderedControlArray(previousControl.Parent.Children) : FocusManager.ToTabOrderedControlArray(_visualRoot.Controls);
        
        //    int previousControlIndex = Array.IndexOf(previousControlSiblingsAndPrevious, previousControl);
        //    if (previousControlIndex == -1)
        //        throw new InvalidOperationException("previous control did not have a parent nor was it a top level control of the visual root.");

        //    for (int i = previousControlIndex + 1; i < previousControlSiblingsAndPrevious.Length; i++)
        //    {
        //        if (IsEligibleForFocus(previousControlSiblingsAndPrevious[i]))
        //            return previousControlSiblingsAndPrevious[i];
        //        else
        //        {
        //            if (searchChildren && IsVisibleAndEnabled(previousControlSiblingsAndPrevious[i]))
        //            {
        //                UIControl[] children = FocusManager.ToTabOrderedControlArray(previousControlSiblingsAndPrevious[i].Children);
        //                if (children.Length > 0 && IsEligibleForFocus(children[0]))
        //                    return children[0];
        //                else
        //                {
        //                    for (int j = 0; j < children.Length; j++)
        //                    {
        //                        UIControl focusableChild = GetNextInTabOrder(children[j], false);
        //                        if (focusableChild != null)
        //                            return focusableChild;
        //                    }
        //                }
        //            }                    
        //        }
        //    }

        //    //***********************************************************************************************************************
        //    //NOTE: What we want to do is start searching up the tree only after we've finished searching down the original subtree.
        //    //***********************************************************************************************************************
        //    if (searchAncestors && previousControl.Parent != null)
        //    {
        //        UIControl[] parentSiblingsAndParent = (previousControl.Parent.Parent != null) ?
        //            FocusManager.ToTabOrderedControlArray(previousControl.Parent.Parent.Children) : FocusManager.ToTabOrderedControlArray(_visualRoot.Controls);
        //        int parentIndex = Array.IndexOf(parentSiblingsAndParent, previousControl.Parent);
        //        if (parentIndex == -1)
        //            throw new InvalidOperationException("prevous control's parent did not have a parent nor was it's parent a top level control of the visual root.");
        //        return GetNextInTabOrder(parentSiblingsAndParent[parentIndex]);
        //    }

        //    return null;
        //}

        public UIControl GetFirstInTabOrder (UIControl startFromControl, bool searchUpwards = true, bool searchSiblings = true, bool searchChildren = true)
        {
            if (startFromControl == null)
                throw new ArgumentNullException();

            UIControl[] startFromControlSiblingsAndSelf = null;

            if (searchSiblings)
            {
                startFromControlSiblingsAndSelf = (startFromControl.Parent != null) ?
                      FocusManager.ToTabOrderedControlArray(startFromControl.Parent.Children) : FocusManager.ToTabOrderedControlArray(_visualRoot.Controls);
            }
            else
                startFromControlSiblingsAndSelf = new UIControl[] { startFromControl };

            int startFromControlIndex = Array.IndexOf(startFromControlSiblingsAndSelf, startFromControl);
            if (startFromControlIndex == -1)
                throw new InvalidOperationException("control did not have a parent nor was it a top level control of the visual root.");

            for (int i = startFromControlIndex; i < startFromControlSiblingsAndSelf.Length; i++)
            {
                if (IsEligibleForFocus(startFromControlSiblingsAndSelf[i]))
                    return startFromControlSiblingsAndSelf[i];

                else
                {
                    if (searchChildren && IsVisibleAndEnabled(startFromControlSiblingsAndSelf[i]))
                    {
                        UIControl[] startFromChildren = FocusManager.ToTabOrderedControlArray(startFromControlSiblingsAndSelf[i].Children);
                        for (int j = 0; j < startFromChildren.Length; j++)
                        {
                            UIControl focusableChild = GetFirstInTabOrder(startFromChildren[j], false);
                            if (focusableChild != null)
                                return focusableChild;
                        }
                    }                    
                }
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
                if (parentIndex + 1 < parentSiblingsAndParent.Length)
                    return GetFirstInTabOrder(parentSiblingsAndParent[parentIndex + 1]);
            }
            
            return null;
        }

        public static bool IsEligibleForFocus(UIControl control)
        {
            return control.Visible && control.Enabled && control.CanFocus && control.EnableFocus;
        }

        public static bool IsVisibleAndEnabled(UIControl control)
        {
            return control.Visible && control.Enabled;
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
                if ((zOrderedSiblings[i].Visible || !mustBeVisible) && zOrderedSiblings[i].Bounds.Contains(mouseLocation.ToDrawingPointF()))
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
                        if (hitSibling.Bounds.Contains(mouseLocation.ToDrawingPointF()))
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

    public interface ICustomFocusManager
    {
        void SetNextFocus(UIControl focusedControl);

        void SetPreviousFocus(UIControl focusedControl);
    } 

    public delegate void FocusChangedEventHandler(object sender, FocusChangedEventArgs args);

    public class FocusChangedEventArgs : EventArgs
    {
        private UIControl _control = null;

        public FocusChangedEventArgs(UIControl control)
        {
            _control = control;
        }

        public UIControl Control { get { return _control; } }
    }
}
