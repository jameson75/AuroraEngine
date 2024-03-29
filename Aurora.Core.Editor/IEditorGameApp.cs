﻿using CipherPark.Aurora.Core;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Scene;
using SharpDX;

namespace Aurora.Core.Editor
{
    public interface IEditorGameApp : IGameApp
    {
        EditorMode EditorMode { get; set; }
        SceneGraph Scene { get; }
        WorldSimulator Simulator { get; }
        EditorTransformMode EditorTransformMode { get; set; }
        UITree UI { get; }

        void ChangeViewportColor(Color newViewportColor);
        void ClearScene(bool resetCamera);
        void ResetCamera();
        Vector3 GetDropLocation(Point point);
    }
}