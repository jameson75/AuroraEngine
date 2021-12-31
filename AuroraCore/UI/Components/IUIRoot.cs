using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using CipherPark.Aurora.Core.UI.Controls;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.UI.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Components
{
    public interface IUIRoot
    {
        IGameApp Game { get; }

        UIControlCollection Controls { get; }
        
        UIStyleCollection Styles { get; }
        
        UIResourceCollection Resources { get; }

        void Update(GameTime gameTime);

        void Draw();

        event EventHandler LoadComplete;

        FocusManager FocusManager { get; }

        Size2F ScreenSize { get; }

        List<ISimulatorController> Animations { get; }

        IUITheme Theme { get; }     
    }
}
