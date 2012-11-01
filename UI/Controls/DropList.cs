using System;
using SharpDX;

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
            Utils.Interop.SpriteFont tempSpriteFont = Utils.Interop.ContentImporter.LoadFont(Game.GraphicsDevice, "Content\\SpriteFont\\StartMenu.spriteFont");
            
            _textBox = new TextBox(this.VisualRoot, null, tempSpriteFont, Colors.White, Colors.Green);
            _textBox.VerticalAlignment = VerticalAlignment.Stretch;
            _textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            _textBox.DivContainerId = Guid.NewGuid();
            Children.Add(_textBox);
            
            _listControl = new ListControl(this.VisualRoot);
            _listControl.VerticalAlignment = VerticalAlignment.Stretch;
            _listControl.HorizontalAlignment = HorizontalAlignment.Stretch;
            _listControl.DivContainerId = Guid.NewGuid();
            _listControl.Items.Add(new ListControlItem(this.VisualRoot, "List_Item_1", "List Item 1", tempSpriteFont, Colors.White, Colors.Red));
             Children.Add(_listControl);
            
            _button = new Button(this.VisualRoot, ">", tempSpriteFont, Colors.White, Colors.Blue);
            _button.VerticalAlignment = VerticalAlignment.Stretch;
            _button.HorizontalAlignment = HorizontalAlignment.Stretch;
            _button.DivContainerId = Guid.NewGuid();
            Children.Add(_button);

            DivLayoutManager divLayoutManager = new DivLayoutManager(this);
            divLayoutManager.Divs.Add(new LayoutDiv(_textBox.DivContainerId, 0, LayoutDivUnits.Span, 20, LayoutDivUnits.Pixels));
            divLayoutManager.Divs.Add(new LayoutDiv(_button.DivContainerId, 20, 20));
            divLayoutManager.Divs.Add(new LayoutDiv(_listControl.DivContainerId, 100, LayoutDivUnits.Percentage, 0, LayoutDivUnits.Span));
            _layoutManager = divLayoutManager;
            UpdateLayout(LayoutUpdateReason.ChildSizeChanged);
        }

        protected override IControlLayoutManager LayoutManager
        {
            get
            {
                return _layoutManager;
            }
        }

        protected override void OnLayoutChanged()
        {
            base.OnLayoutChanged();
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
                    break;
            }
        }
    }

    public enum DropListState
    {
        Open,
        Closed
    }
}
