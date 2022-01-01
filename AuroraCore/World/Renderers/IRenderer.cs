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
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Effects;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World
{
    public interface IRenderer : IDisposable
    {       
        void Update(GameTime gameTime);
        void Draw(ITransformable container);
    }
}

