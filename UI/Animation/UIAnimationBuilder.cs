using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

namespace CipherPark.AngelJacket.Core.UI.Animation
{
    public class UIAnimationBuilder
    {
        public static CompositeAnimationController BuildAlertBoxAnimation(IUIRoot ui, AlertBox alertControl, RectangleF finalPanelRect, ColorContent backgroundContent, TextContent messageContent, ulong displayTime)
        {            
            RectangleF initialPanelRect = new RectangleF(0, finalPanelRect.Top, 0, finalPanelRect.Bottom);
            finalPanelRect.AlignRectangle(ref initialPanelRect, RectangleAlignment.Centered);
            alertControl.Position = initialPanelRect.Position();
            alertControl.Size = initialPanelRect.Size();           
            PanelAnimationController controlController = new PanelAnimationController();
            controlController.Target = alertControl;
            controlController.SetPositionAndSizeAtT(0, initialPanelRect);
            controlController.SetPositionAndSizeAtT(250, finalPanelRect);
            controlController.SetVisibleAtT(0, true);
            controlController.SetVisibleAtT(displayTime, true);
            ui.Controls.Add(alertControl);
            controlController.AnimationComplete += (object sender, EventArgs args) =>
            {
                ui.Controls.Remove(alertControl);
            };
            ColorContentAnimationController backgroundContentController = new ColorContentAnimationController();
            backgroundContentController.Target = backgroundContent;
            backgroundContentController.SetOpacityAtT(0, 0);
            backgroundContentController.SetOpacityAtT(250, .5f);
            UIContentAnimationController<TextContent> messageContentController = new UIContentAnimationController<TextContent>();
            messageContentController.Target = messageContent;
            messageContentController.SetOpacityAtT(0, 0);
            messageContentController.SetOpacityAtT(250, 1);          
            CompositeAnimationController alertBoxController = new CompositeAnimationController(new IAnimationController[] { controlController, backgroundContentController, messageContentController });
            return alertBoxController;
        }

        public static CompositeAnimationController BuildMenuAnimation(IUIRoot ui, Menu menu, RectangleF finalPanelRect, ulong animationTime)
        {
            CompositeAnimationController compController = new CompositeAnimationController();
            RectangleF initialRect = new RectangleF(finalPanelRect.Left, finalPanelRect.Bottom, finalPanelRect.Left, finalPanelRect.Bottom);
            menu.Position = initialRect.Position();
            menu.Size = initialRect.Size();
            MenuAnimationController menuController = new MenuAnimationController();
            menuController.SetPositionAndSizeAtT(0, initialRect);
            menuController.SetPositionAndSizeAtT(animationTime, finalPanelRect);
            DrawingSizeF offset = new DrawingSizeF();
            for (int i = 0; i < menu.Items.Count; i++)
            {
                //menuController.SetItemOffsetAtT(0, i, offset);
                //menuController.SetItemOffsetAtT(animationTime, i, offset);
                //menuController.SetItemOffsetAtT(animationTime + 250, i, DrawingSizeFExtension.Zero);
            }
            compController.Children.Add(menuController);
            return compController;
        }
    }
}
