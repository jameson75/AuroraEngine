﻿using System;
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
        ContentControl _checkedContentControl = null;
        ContentControl _uncheckedContentControl = null;
        Label _label = null;

        private CheckBox(IUIRoot visualRoot)
            : base(visualRoot)
        {
            _checkedContentControl = new ContentControl(visualRoot);
            _uncheckedContentControl = new ContentControl(visualRoot);
            _label = new Label(visualRoot);
            Children.Add(_checkedContentControl);
            Children.Add(_uncheckedContentControl);
            Children.Add(_label);
            UpdateLayout(LayoutUpdateReason.ChildCountChanged);
        }
        
        public CheckBox(IUIRoot visualRoot, string caption, SpriteFont font, Color fontColor, Color checkedColor)
            : base(visualRoot)
        {
            UIContent checkedContent = new ColorContent(checkedColor);
            UIContent uncheckedContent = new ColorContent(Color.Transparent);
            _checkedContentControl = new ContentControl(visualRoot, checkedContent);
            _uncheckedContentControl = new ContentControl(visualRoot, uncheckedContent);
            _label = new Label(visualRoot, caption, font, fontColor);
            Children.Add(_checkedContentControl);
            Children.Add(_uncheckedContentControl);
            Children.Add(_label);
            UpdateLayout(LayoutUpdateReason.ChildCountChanged);
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

        protected override void OnDraw(GameTime gameTime)
        {
            if (IsChecked)
                _checkedContentControl.Draw(gameTime);
            else
                _uncheckedContentControl.Draw(gameTime);
            base.OnDraw(gameTime);
        }

        protected override void OnLayoutChanged()
        {
            _label.Position = new DrawingPointF(0, this.Bounds.Bottom - _label.Size.Height);
            _checkedContentControl.Position = new DrawingPointF(this.Bounds.Left - this.Size.Width, this.Bounds.Bottom - _checkedContentControl.Size.Height);
            _uncheckedContentControl.Position = _checkedContentControl.Position;
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
            get { return _label.TextContent.Text; }
            set
            {
                _label.TextContent.Text = value;
                _label.SizeToContent();
            }
        }

        public override void ApplyTemplate(UIControlTemplate template)
        {
            CheckBoxTemplate cbTemplate = (CheckBoxTemplate)template;

            if (cbTemplate.CaptionTemplate != null)
                _label.ApplyTemplate(cbTemplate.CaptionTemplate);
            
            if (cbTemplate.CheckContentTemplate != null)
                _checkedContentControl.ApplyTemplate(cbTemplate.CheckContentTemplate);

            if (cbTemplate.UncheckContentTemplate != null)
                _uncheckedContentControl.ApplyTemplate(cbTemplate.UncheckContentTemplate);

            base.ApplyTemplate(template);
        }

        public static CheckBox FromTemplate(IUIRoot visualRoot, CheckBoxTemplate template)
        {
            CheckBox checkBox = new CheckBox(visualRoot);
            checkBox.ApplyTemplate(template);
            return checkBox;
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
