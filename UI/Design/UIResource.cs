using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.Module;

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public class UIResource
    {
        IGameApp _game = null;
        
        public UIResource(IGameApp game)
        {
            _game = game;
        }

        public string Name { get; set; }
    }

    public class UIResourceCollection : System.Collections.ObjectModel.ObservableCollection<UIResource>
    {
        public bool Contains(string resourceName)
        {
            foreach (UIResource resource in this)
                if (resource.Name == resourceName)
                    return true;
            return false;
        }

        public UIResource this[string resourceName]
        {
            get
            {
                foreach (UIResource resource in this)
                    if (resource.Name == resourceName)
                        return resource;
                return null;
            }
        }
    }
}
