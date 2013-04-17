using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class Spinner : UIControl
    {
        private TextBox _textBox = null;
        private Button _upButton = null;
        private Button _downButton = null;

        public Spinner(IUIRoot visualRoot)
            : base(visualRoot)
        { 
            _textBox = new TextBox(visualRoot, new TextContent(string.Empty, font, fontColor);
            _upButton = new Button(visualRoot);      
            _downButton = new Button(visualRoot);
            _downButton.Content
            Children.Add(_textBox);
            Children.Add(_upButton);
            Children.Add(_downButton);
            UpdateLayout(LayoutUpdateReason.ChildCountChanged);
        }

        public string Format
        {
            get { return _textBox.Content.Format; }
            set { _textBox.Content.Format = value; }
        }

        public float Increment { get; set; }

        public Range Range { get; set; }

        public float Value
        {
            get
            {
                float result = 0;
                if (!float.TryParse(_textBox.Content.Text, out result))
                    return 0;
                else
                    return result;
            }
            set
            {
                _textBox.Content.Text = value.ToString();
            }                    
        }

        public override void Draw(long gameTime)
        {
            _textBox.Draw(gameTime);
            _upButton.Draw(gameTime);
            _downButton.Draw(gameTime);
            base.Draw(gameTime);
        }

        protected override void OnLayoutChanged()
        {
            _textBox.Position = new DrawingPointF(0, 0);
            _textBox.Size = new DrawingSizeF(this.Size.Width - 5.0f, this.Size.Height);

            _upButton.Position = new DrawingPointF(this.Size.Width - 5.0f, 0);
            _upButton.Size = new DrawingSizeF(5.0f, 5.0f);

            _downButton.Position = new DrawingPointF(this.Size.Width - 5.0f, 5.0f);
            _downButton.Size = new DrawingSizeF(this.Size.Width - 5.0f, this.Size.Height);
        }
    }
}
