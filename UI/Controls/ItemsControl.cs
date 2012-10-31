using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.UI.Components;

namespace CipherPark.AngelJacket.Core.UI.Controls
{   
    public abstract class ItemsControl : UIControl, ICommandDispatcher
    {
        private UIItemControlCollection _items = null;
       
      
        public UIItemControlCollection Items { get { return _items; } }       
        
        protected ItemsControl(IUIRoot visualRoot)
            : base(visualRoot)
        {
            _items = new UIItemControlCollection(this);
            _items.CollectionChanged += this.Items_CollectionChanged;          
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

        public event ControlCommandHandler ControlCommand;

        protected virtual void OnCommand(string commandName)
        {
            ControlCommandHandler handler = ControlCommand;
            if (handler != null)
                handler(this, new ControlCommandArgs(commandName));        
        }
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
