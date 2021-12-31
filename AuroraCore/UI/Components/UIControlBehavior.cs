using CipherPark.KillScript.Core.UI.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.KillScript.Core.UI.Components
{
    public abstract class UIControlBehavior
    {
        public virtual void Update(UIControl control) { }
        
        public virtual void Initialize(UIControl control) { }     

        protected IGameApp GetGameApp(UIControl control)
        {
            return control.VisualRoot.Game;
        }           
    }
}
