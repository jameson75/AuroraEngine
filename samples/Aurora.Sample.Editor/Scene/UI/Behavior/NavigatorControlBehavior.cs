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
                switch (gameApp.EditorMode)
                {
                    case EditorMode.RotateCamera:
                        navigatorControl.NavigationMode = MouseNavigatorService.NavigationMode.PlatformRotate;
                        break;
                    case EditorMode.TraverseCamera:
                        navigatorControl.NavigationMode = MouseNavigatorService.NavigationMode.PlaformTraverse;
                        break;
                    case EditorMode.PanCamera:
                        navigatorControl.NavigationMode = MouseNavigatorService.NavigationMode.Pan;
                        break;
                    default:
                        navigatorControl.NavigationMode = MouseNavigatorService.NavigationMode.None;
                        break;
                }
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
