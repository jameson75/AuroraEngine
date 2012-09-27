using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CipherPark.AngelJacket.Core.Services
{
    public interface IModuleService
    {
        void SignalSwitch(string moduleName);
        void SignalExit();
    }

    public class ModuleService : IModuleService
    {
        IGameApp _game = null;
        string _signaledModuleSwitchName = null;
        bool _exitModuleSignaled = false;

        public ModuleService(IGameApp game)
        {
            _game = game;
        }

        public void SignalSwitch(string moduleName)
        {
            //TODO: Change this to a design where this module service peforms
            //all module management (ie: move module management from AngelJacketGame to this class).
            this._signaledModuleSwitchName = moduleName;
        }

        public void SignalExit()
        {
            this._exitModuleSignaled = true;
        }
    }
}
