using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.World;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////


namespace CipherPark.AngelJacket.Core.Services
{
    public interface IGameContextService
    {
        /// <summary>
        /// Gets the current game context if one exits. If none exists, it returns null.
        /// </summary>
        GameContext? Context { get; }
        /// <summary>
        /// Updates the service with the current game context.
        /// </summary>
        void Update();
    }    
}
