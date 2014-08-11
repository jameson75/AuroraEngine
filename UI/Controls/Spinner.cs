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
        private Label _textArea = null;        
        private Button _upButton = null;
        private Button _downButton = null;        
        private SplitterPanel _mainPanel = null;
        private SplitterPanel _rightSubPanel = null;

        private const int RightPanelFixedLength = 15;
        private const int ButtonsHorzOffset = 4;
        private const int ButtonsVerticalOffset = 4;
        private const int RightLowerDivisionDistance = 50;
        private const string RightDivisionGuid = "FCBFE554-1781-4B88-8C45-720D2082622D";
        private const string RightLowerDivisionGuid = "61686D6A-2D62-491F-A86E-4BD2624BBEBF";      

        private Spinner(IUIRoot visualRoot)
            : base(visualRoot)
        { }

        public Spinner(IUIRoot visualRoot, SpriteFont font, Color fontColor, Color backgroundColor)
            : base(visualRoot)
        {            
            _mainPanel = new SplitterPanel(visualRoot);
            _mainPanel.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;
            _mainPanel.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            _mainPanel.Splitters.Add(new SplitterLayoutDivision(new Guid(RightDivisionGuid), RightPanelFixedLength, SplitterLayoutAnchorSide.Two));            
            _mainPanel.Orientation = SplitterLayoutOrientation.Verticle;            
           
            _rightSubPanel = new SplitterPanel(visualRoot);
            _rightSubPanel.Splitters.Add(new SplitterLayoutDivision(new Guid(RightLowerDivisionGuid), RightLowerDivisionDistance, SplitterLayoutAnchorSide.None, true));
            _rightSubPanel.Orientation = SplitterLayoutOrientation.Horizontal;
            _rightSubPanel.LayoutId = new Guid(RightDivisionGuid);
            _rightSubPanel.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;
            _rightSubPanel.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            _mainPanel.Children.Add(_rightSubPanel);

            _textArea = new Label(visualRoot, string.Empty, font, fontColor, backgroundColor);
            _textArea.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;
            _textArea.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            _mainPanel.Children.Add(_textArea);

            _upButton = new Button(visualRoot) { Content = new ColorContent(fontColor) };
            _upButton.Click += UpButton_Click;
            _upButton.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;
            _upButton.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            _upButton.EnableFocus = false;
            _rightSubPanel.Children.Add(_upButton);

            _downButton = new Button(visualRoot) { Content = new ColorContent(fontColor) };
            _downButton.Click += DownButton_Click;
            _downButton.LayoutId = new Guid(RightLowerDivisionGuid);
            _downButton.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;
            _downButton.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            _downButton.EnableFocus = false;
            _rightSubPanel.Children.Add(_downButton);            

            visualRoot.FocusManager.ControlReceivedFocus += FocusManager_ControlReceivedFocus;

            Value = 0;
            Children.Add(_mainPanel);
            UpdateLayout(LayoutUpdateReason.ChildCountChanged);
        }

        void FocusManager_ControlReceivedFocus(object sender, FocusChangedEventArgs args)
        {
            //re-route focus of children (textarea is a label, which can't receive focus).
            if (args.Control == _upButton ||
                args.Control == _downButton)
                this.HasFocus = true;
        }
     
        public override bool CanReceiveFocus
        {
            get
            {
                return true;
            }
        }

        public string DisplayFormat { get; set; }
  
        public float Increment { get; set; }

        public RangeF Range { get; set; }

        public double Value
        {
            get
            {
                double result = 0;
                if (!double.TryParse(_textArea.TextContent.Text, out result))
                    return 0;
                else
                    return result;
            }
            set
            {
                _textArea.TextContent.Text = value.ToString();
                OnValueChanged(Value);
            }                    
        }

        protected override void OnUpdate(GameTime gameTime)
        {            
            _mainPanel.Update(gameTime);
            base.OnUpdate(gameTime);
        }

        protected override void OnDraw(GameTime gameTime)
        {           
            _mainPanel.Draw(gameTime);
            base.OnDraw(gameTime);
        }

        protected override void OnLayoutChanged()
        {           
            _mainPanel.Position = DrawingPointFExtension.Zero;
            _mainPanel.Size = this.Size;
        }

        protected virtual void OnValueChanged(double value)
        {
            EventHandler handler = ValueChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }              

        private void DownButton_Click(object sender, EventArgs args)
        {
            IncrementValue(-Increment);
        }

        private void UpButton_Click(object sender, EventArgs args)
        {
            IncrementValue(Increment);
        }

        private void IncrementValue(float increment)
        {
            Value += increment;
        }        

        public override void ApplyTemplate(UIControlTemplate template)
        {
            base.ApplyTemplate(template);
        }

        public static Spinner FromTemplate(IUIRoot visualRoot, SpinnerTemplate template)
        {
            Spinner spinner = new Spinner(visualRoot);
            spinner.ApplyTemplate(template);
            return spinner;
        }

        public event EventHandler ValueChanged;
    }
}
