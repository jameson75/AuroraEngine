using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CipherPark.AngelJacket.Core.UI.Components;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public abstract class ContainerControl : UIControl
    {         
        protected ContainerControl(IUIRoot root)
            : base(root)
        { }

        public override void Draw(long gameTime)
        {
            foreach (UIControl control in Children)
                control.Draw(gameTime);
            base.Draw(gameTime);    
        }      

        public override void Update(long gameTime)
        {
            foreach (UIControl child in this.Children)
                child.Update(gameTime);
            base.Update(gameTime);
        }

        public override void UpdateEffect(long gameTime)
        {
            foreach (UIControl child in this.Children)
                child.UpdateEffect(gameTime);
            base.UpdateEffect(gameTime);
        }

        [Obsolete]
        public override UIControl _GetNextFocusableChild(UIControl fromControl)
        {
            if (fromControl == null)
                throw new ArgumentNullException("fromControl");

            if (fromControl.Parent != this)
                throw new InvalidOperationException("starting control is not an immediate child of this control");

            UIControl[] tabOrderedControls = FocusManager.ToTabOrderedControlArray(Children);
            int startAfterIndex = Array.IndexOf(tabOrderedControls, fromControl);            
            for (int i = startAfterIndex + 1; i < tabOrderedControls.Length; i++)
            {
                if (tabOrderedControls[i].Visible && tabOrderedControls[i].Enabled && tabOrderedControls[i].CanFocus)
                    return tabOrderedControls[i];
            }

            return null;
        }

        protected override void OnChildAdded(UIControl child)
        {
            base.OnChildAdded(child);
            UpdateLayout(LayoutUpdateReason.ChildCountChanged);
        }

        protected override void OnChildRemoved(UIControl child)
        {
            base.OnChildRemoved(child);
            UpdateLayout(LayoutUpdateReason.ChildCountChanged);
        }

        protected override void OnChildReset()
        {
            base.OnChildReset();
            UpdateLayout(LayoutUpdateReason.ChildCountChanged);
        }
    }
}
