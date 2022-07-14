using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.UI.Controls;

namespace Aurora.Sample.Editor.Scene.UI.Behavior
{
    public class NavigatorControlBehavior : UIControlBehavior
    {
        private EditorMode? lastEditorMode;

        public override void Update(UIControl control)
        {
            base.Update(control);
            var gameApp = (EditorGameApp)GetGameApp(control);
            var navigatorControl = (NavigatorControl)control;
            UpdateNavigatorMode(gameApp, navigatorControl);
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
    }
}
