using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public interface ICommandDispatcher
    {
        event ControlCommandHandler ControlCommand;
    }

    public interface ICommandControl : ICommandDispatcher
    {
        string CommandName { get; set; }                
    }

    public class ControlCommandArgs : EventArgs
    {
        private string _commandName = null;

        public ControlCommandArgs(string commandName)
        {
            _commandName = commandName;
        }

        public string CommandName { get { return _commandName; } }
    }

    public delegate void ControlCommandHandler(object sender, ControlCommandArgs args);

    internal sealed class CommandControlWireUp
    {
        ICommandDispatcher _commandDispatcher = null;
        List<ICommandDispatcher> _wiredUpChildren = null;

        public CommandControlWireUp(ICommandDispatcher commandDispatcher)
        {
            _wiredUpChildren = new List<ICommandDispatcher>();
            UIControl uiControl = commandDispatcher as UIControl;
            if (uiControl != null)            
                uiControl.Children.CollectionChanged += CommandControl_Children_CollectionChanged;
            _commandDispatcher = commandDispatcher;          
        }

        public event ControlCommandHandler ChildControlCommand;

        private void CommandControl_Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add || args.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (UIControl control in args.NewItems)
                {
                    ICommandDispatcher iChildCommandControl = control as ICommandDispatcher;
                    if (iChildCommandControl != null)
                    {
                        iChildCommandControl.ControlCommand += CommandControl_Child_ControlCommand;
                        //NOTE: We must keep track of the container's children whose ControlCommand event we're listening to.
                        _wiredUpChildren.Add(iChildCommandControl);
                    }
                }
            }
            
            if (args.Action == NotifyCollectionChangedAction.Remove || args.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (UIControl control in args.OldItems)
                {
                    ICommandControl iChildCommandControl = control as ICommandControl;
                    if (iChildCommandControl != null)
                    {
                        //**************************************************************************************************
                        //NOTE: It is possible that a child control being removed from the command-dispatcher control may have
                        //been added to the container control BEFORE this object started listening the command-dispather control's 
                        //children collection. In which case, we would not have been listening to the child's ControlComand
                        //event. We ensure that we only stop listening to ControlCommand events for children we started
                        //listening to. We do this be examining the _wiredUpChildren list.
                        //**************************************************************************************************

                        if (_wiredUpChildren.Contains(iChildCommandControl))
                        {
                            iChildCommandControl.ControlCommand -= CommandControl_Child_ControlCommand;
                            _wiredUpChildren.Remove(iChildCommandControl);
                        }
                    }
                }
            }
        }

        private void CommandControl_Child_ControlCommand(object sender, ControlCommandArgs args)
        {
            ControlCommandHandler handler = ChildControlCommand;
            if (handler != null)
                handler(this, args);
        }
    }

   
}

