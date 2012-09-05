using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.UI.Controls;
using System.Xml.Linq;

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public abstract class UIStyle
    {
        IGameApp _game = null;

        public UIStyle(IGameApp game)
        {
            _game = game;
        }

        public IGameApp Game { get { return _game; } }

        public string Name { get; set; }

        public abstract UIContent GenerateContent();
    }

    public class UIStyleCollection : System.Collections.ObjectModel.ObservableCollection<UIStyle>
    {
        public bool Contains(string styleName)
        {
            foreach (UIStyle style in this)
                if (style.Name == styleName)
                    return true;
            return false;
        }

        public UIStyle this[string styleName]
        {
            get
            {
                foreach (UIStyle style in this)
                    if (style.Name == styleName)
                        return style;
                return null;
            }
        }   
    }
}
