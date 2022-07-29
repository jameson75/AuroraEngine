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
            var gameApp = (IEditorGameApp)GetGameApp(control);            
            UpdateVisibility(gameApp, control);
        }

        private void UpdateVisibility(IEditorGameApp gameApp, UIControl control)
        {
            if (HasEditorModeChanged(gameApp))
            {
                control.Visible = gameApp.EditorMode == EditorMode.RotateCamera;                      
            }
        }

        private bool HasEditorModeChanged(IEditorGameApp gameApp)
        {
            var hasChanged = lastEditorMode != gameApp.EditorMode;
            lastEditorMode = gameApp.EditorMode;
            return hasChanged;
        }
    }
}
