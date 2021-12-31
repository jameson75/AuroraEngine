using System;
using System.Collections.Generic;
using CipherPark.Aurora;
using CipherPark.Aurora.Core.Services;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Module
{
    public abstract class GameModule
    {
        private IGameApp _game = null;

        public IGameApp Game { get { return _game; } }

        public GameModule() { }

        public string Name { get; set; }

        public void Initialize(IGameApp gameApp)
        {
            _game = gameApp;
            OnInitialize();
            IsInitialized = true;
        }

        public void LoadContent()
        {
            OnLoadContent();
            IsLoaded = true;
        }

        public void Update(GameTime gameTime)
        {
            OnUpdate(gameTime);
        }

        public void Draw()
        {
            OnDraw();
        }

        public void UnloadContent()
        {
            OnUnloadContent();
            IsLoaded = false;
        }

        public void Uninitialize()
        {
            OnUninitialize();
            IsInitialized = false;
        }

        #region Handlers

        protected abstract void OnInitialize();

        protected abstract void OnLoadContent();

        protected abstract void OnUpdate(GameTime gameTime);

        protected abstract void OnDraw();

        protected abstract void OnUnloadContent();

        protected abstract void OnUninitialize();

        #endregion

        public bool IsLoaded { get; private set; }

        public bool IsInitialized { get; private set; }
      
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