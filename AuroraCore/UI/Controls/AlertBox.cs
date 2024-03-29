﻿using System;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.UI.Animation;
using CipherPark.Aurora.Core.Animation.Controllers;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.Extensions;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Controls
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
            childSplitter.Splitters.Add(new SplitterLayoutDivision(textContentCellId, 0, SplitterLayoutAnchorSide.One));
           
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
        protected override void OnDraw()
        {            
            base.OnDraw();
        }
       
        private static void ShowAlert(IUIRoot ui, TextContent messageContent, ImageContent iconContent, ColorContent backgroundContent, ulong displayTime)
        {
            if (ui == null || messageContent == null)
                throw new ArgumentNullException();

            AlertBox alertControl = new AlertBox(ui);
            alertControl.Visible = true;
            alertControl.Text = messageContent;
            alertControl.Text.PredefinedBlend = PredefinedBlend.Opacity;
            alertControl.Icon = iconContent;
            alertControl.Background = backgroundContent;
            alertControl.Background.PredefinedBlend = PredefinedBlend.Opacity;
            //alertControl.Background.Opacity = .5f;
            RectangleF screenRect = new RectangleF(0, 0, ui.ScreenSize.Width, ui.ScreenSize.Height);
            Size2F textMargin = new Size2F(5, 5);
            Size2F minMessageFit = messageContent.Font.MeasureString(messageContent.Text).Add(textMargin);
            Size2F minMessageBoxSize = new Size2F(100, 20);
            Size2F messageBoxSize = new Size2F(Math.Max(minMessageFit.Width, minMessageBoxSize.Width),
                                                           Math.Max(minMessageFit.Height, minMessageBoxSize.Height));
            RectangleF finalPanelRect = new RectangleF(0, 0, messageBoxSize.Width, messageBoxSize.Height);
            screenRect.AlignRectangle(ref finalPanelRect, RectangleAlignment.Centered);
            alertControl.Position = finalPanelRect.Position();
            alertControl.Size = finalPanelRect.Size();
            CompositeAnimationController controller = UIAnimationBuilder.BuildAlertBoxAnimation(ui, alertControl, backgroundContent, messageContent, displayTime);
            ui.Animations.Add(controller);
        }

        public static void ShowAlert(IUIRoot ui, string text, Texture2D icon, Color backgroundColor, ulong displayTime)
        {
            //throw new NotImplementedException();
            /*
            ShowAlert(ui, 
                      new TextContent(text, DefaultTheme.Instance.ControlFont, DefaultTheme.Instance.ControlFontColor),
                      (icon != null) ? new ImageContent(icon) : null,
                      (backgroundColor != Color.Transparent) ? new ColorContent(backgroundColor) : null,
                      displayTime);
            */
        }
    }

    [Flags]
    public enum AlertBoxStyle
    {
        Default
    }
}
