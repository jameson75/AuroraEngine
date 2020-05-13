using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.UI.Components;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public abstract class ItemControl : ContainerControl, ICommandControl
    {       
        private bool isSelected = false;      

        public ItemControl(IUIRoot visualRoot)
            : base(visualRoot)
        {
            
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    if (isSelected)
                        OnSelected();
                    else
                        OnUnselected();
                }
            }
        }       

        public string CommandName { get; set; }

        public event ControlCommandHandler ControlCommand;       

        protected void OnCommand(string commandName)
        {
            ControlCommandHandler handler = ControlCommand;
            if (handler != null)
                handler(this, new ControlCommandArgs(commandName));
        }       

        protected virtual void OnSelected()
        { }

        protected virtual void OnUnselected()
        { } 
    }
}

