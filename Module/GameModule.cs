using System;
using System.Collections.Generic;
using CipherPark.AngelJacket;
using CipherPark.AngelJacket.Core.Services;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Module
{
    public abstract class GameModule
    {
        private IGameApp _game = null;

        public IGameApp Game { get { return _game; } }

        public GameModule(IGameApp game) { this._game = game; }

        public string Name { get; set; }

        public abstract void Initialize();       

        public abstract void LoadContent();

        public abstract void Update(long gameTime);
        
        public abstract void Draw(long gameTime);      

        public abstract void UnloadContent();
        
        public abstract void Uninitialize();     

        public abstract bool IsLoaded { get; }

        public abstract bool IsInitialized { get; }

        //public event EventHandler Exit;

        //public event ModuleSwitchEventHandler Switch;

        protected void SignalExitModule()
        {
            IModuleService moduleService = (IModuleService)Game.Services.GetService(typeof(IModuleService));
            moduleService.ExitOnUpdate();
        }

        protected void SignalSwitchModule(string key)
        {
            IModuleService moduleService = (IModuleService)Game.Services.GetService(typeof(IModuleService));
            moduleService.ActivateOnUpdate(key);
        }
    }
}
