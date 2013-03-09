using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.UI.Components;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{    
    public class DockPanel : UIControl
    {        
        public DockPanel(IUIRoot visualRoot)
            : base(visualRoot)
        {
             
        }

        public void DockControl(UIControl control, DockSide side, bool forceEdge = false)
        {
            DockPanelContainer container = new DockPanelContainer(VisualRoot, control, side);
            if (!forceEdge)
                Children.Add(container);
            else
            {
                bool childInserted = false;
                foreach (DockPanelContainer child in Children)
                    if (((DockPanelContainer)child).Side == side)
                    {
                        Children.Insert(Children.IndexOf(child), control);
                        childInserted = true;
                        break;
                    }
                if (!childInserted)
                    Children.Add(container);
            }
            UpdateLayout(LayoutUpdateReason.ClientAreaChanged);
        }

        protected override void OnChildAdded(UIControl child)
        {
            if (child is DockPanelContainer == false)
            {
                DockSide recommendedSide = DockSide.None;
                if (this.Children.Count > 0)
                    recommendedSide = ((DockPanelContainer)this.Children[this.Children.Count - 1]).Side;
                DockControl(child, recommendedSide);
            }
            base.OnChildAdded(child);
        }
    }

    public class DockPanelContainer : UIControl
    {
        public DockSide Side { get; set; }

        public DockPanelContainer(IUIRoot visualRoot, UIControl control, DockSide side) : base(visualRoot)
        {
            Children.Add(control);
            Side = side;
        }      
    }

    public enum DockSide
    {
        None,
        Left,
        Top,
        Right,
        Bottom
    }
}
