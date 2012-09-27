using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.UI.Components;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public abstract class ItemControl : ContainerControl, ICommandControl
    {
        private UIContent _itemContent = null;
        private UIContent _selectContent = null;
        private bool isSelected = false;      

        public ItemControl(IUIRoot visualRoot)
            : base(visualRoot)
        {
            
        }

        public ItemControl(IUIRoot visualRoot, string name, UIContent itemContent, UIContent selectContent = null) : base(visualRoot)
        {
            Name = name;
            ItemContent = itemContent; 
            if(selectContent != null)
                SelectContent = selectContent;
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

        public UIContent ItemContent
        {
            get { return _itemContent; }
            set
            {
                if (_itemContent != null)
                    _itemContent.Container = null;
                _itemContent = value;
                if (value != null)
                    value.Container = this;
            }
        }

        public UIContent SelectContent
        {
            get { return _selectContent; }
            set
            {
                if (_selectContent != null)
                    _selectContent.Container = null;
                _selectContent = value;
                if (value != null)
                    value.Container = this;
            }
        }
        public string CommandName { get; set; }

        public override void Draw(long gameTime)
        {           
            if (ActiveContent != null)
                ActiveContent.Draw(gameTime);
            base.Draw(gameTime);
        }    

        protected UIContent ActiveContent
        {
            get
            {
                if (this.IsSelected && SelectContent != null)
                    return SelectContent;
                else
                    return ItemContent;
            }
        }

        protected virtual void OnSelected()
        { }

        protected virtual void OnUnselected()
        { }

        protected virtual void OnControlCommand(ControlCommandArgs args)
        {
            ControlCommandHandler handler = ControlCommand;
            if (handler != null)
                handler(this, args);
        }

        public event ControlCommandHandler ControlCommand;
    }
}

