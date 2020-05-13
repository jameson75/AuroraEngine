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
                    child.Position = new Vector2(positionX, positionY);
                }

                if (newWidth.HasValue || newHeight.HasValue)
                {
                    float width = newWidth.HasValue ? newWidth.Value : child.Bounds.Width;
                    float height = newHeight.HasValue ? newHeight.Value : child.Bounds.Height;
                    child.Size = new Size2F(width, height);
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

    public class SplitterContainerLayoutManager : IControlLayoutManager
    {        
        private SplitterLayoutDivisions _splitterDivisions = new SplitterLayoutDivisions();
        private UIControl _container = null;
        private RectangleF _cachedBounds = RectangleF.Empty;
        public SplitterLayoutOrientation Orientation { get; set; } 
        public SplitterLayoutDivisions LayoutDivisions { get { return _splitterDivisions; } }
        
        public SplitterContainerLayoutManager(UIControl container)
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
                    child.Position = new Vector2(positionX, positionY);
                }

                if (newWidth.HasValue || newHeight.HasValue)
                {
                    float width = newWidth.HasValue ? newWidth.Value : child.Bounds.Width;
                    float height = newHeight.HasValue ? newHeight.Value : child.Bounds.Height;
                    child.Size = new Size2F(width, height);
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

    public class StackLayoutManager : IControlLayoutManager
    {
        private UIControl _container = null;
        
        public StackLayoutOrientation Orientation { get; set; }       

        public StackLayoutManager(UIControl container)
        {
            _container = container;           
        }

        public void UpdateLayout(LayoutUpdateReason reason)
        {
            float offset = 0f;
            UIControlCollection Items = _container.Children;

            if (Orientation == StackLayoutOrientation.Vertical)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (i == 0)
                        offset += _container.Padding.Top + Items[0].Margin.Top;
                    else
                        offset += (Items[i - 1].Size.Height + Items[i - 1].Margin.Bottom + Items[i].Margin.Top);
                    Items[i].Position = new Vector2(_container.Padding.Left, offset);
                    Items[i].Size = new Size2F(_container.Size.Width - _container.Margin.Right, Items[i].Size.Height);                    
                }
            }
            else
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (i == 0)
                        offset += _container.Padding.Left + Items[0].Margin.Left;
                    else                    
                        offset += (Items[i - 1].Size.Width + Items[i - 1].Margin.Right + Items[i].Margin.Left);
                    Items[i].Position = new Vector2(offset, _container.Padding.Top);
                    Items[i].Size = new Size2F(Items[i].Size.Width, _container.Size.Height - _container.Margin.Bottom);                    
                }
            }
        }
    }

    public enum StackLayoutOrientation
    {
        Vertical,
        Horizontal
    }   
}
