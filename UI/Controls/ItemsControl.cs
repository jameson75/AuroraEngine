using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.UI.Components;

namespace CipherPark.AngelJacket.Core.UI.Controls
{   
    public abstract class ItemsControl : UIControl, ICommandControl
    {
        private UIItemControlCollection _items = null;
        private int _selectedIndex = -1;
        private ControlCommandWireUp _commandWireUp = null;

        public UIItemControlCollection Items { get { return _items; } }       
        
        protected ItemsControl(IUIRoot visualRoot)
            : base(visualRoot)
        {
            _items = new UIItemControlCollection(this);
            _items.CollectionChanged += this.Items_CollectionChanged;
            _commandWireUp = new ControlCommandWireUp(this);
        }      

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (ItemControl item in args.NewItems)
                        OnItemAdded(item);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (ItemControl item in args.OldItems)
                        OnItemRemoved(item);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    OnItemsReset();
                    break;
            }
        }

        protected virtual void OnItemAdded(ItemControl item)
        {
            if( !Children.Contains(item) )
                Children.Add(item);
        }

        protected virtual void OnItemRemoved(ItemControl item)
        {
            if(Children.Contains(item) )
                Children.Remove(item);
        }

        protected virtual void OnItemsReset()
        {
            if( Children.Count > 0)
                Children.Clear();
        }        

        public ItemControl SelectedItem
        {
            get
            {
                if (_selectedIndex < 0 || _selectedIndex > Items.Count - 1)
                    return null;
                else
                    return (ItemControl)Items[_selectedIndex];
            }
            set
            {
                if (Items.Contains(value) == false)
                    throw new InvalidOperationException("Specified item is not a child of this control.");
                else
                    SelectedItemIndex = Items.IndexOf(value);
            }
        }

        public int SelectedItemIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value > Items.Count - 1 || value < -1)
                    throw new ArgumentOutOfRangeException("Specified value for SelectedItemIndex is out of range.");
                               
                OnSelectedItemChanging();
                _selectedIndex = value;
                OnSelectedItemChanged();
            }
        }

        public event EventHandler SelectedItemChanged;

        public event EventHandler SelectedItemChanging;

        protected virtual void OnSelectedItemChanging()
        {
            if (SelectedItem != null)
                SelectedItem.IsSelected = false;
            EventHandler handler = SelectedItemChanging;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnSelectedItemChanged()
        {
            if (SelectedItem != null)
                SelectedItem.IsSelected = true;
            EventHandler handler = SelectedItemChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private int FindNextEnabledItemIndex()
        {
            int start = (_selectedIndex + 1 >= 0) ? _selectedIndex + 1 : 0;
            for (int i = start; i < Items.Count; i++)
                if (Items[i].Enabled)
                    return i;
            return -1;
        }

        private int FindPreviousEnabledItemIndex()
        {
            int start = (_selectedIndex - 1 <= Items.Count - 1) ? _selectedIndex - 1 : Items.Count - 1;
            for (int i = start; i > -1; i--)
                if (Items[i].Enabled)
                    return i;
            return -1;
        }

        public void SelectPreviousItem()
        {
            if (_selectedIndex > 0)
            {
                int previousEnabledItemIndex = FindPreviousEnabledItemIndex();
                if (previousEnabledItemIndex != -1)
                    SelectedItemIndex = previousEnabledItemIndex;
            }
        }

        public void SelectNextItem()
        {
            if (_selectedIndex < Items.Count - 1)
            {

                int nextEnabledItemIndex = FindNextEnabledItemIndex();
                if (nextEnabledItemIndex != -1)
                   SelectedItemIndex = nextEnabledItemIndex;
            }
        }

        public string CommandName { get; set; }

        public event ControlCommandHandler ControlCommand;
    }

    public class UIItemControlCollection : System.Collections.ObjectModel.ObservableCollection<ItemControl>
    {
        private ItemsControl _owner = null;
        public UIItemControlCollection(ItemsControl owner)
        {
            _owner = owner;
        }
    }
}
