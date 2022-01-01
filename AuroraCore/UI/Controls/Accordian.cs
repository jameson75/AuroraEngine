using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.Aurora.Core.UI.Components;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Controls
{
    public class Accordian : ItemsControl
    {
        private bool isLayoutUpdating = false;
        
        public Accordian(IUIRoot visualRoot)
            : base(visualRoot)
        { }

        protected override void OnDraw()
        {
            foreach (ToolMenuItem panel in Items)
                panel.Draw();
            base.OnDraw();
        }
        
        protected override void OnLayoutChanged()
        {
            if (!isLayoutUpdating)
            {
                isLayoutUpdating = true;
                float offset = 0f;
                for (int i = 0; i < Items.Count; i++)
                {
                    if (i == 0)
                        offset += this.Margin.Top;
                    else
                        offset += Items[i - 1].Padding.Bottom;
                    Items[i].Position = new Vector2(this.Margin.Left, offset);
                    Items[i].Size = new Size2F(this.Size.Width - this.Margin.Right, Items[i].Size.Height);
                    offset += Items[0].Size.Height;
                }
                isLayoutUpdating = false;                
            }            
        }

        protected override void OnItemAdded(ItemControl item)
        {
            item.SizeChanged += Items_SizeChanged;           
            base.OnItemAdded(item);
        }

        protected override void OnItemRemoved(ItemControl item)
        {
            item.SizeChanged -= Items_SizeChanged;          
            base.OnItemRemoved(item);
        }

        private void Items_SizeChanged(object sender, EventArgs args)
        {
            UpdateLayout(LayoutUpdateReason.ChildSizeChanged);
        }
    }

    public class ToolMenuItem : ItemControl
    {
        private bool _expanded = false;

        private Button itemButton = null;
        private Panel itemPanel = null;

        public ToolMenuItem(IUIRoot visualRoot)
            : base(visualRoot)
        {
            itemButton = new Button(visualRoot);
            this.Children.Add(itemButton);

            itemPanel = new Panel(visualRoot);
            this.Children.Add(itemPanel);
        }

        public Panel Panel { get { return itemPanel; } }

        public Button Button { get { return itemButton; } }

        protected override void OnDraw()
        {
            itemButton.Draw();
            if (this.Expanded)            
                itemPanel.Draw();          
            base.OnDraw();
        }

        public bool Expanded
        {
            get { return _expanded; }
            set
            {                
                _expanded = false;
                OnExpandedChanged();
            }
        }

        protected virtual void OnExpandedChanged()
        {
            float totalLength = 0f;
            if (this.Expanded)
            {
                totalLength = this.itemButton.Size.Height + this.itemPanel.Size.Height;
            }
            else
            {
                totalLength = this.itemButton.Size.Height;
            }
            this.Size = new Size2F(this.Size.Width, totalLength);
        }

        protected override void OnChildAdded(UIControl child)
        {
            if (child != itemButton && child != itemPanel)
                throw new InvalidOperationException("Adding child controls directly to a ToolMenuControl is not supported. Instead, add it to it's Panel property");
            base.OnChildAdded(child);
        }
    }
}
