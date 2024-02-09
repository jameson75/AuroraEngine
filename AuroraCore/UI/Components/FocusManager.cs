using System;
using System.Collections.Generic;
using System.Linq;
using CipherPark.Aurora.Core.UI.Controls;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.Extensions;
using SharpDX;
using SharpDX.DirectInput;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Components
{
    public class FocusManager
    {
        private UIControl _focusedControl = null;
        private IUIRoot _visualRoot = null;
        private UIControlCollection _hitList = null;
        private UIControl _pendingFocusControl = null;

        public FocusManager(IUIRoot visualRoot)
        {
            _visualRoot = visualRoot;
            _hitList = new UIControlCollection();
        }

        public UIControl FocusedControl
        {
            get { return _focusedControl; }
        }

        public UIControlCollection HitList
        {
            get { return _hitList; }
        }

        public void SetFocus(UIControl control)
        {
            if (control != null)
            {
                if (!IsEligibleForFocus(control))
                    throw new InvalidOperationException("Control is not eligible for receiving focus");               
            }
            
            _SetFocus(control);
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
            ListenForFocusEligibilityChange(false, control);
        }

        protected void OnSetFocus(UIControl control)
        {
            FocusChangedEventHandler handler = ControlReceivedFocus;
            if (handler != null)
                handler(this, new FocusChangedEventArgs(control));
            ListenForFocusEligibilityChange(true, control);
        }

        public event FocusChangedEventHandler ControlLostFocus;

        public event FocusChangedEventHandler ControlReceivedFocus;
   
        private static bool IsEligibleForFocus(UIControl control)
        {
            return control.VisibleInTree &&
                   control.EnabledInTree &&
                   control.CanReceiveFocus &&
                   control.EnableFocus;                   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This method isn't meant to be called from application code. Avoid calling this method from game logic.
        /// </remarks>
        public void Update()
        {
            IInputService inputService = (IInputService)_visualRoot.Game.Services.GetService(typeof(IInputService));
            BufferedInputState state = inputService.GetBufferedInputState();
            InputState.MouseButton[] buttonsDown = state.InputState.GetMouseButtonsDown();
            _hitList.Clear();            
            if (buttonsDown.Any(x => x == InputState.MouseButton.Left || x == InputState.MouseButton.Right))
            {
                PopulateHitList(_visualRoot.Controls, state.InputState.GetMouseLocation(), _hitList);
                UIControl focusTarget = GetHitFocusTarget(_hitList);
                if (focusTarget != null && focusTarget != _focusedControl)
                    _SetFocus(focusTarget);
            }
            else if (state.GetKeysDown().Contains(Key.Tab))
            {
                if (state.GetKeysDown().Any(k => k == Key.LeftShift || k == Key.RightShift))
                {
                    UIControl previousControl = GetPrevious(_focusedControl);
                    if (previousControl != null)
                        _SetFocus(previousControl);
                }
                else
                {
                    UIControl nextControl = GetNext(_focusedControl);
                    if (nextControl != null)
                        _SetFocus(nextControl);
                }
            }            
        }       
        
        public void PostUpdate()
        {
            if(_pendingFocusControl != null)
            {
                _SetFocus(_pendingFocusControl);
                _pendingFocusControl = null;
            }
        }

        public void PostFocus(UIControl control)
        {
            if (!IsEligibleForFocus(control))
                throw new InvalidOperationException();
            _pendingFocusControl = control;
        }

        public UIControl GetPrevious(UIControl fromControl)
        {
            UIControl previousFocusControl = null;
            if (fromControl != null)
            {
                UIControl[] tabOrderedSelfAndSiblings = (fromControl.Parent != null) ? FocusManager.ToTabOrderedControlArray(fromControl.Parent.Children) :
                                                                                        FocusManager.ToTabOrderedControlArray(_visualRoot.Controls);
                int startIndex = Array.IndexOf(tabOrderedSelfAndSiblings, fromControl) - 1;
                for (int i = startIndex; i >= 0; i--)
                {
                    UIControl c = tabOrderedSelfAndSiblings[i];
                    if (IsEligibleForFocus(c))
                    {
                        previousFocusControl = c;
                        break;
                    }
                }

                if (previousFocusControl == null)
                {
                    if (fromControl.Parent != null)
                        previousFocusControl = GetPrevious(fromControl.Parent);
                }                
            }

            return previousFocusControl;
        }            
        
        public UIControl GetNext(UIControl previousControl)
        {
            UIControl nextFocusControl = null;
            if (previousControl != null)
            {
            //    if (previousControl.CustomFocusContainer != null)
            //        ((ICustomFocusContainer)previousControl.CustomFocusContainer).SetNextFocus(previousControl);
            //    else
                    nextFocusControl = GetNextInTabOrder(previousControl);
            }
            else
            {
                UIControl[] tabOrderedControls = FocusManager.ToTabOrderedControlArray(_visualRoot.Controls);
                if (tabOrderedControls.Length > 0)
                    nextFocusControl = GetFirstInTabOrder(tabOrderedControls[0]);
            }

            return nextFocusControl;
        }

        private UIControl GetNextInTabOrder(UIControl previousControl)
        {
            UIControl startFromControl = null;
            bool canStartFromFirstChild = true;
            bool canStartFromNextSibling = true;
            bool canStartFromAncestorNext = true;

            //if the previous control override focus management, determine if can start from firstChild, nextSibling, or firstUncle.
            if (previousControl is ICustomFocusContainer)
            {
                canStartFromFirstChild = ((ICustomFocusContainer)previousControl).CanTabInward;
                //canStartFromNextSibling = ((ICustomFocusContainer)previousControl).CanMoveToSibling;
                canStartFromAncestorNext = ((ICustomFocusContainer)previousControl).CanTabOutward;
            }

            //if we're searching children, use the previous control's first-sibling (in tab order).
            if (canStartFromFirstChild && previousControl.Children.Count > 0)
            {
                UIControl[] tabOrderedPreviousControlChildren = ToTabOrderedControlArray(previousControl.Children);
                startFromControl = tabOrderedPreviousControlChildren[0];
            }

            //if we haven't found a control to start from yet and we're searching siblings, use the previous control's next sibling (in tab ordered).
            if(canStartFromNextSibling && startFromControl == null)
            {
                UIControl[] tabOrderedSiblingsAndPrevious = (previousControl.Parent != null) ?
                    ToTabOrderedControlArray(previousControl.Parent.Children) : ToTabOrderedControlArray(previousControl.VisualRoot.Controls);
                int previousControlIndex = Array.IndexOf(tabOrderedSiblingsAndPrevious, previousControl);
                if( previousControlIndex != -1 && previousControlIndex + 1 < tabOrderedSiblingsAndPrevious.Length)
                    startFromControl = tabOrderedSiblingsAndPrevious[previousControlIndex + 1];
            }

            //if we haven't found a control to start from yet and we're searching ancestors, we use the next-sibling (in tab order) of the closest ancestor who has a next-sibling.
            if (canStartFromAncestorNext && startFromControl == null)
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
                return GetFirstInTabOrder(startFromControl);
            else
                return null;
        }

        private UIControl GetFirstInTabOrder(UIControl startFromControl)
        {
            return GetFirstInTabOrder(startFromControl, true);
        }

        private UIControl GetFirstInTabOrder (UIControl startFromControl, bool searchUpwards)
        {
            //***********************************************
            //TODO: Revise this method to make
            //***********************************************
            if (startFromControl == null)
                throw new ArgumentNullException();

            UIControl[] startFromControlSiblingsAndSelf = null;

            //if (searchSiblings)
            //{
                startFromControlSiblingsAndSelf = (startFromControl.Parent != null) ?
                      FocusManager.ToTabOrderedControlArray(startFromControl.Parent.Children) : FocusManager.ToTabOrderedControlArray(_visualRoot.Controls);
            //}
           // else
            //    startFromControlSiblingsAndSelf = new UIControl[] { startFromControl };
            
            int startFromControlIndex = Array.IndexOf(startFromControlSiblingsAndSelf, startFromControl);
            
            if (startFromControlIndex == -1)
                throw new InvalidOperationException("control did not have a parent nor was it a top level control of the visual root.");
            
            for (int i = startFromControlIndex; i < startFromControlSiblingsAndSelf.Length; i++)
            {
                ICustomFocusContainer focusContainer = startFromControlSiblingsAndSelf[i] as ICustomFocusContainer;
                
                if (IsEligibleForFocus(startFromControlSiblingsAndSelf[i]))
                    return startFromControlSiblingsAndSelf[i];
                else
                {
                    bool canSearchChildren = (focusContainer == null) ? true : focusContainer.CanTabInward;                    
                    if (canSearchChildren && IsVisibleAndEnabledInTree(startFromControlSiblingsAndSelf[i]))
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

                //if (focusContainer != null && !focusContainer.CanMoveToSibling)
                //    break;
            }
             
            //***********************************************************************************************************************
            //NOTE: What we want to do is start searching up the tree only after we've finished searching down the original subtree.
            //***********************************************************************************************************************
            bool canSearchAncestorNext = (startFromControl is ICustomFocusContainer) ? ((ICustomFocusContainer)startFromControl).CanTabOutward : true;
            if (searchUpwards && canSearchAncestorNext && startFromControl.Parent != null)
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

        private void _SetFocus(UIControl control)
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
                    if (_focusedControl != null)
                        LeaveFocus(_focusedControl);
                    _focusedControl = control;
                    OnSetFocus(_focusedControl);
                }
            }
        }

        private static bool IsVisibleAndEnabledInTree(UIControl control)
        {
            return control.VisibleInTree && control.EnabledInTree;
        }       

        private static UIControl GetFirstHitInZOrder(IEnumerable<UIControl> siblings, Point mouseLocation, bool mustBeVisible = true, bool mustBeEnabled = true)
        {
            UIControl[] zOrderedSiblings = FocusManager.ToZOrderedControlArray(siblings);
            for (int i = 0; i < zOrderedSiblings.Length; i++)
                if ((zOrderedSiblings[i].Visible || !mustBeVisible) && (zOrderedSiblings[i].Enabled|| !mustBeEnabled) && zOrderedSiblings[i].BoundsToSurface(zOrderedSiblings[i].Bounds).Contains(mouseLocation.ToVector2()))
                    return zOrderedSiblings[i];
            return null;
        }                

        private static UIControl GetHitFocusTarget(IList<UIControl> hitList)
        {
            if (hitList.Count > 0)
            {
                UIControl targetControl = null;
                UIControl outerMostRestrictedControl = hitList.FirstOrDefault(c => c is ICustomFocusContainer && ((ICustomFocusContainer)c).CanTabInward == false);

                if (outerMostRestrictedControl != null)
                    targetControl = outerMostRestrictedControl;
                else
                    targetControl = hitList.Last();

                if (targetControl.CanReceiveFocus && targetControl.EnableFocus)
                    return targetControl;
                else
                    return null;
            }
            else
                return null;
        }

        private static void PopulateHitList(IEnumerable<UIControl> controls, Point mouseLocation, IList<UIControl> hitList)
        {
            UIControl hitControl = FocusManager.GetFirstHitInZOrder(controls, mouseLocation); //NOTE: we get the first VISIBLE hit sibling.
            if (hitControl != null)
            {
                hitList.Add(hitControl);
                PopulateHitList(hitControl.Children, mouseLocation, hitList);
            }
        }

        private static UIControl[] ToZOrderedControlArray(IEnumerable<UIControl> controls)
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

        private static UIControl[] ToTabOrderedControlArray(IEnumerable<UIControl> controls)
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

        private void ListenForFocusEligibilityChange(bool listening, UIControl control)
        {
            if (listening)
            {
                control.VisibleInTreeChanged += FocusControl_EligibilityPropertyChanged;
                control.EnabledInTreeChanged += FocusControl_EligibilityPropertyChanged;
                control.EnableFocusChanged += FocusControl_EligibilityPropertyChanged;
                control.ParentChanged += FocusControl_EligibilityPropertyChanged;
            }
            else 
            {
                control.VisibleInTreeChanged -= FocusControl_EligibilityPropertyChanged;
                control.EnabledInTreeChanged -= FocusControl_EligibilityPropertyChanged;
                control.EnableFocusChanged -= FocusControl_EligibilityPropertyChanged;
                control.ParentChanged -= FocusControl_EligibilityPropertyChanged;
            }
        }

        private void FocusControl_EligibilityPropertyChanged(object sender, EventArgs args)
        {
            if (!IsEligibleForFocus(this.FocusedControl))
                LeaveFocus(this.FocusedControl);
        }
    }

    public interface ICustomFocusContainer
    {
        bool CanTabOutward { get; }
        bool CanTabInward { get; }
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
