using CipherPark.Aurora.Core;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.World.Scene;
using SharpDX;
using System;

namespace Aurora.Core.Editor
{
    public interface IEditorGameApp : IGameApp
    {
        EditorMode EditorMode { get; set; }
        SceneGraph Scene { get; }
        EditorTransformPlane TransformPlane { get; set; }
        UITree UI { get; }

        event Action<object, NodeTransformedArgs> NodeTransformed;

        void ChangeViewportColor(Color newViewportColor);
        void ClearScene(bool resetCamera);
        void ResetCamera();
    }
}