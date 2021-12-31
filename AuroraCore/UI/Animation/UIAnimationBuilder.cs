using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.KillScript.Core.Animation;
using CipherPark.KillScript.Core.Animation.Controllers;
using CipherPark.KillScript.Core.UI.Controls;
using CipherPark.KillScript.Core.UI.Components;
using CipherPark.KillScript.Core.Utils;
using CipherPark.KillScript.Core.Utils.Toolkit;

namespace CipherPark.KillScript.Core.UI.Animation
{
    public class UIAnimationBuilder
    {
        public static CompositeAnimationController BuildAlertBoxAnimation(IUIRoot ui, AlertBox alertControl, ColorContent backgroundContent, TextContent messageContent, ulong displayTime)
        {
            RectangleF finalPanelRect = alertControl.Bounds;
            RectangleF initialPanelRect = new RectangleF(0, finalPanelRect.Top, 0, finalPanelRect.Height);
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
            controlController.SimulationComplete += (object sender, EventArgs args) =>
            {
                ui.Controls.Remove(alertControl);
            };
            ColorContentAnimationController backgroundContentController = new ColorContentAnimationController();
            backgroundContentController.Target = backgroundContent;
            backgroundContentController.SetBlendFactorAtT(0, new Color(0));
            backgroundContentController.SetBlendFactorAtT(250, new Color(.5f));
            UIContentAnimationController<TextContent> messageContentController = new UIContentAnimationController<TextContent>();
            messageContentController.Target = messageContent;
            messageContentController.SetBlendFactorAtT(0, new Color(0));
            messageContentController.SetBlendFactorAtT(250, new Color(1));          
            CompositeAnimationController alertBoxController = new CompositeAnimationController(new ISimulatorController[] { controlController, backgroundContentController, messageContentController });
            return alertBoxController;
        }

        public static CompositeAnimationController BuildMenuAnimation(Menu menu, ulong animationTime)
        {
            CompositeAnimationController compositeController = new CompositeAnimationController();       
            RectangleF finalMenuRect = menu.Bounds; 
            RectangleF initialPanelRect = new RectangleF(finalMenuRect.Left, finalMenuRect.Bottom, finalMenuRect.Width, finalMenuRect.Height);
            menu.SuspendLayout = true;
            menu.Position = initialPanelRect.Position();
            menu.Size = initialPanelRect.Size();
            MenuAnimationController menuController = new MenuAnimationController();
            menuController.SetPositionAndSizeAtT(0, initialPanelRect);
            menuController.SetPositionAndSizeAtT(animationTime, finalMenuRect);
            menuController.Target = menu;
            menuController.SimulationComplete+= (object sender, EventArgs args) =>
                {
                    menu.SuspendLayout = false;
                };            
            for(int i = 0; i < menu.Items.Count; i++)
            {
                MenuItemAnimationController itemController = new MenuItemAnimationController();
                Vector2 finalPosition = menu.Items[i].Position;
                Vector2 initialPosition = new Vector2(-menu.Items[i].Size.Width, menu.Items[i].Position.Y);
                itemController.SetPositionAtT(0, initialPosition);
                itemController.SetPositionAtT(animationTime, initialPosition);
                itemController.SetPositionAtT(animationTime + 250, finalPosition);
                itemController.Target = (MenuItem)menu.Items[i];
                compositeController.Children.Add(itemController);
            }            
            compositeController.Children.Add(menuController);
            return compositeController;
        }

        public static ISimulatorController CreateBlendFactorFlicker(UIContent uiContent)
        {           
            Random _rand = new Random();
            return new Core.Animation.Controllers.AnonymousActionController()
            {
                FireImmediately = true,
                Frequency = 50,
                Action = (c) =>
                    {
                        float r = (float)_rand.NextDouble(0.7, 1.0);
                        uiContent.BlendFactor = new Color4(r);
                    }
            };
        }
    }
}
