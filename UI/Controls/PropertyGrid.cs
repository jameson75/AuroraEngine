﻿using System;
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

        public void AddProperty(PropertyGridItem item)
        {
            this.Items.Add(item);
        }

        protected override void OnUpdate(long gameTime)
        {
            foreach (ItemControl item in this.Items)
                item.Update(gameTime);
            base.OnUpdate(gameTime);
        }

        protected override void OnDraw(long gameTime)
        {
            foreach (ItemControl item in this.Items)
                item.Draw(gameTime);

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

    public abstract class PropertyGridItem : ItemControl
    {
        private CommandControlWireUp _wireUp = null;
        private Guid childLabelId;

        protected PropertyGridItem(IUIRoot visualRoot, string caption, SpriteFont font, Color fontColor)
            : this(visualRoot, new TextContent(caption, font, fontColor))
        {  }

        protected PropertyGridItem(IUIRoot visualRoot, TextContent text)
            : base(visualRoot)
        {
            Label childLabel = new Label(visualRoot, text);
            childLabelId = Guid.NewGuid();
            childLabel.Id = childLabelId;
            childLabel.HorizontalAlignment = Controls.HorizontalAlignment.Left;
            childLabel.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            childLabel.Size = new DrawingSizeF(100.0f, 1.0f);
            this.Children.Add(childLabel);           
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

    public class BooleanPropertyGridItem : PropertyGridItem
    {
        private CheckBox _checkBox = null;

        public BooleanPropertyGridItem(IUIRoot visualRoot, string caption, SpriteFont font, Color fontColor, bool value = false) 
            : base(visualRoot, caption, font, fontColor)
        {
            _checkBox = new CheckBox(visualRoot);
            _checkBox.IsChecked = value;
            this.Children.Add(_checkBox);
        }

        public bool Value
        {
            get { return _checkBox.IsChecked; }
            set { _checkBox.IsChecked = value; }
        }
    }

    public class ColorPropertyGridItem : PropertyGridItem
    {
        ColorSelect _colorSelect = null;

        public ColorPropertyGridItem(IUIRoot visualRoot, string caption, SpriteFont font, Color fontColor, Color color = Color.Transparent)
            : base(visualRoot, caption, font, fontColor)
        {
            _colorSelect = new ColorSelect(visualRoot);
            _colorSelect.Color = color;
            this.Children.Add(_colorSelect);
        }

        public Color Value
        {
            get { return _colorSelect.Color; }
            set { _colorSelect.Color = value; }
        }
    }

    public class SpinnerPropertyGridItem : PropertyGridItem
    {
        Spinner _spinner = null;

        public SpinnerPropertyGridItem(IUIRoot visualRoot, string caption, SpriteFont font, Color fontColor, double value = 0)
            : base(visualRoot, caption, font, fontColor)
        {
            _spinner = new Spinner(visualRoot);
            _spinner.Value = value;
            this.Children.Add(_spinner);
        }

        public double Value
        {
            get { return _spinner.Value; }
            set { _spinner.Value = value; }
        }
    }

    public class SelectPropertyGridItem : PropertyGridItem
    {
        ListSelect _listSelect = new ListSelect();
        public SelectPropertyGridItem(IUIRoot visualRoot, string caption, SpriteFont font, Color fontColor, IEnumerable<string> itemCaptions, int selectedIndex = 0)
            : base(visualRoot, caption, font, fontColor)
        {
            _listSelect = new _listSelect(visualRoot);
            _listSelect.Choices.AddRange(itemCaptions.Select(c => new ListSelectItem(visualRoot, c, font, fontColor)));
            this.Children.Add(_listSelect);
        }
    }

    public class NumericPropertyGridItem : PropertyGridItem
    {
        NumericSelect _numericSelect = new NumericSelect();
        public NumericPropertyGridItem(IUIRoot visualRoot, string caption, SpriteFont font, Color fontColor, IEnumerable<string> itemCaptions, double value = 0)
            : base(visualRoot, caption, font, fontColor)
        {
            _numericSelect = new NumericSelect(visualRoot);
            _numericSelect.Value = value;
            this.Children.Add(_numericSelect);
        }
    }

    public class SubPropertyGrid : PropertyGridItem
    {
        private PropertyGrid _innerGrid = null;
        public SubPropertyGrid(IUIRoot visualRoot, string caption, SpriteFont font, Color fontColor, PropertyGrid innerGrid)
             : base(visualRoot, caption, font, fontColor)
        {
            
        }

        public PropertyGrid InnerGrid { get { return _innerGrid; } set { _innerGrid = value; } }
    }
}
