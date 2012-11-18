using System;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils.Interop;
using SharpDX;
using SharpDX.DirectInput;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class TextBox : UIControl
    {
        private TextContent _content = null;
        //private ControlInputState cim = null;
        private ContentControl _caret = null;
        private long _lastCaretUpdateTime = 0;

        public TextBox(IUIRoot visualRoot)
            : base(visualRoot)
        {
            _content = new TextContent();
            _content.Container = this;
        }

        public TextBox(Components.IUIRoot visualRoot, string text, SpriteFont font, Color4 fontColor, Color4 backgroundColor) : base(visualRoot)
        {
            _content = new TextContent(text, font, fontColor, backgroundColor);
            _content.Container = this;
        }

        public TextBox(IUIRoot visualRoot, TextContent content) : base(visualRoot)
        {
            _content = content;
            _content.Container = this;
        }

        public TextContent Content 
        { 
            get { return _content; }
            //set
            //{
            //    if (value == null && _content != null)
            //        _content.Container = null;
            //    _content = value;
            //    if (_content != null)
            //        _content.Container = this;
            //}
        }

        public override void Update(long gameTime)
        {
            if (_caret == null)
                BeginCaret();
   
            if (this.HasFocus)
            {        
                Services.InputService inputServices = (Services.InputService)Game.Services.GetService(typeof(Services.InputService));
                if (inputServices == null)
                    throw new InvalidOperationException("Input services not available.");
                ControlInputState cim = inputServices.GetControlInputState();       
                //cim.UpdateState(gameTime);
                if (cim.IsKeyReleased(Key.Return))
                    OnEnterKey();
                else if (cim.GetKeysDown().Length > 0)
                {
                    WritableInput[] cis = ControlInputState.ConvertToWritableInput(cim.GetKeysDown(), WritableInputConversionFlags.IgnoreNewLine | WritableInputConversionFlags.IgnoreTab);
                    foreach (WritableInput ci in cis)
                        if (ci.KeyType == WritableInputType.Printable)
                            this._content.Text += ci.Ascii.ToString();
                        else
                        {
                            if (ci.Key == Key.Back)
                                if (this._content.Text.Length > 0)
                                    this._content.Text = this._content.Text.Substring(0, this._content.Text.Length - 1);
                        }
                    UpdateCaretPosition();
                }
            }            

            if (_lastCaretUpdateTime == 0 || gameTime - _lastCaretUpdateTime > 500)
            {
                _caret.Visible = !_caret.Visible;
                _lastCaretUpdateTime = gameTime;
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
           
            if( this.HasFocus && _caret.Visible )
                _caret.Draw(gameTime);
           
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

        private void BeginCaret()
        {
            //*****************************************************************************************************
            //NOTE: Implementing the caret as a child control is over-kill but, currently, I have no other
            //easy way of doing it. Right now, the only simple way I have of rendering a texture is 
            //by using ImageContent or ColorConent and, as of now, contents do have a shape/size of their own -
            //they assume the same of their container [controls].
            //TODO: Implement the caret a simple texture.
            //****************************************************************************************************
            _caret = new ContentControl(this.VisualRoot, new ColorContent(Color.White));    
            this.Children.Add(_caret);
            _caret.Position = new DrawingPointF(0f, this.Bounds.Height - 15f);
            _caret.Size = new DrawingSizeF(5f, 10f);
            _caret.Visible = false;
        }

        private void UpdateCaretPosition()
        {
            if(_content.Text != null )
                _caret.Position = new DrawingPointF(_content.GetTextLength(0, _content.Text.Length), _caret.Position.Y);
        }

        protected void OnEnterKey()
        {
            EventHandler handler = EnterKeyEvent;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
