﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using CipherPark.AngelJacket.Core.Utils;

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
                        if (reason == LayoutUpdateReason.SizeChanged)
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
                        if (reason == LayoutUpdateReason.SizeChanged)
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
        ChildCountChanged     
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
            _UpdateLayout(_divs, reason);         
        }

        private void _UpdateLayout(LayoutDivCollection divs, LayoutUpdateReason reason)
        {
            RectangleF[] divBoundsArray = DivLayoutManager.CalculateDivBounds(_container.Bounds.GetSize(), _divs);
            for (int i = 0; i < divs.Count; i++)
            {
                ApplyDivContainership(divs[i], divBoundsArray[i], reason);
                if( divs[i].Children.Count > 0 )
                    _UpdateLayout(divs[i].Children, reason);
            }
        }

        private void ApplyDivContainership(LayoutDiv div, RectangleF divBounds, LayoutUpdateReason reason)
        {
            float newPositionX = 0;
            float newPositionY = 0;
            int newWidth = 0;
            int newHeight = 0;
            foreach (UIControl child in _container.Children)
            {
                if (child.DivContainerId == div.Id)
                {                   
                    newPositionX = divBounds.Left + div.Padding.X + child.Margin.X;
                    newWidth = (int)divBounds.Width - (int)(div.Padding.X * 2);                  
                    newPositionY = divBounds.Top + div.Padding.Y + child.Margin.Y;
                    newHeight = (int)divBounds.Height - (int)(div.Padding.Y * 2);                    
                    child.Position = new Vector2((float)newPositionX, (float)newPositionY);      
                    child.Size = new DrawingSizeF((float)newWidth, (float)newHeight);                   
                }
            }
        }

        public static RectangleF[] CalculateDivBounds(DrawingSize containerSize, LayoutDivCollection divs)
        {
            int currentRow = 0;
            int currentColumn = 0;
            float currentDivOriginX = 0;
            float currentDivOriginY = 0;
            float currentRowOffsetY = 0;
            RectangleF[] divBoundsArray = new RectangleF[divs.Count];

            for (int i = 0; i < divs.Count; i++)
            {
                if (currentColumn != 0 && currentDivOriginX >= containerSize.Width)
                {
                    currentRow++;
                    currentColumn = 0;
                    currentDivOriginX = 0;
                    currentDivOriginY += currentRowOffsetY;
                    currentRowOffsetY = 0;
                }
                
                DrawingSizeF divSize = CalculateDivSizeInPixels(containerSize, divs, i, currentDivOriginX, currentDivOriginY);
                RectangleF currentDivBounds = RectangleFExtension.CreateLTWH(currentDivOriginX, currentDivOriginY, divSize.Width, divSize.Height);
                currentDivOriginX += currentDivBounds.Width;
                currentRowOffsetY = Math.Max(currentRowOffsetY, currentDivBounds.Height);
                currentColumn++;

                divBoundsArray[i] = currentDivBounds;
            }

            return divBoundsArray;
        }
     
        private static DrawingSizeF CalculateDivSizeInPixels(DrawingSize containerSize, LayoutDivCollection divs, int divIndex, float divOriginX, float divOriginY)
        {
            LayoutDiv div = divs[divIndex];
            float divWidth = 0.0f;
            float divHeight = 0.0f;

            if (div.WidthUnits == LayoutDivUnits.Span)
            {
                float availableRowSpace = containerSize.Width - divOriginX;
                if (divIndex == divs.Count - 1)
                    divWidth = availableRowSpace;
                else
                {                    
                    for (int i = divIndex + 1; i < divs.Count; i++)
                    {
                        if (divs[i].WidthUnits == LayoutDivUnits.Span)
                            break;
                        else
                        {
                            float divLength = ToPixels(containerSize.Width, divs[i].Width, divs[i].WidthUnits);
                            if (divLength >= availableRowSpace)
                                break;
                            else
                                availableRowSpace -= divLength;
                        }
                    }
                    divWidth = availableRowSpace;
                } 
            }
            else
                divWidth = ToPixels(containerSize.Width, div.Width, div.WidthUnits);

            if (div.HeightUnits == LayoutDivUnits.Span)          
                divHeight = containerSize.Height - Math.Min(divOriginY, containerSize.Height);     
            else
                divHeight = ToPixels(containerSize.Height, div.Height, div.HeightUnits);

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
                    throw new ArgumentException("Unsupported units specified", "units");
            }
        }

        #endregion
    }

    public class LayoutDiv
    {
        private LayoutDivCollection _children = new LayoutDivCollection();

        public LayoutDivCollection Children = new LayoutDivCollection();
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
