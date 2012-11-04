using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.UI.Components;

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

