using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Components;
using SharpDX;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class ToolMenu : ItemsControl
    {
        private bool isLayoutUpdating = false;
        
        public ToolMenu(IUIRoot visualRoot)
            : base(visualRoot)
        { }     

        public override void Draw(long gameTime)
        {
            foreach (ToolMenuItem panel in Items)
                panel.Draw(gameTime);
            base.Draw(gameTime);
        }
        
        protected override void OnLayoutChanged()
        {
            if (!isLayoutUpdating)
            {
                isLayoutUpdating = true;
                float offset = 0.0f;
                for (int i = 0; i < Items.Count; i++)
                {
                    if (i == 0)
                        offset += this.Margin.Y;
                    else
                        offset += Items[i - 1].Padding.Y;
                    Items[i].Position = new Vector2(0.0f, offset);
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

        public override void Draw(long gameTime)
        {
            itemButton.Draw(gameTime);
            if (this.Expanded)            
                itemPanel.Draw(gameTime);          
            base.Draw(gameTime);
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
            float totalLength = 0;
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
