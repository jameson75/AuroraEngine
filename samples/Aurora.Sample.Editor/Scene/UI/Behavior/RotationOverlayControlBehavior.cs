using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.UI.Controls;
using System;

namespace Aurora.Sample.Editor.Scene.UI.Behavior
{
    public class RotationOverlayControlBehavior : UIControlBehavior
    {
        private EditorMode? lastEditorMode;

        public override void Update(UIControl control)
        {
            base.Update(control);
            var gameApp = (EditorGameApp)GetGameApp(control);            
            UpdateVisibility(gameApp, control);
        }

        private void UpdateVisibility(EditorGameApp gameApp, UIControl control)
        {
            if (HasEditorModeChanged(gameApp))
            {
                control.Visible = gameApp.EditorMode == EditorMode.RotateCamera;                      
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
