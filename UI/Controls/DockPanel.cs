using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpDX;
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
    //TODO: Refactor the logic from this control into a layout manager
    //      Then use it help create reusable layouts for 
    //      sanboxes. Also, inherit from Panel.

    public class DockPanel : UIControl
    {
        public DockPanel(IUIRoot visualRoot)
            : base(visualRoot)
        {

        }

        public void DockControl(UIControl control, DockSide side, int? insertAt = null)
        {
            DockPanelContainer container = new DockPanelContainer(VisualRoot, control, side);
            if (insertAt == null)
                Children.Add(container);
            else
            {
                if (insertAt.Value < 0 || insertAt.Value >= Children.Count)
                    throw new IndexOutOfRangeException("insertAt argument is out of range");                   
                Children.Insert(insertAt.Value, container);                
            }
            UpdateLayout(LayoutUpdateReason.ClientAreaChanged);
        }

        protected override void OnChildAdded(UIControl child)
        {
            if (child is DockPanelContainer == false)
            {
                DockSide recommendedSide = DockSide.Client;
                if (this.Children.Count > 0)
                    recommendedSide = ((DockPanelContainer)this.Children[this.Children.Count - 1]).Side;
                DockControl(child, recommendedSide);
            }
            UpdateLayout(LayoutUpdateReason.ChildCountChanged);
            base.OnChildAdded(child);
        }

        protected override void OnChildRemoved(UIControl child)
        {
            UpdateLayout(LayoutUpdateReason.ChildCountChanged);
            base.OnChildRemoved(child);
        }

        protected override void OnChildReset()
        {
            UpdateLayout(LayoutUpdateReason.ChildCountChanged);
            base.OnChildReset();
        }

        protected override void OnLayoutChanged()
        {
            RectangleF clientAreaRect = this.Bounds;
            foreach (DockPanelContainer container in this.Children)
            {
                switch (container.Side)
                {
                    case DockSide.Left:
                        container.Position = clientAreaRect.Position();
                        container.Size = new DrawingSizeF(container.Size.Width, clientAreaRect.Height);
                        clientAreaRect.Left += container.Size.Width;
                        break;
                    case DockSide.Top:
                        container.Position = clientAreaRect.Position();
                        container.Size = new DrawingSizeF(clientAreaRect.Width, container.Size.Height);
                        clientAreaRect.Top += container.Size.Height;
                        break;
                    case DockSide.Right:
                        container.Position = new DrawingPointF(clientAreaRect.Right - container.Size.Width, clientAreaRect.Top);
                        container.Size = new DrawingSizeF(container.Size.Width, clientAreaRect.Height);
                        clientAreaRect.Right -= container.Size.Width;
                        break;
                    case DockSide.Bottom:
                        container.Position = new DrawingPointF(clientAreaRect.Left, clientAreaRect.Bottom - container.Size.Height);
                        container.Size = new DrawingSizeF(clientAreaRect.Width, container.Size.Height);
                        clientAreaRect.Bottom -= container.Size.Height;
                        break;
                }                
            }

            foreach (DockPanelContainer container in this.Children)
            {
                if (container.Side == DockSide.Client)
                {
                    container.Position = clientAreaRect.Position();
                    container.Size = clientAreaRect.Size();
                }
            }

            base.OnLayoutChanged();
        }

        protected override void OnUpdate(long gameTime)
        {
            foreach (UIControl control in this.Children)
                control.Update(gameTime);
            base.OnUpdate(gameTime);
        }

        protected override void OnDraw(long gameTime)
        {
            foreach (UIControl control in this.Children)
                control.Draw(gameTime);
            base.OnDraw(gameTime);
        }

        private class DockPanelContainer : ContainerControl
        {
            public DockSide Side { get; set; }

            public DockPanelContainer(IUIRoot visualRoot, UIControl control, DockSide side)
                : base(visualRoot)
            {           
                Children.Add(control);
                Size = control.Size;
                Side = side;
            }
        }
    }   

    public enum DockSide
    {
        None,       
        Left,
        Top,
        Right,
        Bottom,
        Client = None,
    }
}
