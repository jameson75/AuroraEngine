using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Controls
{
    public class Panel : ContainerControl
    {       
        public Panel(IUIRoot visualRoot)
            : base(visualRoot)
        { }

        public bool IsFullSize { get; set; }

        protected override void OnInitialize()
        {
            Game.BuffersResized += Game_BuffersResized;
            UpdateSize();
            base.OnInitialize();            
        }

        private void Game_BuffersResized()
        {
            UpdateSize();
        }

        private void UpdateSize()
        {
            if (IsFullSize)
                this.Size = Parent != null ? Parent.Size :
                                              Game.RenderTargetView.GetTexture2DSize().ToSize2F();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SplitterPanel : Panel
    {
        private SplitterContainerLayoutManager _layoutManager = null;

        public SplitterPanel(IUIRoot visualRoot)
            : base(visualRoot)
        {
            _layoutManager = new SplitterContainerLayoutManager(this);
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

        public SplitterLayoutDivisions Splitters
        {
            get { return _layoutManager.LayoutDivisions; }
        }       
    }

    /// <summary>
    /// 
    /// </summary>
    public class StackPanel : Panel
    {
        private StackLayoutManager _layoutManager = null;

        public StackPanel(IUIRoot visualRoot)
            : base(visualRoot)
        {
            _layoutManager = new StackLayoutManager(this);
            Orientation = StackLayoutOrientation.Vertical;
        }

        protected override IControlLayoutManager LayoutManager
        {
            get
            {
                return _layoutManager;
            }
        }

        public StackLayoutOrientation Orientation
        {
            get { return _layoutManager.Orientation; }
            set { _layoutManager.Orientation = value; }
        }
    }
}
