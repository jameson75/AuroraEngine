using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.UI.Animation;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

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
        private SplitterPanel childSplitter = null;
        private ContentControl childBackgroundContent = null;
        private ContentControl childIconContent = null;
        private ContentControl childTextContent = null;

        private AlertBox(IUIRoot ui) : base(ui)
        {
            childBackgroundContent = new ContentControl(ui);
            childBackgroundContent.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            childBackgroundContent.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;

            childIconContent = new ContentControl(ui);
            childIconContent.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            childIconContent.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;

            childTextContent = new ContentControl(ui);
            childTextContent.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            childTextContent.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;

            childSplitter = new SplitterPanel(ui);
            childSplitter.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;
            childSplitter.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            Guid textContentCellId = Guid.NewGuid();
            childSplitter.Splitters.Add(new SplitterLayoutDivision(textContentCellId, 0, SplitterLayoutFixedSide.One));
           
            childTextContent.LayoutId = textContentCellId;
            childSplitter.Children.Add(childIconContent);
            childSplitter.Children.Add(childTextContent);
            //childBackgroundContent.Children.Add(childSplitter);
            this.Children.Add(childBackgroundContent);
            this.Children.Add(childSplitter);
        }

        public ImageContent Icon
        {
            get { return (ImageContent)childIconContent.Content; }
            set 
            {                
                childIconContent.Content = value;
                if (value == null && childSplitter.Splitters[0].Distance != 0)
                    childSplitter.Splitters[0].Distance = 0;
                else if (value != null && childSplitter.Splitters[0].Distance < 32)
                    childSplitter.Splitters[0].Distance = 32;
            }
        }

        public TextContent Text
        {
            get { return (TextContent)childTextContent.Content; }
            set { childTextContent.Content = value; }
        }

        public UIContent Background
        {
            get { return childBackgroundContent.Content; }
            set { childBackgroundContent.Content = value; }
        }

        //TODO: Get rid of this override - only used for testing.
        protected override void OnDraw(long gameTime)
        {            
            base.OnDraw(gameTime);
        }
       
        public static void ShowAlert(IUIRoot ui, TextContent messageContent, ImageContent iconContent, ColorContent backgroundContent, ulong displayTime)
        {
            if (ui == null || messageContent == null)
                throw new ArgumentNullException();

            AlertBox alertControl = new AlertBox(ui);
            alertControl.Visible = true;
            alertControl.Text = messageContent;
            alertControl.Icon = iconContent;
            alertControl.Background = backgroundContent;
            //alertControl.Background.Opacity = .5f;
            RectangleF screenRect = new RectangleF(0, 0, ui.ScreenSize.Width, ui.ScreenSize.Height);
            DrawingSizeF textMargin = new DrawingSizeF(5, 5);
            DrawingSizeF minMessageFit = messageContent.Font.MeasureString(messageContent.Text).Add(textMargin);
            DrawingSizeF minMessageBoxSize = new DrawingSizeF(100, 20);
            DrawingSizeF messageBoxSize = new DrawingSizeF(Math.Max(minMessageFit.Width, minMessageBoxSize.Width),
                                                           Math.Max(minMessageFit.Height, minMessageBoxSize.Height));
            RectangleF finalPanelRect = new RectangleF(0, 0, messageBoxSize.Width, messageBoxSize.Height);
            screenRect.AlignRectangle(ref finalPanelRect, RectangleAlignment.Centered);
            CompositeAnimationController controller = UIAnimationBuilder.BuildAlertBoxAnimation(ui, alertControl, finalPanelRect, backgroundContent, messageContent, displayTime);
            ui.Animations.Add(controller);
        }

        public static void ShowAlert(IUIRoot ui, string text, Texture2D icon, Color backgroundColor, ulong displayTime)
        {
            ShowAlert(ui, 
                      new TextContent(text, DefaultTheme.Instance.ControlFont, DefaultTheme.Instance.ControlFontColor),
                      (icon != null) ? new ImageContent(icon) : null,
                      (backgroundColor != Color.Transparent) ? new ColorContent(backgroundColor) : null,
                      displayTime);
        }
    }

    [Flags]
    public enum AlertBoxStyle
    {
        Default
    }
}
