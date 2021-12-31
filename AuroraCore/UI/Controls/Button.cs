using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.KillScript.Core.UI.Components;
using CipherPark.KillScript.Core.Utils;
using CipherPark.KillScript.Core.Utils.Toolkit;
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

namespace CipherPark.KillScript.Core.UI.Controls
{
    public class Button : ContentControl, ICommandControl
    {
        //UIContent _foregroundContent = null;
        //UIContent _backgroundContent = null;

        public Button(IUIRoot visualRoot)
            : base(visualRoot)
        {
            visualRoot?.Theme.Apply(this);
        }

        public Button(IUIRoot visualRoot, string text, SpriteFont font, Color4 fontColor, Color4? bgColor = null)
            : base(visualRoot)
        {
            Content = new TextContent(text, font, fontColor, bgColor.GetValueOrDefault());
            visualRoot?.Theme.Apply(this);
        }      

        public Button(IUIRoot visualRoot, Texture2D image)
            : base(visualRoot)
        {
            Content = new ImageContent(image);
            visualRoot?.Theme.Apply(this);
        }       

        /*
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
        */

        public string CommandName { get; set; }

        public override bool CanReceiveFocus
        {
            get
            {
                return true;
            }
        }

        public void FireClick()
        {
            OnClick();
        }

        /*
        protected override void OnDraw(GameTime gameTime)
        {
            if (BackgroundContent != null)
                BackgroundContent.Draw(gameTime);

            if (Content != null)
                Content.Draw(gameTime);

            base.OnDraw(gameTime);
        }     
        */

        protected override void OnUpdate(GameTime gameTime)
        {
            Services.IInputService inputServices = (Services.IInputService)Game.Services.GetService(typeof(Services.IInputService));
            
            if (inputServices == null)
                throw new InvalidOperationException("Input services not available.");
            
            if (this.HasFocus)
            {         
                BufferedInputState bInputState = inputServices.GetBufferedInputState();
                if (bInputState.IsKeyReleased(Key.Return))
                    OnClick();
            }
            
            if( this.IsHit )
            {
                InputState inputState = inputServices.GetInputState();
                if( inputState.IsMouseButtonPressed(InputState.MouseButton.Left) )            
                    OnClick();
            }
            
            base.OnUpdate(gameTime);
        }

        protected virtual void OnClick()
        {
            EventHandler handler = Click;  
            
            if (handler != null)
                handler(this, EventArgs.Empty);

            if (this.CommandName != null)
                this.OnCommand(this.CommandName);
        }

        protected virtual void OnCommand(string commandName)
        {
            ControlCommandHandler handler = ControlCommand;
            if (handler != null)
                handler(this, new ControlCommandArgs(commandName));
        }

        /*
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
        */

        public event EventHandler Click;
        public event ControlCommandHandler ControlCommand;

        public static Button FromTemplate(IUIRoot visualRoot, ButtonTemplate template)
        {
            Button button = new Button(visualRoot);
            button.ApplyTemplate(template);
            return button;
        }
    }
}
