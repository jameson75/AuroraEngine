using Aurora.Core.Editor;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.UI.Controls;
using System;

namespace Aurora.Sample.Editor.Scene.UI.Behavior
{
    public class EditorModeLabelBehavior : UIControlBehavior
    {
        private EditorMode? lastEditorMode;
        private EditorTransformMode? lastTransformMode;

        public override void Update(UIControl control)
        {
            base.Update(control);
            var gameApp = (IEditorGameApp)GetGameApp(control);
            var contentControl = (ContentControl)control;
            UpdateDisplayText(gameApp, contentControl);
        }

        private void UpdateDisplayText(IEditorGameApp gameApp, ContentControl contentControl)
        {
            if (HasEditorStateChanged(gameApp))
            {
                switch(gameApp.EditorMode)
                {
                    case EditorMode.RotateCamera:
                        contentControl.SetText("Rotate Camera");
                        break;
                    case EditorMode.TraverseCamera:
                        contentControl.SetText("Traverse Camera");
                        break;                 
                    case EditorMode.SelectSceneObject:
                        string caption = "Select Scene Object";
                        switch (gameApp.EditorTransformMode)
                        {
                            case EditorTransformMode.ViewSpaceTranslateY:
                                contentControl.SetText($"{caption} (V-Y)");
                                break;
                            case EditorTransformMode.ViewSpaceTranslateXZ:
                                contentControl.SetText($"{caption} (V-XZ)");
                                break;
                            case EditorTransformMode.LocalSpaceRotateX:
                                contentControl.SetText($"{caption} (R-X)");
                                break;
                            case EditorTransformMode.LocalSpaceRotateY:
                                contentControl.SetText($"{caption} (R-Y)");
                                break;
                            case EditorTransformMode.LocalSpaceRotateZ:
                                contentControl.SetText($"{caption} (R-Z)");
                                break;
                            case EditorTransformMode.ParentSpaceRevolveX:
                                contentControl.SetText($"{caption} (O-X)");
                                break;
                            case EditorTransformMode.ParentSpaceRevolveY:
                                contentControl.SetText($"{caption} (O-Y)");
                                break;
                            case EditorTransformMode.ParentSpaceRevolveZ:
                                contentControl.SetText($"{caption} (O-Z)");
                                break;
                            case EditorTransformMode.OrbitDistanceTranslate:
                                contentControl.SetText($"{caption} (OD)");
                                break;
                            default:
                                contentControl.SetText(caption);
                                break;
                        }
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected editor mode {gameApp.EditorMode}");
                }
            }
        }

        private bool HasEditorStateChanged(IEditorGameApp gameApp)
        {
            var hasChanged = lastEditorMode != gameApp.EditorMode ||
                             lastTransformMode != gameApp.EditorTransformMode;
            lastEditorMode = gameApp.EditorMode;
            lastTransformMode = gameApp.EditorTransformMode;
            return hasChanged;
        }
    }
}
