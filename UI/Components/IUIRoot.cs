using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

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

        DrawingSizeF ScreenSize { get; }

        List<IAnimationController> Animations { get; }

        //IUITheme Theme { get; }     
    }
}
