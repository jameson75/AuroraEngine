using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using CipherPark.AngelJacket.Core.UI.Controls.Extensions;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SelectControl : ItemsControl
    {
        private int _selectedIndex = -1;
        public SelectControl(Components.IUIRoot visualRoot)
            : base(visualRoot)
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

    /// <summary>
    /// 
    /// </summary>
    public class Menu : SelectControl
    {
        //private MenuOrientation _orientation = MenuOrientation.Vertical;
        private StackLayoutManager _layoutManager = null;

        public Menu(Components.IUIRoot visualRoot)
            : base(visualRoot)
        {
            AutoSize = true;
            _layoutManager = new StackLayoutManager(this);
        }

        protected override IControlLayoutManager LayoutManager
        {
            get
            {
                return _layoutManager;
            }
        }

        public override bool CanReceiveFocus
        {
            get
            {
                return true;
            }
        }

        public MenuOrientation Orientation
        {
            get { return (MenuOrientation)_layoutManager.Orientation; }
            set
            {
                _layoutManager.Orientation = (StackLayoutOrientation)value;
                OnOrientationChanged();
            }
        }

        public bool AutoSize { get; set; }        

        public void OpenContextMenu(MenuItem item)
        {
            ContextMenu subMenu = item.SubMenu;

            if (subMenu.Owner != null && subMenu.Owner != this)
                throw new InvalidOperationException("Submenu cannot be opened while owned by another menu.");

            subMenu.BeginContext(this);

            Vector2 subMenuRelativePosition = Vector2Extension.Zero;

            switch (subMenu.DisplaySide)
            {
                case ContextMenuDisplaySide.Left:
                    subMenuRelativePosition = new Vector2(this.Position.X - subMenu.Bounds.Width, this.Position.Y);
                    break;
                case ContextMenuDisplaySide.Above:
                    subMenuRelativePosition = new Vector2(this.Position.X, this.Position.Y - subMenu.Bounds.Height);
                    break;
                case ContextMenuDisplaySide.Right:
                    subMenuRelativePosition = new Vector2(this.Bounds.Right, this.Position.Y);
                    break;
                case ContextMenuDisplaySide.Bottom:
                    subMenuRelativePosition = new Vector2(this.Bounds.X, this.Bounds.Bottom);
                    break;
            }

            subMenu.Position = subMenu.PositionToLocal(this.PositionToSurface(subMenuRelativePosition));            
        }

        public void AddMenuItem(MenuItem item)
        {
            this.Items.Add(item);
        }

        public void AddMenuItem(string text, string name, SpriteFont font, Color fontColor, Color? selectColor = null, string commandName = null, ContextMenu subMenu = null)
        {        
            MenuItem item = new MenuItem(this.VisualRoot, name, text, font, fontColor, selectColor, commandName, subMenu);
            AddMenuItem(item);
        }

        public override void Initialize()
        {
            foreach (MenuItem item in Items)
                item.Initialize();
            base.Initialize();
        }

        /*
        public void SetItemsPadding(BoundaryF padding)
        {
            foreach (ItemControl item in this.Items)
                item.Padding = padding;
            UpdateLayout(LayoutUpdateReason.ChildSizeChanged);
        }

        public void SetItemsMargin(BoundaryF margin)
        {
            foreach(ItemControl item in this.Items)
                item.Margin = margin;
            UpdateLayout(LayoutUpdateReason.ChildSizeChanged);
        }
        */

        protected override void OnDraw(GameTime gameTime)
        {
            foreach (MenuItem item in Items)
                item.Draw(gameTime);
            base.OnDraw(gameTime);
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (this.HasFocus)
            {
                Services.IInputService inputServices = (Services.IInputService)Game.Services.GetService(typeof(Services.IInputService));
                if (inputServices == null)
                    throw new InvalidOperationException("Input services not available.");

                BufferedInputState bufferedInputState = inputServices.GetBufferedInputState();

                bool selectPreviousKeyDown = (Orientation == MenuOrientation.Vertical && bufferedInputState.IsKeyDown(Key.Up)) ||
                                             (Orientation == MenuOrientation.Horizontal && bufferedInputState.IsKeyDown(Key.Left)) ||
                                             (Orientation == MenuOrientation.Vertical && bufferedInputState.InputState.IsGamepadButtonHit(0, SharpDX.XInput.GamepadButtonFlags.DPadUp)) ||
                                             (Orientation == MenuOrientation.Horizontal && bufferedInputState.InputState.IsGamepadButtonHit(0, SharpDX.XInput.GamepadButtonFlags.DPadLeft));

                bool selectNextKeyDown = (Orientation == MenuOrientation.Vertical && bufferedInputState.IsKeyDown(Key.Down)) ||
                                         (Orientation == MenuOrientation.Horizontal && bufferedInputState.IsKeyDown(Key.Right)) ||
                                         (Orientation == MenuOrientation.Vertical && bufferedInputState.InputState.IsGamepadButtonHit(0, SharpDX.XInput.GamepadButtonFlags.DPadDown)) ||
                                         (Orientation == MenuOrientation.Horizontal && bufferedInputState.InputState.IsGamepadButtonHit(0, SharpDX.XInput.GamepadButtonFlags.DPadRight));

                if (selectPreviousKeyDown)
                    this.SelectPreviousItem();

                else if (selectNextKeyDown)
                    this.SelectNextItem();

                else if (bufferedInputState.IsKeyReleased(Key.Return) || bufferedInputState.InputState.IsGamepadButtonHit(0, SharpDX.XInput.GamepadButtonFlags.A))
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

            base.OnUpdate(gameTime);
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
                //this.Size = (this.Orientation == MenuOrientation.Vertical) ? new Size2F(this.Items.Max(x => x.Size.Width) , this.Items.Sum(x => x.Bounds.Height)) : new Size2F(this.Items.Sum(x => x.Bounds.Width), this.Items.Max(x => x.Size.Height));            
                this.Size = CalculateSizeFromItems(); 
            base.OnItemAdded(item);
        }

        protected override void OnItemRemoved(ItemControl item)
        {
            base.OnItemRemoved(item);            
            SelectedItemIndex = -1;
        }

        protected virtual void OnItemClicked(MenuItem item)
        {
            ItemClickedEventHandler handler = this.ItemClicked;
            if (handler != null)
                handler(this, new ItemClickedEventArgs(item));

            if (item.SubMenu != null)
                OpenContextMenu(item);
        }   

        public event ItemClickedEventHandler ItemClicked = null;

        public override void ApplyTemplate(Components.UIControlTemplate template)
        {
            MenuTemplate menuTemplate = (MenuTemplate)template;
            base.ApplyTemplate(template);
        }

        public static UIControl FromTemplate(Components.IUIRoot visualRoot, Components.MenuTemplate menuTemplate)
        {
            Menu menu = new Menu(visualRoot);
            menu.ApplyTemplate(menuTemplate);
            return menu;
        }

        private Size2F CalculateSizeFromItems()
        {
            return (this.Orientation == MenuOrientation.Vertical) ? new Size2F(this.Items.Max(x => x.Extents().Width), this.Items.Sum(x => x.Extents().Height)) : new Size2F(this.Items.Sum(x => x.Extents().Width), this.Items.Max(x => x.Extents().Height));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum MenuOrientation
    {
        Vertical,
        Horizontal
    }

    /// <summary>
    /// 
    /// </summary>
    public class ItemClickedEventArgs : EventArgs
    {
        private MenuItem _item = null;

        public ItemClickedEventArgs(MenuItem menuItem)
        {
            _item = menuItem;
        }

        public MenuItem Item { get { return _item; } }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void ItemClickedEventHandler(object sender, ItemClickedEventArgs args); 
}

namespace CipherPark.AngelJacket.Core.UI.Controls.Extensions
{
    public static class UIControlExtensions
    {
        public static RectangleF Extents(this UIControl control)
        {
            RectangleF r = control.Bounds;
            r.Left -= control.Margin.Left;
            r.Top -= control.Margin.Top;
            r.Right += control.Margin.Right;
            r.Bottom += control.Margin.Bottom;
            return r;
        }
    }
}