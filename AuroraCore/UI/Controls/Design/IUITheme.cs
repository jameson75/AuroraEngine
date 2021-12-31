using CipherPark.Aurora.Core.UI.Controls;
using System.Collections.Generic;

namespace CipherPark.Aurora.Core.UI.Components
{
    public interface IUITheme
    {        
        void Apply(UITree visualRoot);
        void Apply(IEnumerable<UIControl> controls);
        void Apply(UIControl control);
    }
}
