using System;
using SharpDX;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.UI.Components;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class DropList : UIControl, ICustomFocusManager
    {
        SplitterPanel _childPanel = null;
        SplitterPanel _parentPanel = null; 
        TextBox _textBox = null;
        ListControl _listControl = null;
        Button _button = null;               
        DropListState _dropListState = DropListState.Closed;
        ContainerControlLayoutManager _layoutManager = null;

        public DropList(Components.IUIRoot root)
            : base(root)
        {
            _dropListState = DropListState.Closed;             
            root.FocusManager.ControlReceivedFocus += FocusManager_ControlReceivedFocus;
            _layoutManager = new ContainerControlLayoutManager(this);

            this.Size = DefaultTheme.DropList.Size.Value;

            //Utils.Toolkit.SpriteFont tempSpriteFont = Utils.Toolkit.ContentImporter.LoadFont(Game.GraphicsDevice, "Content\\Fonts\\StartMenuFont.spritefont");
            Guid parentSplitterPanel2Guid = Guid.NewGuid();
            _parentPanel = new SplitterPanel(this.VisualRoot);
            _parentPanel.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;
            _parentPanel.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            _parentPanel.Orientation = SplitterLayoutOrientation.Verticle;
            _parentPanel.Splitters.Add(new SplitterLayoutDivision(parentSplitterPanel2Guid));
            _parentPanel.Offset = DefaultTheme.TextBox.Size.Value.Height;
            _parentPanel.Splitters[0].FixedSide = SplitterLayoutFixedSide.One;
            this.Children.Add(_parentPanel);

            Guid childSplitterPanel2Guid = Guid.NewGuid();
            _childPanel = new SplitterPanel(this.VisualRoot);
            _childPanel.Orientation = SplitterLayoutOrientation.Horizontal;
            _childPanel.Splitters.Add(new SplitterLayoutDivision(childSplitterPanel2Guid));
            _childPanel.Offset = _childPanel.Size.Width - DefaultTheme.DropDownButton.Size.Value.Width;
            _textBox.CustomFocusManager = this;
            _childPanel.Splitters[0].FixedSide = SplitterLayoutFixedSide.Two;
            //NOTE: Since no layoutId was specified, childSplitterpanel will get added
            //the parentSplitterpanel's first sub-panel.
            _parentPanel.Children.Add(_childPanel);           

            _textBox = new TextBox(this.VisualRoot);
            _childPanel.Children.Add(_textBox);
            
            _button = new Button(this.VisualRoot); //new Button(this.VisualRoot, "?", tempSpriteFont, SharpDX.Color.White, SharpDX.Color.Blue);
            _button.ApplyTemplate(DefaultTheme.DropDownButton); 
            _button.LayoutId = childSplitterPanel2Guid;
            _button.CustomFocusManager = this;
            _childPanel.Children.Add(_button);
                        
            _listControl = new ListControl(this.VisualRoot);
            _listControl.LayoutId = parentSplitterPanel2Guid;
            _listControl.CustomFocusManager = this;
            _listControl.SelectionChanged += ListControl_SelectionChanged;
             _parentPanel.Children.Add(_listControl);        

            //DivLayoutManager divLayoutManager = new DivLayoutManager(this);
            //divLayoutManager.Divs.Add(new LayoutDiv(_textBox.Id, 0, LayoutDivUnits.Span, 20, LayoutDivUnits.Pixels));
            //divLayoutManager.Divs.Add(new LayoutDiv(_button.Id, 20, 20));
            //divLayoutManager.Divs.Add(new LayoutDiv(_listControl.Id, 100, LayoutDivUnits.Percentage, 0, LayoutDivUnits.Span));
            
            UpdateLayout(LayoutUpdateReason.ChildSizeChanged);                      
        }

        protected override IControlLayoutManager LayoutManager
        {
            get
            {
                return _layoutManager;
            }
        }

        public ListControl List { get { return _listControl; } }     
        
        public override bool CanFocus
        {
            get
            {
                return true;
            }
        }

        public override void Draw(long gameTime)
        {
            switch (_dropListState)
            {
                case DropListState.Closed:
                    _textBox.Draw(gameTime);
                    _button.Draw(gameTime);
                    break;
                case DropListState.Open:
                    _textBox.Draw(gameTime);
                    _button.Draw(gameTime);
                    _listControl.Draw(gameTime);
                    break;
            }

            base.Draw(gameTime);
        }

        public override void Update(long gameTime)
        {
            if (this._textBox.HasFocus)
            {
                Services.IInputService inputService = (Services.IInputService)Game.Services.GetService(typeof(Services.InputService));
                if (inputService == null)
                    throw new InvalidOperationException("Input service not available.");
                ControlInputState cim = inputService.GetControlInputState();
                
                switch (_dropListState)
                {
                    case DropListState.Closed:
                        if (cim.IsKeyReleased(SharpDX.DirectInput.Key.Down))
                            _dropListState = DropListState.Open;
                        break;

                    case DropListState.Open:
                        if (cim.IsKeyReleased(SharpDX.DirectInput.Key.Escape))
                            _dropListState = DropListState.Closed;
                        if (cim.IsKeyReleased(SharpDX.DirectInput.Key.Down))
                        {
                            if (this.List.SelectedItems.Count > 0)
                            {
                                int lastSelectedIndex = this.List.Items.IndexOf(this.List.SelectedItems[this.List.SelectedItems.Count - 1]);
                                if (lastSelectedIndex < this.List.Items.Count - 1)
                                    this.List.SelectItems(new[] { this.List.Items[lastSelectedIndex + 1] });
                            }
                            else if (this.List.Items.Count > 0)
                                this.List.SelectItems(new[] { this.List.Items[0] });                                
                        }
                        break;
                }
            }

            switch (_dropListState)
            {
                case DropListState.Closed:
                    _textBox.Update(gameTime);
                    _button.Update(gameTime);
                    break;

                case DropListState.Open:
                    _textBox.Update(gameTime);
                    _button.Update(gameTime);
                    _listControl.Update(gameTime);
                    break;
            }

            base.Update(gameTime);
        }
        

        public void SetNextFocus(UIControl focusedControl)
        {
            if (focusedControl == _textBox || focusedControl == _button)
            {
                VisualRoot.FocusManager.SetNextFocus(this, false);
            }

            //else if (focusedControl == _listControl)
            //    _listControl.SelectNextItem();
        }

        public void SetPreviousFocus(UIControl focusedControl)
        {
            if (focusedControl == _textBox || focusedControl == _button)
                VisualRoot.FocusManager.SetPreviousFocus(this);
            //else if (focusedControl == _listControl)
            //    _listControl.SelectPreviousItem();
        }

        private void FocusManager_ControlReceivedFocus(object sender, FocusChangedEventArgs args)
        {
            if (args.Control == this)
                _textBox.HasFocus = true;
        }

        protected void OnListClosed()
        {
            if (_listControl.HasFocus)
                _textBox.HasFocus = true;

            EventHandler handler = ListClosed;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void ListControl_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            SelectionChangedHandler handler = SelectionChanged;
            if (handler != null)
                handler(this, args);
        }        

        public event EventHandler ListClosed;

        public event SelectionChangedHandler SelectionChanged;
    }

    public enum DropListState
    {
        Open,
        Closed
    }
}
