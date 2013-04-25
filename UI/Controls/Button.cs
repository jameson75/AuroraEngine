using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class Button : UIControl, ICommandControl
    {
        UIContent _foregroundContent = null;
        UIContent _backgroundContent = null;

        public Button(IUIRoot visualRoot)
            : base(visualRoot)
        {
            Content = DefaultTemplates.Button.ForegroundStyle.GenerateContent();
            BackgroundContent = DefaultTemplates.Button.BackgroundStyle.GenerateContent();
            Size = DefaultTemplates.Button.Size.Value;            
        }

        public Button(IUIRoot visualRoot, string text, SpriteFont font, Color4 fontColor, Color4 bgColor)
            : base(visualRoot)
        {
            Content = new TextContent(text, font, fontColor);
            BackgroundContent = new ColorContent(bgColor);
            Size = DefaultTemplates.Button.Size.Value;
        }

        public Button(IUIRoot visualRoot, Texture2D texture)
            : base(visualRoot)
        {
            BackgroundContent = new ImageContent(texture);
            Size = DefaultTemplates.Button.Size.Value;
        }

        public UIContent Content
        {
            get { return _foregroundContent; }
            set
            {
                if (value == null && _foregroundContent != null)
                    _foregroundContent.Container = null;
                _foregroundContent = value;
                if (_foregroundContent != null)
                    _foregroundContent.Container = this;
                OnForegroundContentChanged();
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
            if (BackgroundContent != null)
                BackgroundContent.Draw(gameTime);

            if (Content != null)
                Content.Draw(gameTime);

            base.Draw(gameTime);
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

        protected virtual void OnForegroundContentChanged()
        {
            EventHandler handler = ForegroundContentChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnBackgroundContentChanged()
        {
            EventHandler handler = BackgroundContentChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public event EventHandler BackgroundContentChanged;
        public event EventHandler ForegroundContentChanged;
        public event EventHandler Click;
        public event ControlCommandHandler ControlCommand;
    }
}
