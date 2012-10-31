using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Interop;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class Button : UIControl, ICommandControl
    {
        UIContent _content = null;

        public Button(IUIRoot visualRoot)
            : base(visualRoot)
        { }

        public Button(IUIRoot visualRoot, string text, SpriteFont font, Color4 fontColor, Color4 bgColor)
            : base(visualRoot)
        {
            Content = new TextContent(text, font, fontColor, bgColor);
        }

        public Button(IUIRoot visualRoot, Texture2D texture)
            : base(visualRoot)
        {
            Content = new ImageContent(texture);
        }

        public UIContent Content
        {
            get { return _content; }
            set
            {
                if (value == null && _content != null)
                    _content.Container = null;
                _content = value;
                if (_content != null)
                    _content.Container = this;
                OnContentChanged();
            }
        }

        public string CommandName { get; set; }

        public override bool CanFocus
        {
            get
            {
                return true;
            }
        }

        public override void Draw(long gameTime)
        {
            if (Content != null)
                Content.Draw(gameTime);
            base.Draw(gameTime);
        }

        protected virtual void OnContentChanged()
        {
            EventHandler handler = ContentChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public override void Update(long gameTime)
        {
            if (this.HasFocus)
            {
                Services.IInputService inputServices = (Services.IInputService)Game.Services.GetService(typeof(Services.IInputService));
                if (inputServices == null)
                    throw new InvalidOperationException("Input services not available.");
                ControlInputState cim = inputServices.GetControlInputState();

                if (cim.IsKeyReleased(Key.Return))
                {
                    OnClick();
                    if (this.CommandName != null)
                        this.OnCommand(this.CommandName);
                }
            }
            base.Update(gameTime);
        }

        protected virtual void OnClick()
        {
            EventHandler handler = Click;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnCommand(string commandName)
        {
            ControlCommandHandler handler = ControlCommand;
            if (handler != null)
                handler(this, new ControlCommandArgs(commandName));
        }

        public event EventHandler ContentChanged;
        public event EventHandler Click;
        public event ControlCommandHandler ControlCommand;
    }
}
