﻿using System;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Services;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core
{ 
    public static class ActiveSceneHelper
    {
        public static SceneGraph GetActiveScene(this IGameApp game)
        {
            IContainActiveScene container = game as IContainActiveScene;
            if (container != null)
                return container.Scene;

            var moduleService = game.Services.GetService<ModuleService>();
            if (moduleService != null && moduleService.ActiveModule != null)
            {
                container = moduleService.ActiveModule as IContainActiveScene;
                if (container != null)
                    return container.Scene;
            }

            var sceneService = game.Services.GetService<ActiveSceneService>();
            if (sceneService?.Scene != null)
            {
                return sceneService.Scene;
            }

            throw new InvalidOperationException("Active scene not accessible.");
        }
    }

    public interface IContainActiveScene
    {
        SceneGraph Scene { get; }
    }
}