using System;
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
    public static class SceneContextHelper
    {
        public static SceneGraph GetActiveScene(this IGameApp game)
        {
            IContainerActiveScene container = game as IContainerActiveScene;
            if (container != null)
                return container.Scene;

            var moduleService = game.Services.GetService<ModuleService>();
            if (moduleService != null && moduleService.ActiveModule != null)
            {
                container = moduleService.ActiveModule as IContainerActiveScene;
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

        public static CameraSceneNode GetRenderingCamera(this IGameApp game)
        {
            IManageRenderingCamera manager = game as IManageRenderingCamera;

            if (manager != null)
            {
                return manager.GetRenderingCamera();
            }

            try
            {
                return GetActiveScene(game).CameraNode;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Active camera not accessible.", ex);
            }
        }

        public static CameraSceneNode GetActiveCamera(this IGameApp game)
        {
            IManageActiveCamera manager = game as IManageActiveCamera;

            if (manager != null)
            {
                return manager.GetActiveCamera();
            }

            try
            {
                return GetActiveScene(game).CameraNode;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Active camera not accessible.", ex);
            }
        }
    }

    public interface IManageRenderingCamera
    {
        CameraSceneNode GetRenderingCamera();
    }

    public interface IManageActiveCamera
    {
        CameraSceneNode GetActiveCamera();
    }

    public interface IContainerActiveScene
    {
        SceneGraph Scene { get; }
    }
}