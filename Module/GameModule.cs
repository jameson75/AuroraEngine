using System;
using System.Collections.Generic;
using CipherPark.AngelJacket;
using CipherPark.AngelJacket.Core.Services;

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

        protected virtual void SignalExitModule()
        {
            IModuleService moduleService = (IModuleService)Game.Services.GetService(typeof(IModuleService));
            moduleService.ExitOnUpdate();
        }

        protected virtual void SignalSwitchModule(string key)
        {
            IModuleService moduleService = (IModuleService)Game.Services.GetService(typeof(IModuleService));
            moduleService.ActivateOnUpdate(key);
        }
    }
}
