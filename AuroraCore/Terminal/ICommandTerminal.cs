﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.Aurora.Core.Terminal
{
    public interface ICommandTerminal
    {
        void WriteError(string message);
    }
}
