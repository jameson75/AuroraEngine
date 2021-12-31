using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.KillScript.Core.Module;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.UI.Components
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
