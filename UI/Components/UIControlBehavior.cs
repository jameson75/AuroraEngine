using CipherPark.AngelJacket.Core.UI.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public abstract class UIControlBehavior
    {
        public abstract void Update(UIControl control);

        protected IGameApp GetGameApp(UIControl control)
        {
            return control.VisualRoot.Game;
        }
    }
}
