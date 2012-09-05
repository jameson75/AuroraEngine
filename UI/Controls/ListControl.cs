using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Components;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class ListControl : ItemsControl
    {       
        public const int SizeInfinite = -1;
      
        private int _maxRowSize = ListControl.SizeInfinite;

        public override bool CanFocus
        {
            get
            {
                return true;
            }
        }

        public int MaxRowSize 
        {
            get { return _maxRowSize; }
            set
            {
                if (value < 1 && value != ListControl.SizeInfinite)
                    throw new InvalidOperationException("MaxRowSize was not greater or equal to one");
            }                    
        }

        public ListControl(IUIRoot root)
            : base(root)
        {
            ColumnDirection = ListColumnDirection.Horizontal;
            MaxRowSize = ListControl.SizeInfinite;
        }

        public ListColumnDirection ColumnDirection { get; set; }

        //public bool WrapContents { get; set; }

        protected override void OnLayoutChanged()
        {
            int currentRowIndex = 0;
            int currentColumnIndex = 0;

            if (ColumnDirection == ListColumnDirection.Horizontal)
            {
                float previousItemsTotalHeight = 0.0f;
                float previousColumnsWidth = 0.0f;
                float maxItemWidth = 0;
                foreach (ItemControl item in this.Items)
                {
                    item.Position = new Vector2(previousColumnsWidth, previousItemsTotalHeight);
                    previousItemsTotalHeight += item.Size.Y;
                    maxItemWidth = Math.Max(maxItemWidth, item.Size.X);
                    currentRowIndex++;
                    if (MaxRowSize != ListControl.SizeInfinite && currentRowIndex >= MaxRowSize)
                    {                       
                        currentColumnIndex++;
                        previousColumnsWidth = maxItemWidth;
                        maxItemWidth = 0;
                        currentRowIndex = 0;
                    }
                }
            }
            else
            {
                float previousItemsTotalWidth = 0.0f;
                float previousColumnsHeight = 0.0f;
                float maxItemHeight = 0;
                foreach (ItemControl item in this.Items)
                {
                    item.Position = new Vector2(previousItemsTotalWidth, previousColumnsHeight);
                    previousItemsTotalWidth += item.Size.X;
                    maxItemHeight = Math.Max(maxItemHeight, item.Size.Y);
                    currentColumnIndex++;
                    if (MaxRowSize != ListControl.SizeInfinite && currentColumnIndex >= MaxRowSize)
                    {
                        currentRowIndex++;
                        previousColumnsHeight = maxItemHeight;
                        maxItemHeight = 0;
                        currentColumnIndex = 0;
                    }
                }
            }
        }
    }

    public enum ListColumnDirection
    {
        Horizontal,
        Vertical
    }

    public class ListControlItem : ItemControl
    {
        public ListControlItem(IUIRoot root)
            : base(root)
        { }

        public ListControlItem(UIControl control) : base(control.VisualRoot)
        {
            this.Children.Add(control);
        }
    }
}
