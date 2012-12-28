using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.UI.Animation;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using CipherPark.AngelJacket.Core.Module;
using SharpDX;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public abstract class UIControl 
    {
        #region Fields
        private IUIRoot _visualRoot = null;
        private IGameApp _game = null;
        private SpriteBatch _spriteBatch = null;
        //private bool _isInitialized = false;
        private UIControlCollection _children = null;
        private DrawingSizeF _size = DrawingSizeFExtension.Zero;
        private DrawingPointF _position = DrawingPointFExtension.Zero;
        private IControlLayoutManager _layoutManager = null;
        #endregion

        #region Constructors
        public UIControl(IUIRoot visualRoot) 
        {
            _visualRoot = visualRoot;
            _visualRoot.LoadComplete += VisualRoot_LoadComplete;            
            _game = visualRoot.Game;
            Padding = DrawingSizeFExtension.Zero;
            Margin = DrawingSizeFExtension.Zero;            
            Enabled = true;
            EnableFocus = true;
            Visible = true;
            _spriteBatch = new SpriteBatch(visualRoot.Game.GraphicsDeviceContext);
            _children = new UIControlCollection();
            _children.CollectionChanged += Children_CollectionChanged;
            _layoutManager = new ContainerControlLayoutManager(this);
        }       
        #endregion

        #region Properties
        //public ControlEffect Effect
        //{
        //    get { return _effect; }
        //    set
        //    {
        //        _effect = value;
        //        OnStyleChanged();
        //    }
        //}

        public IUIRoot VisualRoot { get { return _visualRoot; } }

        protected virtual IControlLayoutManager LayoutManager { get { return _layoutManager; } }

        public ICustomFocusManager CustomFocusManager { get; set; }

        public Guid DivContainerId { get; set; }

        public DrawingPointF Position
        {
            get
            {
                return _position;
            }
            set
            {
                OnPositionChanging();
                _position = value;
                OnPositionChanged();
            }
        }

        public DrawingSizeF Size
        {
            get
            {
                return _size;
            }
            set
            {
                OnSizeChanging();
                _size = value;
                OnSizeChanged();             
            }
        }

        public string Name { get; set; }
       
        public string FullName
        {
            get
            {
                System.Text.StringBuilder fnBuilder = new System.Text.StringBuilder();
                fnBuilder.AppendFormat("\\{0}", this.Name);
                UIControl p = this.Parent;
                while (p != null)
                {
                    fnBuilder.Insert(0, string.Format("\\{0}", p.Name));
                    p = p.Parent;
                }
                return fnBuilder.ToString();   
            }
        }
        public IGameApp Game { get { return _game; } }
        public bool Enabled { get; set; }
        public bool Visible { get; set; }     
        public SpriteBatch ControlSpriteBatch { get { return _spriteBatch; } }
        public float ZOrder { get; set; }
        public float TabOrder { get; set; }
        public bool HasFocus
        {
            get
            {               
                return this.VisualRoot.FocusManager.FocusedControl == this;
            }
            set
            {                
                if (value && this.VisualRoot.FocusManager.FocusedControl != this)
                    this.VisualRoot.FocusManager.SetFocus(this);
                else if (this.VisualRoot.FocusManager.FocusedControl == this)
                    this.VisualRoot.FocusManager.LeaveFocus(this);
            }
        }

        public virtual bool CanFocus { get { return false; } }

        public bool EnableFocus { get; set; }

        public bool IsDescendant(UIControl control)
        {
            if (Children.Contains(control))
                return true;
            else
            {
                foreach (UIControl child in Children)
                    if (child.IsDescendant(control))
                        return true;                
                return false;
            }                   
        }

        public bool IsAncestor(UIControl control)
        {
            if (control == null)
                throw new ArgumentNullException();

            else if (this.Parent == null)
                return false;

            else if (this.Parent == control)
                return true;

            else
                return this.Parent.IsAncestor(control);
        }

#region deprecated
        //[Obsolete]
        //public BoundingBox Bounds_
        //{
        //    get { return BoundingBox.CreateFromPoints(new Vector3[] { new Vector3(Position.X, Position.Y + Size.Y, 0.0f), new Vector3(Position.X + Size.X, Position.Y, 0.0f) }); }
        //} 

        //[Obsolete]
        //public virtual BoundingBox BoundsToSurface_(BoundingBox box)
        //{
        //    Vector3[] transformedCorners = box.GetCorners();
        //    UIControl root = this.Parent;            
        //    while (root != null)
        //    {
        //        Vector3[] rootCorners = root.Bounds_.GetCorners();
        //        for (int i = 0; i < BoundingBox.CornerCount; i++)
        //            transformedCorners[i] += new Vector3(root.Position.X, root.Position.Y, 0.0f);
        //        root = root.Parent;
        //    }
        //    return BoundingBox.CreateFromPoints(transformedCorners);
        //}      
#endregion
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Bounds will always represent dimensions the control is rendered at. See UIControl.NativeBounds property.</remarks>
        public RectangleF Bounds { get { return RectangleFExtension.CreateLTWH(Position.X, Position.Y, this.Size.Width, this.Size.Height); } }
        public DrawingSizeF Padding { get; set; }
        public DrawingSizeF Margin { get; set; }
        public UIControl Parent { get; set; }
        public UIControlCollection Children { get { return _children; } }
        public VerticalAlignment VerticalAlignment { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        //TODO: Implement mouse capture in control tree.
        public bool Capture { get; set; }
        #endregion

        #region Methods       

        public virtual void Initialize() { }
       
        public virtual void Update(long gameTime) { }

        public virtual void Draw(long gameTime) { }

        public virtual void ApplyTemplate(UIControlTemplate template)
        {
            OnStyleChanged();
        }

        public RectangleF BoundsToSurface(RectangleF bounds)
        {
            DrawingPointF boundsOrigin = new DrawingPointF(bounds.Left, bounds.Top);
            DrawingPointF surfaceBoundsOrigin = PositionToSurface(boundsOrigin);
            return RectangleFExtension.CreateLTWH(surfaceBoundsOrigin.X, surfaceBoundsOrigin.Y, bounds.Width, bounds.Height);
        }        

        public DrawingPointF PositionToSurface(DrawingPointF position)
        {
            DrawingPointF transformedPosition = position;
            UIControl root = this.Parent;
            while (root != null)
            {
                transformedPosition = DrawingPointFExtension.Add(transformedPosition, root.Position);
                root = root.Parent;
            }
            return transformedPosition;
        }

        [Obsolete]
        public virtual UIControl _GetNextFocusableChild(UIControl fromControl)
        {
            return null;
        }

        private void VisualRoot_LoadComplete(object sender, EventArgs args)
        {
            OnLoad();
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (UIControl child in args.NewItems)
                        OnChildAdded(child);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (UIControl child in args.OldItems)                                 
                        OnChildRemoved(child);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    OnChildReset();
                    break;
            }
        }

        protected virtual void OnChildAdded(UIControl child)
        {           
            if (child.Parent != this)
                child.Parent = this;
        }

        protected virtual void OnChildRemoved(UIControl child)
        {
            if(child.Parent == this)
                child.Parent = null;
        }

        protected virtual void OnChildReset()
        {
          
        }

        protected virtual void OnLayoutChanged()
        {
            EventHandler handler = LayoutChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnSizeChanging()
        {
            EventHandler handler = SizeChanging;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnSizeChanged()
        {
            EventHandler handler = SizeChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
            UpdateLayout(LayoutUpdateReason.SizeChanged);
        }

        protected virtual void OnPositionChanging()
        {
            EventHandler handler = PositionChanging;
            if (handler != null)
                handler(this, EventArgs.Empty);           
        }

        protected virtual void OnPositionChanged()
        {
            EventHandler handler = PositionChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);          
        }

        protected void UpdateLayout(LayoutUpdateReason reason)
        {
            LayoutManager.UpdateLayout(reason);
            OnLayoutChanged();
        }

        protected virtual void OnPaddingChanged()
        {
            EventHandler handler = PaddingChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
            UpdateLayout(LayoutUpdateReason.ClientAreaChanged);
        }

        protected virtual void OnMarginChanged()
        {
            EventHandler handler = MarginChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnLoad()
        {}

        protected virtual void OnStyleChanged()
        {
            EventHandler handler = EffectChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public event EventHandler LayoutChanged;
        public event EventHandler SizeChanging;
        public event EventHandler SizeChanged;
        public event EventHandler PositionChanging;
        public event EventHandler PositionChanged;
        public event EventHandler PaddingChanged;
        public event EventHandler MarginChanged;
        public event EventHandler EffectChanged;

        #endregion
    }

    public class UIControlCollection : System.Collections.ObjectModel.ObservableCollection<UIControl>
    {
        public bool Contains(string controlName)
        {            
            return FindControl(controlName) != null;
        }

        public UIControl this[string controlName]
        {
            get
            {
                return FindControl(controlName);
            }
        } 

        private UIControl FindControl(string controlName)
        {
            if (!controlName.StartsWith("\\"))
            {
                foreach (UIControl control in this)
                    if (control.Name == controlName)
                        return control;            
                return null;
            }
            else
            {
                foreach (UIControl control in this)
                {
                    if (control.FullName == controlName)
                        return control;
                }

                foreach (UIControl control in this)
                {
                    UIControl discoveredChild = control.Children.FindControl(controlName);
                    if (discoveredChild != null)
                        return discoveredChild;
                }

                return null;
            }            
        }
    }

    public enum VerticalAlignment { Top, Center, Bottom, Stretch }

    public enum HorizontalAlignment { Left, Center, Right, Stretch }
}
