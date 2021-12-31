using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.Utils.Toolkit;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Controls
{
    public class CheckBox : UIControl, ICommandControl
    {
        bool _isChecked = false;
        ContentControl _checkedContentControl = null;
        ContentControl _uncheckedContentControl = null;
        ContentControl _label = null;

        private CheckBox(IUIRoot visualRoot)
            : base(visualRoot)
        {
            _checkedContentControl = new ContentControl(visualRoot);
            _uncheckedContentControl = new ContentControl(visualRoot);
            _label = new ContentControl(visualRoot);
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
            _label = new ContentControl(visualRoot, new TextContent(caption, font, fontColor));
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

        protected override void OnDraw()
        {
            if (IsChecked)
                _checkedContentControl.Draw();
            else
                _uncheckedContentControl.Draw();
            base.OnDraw();
        }

        protected override void OnLayoutChanged()
        {
            _label.Position = new Vector2(0, this.Bounds.Bottom - _label.Size.Height);
            _checkedContentControl.Position = new Vector2(this.Bounds.Left - this.Size.Width, this.Bounds.Bottom - _checkedContentControl.Size.Height);
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
            get { return _label.Content.As<TextContent>().Text; }
            set
            {
                _label.Content.As<TextContent>().Text = value;
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
