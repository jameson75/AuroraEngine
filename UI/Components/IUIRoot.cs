using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Controls;

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public interface IUIRoot
    {
        IGameApp Game { get; }

        UIControlCollection Controls { get; }
        
        UIStyleCollection Styles { get; }
        
        UIResourceCollection Resources { get; }

        void Update(long gameTime);

        void Draw(long gameTime);

        event EventHandler LoadComplete;

        FocusManager FocusManager { get; }
    }
}
