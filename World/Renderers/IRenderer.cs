using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Float4 = SharpDX.Vector4;
using CipherPark.KillScript.Core.Animation;
using CipherPark.KillScript.Core.World.Geometry;
using CipherPark.KillScript.Core.Utils;
using CipherPark.KillScript.Core.World.Scene;
using CipherPark.KillScript.Core.Effects;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.World
{
    public interface IRenderer : IDisposable
    {       
        void Update(GameTime gameTime);
        void Draw(ITransformable container);
    }
}

