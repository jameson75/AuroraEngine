using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.World;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.UI.Components;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////


namespace CipherPark.AngelJacket.Core.Services
{
    public interface IGameContextService
    {
        /// <summary>
        /// Gets the current game context if one exits. If none exists, it returns null.
        /// </summary>
        GameContext Context { get; }
        /// <summary>
        /// Updates the service with the current game context.
        /// </summary>
        void Update();
    }       
}

namespace CipherPark.AngelJacket.Core
{
    using CipherPark.AngelJacket.Core.Services;

    public static class GameContextServiceHelper
    {
        public static GameContext GetGameContext(this IGameApp game)
        {
            var container = game as IContainGameContext;
            if (container != null)           
                return container.GetGameContext();
            else
            {
                IGameContextService contextService = (IGameContextService)game.Services.GetService(typeof(IGameContextService));

                if (contextService == null)
                    throw new InvalidOperationException("Game context not available.");             

                return contextService.Context;
            }
        }
    }

    public interface IContainGameContext
    {
        GameContext GetGameContext();
    }

    public class GameContext
    {
        public GameContext(SceneGraph scene, WorldSimulator simulator, IUIRoot ui)
        {
            Scene = scene;
            Simulator = simulator;
            UI = ui;
        }
        public SceneGraph Scene { get; private set; }
        public WorldSimulator Simulator { get; private set; }
        public IUIRoot UI { get; private set; }
        public Camera CurrentCamera { get { return Scene?.CameraNode.Camera; } }
    }
}