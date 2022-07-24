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
            var gameApp = (EditorGameApp)GetGameApp(control);
            var navigatorControl = (NavigatorControl)control;
            UpdateNavigatorMode(gameApp, navigatorControl);
            UpdateScenePickerMode(gameApp, navigatorControl);
        }

        private void UpdateNavigatorMode(EditorGameApp gameApp, NavigatorControl navigatorControl)
        {
            if (HasEditorModeChanged(gameApp))
            {
                var activatePickingMode = false;
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
                        activatePickingMode = true;
                        break;
                }
                navigatorControl.NavigationMode = navigationMode;
                navigatorControl.IsInPickingMode = activatePickingMode;
            }
        }

        private bool HasEditorModeChanged(EditorGameApp gameApp)
        {
            var hasChanged = lastEditorMode != gameApp.EditorMode;
            lastEditorMode = gameApp.EditorMode;
            return hasChanged;
        }

        private void UpdateScenePickerMode(EditorGameApp gameApp, NavigatorControl navigatorControl)
        {
            if (HasTransformPlaneChange(gameApp))
            {
                switch (gameApp.TransformPlane)
                {
                    case EditorTransformPlane.XZ:
                        navigatorControl.TransformPlane = TranslationPlane.XZ;
                        break;
                    case EditorTransformPlane.XY:
                        navigatorControl.TransformPlane = TranslationPlane.XY;
                        break; 
                }               
            }
        }

        private bool HasTransformPlaneChange(EditorGameApp gameApp)
        {
            var hasChanged = lastTransformPlane != gameApp.TransformPlane;
            lastTransformPlane = gameApp.TransformPlane;
            return hasChanged;
        }
    }
}
