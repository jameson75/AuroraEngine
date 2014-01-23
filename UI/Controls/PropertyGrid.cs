using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.DirectInput;
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
    public class PropertyGrid : SelectControl
    {       
        public const int SizeInfinite = -1;      
        private int _maxRowSize = PropertyGrid.SizeInfinite;
        private CommandControlWireUp _controlWireUp = null;
        private int _selectedIndex = -1;
        private StackLayoutManager _layoutManager = null;

        public PropertyGrid(IUIRoot root)
            : base(root)
        {         
            _controlWireUp = new CommandControlWireUp(this);
            _controlWireUp.ChildControlCommand += ControlWireUp_ChildControlCommand;
            _layoutManager = new StackLayoutManager(this);
        }      

        private void ControlWireUp_ChildControlCommand(object sender, ControlCommandArgs args)
        {
            OnCommand(args.CommandName);
            MaxRowSize = ListControl.SizeInfinite;
        }     
        
        protected override IControlLayoutManager LayoutManager
        {
            get
            {
                return _layoutManager;
            }
        }

        public override bool CanReceiveFocus
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
            if (this.HasFocus || this.ContainsFocus)
            {
                Services.IInputService inputServices = (Services.IInputService)Game.Services.GetService(typeof(Services.IInputService));
                if (inputServices == null)
                    throw new InvalidOperationException("Input services not available.");

                BufferedInputState bufferedInputState = inputServices.GetBufferedInputState();

                bool selectPreviousKeyDown = (bufferedInputState.IsKeyDown(Key.UpArrow)) ||                                            
                                             (bufferedInputState.InputState.IsGamepadButtonHit(0, SharpDX.XInput.GamepadButtonFlags.DPadUp));

                bool selectNextKeyDown = (bufferedInputState.IsKeyDown(Key.Down)) ||
                                         (bufferedInputState.InputState.IsGamepadButtonHit(0, SharpDX.XInput.GamepadButtonFlags.DPadDown));                                      

                if (selectPreviousKeyDown)
                    this.SelectPreviousItem();

                else if (selectNextKeyDown)
                    this.SelectNextItem();               
            }          
        
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

        //protected override void OnLayoutChanged()
        //{
        //    //int currentRowIndex = 0;
        //    //int currentColumnIndex = 0;         
           
        //    //float previousItemsTotalWidth = 0;
        //    //float previousColumnsHeight = 0;
        //    //float maxItemHeight = 0;
        //    //foreach (PropertyGridItem item in this.Items)
        //    //{
        //    //    item.Position = new DrawingPointF(previousItemsTotalWidth, previousColumnsHeight);
        //    //    previousItemsTotalWidth += item.Size.Width;
        //    //    maxItemHeight = Math.Max(maxItemHeight, item.Size.Width);
        //    //    currentColumnIndex++;
        //    //    if (MaxRowSize != PropertyGrid.SizeInfinite && currentColumnIndex >= MaxRowSize)
        //    //    {
        //    //        currentRowIndex++;
        //    //        previousColumnsHeight = maxItemHeight;
        //    //        maxItemHeight = 0;
        //    //        currentColumnIndex = 0;
        //    //    }
        //    //}
        //}    
    }

    public class PropertyGridItem : ItemControl
    {
        private CommandControlWireUp _wireUp = null;      
        private Label childLabel = null;
        private Label selectedChildLabel = null;

        private PropertyGridItem(IUIRoot visualRoot, TextContent nameContent, TextContent selectNameContent)
            : base(visualRoot)
        {
            childLabel = new Label(visualRoot, nameContent);    
            childLabel.HorizontalAlignment = Controls.HorizontalAlignment.Left;
            childLabel.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            childLabel.Size = new DrawingSizeF(100.0f, 1.0f);
            this.Children.Add(childLabel);
            selectedChildLabel = new Label(visualRoot, selectNameContent);
            selectedChildLabel.HorizontalAlignment = Controls.HorizontalAlignment.Left;
            selectedChildLabel.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            selectedChildLabel.Size = new DrawingSizeF(100.0f, 1.0f);
            selectedChildLabel.Visible = false;
            this.Children.Add(selectedChildLabel);
            this.Size = nameContent.Font.MeasureString(nameContent.Text);
            _wireUp = new CommandControlWireUp(this);
            _wireUp.ChildControlCommand += CommandControlWireUp_ChildControlCommand;
        }
       
        private PropertyGridItem(IUIRoot visualRoot, string caption, SpriteFont font, Color fontColor, Color selectedFontColor)
            : this(visualRoot, new TextContent(caption, font, fontColor), new TextContent(caption, font, selectedFontColor))
        {  }        
        
        public PropertyGridItem(UIControl valueControl, string caption, SpriteFont font, Color fontColor, Color selectedFontColor)
            : this(valueControl.VisualRoot, caption, font, fontColor, selectedFontColor)
        { 
            this.Children.Add(valueControl);
        }

        public PropertyGridItem(UIControl valueControl, TextContent nameContent, TextContent selectedContent)
            : this(valueControl.VisualRoot, nameContent, selectedContent)
        {
            this.Children.Add(valueControl);
        }

        private void CommandControlWireUp_ChildControlCommand(object sender, ControlCommandArgs args)
        {
            OnCommand(args.CommandName);
        }

        protected override void OnLayoutChanged()
        {
            DrawingSizeF nameControlSize = childLabel.Size;
            UIControl valueControl = Children.FirstOrDefault(c => c != childLabel && c != selectedChildLabel);
            if (valueControl != null)
            {
                valueControl.Position = new DrawingPointF(nameControlSize.Width, 0.0f);
                valueControl.Size = new DrawingSizeF(this.Size.Width - nameControlSize.Width, this.Size.Height);
            }
            base.OnLayoutChanged();
        }       
     
        protected override void OnSelected()
        {
            if (!this.ContainsFocus)
            {
                UIControl nextFocusableControl = this.VisualRoot.FocusManager.GetNext(this);
                if (nextFocusableControl != null && IsDescendant(nextFocusableControl))
                    this.VisualRoot.FocusManager.SetFocus(nextFocusableControl);
            }            
            
            childLabel.Visible = false;
            selectedChildLabel.Visible = true;

 	        base.OnSelected();
        }

        protected override void OnUnselected()
        {
            if (this.ContainsFocus)
                this.VisualRoot.FocusManager.SetFocus(null);
            
            childLabel.Visible = true;
            selectedChildLabel.Visible = false;
            
            base.OnUnselected();
        }
    }

    //public class CheckboxPropertyGridItem : PropertyGridItem
    //{
    //    private CheckBox _checkBox = null;

    //    public CheckboxPropertyGridItem(IUIRoot visualRoot, string caption, SpriteFont font, Color fontColor, bool value = false) 
    //        : base(visualRoot, caption, font, fontColor)
    //    {
    //        _checkBox = new CheckBox(visualRoot);
    //        _checkBox.IsChecked = value;
    //        this.Children.Add(_checkBox);
    //    }

    //    public bool Value
    //    {
    //        get { return _checkBox.IsChecked; }
    //        set { _checkBox.IsChecked = value; }
    //    }
    //}

    //public class ColorPropertyGridItem : PropertyGridItem
    //{
    //    ColorSelect _colorSelect = null;

    //    public ColorPropertyGridItem(IUIRoot visualRoot, string caption, SpriteFont font, Color fontColor, Color color = Color.Transparent)
    //        : base(visualRoot, caption, font, fontColor)
    //    {
    //        _colorSelect = new ColorSelect(visualRoot);
    //        _colorSelect.Color = color;
    //        this.Children.Add(_colorSelect);
    //    }

    //    public Color Value
    //    {
    //        get { return _colorSelect.Color; }
    //        set { _colorSelect.Color = value; }
    //    }
    //}

    //public class SpinnerPropertyGridItem : PropertyGridItem
    //{
    //    Spinner _spinner = null;

    //    public SpinnerPropertyGridItem(IUIRoot visualRoot, string caption, SpriteFont font, Color fontColor, double value = 0)
    //        : base(visualRoot, caption, font, fontColor)
    //    {
    //        _spinner = new Spinner(visualRoot, font, fontColor, Color.Transparent);
    //        _spinner.Value = value;
    //        this.Children.Add(_spinner);
    //    }

    //    public double Value
    //    {
    //        get { return _spinner.Value; }
    //        set { _spinner.Value = value; }
    //    }
    //}

    //public class SelectPropertyGridItem : PropertyGridItem
    //{
    //    ListSelect _listSelect = new ListSelect();
    //    public SelectPropertyGridItem(IUIRoot visualRoot, string caption, SpriteFont font, Color fontColor, IEnumerable<string> itemCaptions, int selectedIndex = 0)
    //        : base(visualRoot, caption, font, fontColor)
    //    {
    //        _listSelect = new _listSelect(visualRoot);
    //        _listSelect.Choices.AddRange(itemCaptions.Select(c => new ListSelectItem(visualRoot, c, font, fontColor)));
    //        this.Children.Add(_listSelect);
    //    }
    //}

    //public class NumericPropertyGridItem : PropertyGridItem
    //{
    //    Spinner _spinner = null;
    //    public NumericPropertyGridItem(IUIRoot visualRoot, string caption, SpriteFont font, Color fontColor, IEnumerable<string> itemCaptions, double value = 0)
    //        : base(visualRoot, caption, font, fontColor)
    //    {
    //        _spinner = new Spinner(visualRoot);
    //        _spinner.Value = value;
    //        this.Children.Add(_spinner);
    //    }
    //}

    //public class SubPropertyGrid : PropertyGridItem
    //{
    //    private PropertyGrid _innerGrid = null;
    //    public SubPropertyGrid(IUIRoot visualRoot, string caption, SpriteFont font, Color fontColor, PropertyGrid innerGrid)
    //         : base(visualRoot, caption, font, fontColor)
    //    {
            
    //    }

    //    public PropertyGrid InnerGrid { get { return _innerGrid; } set { _innerGrid = value; } }
    //}
}
