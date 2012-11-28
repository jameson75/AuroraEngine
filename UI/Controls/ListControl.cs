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
                    foreach (ItemControl item in args.NewItems)
                        OnSelectedItemAdded(item);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (ItemControl item in args.OldItems)
                        OnSelectedItemRemoved(item);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    OnSelectedItemsReset();
                    break;
            }
        }

        protected virtual void OnSelectedItemAdded(ItemControl item)
        {
           
        }

        protected virtual void OnSelectedItemRemoved(ItemControl item)
        {
           
        }

        protected virtual void OnSelectedItemsReset()
        {
           
        }        

        public UIListItemControlCollection SelectedItems { get { return _selectedItems; } }

        public void SelectItems(IEnumerable<ItemControl> items)
        {
            UnSelectItems(_selectedItems);
            foreach (ListControlItem item in items)
            {
                this._selectedItems.Add((ListControlItem)item);
                item.IsSelected = true;
            }
        }

        public void UnSelectItems(IEnumerable<ItemControl> items)
        {
            List<ItemControl> auxList = new List<ItemControl>(items);
            foreach (ListControlItem item in auxList)
                _selectedItems.Remove(item);
        }            
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
            MaxRowSize = ListControl.SizeInfinite;
        }

        public ListColumnDirection ColumnDirection { get; set; }

        public override void Update(long gameTime)
        {
            foreach (ListControlItem item in this.Items)
                item.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(long gameTime)
        {
            foreach (ListControlItem item in this.Items)
                item.Draw(gameTime);
            base.Draw(gameTime);
        }

        protected override void OnLayoutChanged()
        {
            int currentRowIndex = 0;
            int currentColumnIndex = 0;

            if (ColumnDirection == ListColumnDirection.Horizontal)
            {
                float previousItemsTotalHeight = 0f;
                float previousColumnsWidth = 0f;
                float maxItemWidth = 0f;
                foreach (ListControlItem item in this.Items)
                {
                    item.Position = new DrawingPointF(previousColumnsWidth, previousItemsTotalHeight);
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
                float previousItemsTotalWidth = 0;
                float previousColumnsHeight = 0;
                float maxItemHeight = 0;
                foreach (ListControlItem item in this.Items)
                {
                    item.Position = new DrawingPointF(previousItemsTotalWidth, previousColumnsHeight);
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
        public static readonly DrawingSizeF DefaultItemTextMargin = new DrawingSizeF(10f, 10f);

        private CommandControlWireUp _wireUp = null;
        
        public ListControlItem(IUIRoot root)
            : base(root)
        {
            _wireUp = new CommandControlWireUp(this);
            _wireUp.ChildControlCommand += CommandControlWireUp_ChildControlCommand;
        }       

        public ListControlItem(UIControl control) : base(control.VisualRoot)
        {
            this.Children.Add(control);
            _wireUp = new CommandControlWireUp(this);
            _wireUp.ChildControlCommand += CommandControlWireUp_ChildControlCommand;
        }

        public ListControlItem(IUIRoot visualRoot, string name, string text, SpriteFont font, Color4 fontColor, Color4 backgroundColor, Color4? selectFontColor = null)
            : base(visualRoot)
        {
            Name = name;
            CommandName = name;
            ItemTemplate = new LabelTemplate(text, font, fontColor, backgroundColor);
            if (selectFontColor != null)
                SelectTemplate = new LabelTemplate(text, font, selectFontColor, backgroundColor);
            Label childLabel = new Label(visualRoot);
            childLabel.ApplyTemplate(ItemTemplate);
            childLabel.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            childLabel.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;
            Children.Add(childLabel);
            Size = font.MeasureString(text).Add(DefaultItemTextMargin);
            _wireUp = new CommandControlWireUp(this);
            _wireUp.ChildControlCommand += CommandControlWireUp_ChildControlCommand;
        }

        protected override void OnSelected()
        {
            if (SelectTemplate != null && Children.Count != 0)
                Children[0].ApplyTemplate(SelectTemplate);
            base.OnSelected();
        }

        protected override void OnUnselected()
        {
            if (ItemTemplate != null && Children.Count != 0)
                Children[0].ApplyTemplate(ItemTemplate);
            base.OnUnselected();
        }

        public UIControlTemplate ItemTemplate { get; set; }

        public UIControlTemplate SelectTemplate { get; set; }

        private void CommandControlWireUp_ChildControlCommand(object sender, ControlCommandArgs args)
        {
            OnCommand(args.CommandName);
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
