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
    public class CheckBox : UIControl, ICommandControl
    {
        bool _isChecked = false;
        ContentControl _checkContentControl = null;
        ContentControl _uncheckedContentControl = null;

        //UIContent checkContent = null;
        Label _label = null;

        public CheckBox(IUIRoot visualRoot)
            : base(visualRoot)
        {           
            _checkContentControl = new ContentControl(visualRoot);
            _checkContentControl.ApplyTemplate(DefaultTemplates.CheckBox.CheckContentTemplate);
            Children.Add(_checkContentControl);

            _uncheckedContentControl = new ContentControl(visualRoot);
            _uncheckedContentControl.ApplyTemplate(DefaultTemplates.CheckBox.UncheckContentTemplate);
            Children.Add(_uncheckedContentControl);
            
            Size = DefaultTemplates.CheckBox.Size.Value;           
        }

        public CheckBox(IUIRoot visualRoot, string caption)
            : this(visualRoot)
        {
            this.Caption = caption;
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnIsCheckedChanged();
            }
        }

        public override void Draw(long gameTime)
        {
            if (IsChecked)
                _checkContentControl.Draw(gameTime);
            else
                _uncheckedContentControl.Draw(gameTime);
            base.Draw(gameTime);
        }

        protected override void OnLayoutChanged()
        {
            _label.Position = new DrawingPointF(0, this.Bounds.Bottom - _label.Size.Height);
            _checkContentControl.Position = new DrawingPointF(this.Bounds.Left - this.Size.Width, this.Bounds.Bottom - _checkContentControl.Size.Height);
            _uncheckedContentControl.Position = _checkContentControl.Position;
        }

        protected virtual void OnIsCheckedChanged()
        {
            EventHandler handler = CheckChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
            OnCommand(this.CommandName);
        }

        public event EventHandler CheckChanged;

        public string CommandName { get; set; }

        public event ControlCommandHandler ControlCommand;

        protected virtual void OnCommand(string commandName)
        {
            ControlCommandHandler handler = ControlCommand;
            if (handler != null)
                handler(this, new ControlCommandArgs(commandName));
        }

        public string Caption
        {
            get { return _label.Content.Text; }
            set
            {
                _label.Content.Text = value;
                _label.SizeToContent();
            }
        }

        public override void ApplyTemplate(UIControlTemplate template)
        {
            CheckBoxTemplate cbTemplate = (CheckBoxTemplate)template;
            
            if (cbTemplate.CheckContentTemplate != null)
                _checkContentControl.ApplyTemplate(cbTemplate.CheckContentTemplate);

            if (cbTemplate.UncheckContentTemplate != null)
                _uncheckedContentControl.ApplyTemplate(cbTemplate.UncheckContentTemplate);

            base.ApplyTemplate(template);
        }
    }

    //public class CheckedChangedEventArgs : EventArgs
    //{
    //    bool _isChecked = false;
    //    public CheckedChangedEventArgs(bool isChecked) { _isChecked = isChecked; }
    //    public bool IsChecked { get { return _isChecked; } }
    //}

    //public delegate void CheckChangedHandler(object sender, CheckedChangedEventArgs args);
}
