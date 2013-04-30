using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Components;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class Panel : ContainerControl
    {       
        public Panel(IUIRoot visualRoot)
            : base(visualRoot)
        { }
    }

    public class SplitterPanel : Panel
    {
        public SplitterContainerLayoutManger _layoutManager = null;

        public SplitterPanel(IUIRoot visualRoot)
            : base(visualRoot)
        {
            _layoutManager = new SplitterContainerLayoutManger(this);
        }

        protected override IControlLayoutManager LayoutManager
        {
            get
            {
                return _layoutManager;
            }
        }

        public SplitterLayoutOrientation Orientation
        {
            get { return _layoutManager.Orientation; }
            set { _layoutManager.Orientation = value; }
        }

        public SplitterLayoutDivisions Splitters { get { return _layoutManager.LayoutDivisions; } }       
    }
}
