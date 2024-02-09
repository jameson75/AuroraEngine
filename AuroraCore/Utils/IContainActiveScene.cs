using CipherPark.Aurora.Core.World.Scene;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core
{
    public interface IContainActiveScene
    {
        SceneGraph Scene { get; }
    }
}