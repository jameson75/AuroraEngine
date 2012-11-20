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
        TextContent _textContent = null;
        UIContent _backgroundContent = null;

        public Button(IUIRoot visualRoot)
            : base(visualRoot)
        { }

        public Button(IUIRoot visualRoot, string text, SpriteFont font, Color4 fontColor, Color4 bgColor)
            : base(visualRoot)
        {
            BackgroundContent = new ColorContent(bgColor);
            TextContent = new TextContent(text, font, fontColor);
        }

        public Button(IUIRoot visualRoot, Texture2D texture)
            : base(visualRoot)
        {
            BackgroundContent = new ImageContent(texture);
        }

        public TextContent TextContent
        {
            get { return _textContent; }
            set
            {
                if (value == null && _textContent != null)
                    _textContent.Container = null;
                _textContent = value;
                if (_textContent != null)
                    _textContent.Container = this;
                OnTextContentChanged();
            }
        }

        public UIContent BackgroundContent
        {
            get { return _backgroundContent; }
            set
            {
                if (value == null && _backgroundContent != null)
                    _backgroundContent.Container = null;
                _backgroundContent = value;
                if (_backgroundContent != null)
                    _backgroundContent.Container = this;
                OnBackgroundContentChanged();
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
            if (TextContent != null)
                TextContent.Draw(gameTime);
            base.Draw(gameTime);
        }

        protected virtual void OnTextContentChanged()
        {
            EventHandler handler = TextContentChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnBackgroundContentChanged()
        {
            EventHandler handler = BackgroundContentChanged;
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

        public event EventHandler BackgroundContentChanged;
        public event EventHandler TextContentChanged;
        public event EventHandler Click;
        public event ControlCommandHandler ControlCommand;
    }
}
