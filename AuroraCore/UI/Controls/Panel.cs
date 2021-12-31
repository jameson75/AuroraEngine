using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using CipherPark.KillScript.Core.UI.Components;
using CipherPark.KillScript.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.UI.Controls
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
