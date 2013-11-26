using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using CipherPark.AngelJacket.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public interface IControlLayoutManager
    {
        void UpdateLayout(LayoutUpdateReason reason);
    }

    public class ContainerControlLayoutManager : IControlLayoutManager
    {
        private UIControl _container = null;
        private RectangleF _cachedBounds = RectangleF.Empty;

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
                float? newWidth = null;
                float? newHeight = null;

                switch(child.HorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        newPositionX = (int)((float)_container.Bounds.Width / 2.0f) - (int)((float)child.Bounds.Width / 2.0f);                       
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
                        newPositionX = 0; //_container.Padding.Width + child.Margin.Width;
                        newWidth = _container.Bounds.Width; //- (int)(_container.Padding.Width * 2);
                        break;
                }
                
                switch(child.VerticalAlignment)
                {
                    case VerticalAlignment.Center:
                         newPositionY = (_container.Bounds.Height / 2.0f) - (child.Bounds.Height / 2.0f);
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
                        newPositionY = 0; //_container.Padding.Height + child.Margin.Height;                       
                        newHeight = _container.Bounds.Height; // - (_container.Padding.Width * 2);
                        break;
                }

                if (newPositionX.HasValue || newPositionY.HasValue)
                {
                    float positionX = newPositionX.HasValue ? newPositionX.Value : child.Position.X;
                    float positionY = newPositionY.HasValue ? newPositionY.Value : child.Position.Y;
                    child.Position = new DrawingPointF(positionX, positionY);
                }

                if (newWidth.HasValue || newHeight.HasValue)
                {
                    float width = newWidth.HasValue ? newWidth.Value : child.Bounds.Width;
                    float height = newHeight.HasValue ? newHeight.Value : child.Bounds.Height;
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

    public class SplitterContainerLayoutManger : IControlLayoutManager
    {        
        private SplitterLayoutDivisions _splitterDivisions = new SplitterLayoutDivisions();
        private UIControl _container = null;
        private RectangleF _cachedBounds = RectangleF.Empty;
        public SplitterLayoutOrientation Orientation { get; set; } 
        public SplitterLayoutDivisions LayoutDivisions { get { return _splitterDivisions; } }
        
        public SplitterContainerLayoutManger(UIControl container)
        {
            _container = container;
            _container.SizeChanging += Container_SizeChanging;
            _container.SizeChanged += Container_SizeChanged;
        }     
        
        #region IControlLayoutManager Members
        public void UpdateLayout(LayoutUpdateReason reason)
        {          
            foreach (UIControl child in this._container.Children)
            {
                RectangleF cellRectangle = GetCellFromId(child.LayoutId);
                float? newPositionX = null;
                float? newPositionY = null;
                float? newWidth = null;
                float? newHeight = null;

                switch(child.HorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        newPositionX = (int)((float)cellRectangle.Width / 2.0f) - (int)((float)child.Bounds.Width / 2.0f);                       
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
                        newPositionX = cellRectangle.Left; // + child.Margin.Width;
                        newWidth = cellRectangle.Width;
                        break;
                }
                
                switch(child.VerticalAlignment)
                {
                    case VerticalAlignment.Center:
                         newPositionY = (_container.Bounds.Height / 2.0f) - (child.Bounds.Height / 2.0f);
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
                        newPositionY = cellRectangle.Top; // + child.Margin.Height;                       
                        newHeight = _container.Bounds.Height - (_container.Padding.Height * 2);
                        break;
                }

                if (newPositionX.HasValue || newPositionY.HasValue)
                {
                    float positionX = newPositionX.HasValue ? newPositionX.Value : child.Position.X;
                    float positionY = newPositionY.HasValue ? newPositionY.Value : child.Position.Y;
                    child.Position = new DrawingPointF(positionX, positionY);
                }

                if (newWidth.HasValue || newHeight.HasValue)
                {
                    float width = newWidth.HasValue ? newWidth.Value : child.Bounds.Width;
                    float height = newHeight.HasValue ? newHeight.Value : child.Bounds.Height;
                    child.Size = new DrawingSizeF(width, height);
                }
            }            
        }

        
        #endregion   
        
        private void Container_SizeChanging(object sender, EventArgs args)
        {
            _cachedBounds = _container.Bounds;
        }

        private void Container_SizeChanged(object sender, EventArgs args)
        {

        }
      
        private RectangleF GetCellFromId(Guid id)
        {
            if (id == Guid.Empty)
            {
                if (_splitterDivisions.Count == 0)
                    return _container.ClientRectangle;
                else
                {
                    RectangleF cellRect = _container.ClientRectangle;
                    if (Orientation == SplitterLayoutOrientation.Horizontal)
                        cellRect.Bottom = GetNormalizedDistance(_splitterDivisions[0]);
                    else
                        cellRect.Right = GetNormalizedDistance(_splitterDivisions[0]);
                    return cellRect;
                }
            }
            else
            {
                foreach (SplitterLayoutDivision division in _splitterDivisions)
                {
                    if (division.DivisionId == id)
                    {
                        int i = _splitterDivisions.IndexOf(division);
                        RectangleF cellRect = _container.ClientRectangle;
                        if (Orientation == SplitterLayoutOrientation.Horizontal)
                        {
                            cellRect.Top = GetNormalizedDistance(division);
                            if (i != _splitterDivisions.Count - 1)
                                cellRect.Bottom = GetNormalizedDistance(_splitterDivisions[i + 1]);
                        }
                        else
                        {
                            cellRect.Left = GetNormalizedDistance(division);
                            if (i != _splitterDivisions.Count - 1)
                                cellRect.Right = GetNormalizedDistance(_splitterDivisions[i + 1]);
                        }
                        return cellRect;
                    }
                }

                throw new InvalidOperationException("No layout splitter found with specified guid.");
            }
        }

        private float GetNormalizedDistance(SplitterLayoutDivision division)
        {       
            float distanceInPixels = 0;
            if (division.IsDistancePercentage)
            {
               if(this.Orientation == SplitterLayoutOrientation.Verticle)
                   distanceInPixels = division.Distance / 100.0f * _container.Bounds.Width;
               else 
                   distanceInPixels = division.Distance / 100.0f * _container.Bounds.Height;
            }
            else 
                distanceInPixels = division.Distance;
          
            //if the fixed side is One, then return the distance from the left side of the container.
            if (division.AnchorSide == SplitterLayoutAnchorSide.One)
                return distanceInPixels;
            //otherwise, return the distance from the right side of the container.
            else
                return _container.Bounds.Right - distanceInPixels;          
        }
    }

    public class SplitterLayoutDivision
    {
        private Guid _divisionId;
        public float Distance { get; set; }
        public SplitterLayoutAnchorSide AnchorSide { get; set; }
        public bool IsDistancePercentage { get; set; }
        public Guid DivisionId { get { return _divisionId; } }
        public SplitterLayoutDivision(Guid id) { _divisionId = id; }
        public SplitterLayoutDivision(Guid id, float distance, SplitterLayoutAnchorSide fixedSide, bool isDistancePercentage = false)
        {
            _divisionId = id;
            Distance = distance;
            AnchorSide = fixedSide;
            IsDistancePercentage = isDistancePercentage;
        }
    }

    public enum SplitterLayoutAnchorSide
    {
        /// <summary>
        /// None specified, same effect as One
        /// </summary>
        None,
        /// <summary>
        /// Left or Top
        /// </summary>
        One = None,
        /// <summary>
        /// Right or Bottom
        /// </summary>
        Two
    }

    public enum SplitterLayoutOrientation
    {
        None,
        Verticle = None,
        Horizontal
    }

    public class SplitterLayoutDivisions : List<SplitterLayoutDivision>
    { }
   
    //public class DivLayoutManager : IControlLayoutManager
    //{
    //    private LayoutDivCollection _divs = null;
    //    private UIControl _container = null;
    //    private RectangleF _cachedBounds = RectangleF.Empty;

    //    public DivLayoutManager(UIControl container)
    //    {
    //        _container = container;
    //        _container.SizeChanging += Container_SizeChanging;
    //        _divs = new LayoutDivCollection();
    //    }

    //    public LayoutDivCollection Divs { get { return _divs; } }

    //    private void Container_SizeChanging(object sender, EventArgs e)
    //    {
    //        _cachedBounds = _container.Bounds;
    //    }

    //    #region IControlLayoutManager Members

    //    public void UpdateLayout(LayoutUpdateReason reason)
    //    {
    //        _UpdateLayout(_divs, reason);         
    //    }

    //    private void _UpdateLayout(LayoutDivCollection divs, LayoutUpdateReason reason)
    //    {
    //        RectangleF[] divBoundsArray = DivLayoutManager.CalculateDivBounds(_container.Bounds.GetSize(), _divs);
    //        for (int i = 0; i < divs.Count; i++)
    //        {
    //            ApplyDivContainership(divs[i], divBoundsArray[i], reason);
    //            if( divs[i].Children.Count > 0 )
    //                _UpdateLayout(divs[i].Children, reason);
    //        }
    //    }

    //    private void ApplyDivContainership(LayoutDiv div, RectangleF divBounds, LayoutUpdateReason reason)
    //    {
    //        float newPositionX = 0f;
    //        float newPositionY = 0f;
    //        float newWidth = 0f;
    //        float newHeight = 0f;
    //        foreach (UIControl child in _container.Children)
    //        {
    //            if (child.Id == div.Id)
    //            {                   
    //                newPositionX = divBounds.Left + div.Padding.X + child.Margin.Width;
    //                newWidth = divBounds.Width - (div.Padding.X * 2f);                  
    //                newPositionY = divBounds.Top + div.Padding.Y + child.Margin.Width;
    //                newHeight = divBounds.Height - (div.Padding.Y * 2f);                    
    //                child.Position = new DrawingPointF(newPositionX, newPositionY);      
    //                child.Size = new DrawingSizeF(newWidth, newHeight);                   
    //            }
    //        }
    //    }

    //    public static RectangleF[] CalculateDivBounds(DrawingSizeF containerSize, LayoutDivCollection divs)
    //    {
    //        int currentRow = 0;
    //        int currentColumn = 0;
    //        float currentDivOriginX = 0;
    //        float currentDivOriginY = 0;
    //        float currentRowOffsetY = 0;
    //        RectangleF[] divBoundsArray = new RectangleF[divs.Count];

    //        for (int i = 0; i < divs.Count; i++)
    //        {
    //            if (currentColumn != 0 && currentDivOriginX >= containerSize.Width)
    //            {
    //                currentRow++;
    //                currentColumn = 0;
    //                currentDivOriginX = 0;
    //                currentDivOriginY += currentRowOffsetY;
    //                currentRowOffsetY = 0;
    //            }
                
    //            DrawingSizeF divSize = CalculateDivSizeInPixels(containerSize, divs, i, currentDivOriginX, currentDivOriginY);
    //            RectangleF currentDivBounds = RectangleFExtension.CreateLTWH(currentDivOriginX, currentDivOriginY, divSize.Width, divSize.Height);
    //            currentDivOriginX += currentDivBounds.Width;
    //            currentRowOffsetY = Math.Max(currentRowOffsetY, currentDivBounds.Height);
    //            currentColumn++;

    //            divBoundsArray[i] = currentDivBounds;
    //        }

    //        return divBoundsArray;
    //    }
     
    //    private static DrawingSizeF CalculateDivSizeInPixels(DrawingSizeF containerSize, LayoutDivCollection divs, int divIndex, float divOriginX, float divOriginY)
    //    {
    //        LayoutDiv div = divs[divIndex];
    //        float divWidth = 0;
    //        float divHeight = 0;

    //        if (div.WidthUnits == LayoutDivUnits.Span)
    //        {
    //            float availableRowSpace = containerSize.Width - divOriginX;
    //            if (divIndex == divs.Count - 1)
    //                divWidth = availableRowSpace;
    //            else
    //            {                    
    //                for (int i = divIndex + 1; i < divs.Count; i++)
    //                {
    //                    if (divs[i].WidthUnits == LayoutDivUnits.Span)
    //                        break;
    //                    else
    //                    {
    //                        float divLength = ToPixels(containerSize.Width, divs[i].Width, divs[i].WidthUnits);
    //                        if (divLength >= availableRowSpace)
    //                            break;
    //                        else
    //                            availableRowSpace -= divLength;
    //                    }
    //                }
    //                divWidth = availableRowSpace;
    //            } 
    //        }
    //        else
    //            divWidth = ToPixels(containerSize.Width, div.Width, div.WidthUnits);

    //        if (div.HeightUnits == LayoutDivUnits.Span)          
    //            divHeight = containerSize.Height - Math.Min(divOriginY, containerSize.Height);     
    //        else
    //            divHeight = ToPixels(containerSize.Height, div.Height, div.HeightUnits);

          
    //        return new DrawingSizeF(divWidth, divHeight);
    //    }

    //    private static float ToPixels(float containerLength, float divLength, LayoutDivUnits units)
    //    {
    //        switch( units )
    //        {
    //            case LayoutDivUnits.Pixels:
    //                return divLength;
    //            case LayoutDivUnits.Percentage:
    //                return containerLength * divLength / 100.0f;
    //            default:
    //                throw new ArgumentException("Unsupported units specified", "units");
    //        }
    //    }

    //    #endregion
    //}

    //public class LayoutDiv
    //{
    //    private LayoutDivCollection _children = new LayoutDivCollection();

    //    public LayoutDivCollection Children = new LayoutDivCollection();
    //    public Guid Id { get; set; }
    //    public float Height { get; set; }
    //    public float Width { get; set; }
    //    public LayoutDivUnits HeightUnits { get; set; }
    //    public LayoutDivUnits WidthUnits { get; set; }
    //    public DrawingPoint Padding { get; set; }
    //    public DrawingPoint Margin { get; set; }

    //    public LayoutDiv(Guid divId, int width, int height)
    //    {
    //        Id = divId;
    //        Width = width;
    //        Height = height;
    //        HeightUnits = LayoutDivUnits.Pixels;
    //        WidthUnits = LayoutDivUnits.Pixels;
    //    }

    //    public LayoutDiv(Guid divId, float width, LayoutDivUnits widthUnits, float height, LayoutDivUnits heightUnits)
    //    {
    //        Id = divId;
    //        Width = width;
    //        Height = height;
    //        HeightUnits = heightUnits;
    //        WidthUnits = widthUnits;
    //    }

    //    public void SetWidthAndUnits(float width, LayoutDivUnits units)
    //    {
    //        Width = width;
    //        WidthUnits = units;
    //    }

    //    public void SetHeightAndUnits(float height, LayoutDivUnits units)
    //    {
    //        Height = height;
    //        HeightUnits = units;
    //    }       

    //    public bool IsWidthFixed { get { return WidthUnits == LayoutDivUnits.Pixels; } }

    //    public bool IsHeightFixed { get { return HeightUnits == LayoutDivUnits.Pixels; } }
    //}

    //public enum LayoutDivUnits
    //{
    //    Pixels = 0,
    //    Percentage,
    //    Span
    //}

    //public class LayoutDivCollection : List<LayoutDiv>
    //{ }
}
