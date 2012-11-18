﻿using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core.Utils;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public abstract class SelectControl : ItemsControl
    {
        private int _selectedIndex = -1;
        public SelectControl(Components.IUIRoot visualRoot) : base(visualRoot)
        {

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
    }

    public class Menu : SelectControl
    {
        private MenuOrientation _orientation = MenuOrientation.Vertical;
       

        public Menu(Components.IUIRoot visualRoot)
            : base(visualRoot)
        {
            AutoSize = true;
        }

        public override bool CanFocus
        {
            get
            {
                return true;
            }
        }

        public MenuOrientation Orienation
        {
            get { return _orientation; }
            set
            {
                _orientation = value;
                OnOrientationChanged();
            }
        }

        public bool AutoSize { get; set; }
        
        public override void Draw(long gameTime)
        {
            foreach (MenuItem item in Items)
                item.Draw(gameTime);           
            base.Draw(gameTime);
        }

        public override void Update(long gameTime)
        {
            if (this.HasFocus)
            {
                Services.IInputService inputServices = (Services.IInputService)Game.Services.GetService(typeof(Services.IInputService));
                if (inputServices == null)
                    throw new InvalidOperationException("Input services not available.");
                ControlInputState cim = inputServices.GetControlInputState();

                bool selectPreviousKeyDown = (Orienation == MenuOrientation.Vertical && cim.IsKeyDown(Key.UpArrow)) || (Orienation == MenuOrientation.Horizontal && cim.IsKeyDown(Key.Left));
                bool selectNextKeyDown = (Orienation == MenuOrientation.Vertical && cim.IsKeyDown(Key.Down)) || (Orienation == MenuOrientation.Horizontal && cim.IsKeyDown(Key.Right));

                if (selectPreviousKeyDown)
                    this.SelectPreviousItem();

                else if (selectNextKeyDown)
                    this.SelectNextItem();

                else if (cim.IsKeyReleased(Key.Return))
                {
                    if (this.SelectedItem != null)
                    {
                        this.OnItemClicked((MenuItem)this.SelectedItem);
                        if (SelectedItem.CommandName != null)
                            this.OnCommand(SelectedItem.CommandName);  
                    }                       
                }
            }

            foreach (MenuItem item in this.Items)
                item.Update(gameTime);

            base.Update(gameTime);
        }

        protected virtual void OnOrientationChanged()
        {
            UpdateLayout(LayoutUpdateReason.ClientAreaChanged);
        }

        protected override void OnItemAdded(ItemControl item)
        {           
            if (item is MenuItem == false)
                throw new ArgumentException("item", "item must be of type or derivative of MenuItem");
            if (AutoSize)
            {              
                if (this.Items.Count == 1)
                    this.Size = item.Size;
                else
                    this.Size = (this.Orienation == MenuOrientation.Vertical) ? new DrawingSizeF(this.Size.Width, this.Items.Sum(x => x.Bounds.Height)) : new DrawingSizeF(this.Items.Sum(x => x.Bounds.Width), this.Size.Height);                
            }
            base.OnItemAdded(item);
        }

        protected override void OnItemRemoved(ItemControl item)
        {
            base.OnItemRemoved(item);
            if (AutoSize)
            {
                if (this.Items.Count == 0)
                    this.Size = DrawingSizeFExtension.Zero;
                else
                    this.Size = (this.Orienation == MenuOrientation.Vertical) ? new DrawingSizeF(this.Size.Width, this.Items.Sum(x => x.Bounds.Height)) : new DrawingSizeF(this.Items.Sum(x => x.Bounds.Width), this.Size.Height);
            }            
            SelectedItemIndex = -1;
        }

        protected override void OnLayoutChanged()
        {
            float offset = 0f;
            if (Orienation == MenuOrientation.Vertical)
            {
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
            }
            else
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (i == 0)
                        offset += this.Margin.Width;
                    else
                        offset += Items[i - 1].Padding.Width;
                    Items[i].Position = new DrawingPointF(offset, 0f);
                    Items[i].Size = new DrawingSizeF(Items[i].Size.Width, this.Size.Height);
                    offset += Items[0].Size.Width;
                }
            }
            base.OnLayoutChanged();
        }

        protected virtual void OnItemClicked(MenuItem item)
        {
            ItemClickedEventHandler handler = this.ItemClicked;
            if (handler != null)
                handler(this, new ItemClickedEventArgs(item));
        }      

        public event ItemClickedEventHandler ItemClicked = null;
    } 

    public enum MenuOrientation
    {
        Vertical,
        Horizontal
    }

    public class ItemClickedEventArgs : EventArgs
    {
        private MenuItem _item = null;

        public ItemClickedEventArgs(MenuItem menuItem)
        {            
            _item = menuItem;
        }      

        public MenuItem Item { get { return _item; } }
    }

    public delegate void ItemClickedEventHandler(object sender, ItemClickedEventArgs args);
}
