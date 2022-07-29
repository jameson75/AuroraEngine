using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.UI.Controls;

namespace Aurora.Sample.Editor.Scene.UI.Behavior
{
    public class NavigatorControlBehavior : UIControlBehavior
    {
        private EditorMode? lastEditorMode;
        private EditorTransformPlane? lastTransformPlane;

        public override void Update(UIControl control)
        {
            base.Update(control);
            var gameApp = (IEditorGameApp)GetGameApp(control);            
            UpdateSceneManagemetServices(gameApp);            
        }

        private void UpdateSceneManagemetServices(IEditorGameApp gameApp)
        {
            var mouseNavigatorService = gameApp.Services.GetService<MouseNavigatorService>();
            var sceneModifierService = gameApp.Services.GetService<SceneModifierService>();

            if (HasEditorModeChanged(gameApp))
            {
                var activateScenePicker = false;
                var navigationMode = MouseNavigatorService.NavigationMode.None;
                switch (gameApp.EditorMode)
                {
                    case EditorMode.RotateCamera:
                        navigationMode = MouseNavigatorService.NavigationMode.PlatformRotate;
                        break;
                    case EditorMode.TraverseCamera:
                        navigationMode = MouseNavigatorService.NavigationMode.PlaformTraverse;
                        break;
                    case EditorMode.PanCamera:
                        navigationMode = MouseNavigatorService.NavigationMode.Pan;
                        break;
                    case EditorMode.SelectSceneObject:
                        activateScenePicker = true;
                        break;
                }
                mouseNavigatorService.Mode = navigationMode;
                sceneModifierService.IsActive = activateScenePicker;                
            }

            if (HasTransformPlaneChange(gameApp))
            {
                switch (gameApp.TransformPlane)
                {
                    case EditorTransformPlane.XZ:
                        sceneModifierService.TranslationPlane = TranslationPlane.XZ;
                        break;
                    case EditorTransformPlane.XY:
                        sceneModifierService.TranslationPlane = TranslationPlane.XY;
                        break;
                }
            }
        }

        private bool HasEditorModeChanged(IEditorGameApp gameApp)
        {
            var hasChanged = lastEditorMode != gameApp.EditorMode;
            lastEditorMode = gameApp.EditorMode;
            return hasChanged;
        }

        private bool HasTransformPlaneChange(IEditorGameApp gameApp)
        {
            var hasChanged = lastTransformPlane != gameApp.TransformPlane;
            lastTransformPlane = gameApp.TransformPlane;
            return hasChanged;
        }
    }
}
