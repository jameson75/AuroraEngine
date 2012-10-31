using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.DirectInput;
using CipherPark.AngelJacket.Core.Utils;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class DropList : UIControl
    {
        TextBox _textBox = null;
        ListControl _listControl = null;
        Button _button = null;
        DropListState _dropListState = DropListState.Closed;

        public DropList(Components.IUIRoot root)
            : base(root)
        {
            CreateChildComponents();
        }

        private void CreateChildComponents()
        {
            Utils.Interop.SpriteFont tempSpriteFont = Utils.Interop.ContentImporter.LoadFont(Game.GraphicsDevice, "Content\\SpriteFont\\StartMenu.spriteFont");
            _textBox = new TextBox(this.VisualRoot, null, tempSpriteFont, Colors.White, Colors.Green);           
            _listControl = new ListControl(this.VisualRoot);            
            _listControl.Items.Add(new ListControlItem(this.VisualRoot, "List_Item_1", "List Item 1", tempSpriteFont, Colors.White, Colors.Red));
            _button = new Button(this.VisualRoot, ">", tempSpriteFont, Colors.White, Colors.Blue);
        }

        public override void Draw(long gameTime)
        {
            switch (_dropListState)
            {
                case DropListState.Closed:
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
