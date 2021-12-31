using CipherPark.KillScript.Core.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.KillScript.Core.Services
{
    public interface ICommandTerminalService
    {
        ICommandTerminal Terminal { get; }
    }
}
