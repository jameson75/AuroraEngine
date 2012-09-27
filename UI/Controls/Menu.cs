using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using CipherPark.AngelJacket.Core.Utils;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class Menu : ItemsControl
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
            ControlSpriteBatch.Begin();
            //Vector2 itemPosition = this.Position;
            //foreach (MenuItem item in Items)
            //{
            //    Color fontColor = (!item.Enabled) ? Color.DarkGray : (item == SelectedItem) ? ((TextContent)item.ItemTemplate).FontColor : ((TextContent)item.FocusedItemTemplate).FontColor;
            //    ControlSpriteBatch.DrawString(((TextContent)item.ItemTemplate).Font, ((TextContent)item.ItemTemplate).Text, itemPosition, fontColor);
            //    //TODO: Define spacing between menu items.
            //    itemPosition.Y += 20;
            //}
            foreach (MenuItem item in Items)
                item.Draw(gameTime);
            ControlSpriteBatch.End();
            base.Draw(gameTime);
        }

        //[Obsolete]
        //public override UIControl _GetNextFocusableChild(UIControl fromControl)
        //{
        //    return base._GetNextFocusableChild(fromControl);
        //}

        public override void Update(long gameTime)
        {
            if (this.HasFocus)
            {
                Services.IInputService inputServices = (Services.IInputService)Game.Services.GetService(typeof(Services.IInputService));
                if (inputServices == null)
                    throw new InvalidOperationException("Input services not available.");
                ControlInputState cim = inputServices.GetControlInputState();

                bool selectPreviousKeyDown = (Orienation == MenuOrientation.Vertical && cim.IsKeyDown(VirtualKey.Up)) || (Orienation == MenuOrientation.Horizontal && cim.IsKeyDown(VirtualKey.Left));
                bool selectNextKeyDown = (Orienation == MenuOrientation.Vertical && cim.IsKeyDown(VirtualKey.Down)) || (Orienation == MenuOrientation.Horizontal && cim.IsKeyDown(VirtualKey.Right));

                if (selectPreviousKeyDown)
                    this.SelectPreviousItem();

                else if (selectNextKeyDown)
                    this.SelectNextItem();

                else if (cim.IsKeyReleased(VirtualKey.Enter))
                {
                    if (this.SelectedItem != null && ((MenuItem)this.SelectedItem).CommandName != null)
                        this.OnItemClicked((MenuItem)this.SelectedItem);
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
                    this.Size = (this.Orienation == MenuOrientation.Vertical) ? new Vector2(this.Size.X, this.Items.Sum(x => x.Bounds.Height)) : new Vector2(this.Items.Sum(x => x.Bounds.Width), this.Size.Y);                
            }
            base.OnItemAdded(item);
            UpdateLayout(LayoutUpdateReason.ChildCountChanged);
        }

        protected override void OnItemRemoved(ItemControl item)
        {
            base.OnItemRemoved(item);
            if (AutoSize)
            {
                if (this.Items.Count == 0)
                    this.Size = Vector2.Zero;
                else
                    this.Size = (this.Orienation == MenuOrientation.Vertical) ? new Vector2(this.Size.X, this.Items.Sum(x => x.Bounds.Height)) : new Vector2(this.Items.Sum(x => x.Bounds.Width), this.Size.Y);
            }
            UpdateLayout(LayoutUpdateReason.ChildCountChanged);
            SelectedItemIndex = -1;
        }

        protected override void OnLayoutChanged()
        {
            float offset = 0.0f;
            if (Orienation == MenuOrientation.Vertical)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (i == 0)
                        offset += this.Margin.Y;
                    else
                        offset += Items[i - 1].Padding.Y;
                    Items[i].Position = new Vector2(0.0f, offset);
                    Items[i].Size = new Vector2(this.Size.X, Items[i].Size.Y);
                    offset += Items[0].Size.Y;
                }
            }
            else
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (i == 0)
                        offset += this.Margin.X;
                    else
                        offset += Items[i - 1].Padding.X;
                    Items[i].Position = new Vector2(offset, 0.0f);
                    Items[i].Size = new Vector2(Items[i].Size.X, this.Size.Y);
                    offset += Items[0].Size.X;
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
