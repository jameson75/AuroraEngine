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
                    child.Size = new DrawingSizeF(width, height);
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

    public class DivLayoutManager : IControlLayoutManager
    {
        private LayoutDivCollection _divs = null;
        private UIControl _container = null;
        private Rectangle _cachedBounds = Rectangle.Empty;

        public DivLayoutManager(UIControl container)
        {
            _container = container;
            _container.SizeChanging += Container_SizeChanging;
            _divs = new LayoutDivCollection();
        }

        public LayoutDivCollection Divs { get { return _divs; } }

        private void Container_SizeChanging(object sender, EventArgs e)
        {
            _cachedBounds = _container.Bounds;
        }

        #region IControlLayoutManager Members

        public void UpdateLayout(LayoutUpdateReason reason)
        {
            int currentRow = 0;
            int currentColumn = 0;
            float currentWidthOffset = 0;
            float currentHeightOffset = 0;
            float currentRowHeight = 0;

            for (int i = 0; i < _divs.Count; i++)
            {
                DrawingSizeF divSize = CalculateDivSizeInPixels(i, currentWidthOffset, currentHeightOffset);
                if (currentColumn != 0 && currentWidthOffset + divSize.Width > _container.Bounds.Width)
                {
                    currentRow++;
                    currentColumn = 0;
                    currentWidthOffset = 0;
                    currentHeightOffset += currentRowHeight;
                    currentRowHeight = 0;
                }
                RectangleF currentDivBounds = new RectangleF(currentWidthOffset, currentHeightOffset, divSize.Width, divSize.Height);
                currentWidthOffset += currentDivBounds.Width;
                currentRowHeight = Math.Max(currentRowHeight, currentDivBounds.Height);
                ApplyDivLayoutToContainerChildren(_divs[i], currentDivBounds, reason);
                currentColumn++;
            }
        }

        private void ApplyDivLayoutToContainerChildren(LayoutDiv div, RectangleF divBounds, LayoutUpdateReason reason)
        {
            float? newPositionX = null;
                float? newPositionY = null;
                int? newWidth = null;
                int? newHeight = null;
                foreach (UIControl child in _container.Children)
                {
                    if (child.DivContainerId == div.Id)
                    {
                        switch (child.HorizontalAlignment)
                        {
                            case HorizontalAlignment.Center:
                                newPositionX = ((float)divBounds.Width / 2.0f) - ((float)child.Bounds.Width / 2.0f);
                                break;
                            case HorizontalAlignment.Left:
                                //No need to do anything since child's x position is relative to Left side of container.
                                break;
                            case HorizontalAlignment.Right:
                                if (reason == LayoutUpdateReason.SizeChanged || reason == LayoutUpdateReason.MultipleAspectsChanged)
                                {
                                    float xOffset = divBounds.Width - _cachedBounds.Width;
                                    newPositionX = child.Position.X + xOffset;
                                }
                                break;
                            case HorizontalAlignment.Stretch:
                                newPositionX = div.Padding.X + child.Margin.X;
                                newWidth = (int)divBounds.Width - (int)(div.Padding.X * 2);
                                break;
                        }

                        switch (child.VerticalAlignment)
                        {
                            case VerticalAlignment.Center:
                                newPositionY = ((float)divBounds.Height / 2.0f) - ((float)child.Bounds.Height / 2.0f);
                                break;
                            case VerticalAlignment.Top:
                                //No need to do anything since child's y position is relative to Top side of container.
                                break;
                            case VerticalAlignment.Bottom:
                                if (reason == LayoutUpdateReason.SizeChanged || reason == LayoutUpdateReason.MultipleAspectsChanged)
                                {
                                    float yOffset = divBounds.Height - _cachedBounds.Height;
                                    newPositionY = child.Position.Y + yOffset;
                                }
                                break;
                            case VerticalAlignment.Stretch:
                                newPositionY = div.Padding.Y + child.Margin.Y;
                                newHeight = (int)divBounds.Height - (int)(div.Padding.Y * 2);
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
                            child.Size = new DrawingSizeF(width, height);
                        }
                    }
                }
        }

        private DrawingSizeF CalculateDivSizeInPixels(int divIndex, float xDivOffset, float yDivOffset)
        {
            LayoutDiv div = _divs[divIndex];
            float divWidth = 0.0f;
            float divHeight = 0.0f;
            if (div.WidthUnits == LayoutDivUnits.Span)
            {
                if (xDivOffset == _divs.Count - 1)
                    divWidth = _container.Bounds.Width - xDivOffset;
                else
                {

                } 
            }
            else
                divWidth = ToPixels(_container.Bounds.Width, div.Width, div.WidthUnits);
            if (div.HeightUnits == LayoutDivUnits.Span)
            {
                if (xDivOffset == _divs.Count - 1)
                    divHeight = _container.Bounds.Height - yDivOffset;
                else
                {

                }
            }
            else
                divHeight = ToPixels(_container.Bounds.Height, div.Height, div.HeightUnits);
            return new DrawingSizeF(divWidth, divHeight);
        }

        private static float ToPixels(int containerLength, float divLength, LayoutDivUnits units)
        {
            switch( units )
            {
                case LayoutDivUnits.Pixels:
                    return divLength;
                case LayoutDivUnits.Percentage:
                    return (float)containerLength * divLength / 100.0f;
                default:
                    throw new ArgumentException("Unrecognized units specified", "units");
            }
        }

        #endregion
    }

    public class LayoutDiv
    {
        public Guid Id { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public LayoutDivUnits HeightUnits { get; set; }
        public LayoutDivUnits WidthUnits { get; set; }
        public DrawingPoint Padding { get; set; }
        public DrawingPoint Margin { get; set; }

        public LayoutDiv(Guid divId, int width, int height)
        {
            Id = divId;
            Width = width;
            Height = height;
            HeightUnits = LayoutDivUnits.Pixels;
            WidthUnits = LayoutDivUnits.Pixels;
        }

        public LayoutDiv(Guid divId, float width, LayoutDivUnits widthUnits, float height, LayoutDivUnits heightUnits)
        {
            Id = divId;
            Width = width;
            Height = height;
            HeightUnits = heightUnits;
            WidthUnits = widthUnits;
        }

        public void SetWidthAndUnits(float width, LayoutDivUnits units)
        {
            Width = width;
            WidthUnits = units;
        }

        public void SetHeightAndUnits(float height, LayoutDivUnits units)
        {
            Height = height;
            HeightUnits = units;
        }       

        public bool IsWidthFixed { get { return WidthUnits == LayoutDivUnits.Pixels; } }

        public bool IsHeightFixed { get { return HeightUnits == LayoutDivUnits.Pixels; } }
    }

    public enum LayoutDivUnits
    {
        Pixels = 0,
        Percentage,
        Span
    }

    public class LayoutDivCollection : List<LayoutDiv>
    { }
}
