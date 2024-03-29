﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.Aurora.Core.UI.Components;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Controls
{
    public abstract class ContainerControl : UIControl
    {
        private IControlLayoutManager _layoutManager = null;

        protected ContainerControl(IUIRoot root)
            : base(root)
        {
            _layoutManager = new ContainerControlLayoutManager(this);
        }

        protected override IControlLayoutManager LayoutManager
        {
            get
            {
                return _layoutManager;
            }
        }

        protected override void OnDraw()
        {
            foreach (UIControl control in Children)
                control.Draw();
            base.OnDraw();    
        }      

        protected override void OnUpdate(GameTime gameTime)
        {
            foreach (UIControl child in this.Children)
                child.Update(gameTime);
            base.OnUpdate(gameTime);
        }

        protected override void OnInitialize()
        {
            foreach (UIControl child in this.Children)
                child.Initialize();
            base.OnInitialize();
        }

        public override void ApplyTemplate(UIControlTemplate template)
        {          
            base.ApplyTemplate(template);
        }

        //[Obsolete]
        //public override UIControl _GetNextFocusableChild(UIControl fromControl)
        //{
        //    if (fromControl == null)
        //        throw new ArgumentNullException("fromControl");

        //    if (fromControl.Parent != this)
        //        throw new InvalidOperationException("starting control is not an immediate child of this control");

        //    UIControl[] tabOrderedControls = FocusManager.ToTabOrderedControlArray(Children);
        //    int startAfterIndex = Array.IndexOf(tabOrderedControls, fromControl);            
        //    for (int i = startAfterIndex + 1; i < tabOrderedControls.Length; i++)
        //    {
        //        if (tabOrderedControls[i].Visible && tabOrderedControls[i].Enabled && tabOrderedControls[i].CanReceiveFocus)
        //            return tabOrderedControls[i];
        //    }

        //    return null;
        //}

        protected override void OnChildAdded(UIControl child)
        {
            base.OnChildAdded(child);
            UpdateLayout(LayoutUpdateReason.ChildCountChanged);
        }

        protected override void OnChildRemoved(UIControl child)
        {
            base.OnChildRemoved(child);
            UpdateLayout(LayoutUpdateReason.ChildCountChanged);
        }

        protected override void OnChildReset()
        {
            base.OnChildReset();
            UpdateLayout(LayoutUpdateReason.ChildCountChanged);
        }
    }
}
