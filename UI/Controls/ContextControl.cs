using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

namespace CipherPark.AngelJacket.Core.UI.Controls.Design
{
    public abstract class ContextControl<T> : ContainerControl, ICommandDispatcher where T : UIControl
    {
        private T _subControl = null;

        protected ContextControl(IUIRoot visualRoot, Func<IUIRoot, T> creator)
            : base(visualRoot)
        {
            _subControl = creator(visualRoot);
            _subControl.VerticalAlignment = Controls.VerticalAlignment.Stretch;
            _subControl.HorizontalAlignment = Controls.HorizontalAlignment.Stretch;
            this.Children.Add(_subControl);
        }
    }

    public class ListSelect : UIControl
    {

    }
}
