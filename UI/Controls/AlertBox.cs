using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.UI.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class AlertBox : Panel
    {
        private AlertBox(IUIRoot ui) : base(ui)
        {}

        public static void ShowAlert(IUIRoot ui, TextStyle textStyle, AlertBoxStyle style, ulong displayTime)
        {
            AlertBox alertControl = new AlertBox(ui);
            alertControl.Visible = false;
            RectangleF screenRect = new RectangleF(0, 0, ui.ScreenSize.Width, ui.ScreenSize.Height);
            DrawingSizeF textMargin = new DrawingSizeF(5, 5);
            DrawingSizeF minMessageFit = textStyle.Font.MeasureString(textStyle.Text).Add(textMargin);
            DrawingSizeF minMessageBoxSize = new DrawingSizeF(100, 20);
            DrawingSizeF messageBoxSize = new DrawingSizeF(Math.Max(minMessageFit.Width, minMessageBoxSize.Width),
                                                           Math.Max(minMessageFit.Height, minMessageBoxSize.Height));
            RectangleF finalPanelRect = new RectangleF(0, 0, messageBoxSize.Width, messageBoxSize.Height);
            screenRect.AlignRectangle(ref finalPanelRect, RectangleAlignment.Centered);
            RectangleF initialPanelRect = new RectangleF(0, finalPanelRect.Top, 0, finalPanelRect.Bottom);
            finalPanelRect.AlignRectangle(ref initialPanelRect, RectangleAlignment.Centered);
            alertControl.Position = initialPanelRect.Position();
            alertControl.Size = initialPanelRect.Size();
            PanelAnimationController controller = new PanelAnimationController();
            controller.Target = (Panel)alertControl;
            controller.SetPositionAndSizeAtT(0, initialPanelRect);
            controller.SetPositionAndSizeAtT(displayTime, finalPanelRect);
            ui.Controls.Add(alertControl);
            ui.Animations.Add(controller);
            controller.AnimationComplete += (object sender, EventArgs args) =>
                {
                    ui.Controls.Remove(alertControl);
                };
        }
    }

    [Flags]
    public enum AlertBoxStyle
    {
        Default
    }
}
