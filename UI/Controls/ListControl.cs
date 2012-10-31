using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Interop;
using SharpDX;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public abstract class MultiSelectControl : ItemsControl
    {       
        private UIListItemControlCollection _selectedItems = null;
        private CommandControlWireUp _controlWireUp = null;

        public MultiSelectControl(IUIRoot root)
            : base(root)
        {
            _selectedItems = new UIListItemControlCollection(this);
            _selectedItems.CollectionChanged += this.SelectedItems_CollectionChanged;

            _controlWireUp = new CommandControlWireUp(this);
            _controlWireUp.ChildControlCommand += ControlWireUp_ChildControlCommand;
        }      

        private void ControlWireUp_ChildControlCommand(object sender, ControlCommandArgs args)
        {
            OnCommand(args.CommandName);
        }

        private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (ItemsControl item in args.NewItems)
                        OnSelectedItemAdded(item);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (ItemsControl item in args.OldItems)
                        OnSelectedItemRemoved(item);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    OnSelectedItemsReset();
                    break;
            }
        }

        protected virtual void OnSelectedItemAdded(ItemsControl item)
        {
           
        }

        protected virtual void OnSelectedItemRemoved(ItemsControl item)
        {
           
        }

        protected virtual void OnSelectedItemsReset()
        {
           
        }        

        public UIListItemControlCollection SelectedItems { get { return _selectedItems; } }
    }

    public class ListControl : MultiSelectControl
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
                foreach (ListControlItem item in this.Items)
                {
                    item.Position = new Vector2(previousColumnsWidth, previousItemsTotalHeight);
                    previousItemsTotalHeight += item.Size.Height;
                    maxItemWidth = Math.Max(maxItemWidth, item.Size.Width);
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
                foreach (ListControlItem item in this.Items)
                {
                    item.Position = new Vector2(previousItemsTotalWidth, previousColumnsHeight);
                    previousItemsTotalWidth += item.Size.Width;
                    maxItemHeight = Math.Max(maxItemHeight, item.Size.Width);
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
        public static readonly DrawingSizeF DefaultItemTextMargin = new DrawingSizeF(10, 10);

        private CommandControlWireUp _wireUp = null;

        public ListControlItem(IUIRoot root)
            : base(root)
        {
            _wireUp = new CommandControlWireUp(this);
            _wireUp.ChildControlCommand += CommandControlWireUp_ChildControlCommand;
        }

        private void CommandControlWireUp_ChildControlCommand(object sender, ControlCommandArgs args)
        {
            OnCommand(args.CommandName);
        }

        public ListControlItem(UIControl control) : base(control.VisualRoot)
        {
            this.Children.Add(control);
        }

        public ListControlItem(IUIRoot visualRoot, string name, string text, SpriteFont font, Color4 fontColor, Color4 backgroundColor)
            : base(visualRoot)
        {
            Name = name;
            CommandName = name;
            Label childLabel = new Label(visualRoot, text, font, fontColor, backgroundColor);
            Children.Add(childLabel);
            Size = font.MeasureString(text).Add(DefaultItemTextMargin); 
        }
    }

    public class UIListItemControlCollection : System.Collections.ObjectModel.ObservableCollection<ListControlItem>
    {
        private MultiSelectControl _owner = null;
        public UIListItemControlCollection(MultiSelectControl owner)
        {
            _owner = owner;
        }
    }
}
