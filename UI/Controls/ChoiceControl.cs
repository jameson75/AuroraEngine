using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.DirectInput;
using CipherPark.KillScript.Core.Utils;
using CipherPark.KillScript.Core.UI.Components;
using CipherPark.KillScript.Core.Utils.Toolkit;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
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

            Vector2 subMenuRelativePosition = Vector2Extension.Zero;

            switch (_contextMenu.DisplaySide)
            {
                case ContextMenuDisplaySide.Left:
                    subMenuRelativePosition = new Vector2(this.Position.X - _contextMenu.Bounds.Width, this.Position.Y);
                    break;
                case ContextMenuDisplaySide.Above:
                    subMenuRelativePosition = new Vector2(this.Position.X, this.Position.Y - _contextMenu.Bounds.Height);
                    break;
                case ContextMenuDisplaySide.Right:
                    subMenuRelativePosition = new Vector2(this.Bounds.Right, this.Position.Y);
                    break;
                case ContextMenuDisplaySide.Bottom:
                    subMenuRelativePosition = new Vector2(this.Bounds.X, this.Bounds.Bottom);
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

        protected override void OnUpdate(GameTime gameTime)
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
            if (_contextMenu.Result == ContextControlResult.SelectOK)
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
