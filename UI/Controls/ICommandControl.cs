using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public interface ICommandControl
    {
        string CommandName { get; set; }
        event ControlCommandHandler ControlCommand;
    }

    public class ControlCommandArgs : EventArgs
    {
        public string _commandName = null;

        public ControlCommandArgs(string commandName)
        {
            _commandName = commandName;
        }

        public string CommandName { get { return _commandName; } }
    }

    public delegate void ControlCommandHandler(object sender, ControlCommandArgs args);

    internal sealed class CommandControlWireUp
    {
        ICommandControl _commandControl = null;

        public CommandControlWireUp(UIControl control)
        {

        }
    }
}

