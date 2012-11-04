﻿using System;
using SharpDX;
using CipherPark.AngelJacket.Core.Utils;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class DropList : UIControl
    {
        TextBox _textBox = null;
        ListControl _listControl = null;
        Button _button = null;
        DropListState _dropListState = DropListState.Closed;
        private IControlLayoutManager _layoutManager = null;

        public DropList(Components.IUIRoot root)
            : base(root)
        {
            InitializeControl();
        }

        private void InitializeControl()
        {
            Utils.Interop.SpriteFont tempSpriteFont = Utils.Interop.ContentImporter.LoadFont(Game.GraphicsDevice, "Content\\Fonts\\StartMenuFont.spritefont");
            
            _textBox = new TextBox(this.VisualRoot, null, tempSpriteFont, Colors.White, Colors.Yellow);
            _textBox.DivContainerId = Guid.NewGuid();
            Children.Add(_textBox);
            
            _listControl = new ListControl(this.VisualRoot);
            _listControl.DivContainerId = Guid.NewGuid();
            _listControl.Items.Add(new ListControlItem(this.VisualRoot, "List_Item_1", "List Item 1", tempSpriteFont, Colors.White, Colors.Aqua));
             Children.Add(_listControl);
            
            _button = new Button(this.VisualRoot, "?", tempSpriteFont, Colors.White, Colors.Blue);
            _button.DivContainerId = Guid.NewGuid();
            Children.Add(_button);

            DivLayoutManager divLayoutManager = new DivLayoutManager(this);
            divLayoutManager.Divs.Add(new LayoutDiv(_textBox.DivContainerId, 0, LayoutDivUnits.Span, 20, LayoutDivUnits.Pixels));
            divLayoutManager.Divs.Add(new LayoutDiv(_button.DivContainerId, 20, 20));
            divLayoutManager.Divs.Add(new LayoutDiv(_listControl.DivContainerId, 100, LayoutDivUnits.Percentage, 0, LayoutDivUnits.Span));
            _layoutManager = divLayoutManager;

            this._dropListState = DropListState.Open;

            UpdateLayout(LayoutUpdateReason.ChildSizeChanged);
        }

        protected override IControlLayoutManager LayoutManager
        {
            get
            {
                return _layoutManager;
            }
        }

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
        }

        public override void Update(long gameTime)
        {
            if (this.HasFocus)
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
    }

    public enum DropListState
    {
        Open,
        Closed
    }
}
