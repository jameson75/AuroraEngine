using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.Utils.Toolkit;
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
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (ItemControl item in args.OldItems)
                        OnSelectedItemRemoved(item);
                    break;
                
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (ItemControl item in args.NewItems)
                        OnSelectedItemAdded(item);
                    break;               
                
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (ItemControl item in args.OldItems)
                        OnSelectedItemRemoved(item);
                    foreach (ItemControl item in args.NewItems)
                        OnSelectedItemAdded(item);
                    break;
                
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    OnSelectedItemsReset();
                    break;
            }            
        }

        protected virtual void OnSelectedItemAdded(ItemControl item)
        {
             SelectionChangedHandler handler = SelectionChanged;
             if (handler != null)
                 handler(this, new SelectionChangedEventArgs(item, true));
        }

        protected virtual void OnSelectedItemRemoved(ItemControl item)
        {
            SelectionChangedHandler handler = SelectionChanged;
            if (handler != null)
                handler(this, new SelectionChangedEventArgs(item, false));
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

        public event SelectionChangedHandler SelectionChanged;
    }

    public class SelectionChangedEventArgs : EventArgs 
    { 
        private ItemControl _item = null;
        private bool _isSelected = false;        

        public ItemControl Item { get { return _item; } }
        public bool IsSelected { get { return _isSelected; } }

        public SelectionChangedEventArgs(ItemControl item, bool isSelected)
        {
            _isSelected = isSelected;
            _item = item;
        }
    }

    public delegate void SelectionChangedHandler(object sender, SelectionChangedEventArgs args);

    public class ListControl : MultiSelectControl
    {       
        public const int SizeInfinite = -1;      
        private int _maxRowSize = ListControl.SizeInfinite;
        private UIContent _backgroundContent= null;

        public UIContent BackgroundContent
        {
            get { return _backgroundContent; }
            set
            {
                if (_backgroundContent != null)
                    _backgroundContent.Container = null;
                
                if (value != null)
                    value.Container = this;
                
                _backgroundContent = value;
            }                
        }

        public override bool CanReceiveFocus
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

        protected override void OnUpdate(GameTime gameTime)
        {
            foreach (ListControlItem item in this.Items)
                item.Update(gameTime);
            base.OnUpdate(gameTime);
        }

        protected override void OnDraw()
        {
            if (_backgroundContent != null)
                _backgroundContent.Draw();

            foreach (ListControlItem item in this.Items)
                item.Draw();

            base.OnDraw();
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
                float previousItemsTotalWidth = 0;
                float previousColumnsHeight = 0;
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

        public void AddListItem(ListControlItem item)
        {
            this.Items.Add(item);
        }

        public void AddListItem(string text, string name, SpriteFont font, Color fontColor, Color? bgColor = null, Color? selectColor = null)
        {            
            ListControlItem item = new ListControlItem(this.VisualRoot,
                                                       name,
                                                       text,
                                                       font,
                                                       fontColor,
                                                       bgColor,
                                                       selectColor);
            AddListItem(item);
        }

        public override void ApplyTemplate(UIControlTemplate template)
        {          
            base.ApplyTemplate(template);
        }

        public static ListControl FromTemplate(IUIRoot visualRoot, ListControlTemplate template)
        {
            ListControl listControl = new ListControl(visualRoot);
            listControl.ApplyTemplate(template);
            return listControl;
        }
    }

    public enum ListColumnDirection
    {
        Horizontal,
        Vertical
    }

    public class ListControlItem : ItemControl
    {
        public static readonly Size2F DefaultItemTextMargin = new Size2F(10f, 10f);

        private CommandControlWireUp _wireUp = null;
        
        public ListControlItem(IUIRoot root)
            : base(root)
        {
            _wireUp = new CommandControlWireUp(this);
            _wireUp.ChildControlCommand += CommandControlWireUp_ChildControlCommand;
        }       

        public ListControlItem(UIControl content) : base(content.VisualRoot)
        {
            SetContent(content);
            _wireUp = new CommandControlWireUp(this);
            _wireUp.ChildControlCommand += CommandControlWireUp_ChildControlCommand;
        }

        public ListControlItem(IUIRoot visualRoot, string name, string text, SpriteFont font, Color fontColor, Color? backgroundColor = null, Color? selectFontColor = null)
            : base(visualRoot)
        {
            Name = name;
            CommandName = name;
            UIControlTemplate itemTemplate = ContentControlTemplate.CreateLabelTemplate(text, font, fontColor, backgroundColor);
            UIControlTemplate selectTemplate = null;
            if (selectFontColor != null)
                selectTemplate = ContentControlTemplate.CreateLabelTemplate(text, font, selectFontColor.Value, backgroundColor);
            ContentControl childLabel = new ContentControl(visualRoot);
            childLabel.Size = font.MeasureString(text).Add(DefaultItemTextMargin);
            SetContent(childLabel, itemTemplate, selectTemplate);
            _wireUp = new CommandControlWireUp(this);
            _wireUp.ChildControlCommand += CommandControlWireUp_ChildControlCommand;
        }

        public static ListControlItem FromTemplate(IUIRoot visualRoot, ListControlItemTemplate template)
        {
            ListControlItem item = new ListControlItem(visualRoot);
            item.ApplyTemplate(template);
            return item;
        }

        protected override void OnSelected()
        {
            if (SelectTemplate != null && Children.Count != 0)
                Children[0].ApplyTemplate(SelectTemplate);
            base.OnSelected();
        }

        protected override void OnUnselected()
        {
            //*******************************************************************************************
            //TODO: Fix flaw - a condition exists where if a SelectItem template is specified but
            //a ItemTemplate wasn't specified, this control's display state would be stuck
            //in a "selected state".
            //*******************************************************************************************
            if (ItemTemplate != null && Children.Count != 0)
                Children[0].ApplyTemplate(ItemTemplate);
            base.OnUnselected();
        }
     
        public void SetContent(TextContent content, ContentControlTemplate itemTemplate = null, ContentControlTemplate selectTemplate = null)
        {
            ContentControl label = new ContentControl(VisualRoot, content);
            SetContent(label, itemTemplate, selectTemplate);
        }           

        public void SetContent(UIControl content, UIControlTemplate itemTemplate = null, UIControlTemplate selectTemplate = null)
        {
            if (content != null)
            {
                Children.Clear();
                if (itemTemplate != null)
                    content.ApplyTemplate(itemTemplate);
                content.VerticalAlignment = Controls.VerticalAlignment.Stretch;
                content.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;
                Size = content.Size;
                Children.Add(content);
            }

            if(itemTemplate != null)
                ItemTemplate = itemTemplate;

            if (selectTemplate != null)
                SelectTemplate = selectTemplate;     
        }

        public void ClearContent()
        {
            Children.Clear();
            ItemTemplate = null;
            SelectTemplate = null;
        }

        public UIControlTemplate ItemTemplate { get; private set; }

        public UIControlTemplate SelectTemplate { get; private set; }

        public UIControl Content { get; private set; }

        public override void ApplyTemplate(UIControlTemplate template)
        {
            ListControlItemTemplate listControlTemplate = (ListControlItemTemplate)template;
            SetContent(listControlTemplate.Content.CreateControl(this.VisualRoot), 
                       listControlTemplate.ItemTemplate, 
                       listControlTemplate.SelectTemplate);
            base.ApplyTemplate(template);
        }

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
