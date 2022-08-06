using Aurora.Core.Editor;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.UI.Controls;
using System;

namespace Aurora.Sample.Editor.Scene.UI.Behavior
{
    public class EditorModeLabelBehavior : UIControlBehavior
    {
        private EditorMode? lastEditorMode;

        public override void Update(UIControl control)
        {
            base.Update(control);
            var gameApp = (IEditorGameApp)GetGameApp(control);
            var contentControl = (ContentControl)control;
            UpdateDisplayText(gameApp, contentControl);
        }

        private void UpdateDisplayText(IEditorGameApp gameApp, ContentControl contentControl)
        {
            if (HasEditorModeChanged(gameApp))
            {
                switch(gameApp.EditorMode)
                {
                    case EditorMode.RotateCamera:
                        contentControl.SetText("Rotate Camera");
                        break;
                    case EditorMode.TraverseCamera:
                        contentControl.SetText("Traverse Camera");
                        break;
                    case EditorMode.PanCamera:
                        contentControl.SetText("Pan Camera");
                        break;
                    case EditorMode.SelectSceneObject:
                        contentControl.SetText("Select Scene Object");
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected editor mode {gameApp.EditorMode}");
                }
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
