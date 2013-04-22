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
        ContentControl checkContentControl = null;
        UIContent checkContent = null;

        public CheckBox(IUIRoot visualRoot)
            : base(visualRoot)
        {
            checkContent = new ColorContent(Color.Gray);
            checkContentControl = new ContentControl(visualRoot, checkContent);
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

        public override void Draw(long gameTime)
        {
            checkContentControl.Draw(gameTime);
            base.Draw(gameTime);
        }

        protected override void OnLayoutChanged()
        {
            checkContentControl.Position = new DrawingPointF(this.Size.Width - 10.0f, 0.0f);
            checkContentControl.Size = new DrawingSizeF(10.0f, 10.0f);
        }

        protected virtual void OnIsCheckedChanged()
        {
            Color checkContentColor = (IsChecked) ? Color.Gray : Color.Blue;
            ((ColorContent)checkContent).Color = checkContentColor;
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
    }

    //public class CheckedChangedEventArgs : EventArgs
    //{
    //    bool _isChecked = false;
    //    public CheckedChangedEventArgs(bool isChecked) { _isChecked = isChecked; }
    //    public bool IsChecked { get { return _isChecked; } }
    //}

    //public delegate void CheckChangedHandler(object sender, CheckedChangedEventArgs args);
}
