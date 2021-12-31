using System;
using CipherPark.KillScript.Core.Terminal;
using CipherPark.KillScript.Core.World;
using CipherPark.KillScript.Core.World.Scene;
using CipherPark.KillScript.Core.UI.Components;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////


namespace CipherPark.KillScript.Core.Services
{
    public interface IActiveModuleContextService
    {
        /// <summary>
        /// Gets the active module context if one exits. If none exists, it returns null.
        /// </summary>
        ModuleContext Context { get; }
        /// <summary>
        /// Updates the service with the current active module context.
        /// </summary>
        void Update();
    }       
}

namespace CipherPark.KillScript.Core
{
    using CipherPark.KillScript.Core.Services;

    public static class ActiveModuleContextHelper
    {
        public static ModuleContext GetActiveModuleContext(this IGameApp game)
        {
            var container = game as IContainActiveModuleContext;
            if (container != null)           
                return container.GetActiveModuleContext();
            else
            {
                IActiveModuleContextService contextService = (IActiveModuleContextService)game.Services.GetService(typeof(IActiveModuleContextService));

                if (contextService == null)
                    throw new InvalidOperationException("Module context not available.");             

                return contextService.Context;
            }
        }
    }

    public interface IContainActiveModuleContext
    {
        ModuleContext GetActiveModuleContext();
    }

    public class ModuleContext
    {
        public ModuleContext(SceneGraph scene, WorldSimulator simulator, IUIRoot ui)
        {
            Scene = scene;
            Simulator = simulator;
            UI = ui;           
        }
        public SceneGraph Scene { get; }
        public WorldSimulator Simulator { get; }
        public IUIRoot UI { get; }        
    }
}