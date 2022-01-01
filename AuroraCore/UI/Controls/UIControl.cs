using System;
using System.Collections.Generic;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.UI.Animation;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.Utils.Toolkit;
using CipherPark.Aurora.Core.Module;
using System.Linq;
using SharpDX;
using CipherPark.Aurora.Core.World.Scene;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Controls
{
    public abstract class UIControl 
    {
        #region Fields
        private IUIRoot _visualRoot = null;
        private IGameApp _game = null;
        private SpriteBatch _spriteBatch = null;
        //private bool _isInitialized = false;
        private UIControlCollection _children = null;
        private Size2F _size = Size2FExtension.Zero;
        private Vector2 _position = Vector2Extension.Zero;
        private UIControl _parent = null;
        private bool _visible = false;
        private bool _enabled = false;
        private bool _enableFocus = false;
        private bool _initialized = false;
        #endregion

        #region Constructors
        public UIControl(IUIRoot visualRoot) 
        {
            _visualRoot = visualRoot;
            _visualRoot.LoadComplete += VisualRoot_LoadComplete;            
            _game = visualRoot.Game;
            Padding = BoundaryF.Zero;
            Margin = BoundaryF.Zero;            
            _enabled = true;
            _enableFocus = true;
            _visible = true;
            _spriteBatch = new SpriteBatch(visualRoot.Game.GraphicsDeviceContext);
            _children = new UIControlCollection();
            _children.CollectionChanged += Children_CollectionChanged;            
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

        protected virtual IControlLayoutManager LayoutManager { get { return null; } }

        ///// <summary>
        ///// Sets the custom focus manager for this control and all descendants which
        ///// don't have a CustomFocusManager explicitly set.
        ///// Gets the custom focus manager for this control. If none was set this property
        ///// returns the first custom focus manager found while search up the control's lineage.
        ///// </summary>       
        //public UIControl CustomFocusContainer 
        //{
        //    get
        //    {
        //        if (this is ICustomFocusContainer)
        //            return this;
        //        else if (this.Parent != null)
        //            return this.Parent.CustomFocusContainer;
        //    }
        //}

        public Guid Id { get; set; }

        public Guid LayoutId { get; set; }

        public Vector2 Position
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

        public Size2F Size
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
        
        public bool Enabled 
        { 
            get { return _enabled; }
            set { 
                _enabled = value;
                OnEnabledChanged();
            } 
        }
        
        public bool Visible
        {
            get { return _visible; }
            set { 
                _visible = value;
                OnVisibleChanged();
            }
        }
        
        public SpriteBatch ControlSpriteBatch 
        { 
            get { return _spriteBatch; } 
        }
        
        public float ZOrder { get; set; }

        public float TabOrder { get; set; }

        /// <summary>
        /// Determines whether this control instance currently has input (keyboard/gamepad) focus.
        /// </summary>
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

        /// <summary>
        /// Determines whether the focus control is a child of this control.
        /// </summary>
        public bool ContainsFocus
        {
            get
            {
                return (VisualRoot.FocusManager.FocusedControl != null &&
                        IsDescendant(VisualRoot.FocusManager.FocusedControl));
            }
        }

        /// <summary>
        /// Determines whether this control was the innermost control to be hit.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public bool IsHit
        {
            get
            {
                return this.VisualRoot.FocusManager.HitList.LastOrDefault() == this;
            }
        }

        /// <summary>
        /// Determines whether this type (class) of control can receive focus.
        /// </summary>
        /// <remarks>
        /// Unlike UIControl.EnableFocus, this value will be the same for all instances of this control-type.
        /// </remarks>
        public virtual bool CanReceiveFocus { get { return false; } }

        /// <summary>
        /// Determines whether this control-instance is able to receive focus.
        /// </summary>
        public bool EnableFocus 
        {
            get { return _enableFocus; }
            set
            {
                _enableFocus = value;
                OnEnableFocusChanged();
            }                
        }      

        /// <summary>
        /// Determines if the specified control is a descendant of this control.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public bool IsDescendant(UIControl control)
        {
            //if (control == null)
            //    return false; 

            //else  if (Children.Contains(control))
            //    return true;

            //else
            //{
            //    foreach (UIControl child in Children)
            //        if (child.IsDescendant(control))
            //            return true;                
            //    return false;
            //}           
            return control.IsAncestor(this);
        }

        /// <summary>
        /// Determines if the specified control is an ancestor of this control.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public bool IsAncestor(UIControl control)
        {
            if (control == null)
                return false;

            else if (this.Parent == null)
                return false;

            else if (this.Parent == control)
                return true;

            else
                return this.Parent.IsAncestor(control);
        }

        /// <summary>
        /// Determines if this control and all its ancestors are visible.
        /// </summary>
        public bool VisibleInTree
        {
            get
            {
                UIControl c = this;
                while (c != null)
                {
                    if (c.Visible == false)
                        return false;
                    c = c.Parent;
                }
                return true;
            }
        }

        /// <summary>
        /// Determins if this control and all its ancestors are enabled.
        /// </summary>
        public bool EnabledInTree
        {
            get
            {
                UIControl c = this;
                while (c != null)
                {
                    if (c.Enabled == false)
                        return false;
                    c = c.Parent;
                }
                return true;
            }
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
        public RectangleF Bounds { get { return new RectangleF(Position.X, Position.Y, this.Size.Width, this.Size.Height); } }
        public virtual RectangleF ClientRectangle { get { return new RectangleF(0, 0, this.Size.Width, this.Size.Height); } }
        public BoundaryF Padding { get; set; }
        public BoundaryF Margin { get; set; }
        public UIControl Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null && _parent.Children.Contains(this))
                    _parent.Children.Remove(this);
                _parent = value;
                if (_parent != null && !_parent.Children.Contains(this))
                    _parent.Children.Add(this);
                OnParentChanged();
            }
        }

        public UIControlCollection Children { get { return _children; } }
        public VerticalAlignment VerticalAlignment { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        //TODO: Implement mouse capture in control tree.
        public bool Capture { get; set; }
        public bool SuspendLayout { get; set; }
        public UIControlBehavior Behavior { get; set; }

        #endregion

        #region Methods       

        public void Initialize() 
        {
            Behavior?.Initialize(this);
            OnInitialize();
            _initialized = true;
        }

        protected virtual void OnInitialize() { }

        public void Update(GameTime gameTime)
        {
            if (!_initialized)
                Initialize();
            Behavior?.Update(this);
            OnUpdate(gameTime);
        }

        protected virtual void OnUpdate(GameTime gameTime) { }

        public void Draw()
        {
            if(Visible)
                OnDraw();
        }

        protected virtual void OnDraw() { }

        public virtual void ApplyTemplate(UIControlTemplate template)
        {
            if (template.Size != null)
                this.Size = template.Size.Value;          
        }      

        public RectangleF BoundsToSurface(RectangleF bounds)
        {
            Vector2 boundsOrigin = new Vector2(bounds.Left, bounds.Top);
            Vector2 surfaceBoundsOrigin = PositionToSurface(boundsOrigin);
            return new RectangleF(surfaceBoundsOrigin.X, surfaceBoundsOrigin.Y, bounds.Width, bounds.Height);
        }        

        public Vector2 PositionToSurface(Vector2 position)
        {
            Vector2 transformedPosition = position;
            UIControl root = this.Parent;
            while (root != null)
            {
                transformedPosition = Vector2Extension.Add(transformedPosition, root.Position);
                root = root.Parent;
            }
            return transformedPosition;
        }

        public RectangleF BoundsToLocal(RectangleF surfaceBounds)
        {
            Vector2 surfaceBoundsOrigin = new Vector2(surfaceBounds.Left, surfaceBounds.Top);
            Vector2 boundsOrigin = PositionToLocal(surfaceBoundsOrigin);
            return new RectangleF(boundsOrigin.X, boundsOrigin.Y, surfaceBounds.Width, surfaceBounds.Height);
        }

        public Vector2 PositionToLocal(Vector2 position)
        {
            Vector2 transformedPosition = position;
            UIControl root = this.Parent;
            while (root != null)
            {
                transformedPosition = Vector2Extension.Subtract(transformedPosition, root.Position);
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
                
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (UIControl child in args.OldItems)
                        OnChildRemoved(child);
                    foreach (UIControl child in args.NewItems)
                        OnChildAdded(child);
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
            if (!SuspendLayout)
            {
                if (LayoutManager != null)
                    LayoutManager.UpdateLayout(reason);
                OnLayoutChanged();
            }
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

        protected virtual void OnParentChanged()
        {
            EventHandler handler = ParentChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnVisibleChanged()
        {
            EventHandler handler = VisibleChanged;
            
            if (handler != null)
                handler(this, EventArgs.Empty);
            
            Action<UIControl> notifyVisibleInTreeChanged = null;
            notifyVisibleInTreeChanged  = new Action<UIControl>( (c) => 
            {
                EventHandler _handler = c.VisibleInTreeChanged;
                if (handler != null)
                    handler(this, EventArgs.Empty);
                foreach (UIControl child in c.Children)
                    notifyVisibleInTreeChanged(child);
            });
            notifyVisibleInTreeChanged(this);
        }

        protected virtual void OnEnabledChanged()
        {
            EventHandler handler = EnabledChanged;
            
            if (handler != null)
                handler(this, EventArgs.Empty);

            Action<UIControl> notify = null;
            notify = new Action<UIControl>((c) =>
            {
                EventHandler _handler = c.EnabledInTreeChanged;
                if (handler != null)
                    handler(this, EventArgs.Empty);
                foreach (UIControl child in c.Children)
                    notify(child);
            });

            notify(this);
        }

        protected virtual void OnEnableFocusChanged()
        {
            EventHandler handler = EnableFocusChanged;
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
        public event EventHandler ParentChanged;
        public event EventHandler VisibleChanged;
        public event EventHandler EnabledChanged;
        public event EventHandler VisibleInTreeChanged;
        public event EventHandler EnabledInTreeChanged;
        public event EventHandler EnableFocusChanged;

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
