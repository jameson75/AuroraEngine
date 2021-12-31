using System;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using SharpDX;
using SharpDX.DirectInput;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class TextBox : UIControl
    {
        private ColorContent _backgroundContent = null;
        private TextContent _textContent = null;
        //private ControlInputState cim = null;
        private ContentControl _caret = null;
        private long _lastCaretUpdateTime = 0;
        private bool _textEdited = false;

        public TextBox(IUIRoot visualRoot)
            : base(visualRoot)
        {
            _backgroundContent = new ColorContent();
            _backgroundContent.Container = this;
            _textContent = new TextContent();
            _textContent.Container = this;            
        }

        public TextBox(Components.IUIRoot visualRoot, string text, SpriteFont font, Color4 fontColor, Color4 backgroundColor) : base(visualRoot)
        {
            _backgroundContent = new ColorContent(backgroundColor);
            _backgroundContent.Container = this;
            _textContent = new TextContent(text, font, fontColor);
            _textContent.Container = this;
        }

        public TextBox(IUIRoot visualRoot, TextContent textContent, ColorContent backgroundContent) : base(visualRoot)
        {
            _backgroundContent = backgroundContent;
            _backgroundContent.Container = this;
            _textContent = textContent;
            _textContent.Container = this;
        }

        public TextContent Content 
        { 
            get { return _textContent; }
            //set
            //{
            //    if (value == null && _content != null)
            //        _content.Container = null;
            //    _content = value;
            //    if (_content != null)
            //        _content.Container = this;
            //}
        }       

        public ColorContent BackgroundColor
        {
            get { return _backgroundContent; }
        }

        public override void Initialize()
        {
            VisualRoot.FocusManager.ControlLostFocus += FocusManager_ControlLostFocus;
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (_caret == null)
                BeginCaret();
   
            if (this.HasFocus)
            {        
                Services.InputService inputServices = (Services.InputService)Game.Services.GetService(typeof(Services.InputService));
                if (inputServices == null)
                    throw new InvalidOperationException("Input services not available.");
                BufferedInputState cim = inputServices.GetBufferedInputState();                
                //cim.UpdateState(gameTime);
                if (cim.IsKeyReleased(Key.Return))
                    OnEnterKey();
                else if (cim.GetKeysDown().Length > 0)
                {
                    AsciiCharacterInfo[] cis = BufferedInputState.ConvertToAsciiCharacters(cim.GetKeysDown(), AsciiCharacterConversionFlags.IgnoreNewLine | AsciiCharacterConversionFlags.IgnoreTab);                                           
                    foreach (AsciiCharacterInfo ci in cis)
                        if (ci.KeyType == AsciiCharacterType.Printable)
                            this._textContent.Text += ci.Ascii.ToString();
                        else
                        {
                            if (ci.Key == Key.Back)
                                if (this._textContent.Text.Length > 0)
                                    this._textContent.Text = this._textContent.Text.Substring(0, this._textContent.Text.Length - 1);
                        }
                    UpdateCaretPosition();
                    _textEdited = true;
                }
            }            

            if (_lastCaretUpdateTime == 0 || gameTime.GetTotalRealtime() - _lastCaretUpdateTime > 500)
            {
                _caret.Visible = !_caret.Visible;
                _lastCaretUpdateTime = gameTime.GetTotalRealtime();
            }
        }

        protected override void OnDraw(GameTime gameTime)
        {
            if (this.Size == Size2FExtension.Zero)
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

            if (_backgroundContent != null)
                _backgroundContent.Draw(gameTime);

            if(_textContent != null)
                _textContent.Draw(gameTime);
           
            if( this.HasFocus && _caret.Visible )
                _caret.Draw(gameTime);
           
            base.OnDraw(gameTime);
        }

        public event EventHandler EnterKeyEvent;

        //****************************************************************
        // NOTE: As of XNA 4.0 There is no more fixed-function clipping!! *
        // It has to be implemented as/into shader                        *
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
            //by using ImageContent or ColorConent and, as of now, contents do not have a shape/size of their own -
            //they assume the same of their container (control).
            //TODO: Implement the caret a simple texture.
            //****************************************************************************************************
            _caret = new ContentControl(this.VisualRoot, new ColorContent(Color.White));    
            this.Children.Add(_caret);
            _caret.Position = new Vector2(0f, this.Bounds.Height - 15f);
            _caret.Size = new Size2F(5f, 10f);
            _caret.Visible = false;
        }

        private void UpdateCaretPosition()
        {
            if(_textContent.Text != null )
                _caret.Position = new Vector2(_textContent.GetTextLength(0, _textContent.Text.Length), _caret.Position.Y);
        }

        protected virtual void OnEnterKey()
        {
            EventHandler handler = EnterKeyEvent;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnFocusLost()
        {
            if (_textEdited)
            {
                _textEdited = false;
                OnEditComplete();                
            }
        }

        protected virtual void OnEditComplete()
        {
            EventHandler handler = EditComplete;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void FocusManager_ControlLostFocus(object sender, FocusChangedEventArgs args)
        {
            if (args.Control == this)
                OnFocusLost();
        }

        public event EventHandler EditComplete;

        public override void ApplyTemplate(UIControlTemplate template)
        {
            base.ApplyTemplate(template);
        }

        public static TextBox FromTemplate(IUIRoot visualRoot, TextBoxTemplate template)
        {
            TextBox textBox = new TextBox(visualRoot);
            textBox.ApplyTemplate(template);
            return textBox;
        }
    }
}
