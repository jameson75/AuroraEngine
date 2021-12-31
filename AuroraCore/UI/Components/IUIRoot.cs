using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using CipherPark.KillScript.Core.UI.Controls;
using CipherPark.KillScript.Core.Animation;
using CipherPark.KillScript.Core.UI.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.UI.Components
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
