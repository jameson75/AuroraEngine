using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.UI.Components;
using SharpDX;
using SharpDX.DirectInput;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class TextBox : UIControl
    {
        private TextContent _content = null;
        //private ControlInputState cim = null;

        public TextBox(IUIRoot visualRoot)
            : base(visualRoot)
        {
            
        }

        public TextBox(IUIRoot visualRoot, TextContent content) : base(visualRoot)
        {
            _content = content;
            _content.Container = this;
        }

        public TextContent Content 
        { 
            get { return _content; }
            set
            {
                if (value == null && _content != null)
                    _content.Container = null;
                _content = value;
                if (_content != null)
                    _content.Container = this;
            }
        }

        public override void Update(long gameTime)
        {       
            if (this.HasFocus)
            {        
                Services.InputService inputServices = (Services.InputService)Game.Services.GetService(typeof(Services.InputService));
                if (inputServices == null)
                    throw new InvalidOperationException("Input services not available.");
                ControlInputState cim = inputServices.GetControlInputState();       
                //cim.UpdateState(gameTime);
                if (cim.IsKeyReleased(Key.Return))
                    EnterKeyEvent(this, EventArgs.Empty);
                WritableInput[] cis = ControlInputState.ConvertToWritableInput(cim.GetKeysDown(), true);
                foreach (WritableInput ci in cis)
                    if (ci.KeyType == WritableInputType.Printable)
                        this._content.Text += ci.Ascii.ToString();
                    else
                    {
                        if (ci.Key == Key.Back)
                            if (this._content.Text.Length > 0)
                                this._content.Text = this._content.Text.Substring(0, this._content.Text.Length - 1);
                    }
            }
        }

        public override void Draw(long gameTime)
        {
            if (this.Size == DrawingSizeFExtension.Zero)
                return;
          
            /*
            ControlSpriteBatch.Begin();
            //BeginClipping();
            //Vector2 screenPosition = this.PositionToSurface(this.Position);
            Rectangle screenRectangle = this.BoundsToSurface(this.Bounds); // new Rectangle((int)screenPosition.X, (int)screenPosition.Y, (int)this.Size.X, (int)this.Size.Y);
            Texture2D backgroundTexture = new Texture2D(this.Game.GraphicsDevice, (int)this.Size.X, (int)this.Size.Y);
            int dataLength = (int)this.Size.X * (int)this.Size.Y;
            Color[] colorData = new Color[dataLength];
            for (int i = 0; i < dataLength; i++)
                colorData[i] = Color.DarkGray;
            backgroundTexture.SetData<Color>(colorData);
            ControlSpriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            //EndClipping();
            ControlSpriteBatch.End();                             
            */
            if(_content != null)
                _content.Draw(gameTime);           
            base.Draw(gameTime);
        }

        public event EventHandler EnterKeyEvent;

        //****************************************************************
        // NOTE: As of XNA 4.0 There is no more fixed-function clipping!! *
        // It has to be implemented as shader                             *
        //****************************************************************
        //RasterizerState preClippingState = null;
        //
        //private void BeginClipping()
        //{
        //    preClippingState = Game.GraphicsDevice.RasterizerState;
        //    RasterizerState state = new RasterizerState();
        //    state.ScissorTestEnable = true;
        //     Rectangle clippingRectangle = this.BoundsToSurface(this.Bounds);
        //    clippingRectangle.Width = 10;
        //    Game.GraphicsDevice.RasterizerState = state;
        //    Game.GraphicsDevice.ScissorRectangle = clippingRectangle;   
        //}

        //private void EndClipping()
        //{
        //    Game.GraphicsDevice.RasterizerState = preClippingState;
        //}
    }
}
