using CipherPark.Aurora.Core.UI.Controls;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core;
using Aurora.Core.Editor.Util;

namespace Aurora.Sample.Editor.Scene.UI.Behavior
{
    public class CoordinatesLabelBehavior : UIControlBehavior
    {
        public override void Update(UIControl control)
        {
            var gameApp = GetGameApp(control);
            IInputService inputService = gameApp.Services.GetService<IInputService>();
            InputState inputState = inputService.GetInputState();

            if (inputService.IsMouseInActiveViewport(inputState) && gameApp.IsViewportWindowActive)
            {
                var mouseLocation = inputState.GetMouseLocation();
                var cameraNode = gameApp.GetActiveCamera();
                var camera = cameraNode.Camera;
                var pickInfo = ScenePicker.PickNodes(
                    gameApp,
                    mouseLocation.X,
                    mouseLocation.Y,
                    n => n.GameObject.IsReferenceGridObject())
                    .GetClosest(camera.Location);

                //NOTE: We expect the control to be a content control with text content.
                var controlContent = (TextContent)((ContentControl)control).Content;

                if (pickInfo != null)
                {
                    var p = pickInfo.IntersectionPoint;
                    string caption = $"X:{p.X:F2} Y:{p.Y:F2} Z:{p.Z:F2}";
                    controlContent.Text = caption;
                }

                else if (controlContent.Text != null)
                    controlContent.Text = null;
            }

            base.Update(control);
        }
    }   
}
