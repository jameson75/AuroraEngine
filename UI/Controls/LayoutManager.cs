using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public interface IControlLayoutManager
    {
        void UpdateLayout(LayoutUpdateReason reason);
    }

    public class ContainerControlLayoutManager : IControlLayoutManager
    {
        private UIControl _container = null;
        private Rectangle _cachedBounds = Rectangle.Empty;

        public ContainerControlLayoutManager(UIControl container)
        {
            _container = container;
            _container.SizeChanging += Container_SizeChanging;
        }

        private void Container_SizeChanging(object sender, EventArgs args)
        {
            _cachedBounds = _container.Bounds;
        }

        public void UpdateLayout(LayoutUpdateReason reason)
        {
            foreach (UIControl child in _container.Children)
            {
                float? newPositionX = null;
                float? newPositionY = null;
                int? newWidth = null;
                int? newHeight = null;

                switch(child.HorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        newPositionX = ((float)_container.Bounds.Width / 2.0f) - ((float)child.Bounds.Width / 2.0f);                       
                        break;
                    case HorizontalAlignment.Left:
                        //No need to do anything since child's x position is relative to Left side of container.
                        break;
                    case HorizontalAlignment.Right:
                        if (reason == LayoutUpdateReason.SizeChanged || reason == LayoutUpdateReason.MultipleAspectsChanged)
                        {
                            float xOffset = _container.Bounds.Width - _cachedBounds.Width;
                            newPositionX = child.Position.X + xOffset;
                        }
                        break;
                    case HorizontalAlignment.Stretch:
                        newPositionX = _container.Padding.X + child.Margin.X;
                        newWidth = _container.Bounds.Width - (int)(_container.Padding.X * 2);
                        break;
                }
                
                switch(child.VerticalAlignment)
                {
                    case VerticalAlignment.Center:
                         newPositionY = ((float)_container.Bounds.Height / 2.0f) - ((float)child.Bounds.Height / 2.0f);
                        break;
                    case VerticalAlignment.Top:
                        //No need to do anything since child's y position is relative to Top side of container.
                        break;
                    case VerticalAlignment.Bottom:
                        if (reason == LayoutUpdateReason.SizeChanged || reason == LayoutUpdateReason.MultipleAspectsChanged)
                        {
                            float yOffset = _container.Bounds.Height - _cachedBounds.Height;
                            newPositionY = child.Position.Y + yOffset;
                        }
                        break;
                    case VerticalAlignment.Stretch:
                        newPositionY = _container.Padding.Y + child.Margin.Y;                       
                        newHeight = _container.Bounds.Height - (int)(_container.Padding.Y * 2);
                        break;
                }

                if (newPositionX.HasValue || newPositionY.HasValue)
                {
                    float positionX = newPositionX.HasValue ? newPositionX.Value : child.Position.X;
                    float positionY = newPositionY.HasValue ? newPositionY.Value : child.Position.Y;
                    child.Position = new Vector2(positionX, positionY);
                }

                if (newWidth.HasValue || newHeight.HasValue)
                {
                    int width = newWidth.HasValue ? newWidth.Value : child.Bounds.Width;
                    int height = newHeight.HasValue ? newHeight.Value : child.Bounds.Height;
                    child.Size = new Vector2(width, height);
                }
            }
        }      
    }

    public enum LayoutUpdateReason
    {
        /// <summary>
        /// Indicates that the size of the updating control has changed.
        /// </summary>
        SizeChanged,
        /// <summary>
        /// Indicates that, while the size of the updating control hasn't changed, but the 
        /// client area (area reserved to display child controls) has changed.
        /// </summary>
        ClientAreaChanged,
        /// <summary>
        /// Indicates that the size of one or more children of the updating control has changed.
        /// </summary>
        ChildSizeChanged,
        /// <summary>
        /// Indicates that the number of child controls contained in the updating control has changed.
        /// </summary>
        ChildCountChanged,
        /// <summary>
        /// Indicates that more than a single reason the layout is changing.
        /// </summary>
        MultipleAspectsChanged
    }
}
