using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class PropertyGrid : ItemsControl
    {       
        public const int SizeInfinite = -1;      
        private int _maxRowSize = PropertyGrid.SizeInfinite;
        private CommandControlWireUp _controlWireUp = null;

        public PropertyGrid(IUIRoot root)
            : base(root)
        {          

            _controlWireUp = new CommandControlWireUp(this);
            _controlWireUp.ChildControlCommand += ControlWireUp_ChildControlCommand;
        }      

        private void ControlWireUp_ChildControlCommand(object sender, ControlCommandArgs args)
        {
            OnCommand(args.CommandName);
            MaxRowSize = ListControl.SizeInfinite;
        }

        public override bool CanFocus
        {
            get
            {
                return true;
            }
        }

        public int MaxRowSize 
        {
            get { return _maxRowSize; }
            set
            {
                if (value < 1 && value != PropertyGrid.SizeInfinite)
                    throw new InvalidOperationException("MaxRowSize was not greater or equal to one");
            }                    
        }      

        protected override void OnUpdate(long gameTime)
        {
            foreach (ItemControl item in this.Items)
                item.OnUpdate(gameTime);
            base.OnUpdate(gameTime);
        }

        protected override void OnDraw(long gameTime)
        {
            foreach (ItemControl item in this.Items)
                item.OnDraw(gameTime);
            base.OnDraw(gameTime);
        }

        protected override void OnLayoutChanged()
        {
            int currentRowIndex = 0;
            int currentColumnIndex = 0;         
           
            float previousItemsTotalWidth = 0;
            float previousColumnsHeight = 0;
            float maxItemHeight = 0;
            foreach (PropertyGridItem item in this.Items)
            {
                item.Position = new DrawingPointF(previousItemsTotalWidth, previousColumnsHeight);
                previousItemsTotalWidth += item.Size.Width;
                maxItemHeight = Math.Max(maxItemHeight, item.Size.Width);
                currentColumnIndex++;
                if (MaxRowSize != PropertyGrid.SizeInfinite && currentColumnIndex >= MaxRowSize)
                {
                    currentRowIndex++;
                    previousColumnsHeight = maxItemHeight;
                    maxItemHeight = 0;
                    currentColumnIndex = 0;
                }
            }
        }    
    }

    public class PropertyGridItem : ItemControl
    {
        private CommandControlWireUp _wireUp = null;
        private Guid childLabelId;

        public PropertyGridItem(string text, SpriteFont font, Color4 fontColor, UIControl control)
            : base(control.VisualRoot)
        {
            Label childLabel = new Label(control.VisualRoot, new TextContent(text, font, fontColor));
            childLabelId = Guid.NewGuid();
            childLabel.Id = childLabelId;
            childLabel.HorizontalAlignment = Controls.HorizontalAlignment.Left;
            childLabel.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            childLabel.Size = new DrawingSizeF(100.0f, 1.0f);
            this.Children.Add(childLabel);
            this.Children.Add(control);
            _wireUp = new CommandControlWireUp(this);
            _wireUp.ChildControlCommand += CommandControlWireUp_ChildControlCommand;
        }

        private void CommandControlWireUp_ChildControlCommand(object sender, ControlCommandArgs args)
        {
            OnCommand(args.CommandName);
        }

        protected override void OnLayoutChanged()
        {
            UIControl childLabel = Children.First(c => c.Id == childLabelId);
            foreach (UIControl child in Children)
            {
                if (child != childLabel)
                {
                    child.Position = new DrawingPointF(childLabel.Size.Width, 0.0f);
                    child.Size = new DrawingSizeF(this.Size.Width - child.Size.Width, this.Size.Height);
                }
            }
            base.OnLayoutChanged();
        }
    }
}
