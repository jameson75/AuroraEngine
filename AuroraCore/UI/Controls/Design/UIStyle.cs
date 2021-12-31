using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.Aurora.Core.Module;
using CipherPark.Aurora.Core.UI.Controls;
using System.Xml.Linq;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Components
{
    public abstract class UIStyle
    {  
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
