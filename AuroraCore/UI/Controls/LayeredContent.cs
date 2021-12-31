using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.Utils;
using System.Collections.ObjectModel;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Controls
{
    /*
    /// <summary>
    /// Renders child content, first to last.
    /// </summary>
    public class LayeredContent : UIContent
    {
        private ObservableCollection<UIContent> _childContent = new ObservableCollection<UIContent>();

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<UIContent> ChildContents { get { return _childContent; } }

        /// <summary>
        /// 
        /// </summary>
        public LayeredContent()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="children"></param>
        public LayeredContent(IEnumerable<UIContent> children)
        {       
            _childContent.AddRange(children);
        }

        /// <summary>
        /// Returns the union of the smallest bounding rectangles for each content layer.
        /// </summary>
        /// <returns>Returns the smallest rectangle containing all content.</returns>
        public override RectangleF CalculateSmallestBoundingRect()
        {
            RectangleF rectangle = RectangleF.Empty;
            for (int i = 0; i < _childContent.Count(); i++)
                rectangle = RectangleF.Union(rectangle, _childContent[i].CalculateSmallestBoundingRect());
            return rectangle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw()
        {
            for (int i = 0; i < _childContent.Count(); i++)
                _childContent[i].Draw();
            base.Draw();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnContainerChanged()
        {
            base.OnContainerChanged();
            foreach (UIContent content in _childContent)
                content.Container = this.Container;
        }
    }
    */

    public static class ObservableCollectionExtension
    {
        public static void AddRange<T>(this ObservableCollection<T> target, IEnumerable<T> range)
        {
            foreach (T r in range)
                target.Add(r);
        }      
    }
}
