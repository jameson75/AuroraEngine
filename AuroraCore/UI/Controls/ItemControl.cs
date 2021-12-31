using System;
using System.Collections.Generic;
using CipherPark.Aurora.Core.UI.Components;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Controls
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

