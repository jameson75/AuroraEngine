using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.KillScript.Core.Module;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.Services
{  
    public interface IModuleService
    {
        GameModule ActiveModule { get; }
        bool ExitSignaled { get; }
        void ActivateOnUpdate(string moduleName);
        void ExitOnUpdate();
        void RegisterModule(string registeredName, GameModule module);
        void Update();
        void Clear();
    }

    public class ModuleService : IModuleService
    {
        IGameApp _game = null;
        GameModule _pendingActiveModule = null;
        GameModule _activeModule = null;
        Dictionary<string, GameModule> _modules = null;

        public bool ExitSignaled { get; private set; }

        public ModuleService(IGameApp game)
        {
            _game = game;
            _modules = new Dictionary<string, GameModule>();
        }

        /// <summary>
        /// Causes the module with specified name to become the active module 
        /// the next time ModuleService.Update() is called for this instance.
        /// </summary>
        /// <param name="registeredModuleName"></param>
        public void ActivateOnUpdate(string registeredModuleName)
        {
            if (!_modules.ContainsKey(registeredModuleName))
                throw new InvalidOperationException("The registered module name does not exist.");

            this._pendingActiveModule = _modules[registeredModuleName];
        }

        public void ExitOnUpdate()
        {
            this.ExitSignaled = true;
        }

        public void RegisterModule(string registeredName, GameModule module)
        {
            if (string.IsNullOrEmpty(registeredName))
                throw new ArgumentException("A registered name was not specified", "registeredName");

            if (module == null)
                throw new ArgumentNullException("module");

            if (_modules.ContainsKey(registeredName))
                throw new InvalidOperationException("An entry with the registered name already exists.");

            _modules.Add(registeredName, module);
        }

        public void Update()
        {
            if (_pendingActiveModule != null)
            {
                if (_activeModule != null && _activeModule.IsLoaded)
                    _activeModule.UnloadContent();

                if (!_pendingActiveModule.IsLoaded)
                    _pendingActiveModule.LoadContent();
                
                _activeModule = _pendingActiveModule;
                _pendingActiveModule = null;
            }
        }

        public void Clear()
        {
            if (_activeModule != null && _activeModule.IsLoaded)
                _activeModule.UnloadContent();

            _modules.Clear();
        }

        public GameModule ActiveModule { get { return _activeModule; } }
    }
}
