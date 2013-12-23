using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
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

    public class Menu : SelectControl
    {
        private MenuOrientation _orientation = MenuOrientation.Vertical;

        public Menu(Components.IUIRoot visualRoot)
            : base(visualRoot)
        {
            AutoSize = true;
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
            get { return _orientation; }
            set
            {
                _orientation = value;
                OnOrientationChanged();
            }
        }

        public bool AutoSize { get; set; }

        public LabelTemplate DefaultItemTemplate { get; set; }

        public void OpenContextMenu(MenuItem item)
        {
            ContextMenu subMenu = item.SubMenu;

            if (subMenu.Owner != null && subMenu.Owner != this)
                throw new InvalidOperationException("Submenu cannot be opened while owned by another menu.");

            subMenu.BeginContext(this);

            DrawingPointF subMenuRelativePosition = DrawingPointFExtension.Zero;

            switch (subMenu.DisplaySide)
            {
                case ContextMenuDisplaySide.Left:
                    subMenuRelativePosition = new DrawingPointF(this.Position.X - subMenu.Bounds.Width, this.Position.Y);
                    break;
                case ContextMenuDisplaySide.Above:
                    subMenuRelativePosition = new DrawingPointF(this.Position.X, this.Position.Y - subMenu.Bounds.Height);
                    break;
                case ContextMenuDisplaySide.Right:
                    subMenuRelativePosition = new DrawingPointF(this.Bounds.Right, this.Position.Y);
                    break;
                case ContextMenuDisplaySide.Bottom:
                    subMenuRelativePosition = new DrawingPointF(this.Bounds.X, this.Bounds.Bottom);
                    break;
            }

            subMenu.Position = subMenu.PositionToLocal(this.PositionToSurface(subMenuRelativePosition));            
        }

        public void AddMenuItem(MenuItem item)
        {
            this.Items.Add(item);
        }

        public void AddMenuItem(string text, string name = null, SpriteFont font = null, Color? fontColor = null, Color? bgColor = null, Color? selectColor = null)
        {
            LabelTemplate itemTemplate = this.DefaultItemTemplate != null ? this.DefaultItemTemplate : DefaultTheme.Instance.Label;
            MenuItem item = new MenuItem(this.VisualRoot,
                                                       name,
                                                       text,
                                                       font != null ? font : itemTemplate.CaptionStyle.Font,
                                                       fontColor != null ? fontColor.Value : itemTemplate.CaptionStyle.FontColor.Value,
                                                       selectColor != null ? selectColor.Value : fontColor != null ? fontColor.Value : itemTemplate.CaptionStyle.FontColor.Value);
            AddMenuItem(item);
        }

        public override void Initialize()
        {
            foreach (MenuItem item in Items)
                item.Initialize();
            base.Initialize();
        }

        protected override void OnDraw(long gameTime)
        {
            foreach (MenuItem item in Items)
                item.Draw(gameTime);
            base.OnDraw(gameTime);
        }

        protected override void OnUpdate(long gameTime)
        {
            if (this.HasFocus)
            {
                Services.IInputService inputServices = (Services.IInputService)Game.Services.GetService(typeof(Services.IInputService));
                if (inputServices == null)
                    throw new InvalidOperationException("Input services not available.");

                BufferedInputState bufferedInputState = inputServices.GetBufferedInputState();

                bool selectPreviousKeyDown = (Orientation == MenuOrientation.Vertical && bufferedInputState.IsKeyDown(Key.UpArrow)) ||
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
            {
                if (this.Items.Count == 1)
                    this.Size = item.Size;
                else
                    this.Size = (this.Orientation == MenuOrientation.Vertical) ? new DrawingSizeF(this.Size.Width, this.Items.Sum(x => x.Bounds.Height)) : new DrawingSizeF(this.Items.Sum(x => x.Bounds.Width), this.Size.Height);
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
                    this.Size = (this.Orientation == MenuOrientation.Vertical) ? new DrawingSizeF(this.Size.Width, this.Items.Sum(x => x.Bounds.Height)) : new DrawingSizeF(this.Items.Sum(x => x.Bounds.Width), this.Size.Height);
            }
            SelectedItemIndex = -1;
        }

        protected override void OnLayoutChanged()
        {
            float offset = 0f;
            if (Orientation == MenuOrientation.Vertical)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (i == 0)
                        offset += this.Margin.Top;
                    else
                        offset += Items[i - 1].Padding.Bottom;
                    Items[i].Position = new DrawingPointF(this.Margin.Left, offset);
                    Items[i].Size = new DrawingSizeF(this.Size.Width - this.Margin.Right, Items[i].Size.Height);
                    offset += Items[0].Size.Height;
                }
            }
            else
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (i == 0)
                        offset += this.Margin.Left;
                    else
                        offset += Items[i - 1].Padding.Right;
                    Items[i].Position = new DrawingPointF(offset, this.Margin.Top);
                    Items[i].Size = new DrawingSizeF(Items[i].Size.Width, this.Size.Height - this.Margin.Bottom);
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

    public class ChoiceControl : ContentControl        
    {        
        ContextMenu _contextMenu = null;
        
        public ChoiceControl(IUIRoot visualRoot)
            : base(visualRoot)
        {
            _contextMenu = new ContextMenu(visualRoot);
            _contextMenu.ContextClosed += ContextMenu_ContextClosed;
            _contextMenu.DisplaySide = ContextMenuDisplaySide.Right;
            _contextMenu.Visible = false;
            visualRoot.Controls.Add(_contextMenu);
        }

        public UIItemControlCollection Items
        {
            get { return _contextMenu.SubControl.Items; }
        }

        public int SelectedItemIndex
        {
            get { return _contextMenu.SubControl.SelectedItemIndex; }
            set 
            { 
                _contextMenu.SubControl.SelectedItemIndex = value;
                UpdateContent();
            }
        }

        public void OpenContextMenu()
        {
            if (_contextMenu.Owner != null && _contextMenu.Owner != this)
                throw new InvalidOperationException("Context menu cannot be opened while owned by another control.");

            _contextMenu.BeginContext(this);

            DrawingPointF subMenuRelativePosition = DrawingPointFExtension.Zero;

            switch (_contextMenu.DisplaySide)
            {
                case ContextMenuDisplaySide.Left:
                    subMenuRelativePosition = new DrawingPointF(this.Position.X - _contextMenu.Bounds.Width, this.Position.Y);
                    break;
                case ContextMenuDisplaySide.Above:
                    subMenuRelativePosition = new DrawingPointF(this.Position.X, this.Position.Y - _contextMenu.Bounds.Height);
                    break;
                case ContextMenuDisplaySide.Right:
                    subMenuRelativePosition = new DrawingPointF(this.Bounds.Right, this.Position.Y);
                    break;
                case ContextMenuDisplaySide.Bottom:
                    subMenuRelativePosition = new DrawingPointF(this.Bounds.X, this.Bounds.Bottom);
                    break;
            }

            _contextMenu.Position = _contextMenu.PositionToLocal(this.PositionToSurface(subMenuRelativePosition));
        }

        public override bool CanReceiveFocus
        {
            get
            {
                return true;
            }
        }

        protected override void OnUpdate(long gameTime)
        {
            Services.IInputService inputServices = (Services.IInputService)Game.Services.GetService(typeof(Services.IInputService));

            if (inputServices == null)
                throw new InvalidOperationException("Input services not available.");

            if (this.HasFocus)
            {
                BufferedInputState bInputState = inputServices.GetBufferedInputState();
                if (bInputState.IsKeyReleased(Key.Return))
                    OpenContextMenu();
            }

            if (this.IsHit)
            {
                InputState inputState = inputServices.GetInputState();
                if (inputState.IsMouseButtonDown(InputState.MouseButton.Left))
                    OpenContextMenu();
            }

            base.OnUpdate(gameTime);
        }

        private void ContextMenu_ContextClosed(object sender, EventArgs e)
        {
            if(_contextMenu.Result == ContextControlResult.SelectOK)              
                UpdateContent();
        }

        private void UpdateContent()
        {
            if (_contextMenu.SubControl.SelectedItem != null)
            {
                //TODO: Implement the ability to clone content.
                //Right now, we relegate to copying text from text content.
                TextContent t = ((MenuItem)_contextMenu.SubControl.SelectedItem).ItemContent as TextContent;
                if (t != null)
                    this.Content = new TextContent(t.Text, t.Font, t.FontColor);
                else
                    this.Content = null;
            }
            else
                this.Content = null;

            OnSelectedContentChanged();
        }

        protected virtual void OnSelectedContentChanged()
        {
            EventHandler handler = SelectedContentChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public event EventHandler SelectedContentChanged;
    }
}