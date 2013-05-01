using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Components;
using SharpDX;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class ToolMenu : ItemsControl
    {
        private bool isLayoutUpdating = false;
        
        public ToolMenu(IUIRoot visualRoot)
            : base(visualRoot)
        { }     

        protected override void OnDraw(long gameTime)
        {
            foreach (ToolMenuItem panel in Items)
                panel.OnDraw(gameTime);
            base.OnDraw(gameTime);
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
                        offset += this.Margin.Width;
                    else
                        offset += Items[i - 1].Padding.Height;
                    Items[i].Position = new DrawingPointF(0f, offset);
                    Items[i].Size = new DrawingSizeF(this.Size.Width, Items[i].Size.Height);
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

        protected override void OnDraw(long gameTime)
        {
            itemButton.OnDraw(gameTime);
            if (this.Expanded)            
                itemPanel.OnDraw(gameTime);          
            base.OnDraw(gameTime);
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
            this.Size = new DrawingSizeF(this.Size.Width, totalLength);
        }

        protected override void OnChildAdded(UIControl child)
        {
            if (child != itemButton && child != itemPanel)
                throw new InvalidOperationException("Adding child controls directly to a ToolMenuControl is not supported. Instead, add it to it's Panel property");
            base.OnChildAdded(child);
        }
    }
}
